using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class SetiFiscalService(ISetiMarineDbContext ctx, IHttpClientFactory httpFactory)
{
    public async Task<ResultadoFiscal> EmitirAsync(int notaId, int empresaId)
    {
        var nota = await ctx.Notas
            .Include(n => n.Cliente)
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == notaId && n.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Nota não encontrada.");

        var cfg = await ctx.ConfiguracoesMarina
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Configuração da marina não encontrada.");

        if (string.IsNullOrWhiteSpace(cfg.SetiFiscalBaseUrl) ||
            string.IsNullOrWhiteSpace(cfg.SetiFiscalApiKey)  ||
            !cfg.SetiFiscalEmpresaId.HasValue)
            throw new InvalidOperationException(
                "Configuração do SetiFiscal incompleta. Verifique URL, API Key e EmpresaId nas configurações.");

        var client = CriarClient(cfg);

        ResultadoFiscal resultado = nota.Tipo switch
        {
            TipoNota.NFCe => await EmitirNfce(client, nota, cfg),
            TipoNota.NFe  => await EmitirNfe(client, nota, cfg),
            TipoNota.NFSe => await EmitirNfse(client, nota, cfg),
            _             => throw new InvalidOperationException("Tipo de nota inválido.")
        };

        if (resultado.Sucesso)
        {
            nota.ChaveNFe      = resultado.ChaveAcesso;
            nota.Status        = StatusNota.Emitida;
            nota.CodigoSefaz   = resultado.CodigoSefaz;
            nota.MensagemSefaz = resultado.MensagemSefaz;

            // Sobrescreve o número do rascunho com o número oficial retornado pelo SetiFiscal/SEFAZ
            var numeroOficial = resultado.NumeroNfse ?? resultado.NumeroNota;
            if (int.TryParse(numeroOficial, out var num) && num > 0)
                nota.Numero = num;

            await ctx.SaveChangesAsync();
        }
        else
        {
            nota.CodigoSefaz   = resultado.CodigoSefaz;
            nota.MensagemSefaz = resultado.Erro;
            await ctx.SaveChangesAsync();
        }

        return resultado;
    }

    public async Task<ResultadoFiscal> CancelarAsync(int notaId, int empresaId, string justificativa)
    {
        var nota = await ctx.Notas
            .FirstOrDefaultAsync(n => n.Id == notaId && n.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Nota não encontrada.");

        if (string.IsNullOrWhiteSpace(nota.ChaveNFe))
            throw new InvalidOperationException("Nota sem chave de acesso — não pode ser cancelada.");

        var cfg = await ctx.ConfiguracoesMarina
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Configuração da marina não encontrada.");

        var client = CriarClient(cfg);
        var resultado = await client.CancelarAsync(nota.ChaveNFe, justificativa);

        if (resultado.Sucesso)
        {
            nota.Status        = StatusNota.Cancelada;
            nota.CodigoSefaz   = resultado.CodigoSefaz;
            nota.MensagemSefaz = resultado.MensagemSefaz;
            await ctx.SaveChangesAsync();
        }

        return resultado;
    }

    // ─────────────────────────────────────────────────────────────────

    private SetiFiscalClient CriarClient(Domain.Entities.ConfiguracaoMarina cfg)
    {
        var http = httpFactory.CreateClient("setifiscal");
        var options = new SetiFiscalOptions
        {
            BaseUrl   = cfg.SetiFiscalBaseUrl!,
            ApiKey    = cfg.SetiFiscalApiKey!,
            EmpresaId = cfg.SetiFiscalEmpresaId!.Value
        };
        return new SetiFiscalClient(http, options);
    }

    private static Task<ResultadoFiscal> EmitirNfce(SetiFiscalClient client, Nota nota, Domain.Entities.ConfiguracaoMarina cfg)
    {
        var req = new EmitirNfceRequest
        {
            Consumidor  = nota.Cliente is { } c ? new ConsumidorNfce { CpfCnpj = c.CpfCnpj, Nome = c.Nome } : null,
            Itens       = MapearItens(nota.Itens),
            Pagamentos  = MapearPagamentos(nota),
            ValorTotal  = nota.ValorTotal,
            InformacoesAdicionais = nota.Observacoes
        };
        return client.EmitirNfceAsync(req);
    }

    private static Task<ResultadoFiscal> EmitirNfe(SetiFiscalClient client, Nota nota, Domain.Entities.ConfiguracaoMarina cfg)
    {
        var cli = nota.Cliente
            ?? throw new InvalidOperationException("NF-e requer cliente vinculado.");

        if (string.IsNullOrWhiteSpace(cli.Logradouro) || !cli.CodigoMunicipio.HasValue)
            throw new InvalidOperationException(
                "Para emitir NF-e o cliente precisa ter endereço completo (logradouro, município com código IBGE, UF, CEP).");

        var req = new EmitirNfeRequest
        {
            Destinatario = new Destinatario
            {
                CpfCnpj        = cli.CpfCnpj ?? "",
                Nome           = cli.Nome,
                Email          = cli.Email,
                Logradouro     = cli.Logradouro ?? "",
                Numero         = cli.NumeroEnd  ?? "S/N",
                Complemento    = cli.Complemento,
                Bairro         = cli.Bairro     ?? "",
                Municipio      = cli.Municipio  ?? "",
                CodigoMunicipio= cli.CodigoMunicipio!.Value,
                Uf             = cli.Uf         ?? "",
                Cep            = (cli.Cep ?? "").Replace("-", ""),
                Telefone       = cli.Telefone,
                Ie             = cli.Ie,
            },
            Itens      = MapearItens(nota.Itens),
            Pagamentos = MapearPagamentos(nota),
            ValorTotal = nota.ValorTotal,
            InformacoesAdicionais = nota.Observacoes
        };
        return client.EmitirNfeAsync(req);
    }

    private static Task<ResultadoFiscal> EmitirNfse(SetiFiscalClient client, Nota nota, Domain.Entities.ConfiguracaoMarina cfg)
    {
        var cli = nota.Cliente
            ?? throw new InvalidOperationException("NFS-e requer cliente vinculado.");

        if (string.IsNullOrWhiteSpace(cli.Logradouro) || !cli.CodigoMunicipio.HasValue)
            throw new InvalidOperationException(
                "Para emitir NFS-e o cliente precisa ter endereço completo.");

        var item = nota.Itens.FirstOrDefault()
            ?? throw new InvalidOperationException("NFS-e precisa de pelo menos um item.");

        var codigoServico = item.CodigoServico ?? cfg.CodigoServicoDefault
            ?? throw new InvalidOperationException(
                "Código de serviço (LC 116) não definido. Configure um padrão nas Configurações.");

        var codigoNbs = item.CodigoNbs ?? cfg.CodigoNbsDefault
            ?? throw new InvalidOperationException(
                "Código NBS não definido. Configure um padrão nas Configurações.");

        var req = new EmitirNfseRequest
        {
            NumeroRps = nota.Numero,
            SerieRps  = nota.Serie,
            Tomador   = new Tomador
            {
                CpfCnpj        = cli.CpfCnpj ?? "",
                Nome           = cli.Nome,
                Email          = cli.Email,
                Logradouro     = cli.Logradouro ?? "",
                Numero         = cli.NumeroEnd  ?? "S/N",
                Complemento    = cli.Complemento,
                Bairro         = cli.Bairro     ?? "",
                Municipio      = cli.Municipio  ?? "",
                CodigoMunicipio= cli.CodigoMunicipio!.Value,
                Uf             = cli.Uf         ?? "",
                Cep            = (cli.Cep ?? "").Replace("-", ""),
                Telefone       = cli.Telefone,
                Ie             = cli.Ie,
            },
            Servico = new ServicoNfse
            {
                Descricao     = string.Join("; ", nota.Itens.Select(i => i.Descricao)),
                CodigoServico = codigoServico,
                CodigoNbs     = codigoNbs,
                ValorServico  = nota.ValorTotal,
                AliquotaIss   = nota.ValorISS > 0 ? Math.Round(nota.ValorISS / nota.ValorTotal * 100, 4) : 0,
                ValorIss      = nota.ValorISS,
                IssRetido     = false,
            }
        };
        return client.EmitirNfseAsync(req);
    }

    private static List<ItemNota> MapearItens(IEnumerable<Domain.Entities.NotaItem> itens)
        => itens.Select((i, idx) => new ItemNota
        {
            NumeroItem    = idx + 1,
            CodigoProduto = i.ProdutoId?.ToString() ?? "001",
            Descricao     = i.Descricao,
            Ncm           = i.Ncm,
            Cfop          = i.Cfop ?? "5102",
            Quantidade    = i.Quantidade,
            ValorUnitario = i.ValorUnitario,
            ValorTotal    = i.ValorTotal,
            CstOuCsosn    = i.CstIcms,
        }).ToList();

    private static List<PagamentoFiscal> MapearPagamentos(Nota nota)
        => new()
        {
            new PagamentoFiscal { FormaPagamento = "99", Valor = nota.ValorTotal }
        };
}
