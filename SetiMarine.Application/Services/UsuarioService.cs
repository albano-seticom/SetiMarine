using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class UsuarioService(ISetiMarineDbContext ctx)
{
    public async Task<List<Usuario>> ListarPorEmpresaAsync(int empresaId, string? busca = null)
    {
        var q = ctx.Usuarios.Where(u => u.EmpresaId == empresaId);
        if (!string.IsNullOrWhiteSpace(busca))
            q = q.Where(u => u.Nome.Contains(busca) || u.Email.Contains(busca));
        return await q.OrderBy(u => u.Nome).ToListAsync();
    }

    public async Task<Usuario?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.EmpresaId == empresaId);

    public async Task<Usuario?> ObterPorIdAsync(int id)
        => await ctx.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

    public async Task CriarAsync(Usuario usuario, string senha)
    {
        if (await ctx.Usuarios.AnyAsync(u => u.Email == usuario.Email.ToLower().Trim()))
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");

        usuario.Email     = usuario.Email.ToLower().Trim();
        usuario.SenhaHash = AuthService.HashSenha(senha);
        usuario.CriadoEm  = DateTime.UtcNow;
        ctx.Usuarios.Add(usuario);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(Usuario usuario, string? novaSenha)
    {
        var conflict = await ctx.Usuarios
            .AnyAsync(u => u.Email == usuario.Email.ToLower().Trim() && u.Id != usuario.Id);
        if (conflict)
            throw new InvalidOperationException("Já existe outro usuário com este e-mail.");

        usuario.Email = usuario.Email.ToLower().Trim();
        if (!string.IsNullOrWhiteSpace(novaSenha))
            usuario.SenhaHash = AuthService.HashSenha(novaSenha);

        ctx.Usuarios.Update(usuario);
        await ctx.SaveChangesAsync();
    }

    public async Task AlternarAtivoAsync(int id, int empresaId)
    {
        var u = await ObterPorIdAsync(id, empresaId)
            ?? throw new InvalidOperationException("Usuário não encontrado.");
        u.Ativo = !u.Ativo;
        await ctx.SaveChangesAsync();
    }
}
