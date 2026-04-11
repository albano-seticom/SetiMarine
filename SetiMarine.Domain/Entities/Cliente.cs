namespace SetiMarine.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Email { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string? Endereco { get; set; }
    public bool Ativo { get; set; } = true;
    public string? SenhaHash { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<Embarcacao> Embarcacoes { get; set; } = new List<Embarcacao>();
}
