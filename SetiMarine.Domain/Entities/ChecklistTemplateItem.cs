namespace SetiMarine.Domain.Entities;

public class ChecklistTemplateItem
{
    public int Id { get; set; }
    public int ChecklistTemplateId { get; set; }
    public ChecklistTemplate? ChecklistTemplate { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public bool Obrigatorio { get; set; } = true;
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
}
