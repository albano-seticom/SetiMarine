namespace SetiMarine.Domain.Entities;

public class Empresa
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public int? PlanoId { get; set; }
    public Plano? Plano { get; set; }
    public DateTime? PlanoVenceEm { get; set; }
    public bool Bloqueada { get; set; } = false;
    public string? MotivoBloqueio { get; set; }
    public string? LogoUrl { get; set; }
    public string? EnderecoCompleto { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? InstanciaWhatsApp { get; set; }
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public ICollection<Vaga> Vagas { get; set; } = new List<Vaga>();
}
