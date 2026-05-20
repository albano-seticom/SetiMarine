using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class ContratoService(ISetiMarineDbContext ctx)
{
    public async Task<List<Contrato>> ListarPorEmpresaAsync(int empresaId, bool? apenasAtivos = null)
    {
        var q = ctx.Contratos
            .Include(c => c.Cliente)
            .Include(c => c.Embarcacao)
            .Include(c => c.Vaga)
            .Where(c => c.EmpresaId == empresaId);

        if (apenasAtivos.HasValue)
            q = q.Where(c => c.Ativo == apenasAtivos.Value);

        return await q.OrderByDescending(c => c.Inicio).ToListAsync();
    }

    public async Task<Contrato?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Contratos
            .Include(c => c.Cliente)
            .Include(c => c.Embarcacao)
            .Include(c => c.Vaga)
            .FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == empresaId);

    public async Task CriarAsync(Contrato contrato)
    {
        contrato.CriadoEm = DateTime.UtcNow;
        ctx.Contratos.Add(contrato);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(Contrato contrato)
    {
        ctx.Contratos.Update(contrato);
        await ctx.SaveChangesAsync();
    }

    public async Task EncerrarAsync(int id, int empresaId)
    {
        var c = await ctx.Contratos.FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Contrato não encontrado.");
        c.Ativo = false;
        c.Fim   = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
    }

    public async Task<List<Cliente>> ListarClientesAsync(int empresaId)
        => await ctx.Clientes
            .Where(c => c.EmpresaId == empresaId && c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();

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
}
