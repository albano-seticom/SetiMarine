using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class AgendamentoUso
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int? EmbarcacaoId { get; set; }
    public Embarcacao? Embarcacao { get; set; }
    public DateTime DataHoraUso { get; set; }
    public DateTime? DataHoraPreparacao { get; set; }
    public int MinutosAntecedencia { get; set; } = 60;
    public TipoUsoEmbarcacao TipoUso { get; set; } = TipoUsoEmbarcacao.DescidaNaAgua;
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Pendente;
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
