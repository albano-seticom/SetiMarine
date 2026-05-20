using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public class EmbarcacaoService(ISetiMarineDbContext ctx)
{
    public async Task<List<Embarcacao>> ListarPorEmpresaAsync(int empresaId, string? busca = null)
    {
        var q = ctx.Embarcacoes
            .Include(e => e.Cliente)
            .Where(e => e.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(busca))
            q = q.Where(e => e.Nome.Contains(busca) ||
                             (e.NumeroRegistro != null && e.NumeroRegistro.Contains(busca)) ||
                             (e.Modelo != null && e.Modelo.Contains(busca)));
        return await q.OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<Embarcacao?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Embarcacoes
            .Include(e => e.Cliente)
            .FirstOrDefaultAsync(e => e.Id == id && e.EmpresaId == empresaId);

    public async Task CriarAsync(Embarcacao embarcacao)
    {
        ctx.Embarcacoes.Add(embarcacao);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(Embarcacao embarcacao)
    {
        ctx.Embarcacoes.Update(embarcacao);
        await ctx.SaveChangesAsync();
    }

    public async Task<List<Cliente>> ListarClientesAsync(int empresaId)
        => await ctx.Clientes
            .Where(c => c.EmpresaId == empresaId && c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();
}
