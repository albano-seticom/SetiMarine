using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Movimentacao
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int EmbarcacaoId { get; set; }
    public Embarcacao? Embarcacao { get; set; }
    public int VagaId { get; set; }
    public Vaga? Vaga { get; set; }
    public StatusMovimentacao Status { get; set; } = StatusMovimentacao.Agendado;
    public int? ResponsavelId { get; set; }
    public Usuario? Responsavel { get; set; }
    public DateTime AgendadoPara { get; set; }
    public string? Observacao { get; set; }
    public string? OrigemWhatsApp { get; set; }
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public ICollection<HistoricoMovimentacao> Historico { get; set; } = new List<HistoricoMovimentacao>();
}
