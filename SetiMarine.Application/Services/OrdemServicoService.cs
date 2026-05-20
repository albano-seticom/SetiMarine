using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class OrdemServicoService(ISetiMarineDbContext ctx)
{
    public async Task<List<OrdemServico>> ListarPorEmpresaAsync(
        int empresaId, StatusOS? status = null, int? responsavelId = null)
    {
        var q = ctx.OrdensServico
            .Include(o => o.Embarcacao).ThenInclude(e => e!.Cliente)
            .Include(o => o.Responsavel)
            .Where(o => o.EmpresaId == empresaId);

        if (status.HasValue)
            q = q.Where(o => o.Status == status.Value);

        if (responsavelId.HasValue)
            q = q.Where(o => o.ResponsavelId == responsavelId.Value);

        return await q.OrderByDescending(o => o.CriadaEm).ToListAsync();
    }

    public async Task<OrdemServico?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.OrdensServico
            .Include(o => o.Embarcacao).ThenInclude(e => e!.Cliente)
            .Include(o => o.Responsavel)
            .FirstOrDefaultAsync(o => o.Id == id && o.EmpresaId == empresaId);

    public async Task CriarAsync(OrdemServico os)
    {
        os.CriadaEm = DateTime.UtcNow;
        os.Status   = StatusOS.Pendente;
        ctx.OrdensServico.Add(os);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(OrdemServico os)
    {
        ctx.OrdensServico.Update(os);
        await ctx.SaveChangesAsync();
    }

    public async Task AvancarStatusAsync(int id, int empresaId)
    {
        var os = await ctx.OrdensServico.FirstOrDefaultAsync(o => o.Id == id && o.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Ordem de serviço não encontrada.");

        os.Status = os.Status switch
        {
            StatusOS.Pendente   => StatusOS.EmExecucao,
            StatusOS.EmExecucao => StatusOS.Finalizado,
            _                   => os.Status
        };

        if (os.Status == StatusOS.Finalizado)
            os.FinalizadaEm = DateTime.UtcNow;

        await ctx.SaveChangesAsync();
    }

    public async Task<List<Embarcacao>> ListarEmbarcacoesAsync(int empresaId)
        => await ctx.Embarcacoes
            .Include(e => e.Cliente)
            .Where(e => e.EmpresaId == empresaId && e.Ativa)
            .OrderBy(e => e.Nome)
            .ToListAsync();

    public async Task<List<Usuario>> ListarColaboradoresAsync(int empresaId)
        => await ctx.Usuarios
            .Where(u => u.EmpresaId == empresaId && u.Ativo)
            .OrderBy(u => u.Nome)
            .ToListAsync();
}
