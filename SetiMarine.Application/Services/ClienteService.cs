using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public class ClienteService(ISetiMarineDbContext ctx)
{
    public async Task<List<Cliente>> ListarPorEmpresaAsync(int empresaId, string? busca = null)
    {
        var q = ctx.Clientes.Include(c => c.Embarcacoes).Where(c => c.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(busca))
            q = q.Where(c => c.Nome.Contains(busca) ||
                             (c.Email != null && c.Email.Contains(busca)) ||
                             (c.CpfCnpj != null && c.CpfCnpj.Contains(busca)) ||
                             c.Telefone.Contains(busca));
        return await q.OrderBy(c => c.Nome).ToListAsync();
    }

    public async Task<Cliente?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Clientes.FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == empresaId);

    public async Task CriarAsync(Cliente cliente)
    {
        ctx.Clientes.Add(cliente);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(Cliente cliente)
    {
        ctx.Clientes.Update(cliente);
        await ctx.SaveChangesAsync();
    }

    public async Task ExcluirAsync(int id, int empresaId)
    {
        var c = await ObterPorIdAsync(id, empresaId)
            ?? throw new InvalidOperationException("Cliente nao encontrado.");
        ctx.Clientes.Remove(c);
        await ctx.SaveChangesAsync();
    }
}
