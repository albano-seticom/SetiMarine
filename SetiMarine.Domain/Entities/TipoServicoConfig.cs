using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class TipoServicoConfig
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoServico Tipo { get; set; }
    public int FrequenciaDias { get; set; } = 15;
    public decimal? ValorPadrao { get; set; }
    public decimal? DuracaoHoras { get; set; }
    public int? ChecklistTemplateId { get; set; }
    public ChecklistTemplate? ChecklistTemplate { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
