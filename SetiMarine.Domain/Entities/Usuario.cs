using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public int? EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; } = true;
    public bool TemaEscuro { get; set; } = false;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
