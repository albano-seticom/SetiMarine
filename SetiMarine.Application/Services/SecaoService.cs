using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public class SecaoService(ISetiMarineDbContext ctx)
{
    public async Task<List<Secao>> ListarPorEmpresaAsync(int empresaId)
        => await ctx.Secoes
            .Where(s => s.EmpresaId == empresaId && s.Ativo)
            .OrderBy(s => s.Ordem).ThenBy(s => s.Nome)
            .Include(s => s.Corredores)
            .ToListAsync();

    public async Task<Secao?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Secoes.FirstOrDefaultAsync(s => s.Id == id && s.EmpresaId == empresaId);

    public async Task<Secao> CriarAsync(Secao secao)
    {
        var ultimaOrdem = await ctx.Secoes
            .Where(s => s.EmpresaId == secao.EmpresaId)
            .MaxAsync(s => (int?)s.Ordem) ?? 0;
        secao.Ordem    = ultimaOrdem + 1;
        secao.CriadoEm = DateTime.UtcNow;
        ctx.Secoes.Add(secao);
        await ctx.SaveChangesAsync();
        return secao;
    }

    public async Task EditarAsync(Secao secao)
    {
        ctx.Secoes.Update(secao);
        await ctx.SaveChangesAsync();
    }

    public async Task ExcluirAsync(int id, int empresaId)
    {
        var secao = await ctx.Secoes
            .Include(s => s.Corredores)
            .FirstOrDefaultAsync(s => s.Id == id && s.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Seção não encontrada.");
        if (secao.Corredores.Any(c => c.Ativo))
            throw new InvalidOperationException("Não é possível excluir uma seção com corredores ativos.");
        secao.Ativo = false;
        await ctx.SaveChangesAsync();
    }
}
