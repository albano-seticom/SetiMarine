using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class CaixaService(ISetiMarineDbContext ctx)
{
    public async Task<SessaoCaixa?> ObterSessaoAbertaAsync(int empresaId)
        => await ctx.SessoesCaixa
            .Include(s => s.Operador)
            .Include(s => s.Movimentos)
            .FirstOrDefaultAsync(s => s.EmpresaId == empresaId && s.Aberto);

    public async Task<List<SessaoCaixa>> ListarSessoesAsync(int empresaId, int take = 30)
        => await ctx.SessoesCaixa
            .Include(s => s.Operador)
            .Where(s => s.EmpresaId == empresaId)
            .OrderByDescending(s => s.DataAbertura)
            .Take(take)
            .ToListAsync();

    public async Task<List<MovimentoCaixa>> ListarMovimentosSessaoAsync(int sessaoId, int empresaId)
        => await ctx.MovimentosCaixa
            .Include(m => m.Pedido)
            .Include(m => m.VendaProduto)
            .Where(m => m.SessaoCaixaId == sessaoId && m.EmpresaId == empresaId)
            .OrderByDescending(m => m.CriadoEm)
            .ToListAsync();

    public async Task<SessaoCaixa> AbrirAsync(int empresaId, decimal valorAbertura, int? operadorId)
    {
        if (await ctx.SessoesCaixa.AnyAsync(s => s.EmpresaId == empresaId && s.Aberto))
            throw new InvalidOperationException("Já existe um caixa aberto. Feche-o antes de abrir um novo.");

        var sessao = new SessaoCaixa
        {
            EmpresaId    = empresaId,
            OperadorId   = operadorId,
            ValorAbertura = valorAbertura,
            DataAbertura = DateTime.UtcNow,
            Aberto       = true,
            CriadoEm    = DateTime.UtcNow,
        };
        ctx.SessoesCaixa.Add(sessao);
        await ctx.SaveChangesAsync();
        return sessao;
    }

    public async Task FecharAsync(int sessaoId, int empresaId, decimal valorContado, string? observacoes)
    {
        var sessao = await ctx.SessoesCaixa
            .Include(s => s.Movimentos)
            .FirstOrDefaultAsync(s => s.Id == sessaoId && s.EmpresaId == empresaId && s.Aberto)
            ?? throw new InvalidOperationException("Sessão de caixa não encontrada ou já fechada.");

        var totalEntradas = sessao.Movimentos
            .Where(m => m.Tipo == TipoMovimentoCaixa.Entrada)
            .Sum(m => m.Valor);
        var totalSaidas = sessao.Movimentos
            .Where(m => m.Tipo == TipoMovimentoCaixa.Saida)
            .Sum(m => m.Valor);

        var esperado = sessao.ValorAbertura + totalEntradas - totalSaidas;

        sessao.DataFechamento          = DateTime.UtcNow;
        sessao.ValorFechamentoContado  = valorContado;
        sessao.ValorFechamentoEsperado = esperado;
        sessao.Diferenca               = valorContado - esperado;
        sessao.ObservacoesFechamento   = observacoes;
        sessao.Aberto                  = false;

        await ctx.SaveChangesAsync();
    }

    public async Task<MovimentoCaixa> RegistrarMovimentoAsync(
        int empresaId, int sessaoId,
        TipoMovimentoCaixa tipo, OrigemMovimentoCaixa origem,
        decimal valor, string descricao,
        int? pedidoId = null, int? vendaProdutoId = null)
    {
        var sessao = await ctx.SessoesCaixa
            .FirstOrDefaultAsync(s => s.Id == sessaoId && s.EmpresaId == empresaId && s.Aberto)
            ?? throw new InvalidOperationException("Caixa não está aberto.");

        var mov = new MovimentoCaixa
        {
            EmpresaId      = empresaId,
            SessaoCaixaId  = sessaoId,
            Tipo           = tipo,
            Origem         = origem,
            Valor          = valor,
            Descricao      = descricao,
            PedidoId       = pedidoId,
            VendaProdutoId = vendaProdutoId,
            CriadoEm       = DateTime.UtcNow,
        };
        ctx.MovimentosCaixa.Add(mov);
        await ctx.SaveChangesAsync();
        return mov;
    }

    // Registra automaticamente quando CaixaAutomatico = true (chamado por outros serviços)
    public async Task RegistrarAutomaticoAsync(
        int empresaId, TipoMovimentoCaixa tipo, OrigemMovimentoCaixa origem,
        decimal valor, string descricao, int? pedidoId = null, int? vendaProdutoId = null)
    {
        var sessao = await ctx.SessoesCaixa
            .FirstOrDefaultAsync(s => s.EmpresaId == empresaId && s.Aberto);

        if (sessao == null) return; // sem caixa aberto — ignora silenciosamente

        var mov = new MovimentoCaixa
        {
            EmpresaId      = empresaId,
            SessaoCaixaId  = sessao.Id,
            Tipo           = tipo,
            Origem         = origem,
            Valor          = valor,
            Descricao      = descricao,
            PedidoId       = pedidoId,
            VendaProdutoId = vendaProdutoId,
            CriadoEm       = DateTime.UtcNow,
        };
        ctx.MovimentosCaixa.Add(mov);
        await ctx.SaveChangesAsync();
    }
}
