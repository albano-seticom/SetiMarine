using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class OrdemServico
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int EmbarcacaoId { get; set; }
    public Embarcacao? Embarcacao { get; set; }
    public string TipoServico { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusOS Status { get; set; } = StatusOS.Pendente;
    public int? ResponsavelId { get; set; }
    public Usuario? Responsavel { get; set; }
    public decimal? ValorEstimado { get; set; }
    public decimal? ValorFinal { get; set; }
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public DateTime? FinalizadaEm { get; set; }
}
