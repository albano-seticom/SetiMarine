using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class RegistroServicoItem
{
    public int Id { get; set; }
    public int RegistroServicoId { get; set; }
    public RegistroServico? RegistroServico { get; set; }
    public int? ChecklistTemplateItemId { get; set; }
    public ChecklistTemplateItem? ChecklistTemplateItem { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public StatusItemChecklist Status { get; set; } = StatusItemChecklist.Pendente;
    public string? Observacao { get; set; }
}
