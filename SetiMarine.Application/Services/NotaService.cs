using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class NotaService(ISetiMarineDbContext ctx)
{
    public async Task<List<Nota>> ListarAsync(int empresaId, StatusNota? status = null)
    {
        var q = ctx.Notas
            .Include(n => n.Cliente)
            .Include(n => n.Pedido)
            .Where(n => n.EmpresaId == empresaId);

        if (status.HasValue)
            q = q.Where(n => n.Status == status.Value);

        return await q.OrderByDescending(n => n.CriadoEm).ToListAsync();
    }

    public async Task<Nota?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Notas
            .Include(n => n.Cliente)
            .Include(n => n.Pedido)
            .Include(n => n.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(n => n.Id == id && n.EmpresaId == empresaId);

    public async Task<int> ProximoNumeroAsync(int empresaId, string serie = "1")
    {
        var ultimo = await ctx.Notas
            .Where(n => n.EmpresaId == empresaId && n.Serie == serie)
            .MaxAsync(n => (int?)n.Numero) ?? 0;
        return ultimo + 1;
    }

    public async Task<Nota> CriarAsync(Nota nota, List<NotaItem> itens)
    {
        nota.CriadoEm  = DateTime.UtcNow;
        nota.DataEmissao = nota.DataEmissao == default ? DateTime.UtcNow : nota.DataEmissao;

        if (nota.Numero == 0)
            nota.Numero = await ProximoNumeroAsync(nota.EmpresaId, nota.Serie);

        RecalcularTotais(nota, itens);
        nota.Itens = itens;
        ctx.Notas.Add(nota);
        await ctx.SaveChangesAsync();
        return nota;
    }

    public async Task EditarAsync(Nota nota, List<NotaItem> itens)
    {
        var existente = await ctx.Notas
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == nota.Id && n.EmpresaId == nota.EmpresaId)
            ?? throw new InvalidOperationException("Nota não encontrada.");

        ctx.NotaItens.RemoveRange(existente.Itens);

        existente.ClienteId   = nota.ClienteId;
        existente.PedidoId    = nota.PedidoId;
        existente.Serie       = nota.Serie;
        existente.ChaveNFe    = nota.ChaveNFe;
        existente.Status      = nota.Status;
        existente.DataEmissao = nota.DataEmissao;
        existente.DataSaida   = nota.DataSaida;
        existente.Observacoes = nota.Observacoes;

        RecalcularTotais(existente, itens);
        existente.Itens = itens;
        await ctx.SaveChangesAsync();
    }

    public async Task AlterarStatusAsync(int id, int empresaId, StatusNota novoStatus)
    {
        var nota = await ctx.Notas.FirstOrDefaultAsync(n => n.Id == id && n.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Nota não encontrada.");
        nota.Status = novoStatus;
        await ctx.SaveChangesAsync();
    }

    public async Task<List<Nota>> ListarPorPedidoAsync(int pedidoId, int empresaId)
        => await ctx.Notas
            .Where(n => n.PedidoId == pedidoId && n.EmpresaId == empresaId)
            .ToListAsync();

    public async Task<Nota> GerarDePedidoAsync(int pedidoId, int empresaId, TipoNota tipo = TipoNota.NFe)
    {
        var pedido = await ctx.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens).ThenInclude(i => i.Produto).ThenInclude(pr => pr!.Aux)
            .Include(p => p.Servicos)
            .FirstOrDefaultAsync(p => p.Id == pedidoId && p.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Pedido não encontrado.");

        List<NotaItem> itens;

        if (tipo == TipoNota.NFSe)
        {
            // NFS-e: gerada a partir dos serviços do pedido
            itens = pedido.Servicos
                .Where(s => !string.IsNullOrWhiteSpace(s.Descricao))
                .Select(s => new NotaItem
                {
                    Descricao     = s.Descricao,
                    Quantidade    = 1,
                    ValorUnitario = s.Valor ?? 0,
                    ValorTotal    = s.Valor ?? 0,
                    AliquotaISS   = 0,
                }).ToList();
        }
        else
        {
            // NF-e / NFC-e: gerada a partir dos itens de produto
            itens = pedido.Itens.Select(i => new NotaItem
            {
                ProdutoId      = i.ProdutoId,
                Descricao      = i.Produto?.Nome ?? "",
                Quantidade     = i.Quantidade,
                ValorUnitario  = i.PrecoUnitario,
                ValorTotal     = i.ValorTotal,
                Cfop           = i.Cfop           ?? i.Produto?.Aux?.Cfop,
                Ncm            = i.Ncm            ?? i.Produto?.Aux?.Ncm,
                CstIcms        = i.CstIcms        ?? i.Produto?.Aux?.CstIcms,
                AliquotaICMS   = i.AliquotaICMS   ?? i.Produto?.Aux?.AliquotaICMS   ?? 0,
                AliquotaIPI    = i.AliquotaIPI    ?? i.Produto?.Aux?.AliquotaIPI    ?? 0,
                AliquotaPIS    = i.AliquotaPIS    ?? i.Produto?.Aux?.AliquotaPIS    ?? 0,
                AliquotaCOFINS = i.AliquotaCOFINS ?? i.Produto?.Aux?.AliquotaCOFINS ?? 0,
                AliquotaISS    = i.AliquotaISS    ?? i.Produto?.Aux?.AliquotaISS    ?? 0,
            }).ToList();
        }

        foreach (var item in itens)
        {
            item.ValorICMS   = item.ValorTotal * (item.AliquotaICMS   / 100);
            item.ValorIPI    = item.ValorTotal * (item.AliquotaIPI    / 100);
            item.ValorPIS    = item.ValorTotal * (item.AliquotaPIS    / 100);
            item.ValorCOFINS = item.ValorTotal * (item.AliquotaCOFINS / 100);
            item.ValorISS    = item.ValorTotal * (item.AliquotaISS    / 100);
        }

        var nota = new Nota
        {
            EmpresaId   = empresaId,
            PedidoId    = pedidoId,
            ClienteId   = pedido.ClienteId,
            Tipo        = tipo,
            Status      = StatusNota.Rascunho,
            DataEmissao = DateTime.UtcNow,
        };

        return await CriarAsync(nota, itens);
    }

    private static void RecalcularTotais(Nota nota, List<NotaItem> itens)
    {
        nota.ValorProdutos = itens.Sum(i => i.ValorTotal);
        nota.ValorICMS     = itens.Sum(i => i.ValorICMS);
        nota.ValorIPI      = itens.Sum(i => i.ValorIPI);
        nota.ValorPIS      = itens.Sum(i => i.ValorPIS);
        nota.ValorCOFINS   = itens.Sum(i => i.ValorCOFINS);
        nota.ValorISS      = itens.Sum(i => i.ValorISS);
        nota.ValorTotal    = nota.ValorProdutos;
    }
}
