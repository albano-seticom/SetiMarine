using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class MovimentacaoService(ISetiMarineDbContext ctx)
{
    public async Task<List<Movimentacao>> ListarPorEmpresaAsync(
        int empresaId, StatusMovimentacao? status = null)
    {
        var q = ctx.Movimentacoes
            .Include(m => m.Embarcacao).ThenInclude(e => e!.Cliente)
            .Include(m => m.Vaga).ThenInclude(v => v!.Corredor)
            .Include(m => m.Responsavel)
            .Where(m => m.EmpresaId == empresaId);

        if (status.HasValue)
            q = q.Where(m => m.Status == status.Value);

        return await q.OrderByDescending(m => m.AgendadoPara).ToListAsync();
    }

    public async Task<Movimentacao?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Movimentacoes
            .Include(m => m.Embarcacao).ThenInclude(e => e!.Cliente)
            .Include(m => m.Vaga).ThenInclude(v => v!.Corredor)
            .Include(m => m.Responsavel)
            .Include(m => m.Historico)
            .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == empresaId);

    public async Task CriarAsync(Movimentacao mov)
    {
        mov.CriadaEm = DateTime.UtcNow;
        mov.Status   = StatusMovimentacao.Agendado;
        ctx.Movimentacoes.Add(mov);
        await ctx.SaveChangesAsync();
        await RegistrarHistoricoAsync(mov, StatusMovimentacao.Agendado);
    }

    public async Task AtualizarAsync(Movimentacao mov)
    {
        ctx.Movimentacoes.Update(mov);
        await ctx.SaveChangesAsync();
    }

    public async Task AvancarStatusAsync(int id, int empresaId, string? obs = null)
    {
        var mov = await ctx.Movimentacoes
            .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Movimentação não encontrada.");

        var proximo = ProximoStatus(mov.Status);
        if (proximo == mov.Status) return;

        mov.Status = proximo;

        /* quando a embarcação desce para água, atualiza status da vaga */
        if (proximo == StatusMovimentacao.NaAgua)
        {
            var vaga = await ctx.Vagas.FindAsync(mov.VagaId);
            if (vaga != null) vaga.Status = StatusVaga.EmMovimentacao;
        }

        if (proximo == StatusMovimentacao.Finalizado)
        {
            var vaga = await ctx.Vagas.FindAsync(mov.VagaId);
            if (vaga != null) vaga.Status = StatusVaga.Livre;
        }

        await ctx.SaveChangesAsync();
        await RegistrarHistoricoAsync(mov, proximo, obs);
    }

    public async Task<List<Embarcacao>> ListarEmbarcacoesAsync(int empresaId)
        => await ctx.Embarcacoes
            .Include(e => e.Cliente)
            .Where(e => e.EmpresaId == empresaId && e.Ativa)
            .OrderBy(e => e.Nome)
            .ToListAsync();

    public async Task<List<Vaga>> ListarVagasAsync(int empresaId)
        => await ctx.Vagas
            .Include(v => v.Corredor)
            .Where(v => v.EmpresaId == empresaId && v.Ativa)
            .OrderBy(v => v.Codigo)
            .ToListAsync();

    public async Task<List<Usuario>> ListarColaboradoresAsync(int empresaId)
        => await ctx.Usuarios
            .Where(u => u.EmpresaId == empresaId && u.Ativo)
            .OrderBy(u => u.Nome)
            .ToListAsync();

    private static StatusMovimentacao ProximoStatus(StatusMovimentacao atual) => atual switch
    {
        StatusMovimentacao.Agendado         => StatusMovimentacao.AguardandoPreparo,
        StatusMovimentacao.AguardandoPreparo=> StatusMovimentacao.EmPreparo,
        StatusMovimentacao.EmPreparo        => StatusMovimentacao.Pronto,
        StatusMovimentacao.Pronto           => StatusMovimentacao.Descendo,
        StatusMovimentacao.Descendo         => StatusMovimentacao.NaAgua,
        StatusMovimentacao.NaAgua           => StatusMovimentacao.Retornando,
        StatusMovimentacao.Retornando       => StatusMovimentacao.Subindo,
        StatusMovimentacao.Subindo          => StatusMovimentacao.Finalizado,
        _                                   => atual
    };

    private async Task RegistrarHistoricoAsync(
        Movimentacao mov, StatusMovimentacao novoStatus, string? obs = null)
    {
        ctx.HistoricoMovimentacoes.Add(new HistoricoMovimentacao
        {
            MovimentacaoId  = mov.Id,
            StatusAnterior  = mov.Status,
            StatusNovo      = novoStatus,
            Observacao      = obs,
            Em              = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();
    }
}
