namespace SetiMarine.Domain.Entities;

public class Plano
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int LimiteEmbarcacoes { get; set; }
    public int LimiteUsuarios { get; set; }
    public bool Ativo { get; set; } = true;
    public bool Destaque { get; set; } = false;
    public string? LinkPagamento { get; set; }
    public string? Recursos { get; set; }  // uma linha por recurso
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
