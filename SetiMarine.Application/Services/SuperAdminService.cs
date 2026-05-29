using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class SuperAdminService(ISetiMarineDbContext ctx)
{
    // ── Empresas ─────────────────────────────────────────────────

    public async Task<List<Empresa>> ListarEmpresasAsync()
        => await ctx.Empresas
            .Include(e => e.Plano)
            .Include(e => e.Usuarios)
            .OrderBy(e => e.RazaoSocial)
            .ToListAsync();

    public async Task SalvarEmpresaAsync(Empresa empresa)
    {
        if (empresa.Id == 0)
        {
            empresa.CriadaEm = DateTime.UtcNow;
            ctx.Empresas.Add(empresa);
        }
        else
        {
            var ex = await ctx.Empresas.FindAsync(empresa.Id)
                ?? throw new InvalidOperationException("Empresa nao encontrada.");
            ex.RazaoSocial    = empresa.RazaoSocial;
            ex.NomeFantasia   = empresa.NomeFantasia;
            ex.Cnpj           = empresa.Cnpj;
            ex.Email          = empresa.Email;
            ex.Telefone       = empresa.Telefone;
            ex.PlanoId        = empresa.PlanoId;
            ex.PlanoVenceEm   = empresa.PlanoVenceEm;
            ex.Ativa          = empresa.Ativa;
            ex.Bloqueada      = empresa.Bloqueada;
            ex.MotivoBloqueio = empresa.MotivoBloqueio;
            ex.Cidade         = empresa.Cidade;
            ex.Estado         = empresa.Estado;
        }
        await ctx.SaveChangesAsync();
    }

    public async Task CriarEmpresaComAdminAsync(
        Empresa empresa,
        string nomeAdmin, string emailAdmin, string senhaAdmin)
    {
        if (await ctx.Empresas.AnyAsync(e => e.Cnpj == empresa.Cnpj))
            throw new InvalidOperationException("Ja existe uma empresa com este CNPJ.");
        if (await ctx.Usuarios.AnyAsync(u => u.Email == emailAdmin.ToLower().Trim()))
            throw new InvalidOperationException("Ja existe um usuario com este e-mail.");

        empresa.CriadaEm = DateTime.UtcNow;
        ctx.Empresas.Add(empresa);
        await ctx.SaveChangesAsync();

        ctx.Usuarios.Add(new Usuario
        {
            EmpresaId = empresa.Id,
            Nome      = nomeAdmin.Trim(),
            Email     = emailAdmin.ToLower().Trim(),
            SenhaHash = AuthService.HashSenha(senhaAdmin),
            Perfil    = PerfilUsuario.Admin,
            Ativo     = true,
            CriadoEm  = DateTime.UtcNow,
        });
        await ctx.SaveChangesAsync();
    }

    public async Task ToggleEmpresaAtivaAsync(int id)
    {
        var e = await ctx.Empresas.FindAsync(id)
            ?? throw new InvalidOperationException("Empresa nao encontrada.");
        e.Ativa = !e.Ativa;
        await ctx.SaveChangesAsync();
    }

    // ── Planos ───────────────────────────────────────────────────

    public async Task<List<Plano>> ListarPlanosAsync()
        => await ctx.Planos.OrderBy(p => p.Ordem).ThenBy(p => p.Valor).ToListAsync();

    public async Task SalvarPlanoAsync(Plano plano)
    {
        if (plano.Id == 0)
        {
            plano.CriadoEm = DateTime.UtcNow;
            ctx.Planos.Add(plano);
        }
        else
        {
            var ex = await ctx.Planos.FindAsync(plano.Id)
                ?? throw new InvalidOperationException("Plano nao encontrado.");
            ex.Nome              = plano.Nome;
            ex.Descricao         = plano.Descricao;
            ex.Valor             = plano.Valor;
            ex.LimiteEmbarcacoes = plano.LimiteEmbarcacoes;
            ex.LimiteUsuarios    = plano.LimiteUsuarios;
            ex.Ativo             = plano.Ativo;
            ex.Destaque          = plano.Destaque;
            ex.Ordem             = plano.Ordem;
            ex.TextoBotao        = plano.TextoBotao;
            ex.LinkPagamento     = plano.LinkPagamento;
            ex.Recursos          = plano.Recursos;
        }
        await ctx.SaveChangesAsync();
    }

    public async Task TogglePlanoAtivoAsync(int id)
    {
        var p = await ctx.Planos.FindAsync(id)
            ?? throw new InvalidOperationException("Plano nao encontrado.");
        p.Ativo = !p.Ativo;
        await ctx.SaveChangesAsync();
    }

    // ── Usuarios ─────────────────────────────────────────────────

    public async Task<List<Usuario>> ListarUsuariosAsync()
        => await ctx.Usuarios
            .Include(u => u.Empresa)
            .Where(u => u.Perfil != PerfilUsuario.SuperAdmin)
            .OrderBy(u => u.Empresa!.RazaoSocial).ThenBy(u => u.Nome)
            .ToListAsync();

    public async Task SalvarUsuarioAsync(Usuario usuario, string? novaSenha)
    {
        if (usuario.Id == 0)
        {
            if (await ctx.Usuarios.AnyAsync(u => u.Email == usuario.Email.ToLower().Trim()))
                throw new InvalidOperationException("Ja existe um usuario com este e-mail.");
            usuario.Email     = usuario.Email.ToLower().Trim();
            usuario.SenhaHash = AuthService.HashSenha(novaSenha!);
            usuario.CriadoEm  = DateTime.UtcNow;
            ctx.Usuarios.Add(usuario);
        }
        else
        {
            var ex = await ctx.Usuarios.FindAsync(usuario.Id)
                ?? throw new InvalidOperationException("Usuario nao encontrado.");
            ex.Nome      = usuario.Nome;
            ex.Email     = usuario.Email.ToLower().Trim();
            ex.Perfil    = usuario.Perfil;
            ex.EmpresaId = usuario.EmpresaId;
            ex.Ativo     = usuario.Ativo;
            if (!string.IsNullOrWhiteSpace(novaSenha))
                ex.SenhaHash = AuthService.HashSenha(novaSenha);
        }
        await ctx.SaveChangesAsync();
    }

    public async Task ToggleUsuarioAtivoAsync(int id)
    {
        var u = await ctx.Usuarios.FindAsync(id)
            ?? throw new InvalidOperationException("Usuario nao encontrado.");
        u.Ativo = !u.Ativo;
        await ctx.SaveChangesAsync();
    }

    // ── Stats ────────────────────────────────────────────────────

    public async Task<(int empresas, int usuarios, int planos, int embarcacoes)> ObterStatsAsync()
    {
        var empresas    = await ctx.Empresas.CountAsync(e => e.Ativa);
        var usuarios    = await ctx.Usuarios.CountAsync(u => u.Ativo && u.Perfil != PerfilUsuario.SuperAdmin);
        var planos      = await ctx.Planos.CountAsync(p => p.Ativo);
        var embarcacoes = await ctx.Embarcacoes.CountAsync();
        return (empresas, usuarios, planos, embarcacoes);
    }
}
