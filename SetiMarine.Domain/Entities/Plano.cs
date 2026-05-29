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
    public int Ordem { get; set; } = 0;
    public string? TextoBotao { get; set; }
    public string? LinkPagamento { get; set; }
    public string? Recursos { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
