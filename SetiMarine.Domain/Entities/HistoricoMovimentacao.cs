using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class HistoricoMovimentacao
{
    public int Id { get; set; }
    public int MovimentacaoId { get; set; }
    public Movimentacao? Movimentacao { get; set; }
    public StatusMovimentacao StatusAnterior { get; set; }
    public StatusMovimentacao StatusNovo { get; set; }
    public int? UsuarioId { get; set; }
    public string? Observacao { get; set; }
    public DateTime Em { get; set; } = DateTime.UtcNow;
}
