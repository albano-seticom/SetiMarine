using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public class CorredorService(ISetiMarineDbContext ctx)
{
    public async Task<List<Corredor>> ListarPorEmpresaAsync(int empresaId)
        => await ctx.Corredores
            .Where(c => c.EmpresaId == empresaId && c.Ativo)
            .OrderBy(c => c.Ordem)
            .Include(c => c.Secao)
            .Include(c => c.Vagas)
            .ToListAsync();

    public async Task<Corredor?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Corredores
            .Include(c => c.Secao)
            .FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == empresaId);

    public async Task<Corredor> CriarAsync(Corredor corredor)
    {
        var ultimaOrdem = await ctx.Corredores
            .Where(c => c.EmpresaId == corredor.EmpresaId)
            .MaxAsync(c => (int?)c.Ordem) ?? 0;

        corredor.Ordem = ultimaOrdem + 1;
        corredor.CriadoEm = DateTime.UtcNow;

        ctx.Corredores.Add(corredor);
        await ctx.SaveChangesAsync();
        return corredor;
    }

    public async Task<Corredor> EditarAsync(Corredor corredor)
    {
        ctx.Corredores.Update(corredor);
        await ctx.SaveChangesAsync();
        return corredor;
    }

    public async Task ExcluirAsync(int id, int empresaId)
    {
        var corredor = await ctx.Corredores
            .Include(c => c.Vagas)
            .FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Corredor não encontrado.");

        if (corredor.Vagas.Any(v => v.Ativa))
            throw new InvalidOperationException("Não é possível excluir um corredor que possui vagas ativas.");

        corredor.Ativo = false;
        await ctx.SaveChangesAsync();
    }

    public async Task ReordenarAsync(int empresaId, List<int> idsOrdenados)
    {
        var corredores = await ctx.Corredores
            .Where(c => c.EmpresaId == empresaId)
            .ToListAsync();

        for (int i = 0; i < idsOrdenados.Count; i++)
        {
            var corredor = corredores.FirstOrDefault(c => c.Id == idsOrdenados[i]);
            if (corredor != null)
                corredor.Ordem = i + 1;
        }

        await ctx.SaveChangesAsync();
    }
}
