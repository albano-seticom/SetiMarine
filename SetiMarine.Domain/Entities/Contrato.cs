using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Contrato
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int EmbarcacaoId { get; set; }
    public Embarcacao? Embarcacao { get; set; }
    public int VagaId { get; set; }
    public Vaga? Vaga { get; set; }
    public TipoContrato Tipo { get; set; }
    public decimal ValorMensal { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime? Fim { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
