using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SetiMarine.Application.Services;

public class AuthService
{
    private readonly ISetiMarineDbContext _context;
    private readonly SeederService _seeder;

    public AuthService(ISetiMarineDbContext context, SeederService seeder)
    {
        _context = context;
        _seeder  = seeder;
    }

    public async Task<(ClaimsPrincipal? principal, string? erro)> LoginAsync(string email, string senha)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Empresa)
            .FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());

        if (usuario == null)
            return (null, "E-mail ou senha invalidos.");

        if (!usuario.Ativo)
            return (null, "Usuario inativo. Entre em contato com o administrador.");

        if (!VerificarSenha(senha, usuario.SenhaHash))
            return (null, "E-mail ou senha invalidos.");

        var sessao = new SessaoAtiva
        {
            UsuarioId  = usuario.Id,
            EmpresaId  = usuario.EmpresaId,
            IniciadaEm = DateTime.UtcNow,
            ExpiraEm   = DateTime.UtcNow.AddHours(12),
            Token      = Guid.NewGuid().ToString()
        };
        _context.SessoesAtivas.Add(sessao);
        await _context.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name,           usuario.Nome),
            new(ClaimTypes.Email,          usuario.Email),
            new("EmpresaId",               usuario.EmpresaId?.ToString() ?? ""),
            new("EmpresaNome",             usuario.Empresa?.NomeFantasia ?? usuario.Empresa?.RazaoSocial ?? "SETICOM"),
            new(ClaimTypes.Role,           usuario.Perfil.ToString()),
            new("SessaoToken",             sessao.Token),
        };

        var identity  = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);
        return (principal, null);
    }

    public async Task<(bool sucesso, string? erro)> CadastrarEmpresaAsync(
        string razaoSocial, string? nomeFantasia, string cnpj, string? telefone, string emailEmpresa,
        int planoId,
        string nomeAdmin, string emailAdmin, string senhaAdmin)
    {
        if (await _context.Empresas.AnyAsync(e => e.Cnpj == cnpj))
            return (false, "Ja existe uma empresa cadastrada com este CNPJ.");

        if (await _context.Usuarios.AnyAsync(u => u.Email == emailAdmin.ToLower().Trim()))
            return (false, "Ja existe um usuario com este e-mail.");

        var empresa = new Empresa
        {
            RazaoSocial  = razaoSocial.Trim(),
            NomeFantasia = nomeFantasia?.Trim(),
            Cnpj         = cnpj.Trim(),
            Telefone     = telefone?.Trim(),
            Email        = emailEmpresa.ToLower().Trim(),
            PlanoId      = planoId,
            Ativa        = true,
            CriadaEm     = DateTime.UtcNow
        };
        _context.Empresas.Add(empresa);
        await _context.SaveChangesAsync();

        var admin = new Usuario
        {
            EmpresaId  = (int?)empresa.Id,
            Nome       = nomeAdmin.Trim(),
            Email      = emailAdmin.ToLower().Trim(),
            SenhaHash  = HashSenha(senhaAdmin),
            Perfil     = PerfilUsuario.Admin,
            Ativo      = true,
            CriadoEm   = DateTime.UtcNow
        };
        _context.Usuarios.Add(admin);
        await _context.SaveChangesAsync();

        await _seeder.SeedEmpresaAsync(empresa.Id);

        return (true, null);
    }

    public static string HashSenha(string senha)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(senha + "SetiMarine_Salt_2026"));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerificarSenha(string senha, string hash)
        => HashSenha(senha) == hash;
}
