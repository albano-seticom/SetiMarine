namespace SetiMarine.Domain.Entities;

public class VagaEmbarcacao
{
    public int Id { get; set; }
    public int VagaId { get; set; }
    public Vaga? Vaga { get; set; }
    public int EmbarcacaoId { get; set; }
    public Embarcacao? Embarcacao { get; set; }
    public DateTime EntradaEm { get; set; } = DateTime.UtcNow;
    public DateTime? SaidaEm { get; set; }
    public bool Ativa { get; set; } = true;
}
