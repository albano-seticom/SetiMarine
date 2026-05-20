namespace SetiMarine.Domain.Entities;

public class Corredor
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal TamanhoMaxPes { get; set; }
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<Vaga> Vagas { get; set; } = new List<Vaga>();
}
