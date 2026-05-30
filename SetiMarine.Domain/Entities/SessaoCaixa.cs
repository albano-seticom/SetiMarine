namespace SetiMarine.Domain.Entities;

public class SessaoCaixa
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int? OperadorId { get; set; }
    public Usuario? Operador { get; set; }
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public decimal ValorAbertura { get; set; } = 0;
    public DateTime? DataFechamento { get; set; }
    public decimal? ValorFechamentoContado { get; set; }
    public decimal? ValorFechamentoEsperado { get; set; }
    public decimal? Diferenca { get; set; }
    public string? ObservacoesFechamento { get; set; }
    public bool Aberto { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<MovimentoCaixa> Movimentos { get; set; } = new List<MovimentoCaixa>();
}
