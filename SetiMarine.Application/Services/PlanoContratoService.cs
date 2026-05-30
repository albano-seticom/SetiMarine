using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public class PlanoContratoService(ISetiMarineDbContext ctx)
{
    public async Task<List<PlanoContrato>> ListarAsync(int empresaId)
        => await ctx.PlanosContrato
            .Include(p => p.Servicos)
                .ThenInclude(s => s.TipoServico)
            .Where(p => p.EmpresaId == empresaId)
            .OrderBy(p => p.Nome)
            .ToListAsync();

    public async Task<PlanoContrato?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.PlanosContrato
            .Include(p => p.Servicos)
                .ThenInclude(s => s.TipoServico)
            .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId);

    public async Task CriarAsync(PlanoContrato plano, List<PlanoContratoServico> servicos)
    {
        plano.CriadoEm = DateTime.UtcNow;
        plano.Servicos = servicos;
        ctx.PlanosContrato.Add(plano);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(PlanoContrato plano, List<PlanoContratoServico> servicos)
    {
        var existente = await ctx.PlanosContrato
            .Include(p => p.Servicos)
            .FirstOrDefaultAsync(p => p.Id == plano.Id && p.EmpresaId == plano.EmpresaId)
            ?? throw new InvalidOperationException("Plano não encontrado.");

        existente.Nome        = plano.Nome;
        existente.Descricao   = plano.Descricao;
        existente.Mensalidade = plano.Mensalidade;
        existente.Ativo       = plano.Ativo;

        ctx.PlanoContratoServicos.RemoveRange(existente.Servicos);
        existente.Servicos = servicos;

        await ctx.SaveChangesAsync();
    }

    public async Task ExcluirAsync(int id, int empresaId)
    {
        var plano = await ctx.PlanosContrato
            .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Plano não encontrado.");
        ctx.PlanosContrato.Remove(plano);
        await ctx.SaveChangesAsync();
    }

    public async Task<List<TipoServicoConfig>> ListarTiposServicoAsync(int empresaId)
        => await ctx.TiposServicoConfig
            .Where(t => t.EmpresaId == empresaId && t.Ativo)
            .OrderBy(t => t.Nome)
            .ToListAsync();
}
