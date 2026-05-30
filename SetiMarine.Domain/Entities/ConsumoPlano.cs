namespace SetiMarine.Domain.Entities;

public class ConsumoPlano
{
    public int Id { get; set; }
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }
    public int PlanoContratoServicoId { get; set; }
    public PlanoContratoServico? PlanoContratoServico { get; set; }
    public string Periodo { get; set; } = "";  // "2026-05"
    public int QuantidadeUsada { get; set; } = 0;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
