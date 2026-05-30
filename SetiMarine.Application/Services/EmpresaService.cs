using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public class EmpresaService(ISetiMarineDbContext ctx)
{
    public async Task<Empresa?> ObterAsync(int empresaId)
        => await ctx.Empresas
            .Include(e => e.Plano)
            .FirstOrDefaultAsync(e => e.Id == empresaId);

    public async Task<List<Plano>> ListarPlanosAtivosAsync()
        => await ctx.Planos
            .Where(p => p.Ativo)
            .OrderBy(p => p.Ordem)
            .ThenBy(p => p.Valor)
            .ToListAsync();

    public async Task EditarAsync(Empresa empresa)
    {
        ctx.Empresas.Update(empresa);
        await ctx.SaveChangesAsync();
    }
}
