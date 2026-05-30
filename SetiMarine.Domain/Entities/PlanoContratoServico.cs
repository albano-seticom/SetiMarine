namespace SetiMarine.Domain.Entities;

public class PlanoContratoServico
{
    public int Id { get; set; }
    public int PlanoContratoId { get; set; }
    public PlanoContrato? PlanoContrato { get; set; }
    public int TipoServicoConfigId { get; set; }
    public TipoServicoConfig? TipoServico { get; set; }
    public int Quantidade { get; set; } = 1;
    public string? Observacao { get; set; }
}
