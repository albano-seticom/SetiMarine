using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class RegistroServico
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int EmbarcacaoId { get; set; }
    public Embarcacao? Embarcacao { get; set; }
    public int? VagaId { get; set; }
    public Vaga? Vaga { get; set; }
    public int? TipoServicoConfigId { get; set; }
    public TipoServicoConfig? TipoServicoConfig { get; set; }
    public int? MovimentacaoId { get; set; }
    public Movimentacao? Movimentacao { get; set; }
    public TipoServico Tipo { get; set; }
    public StatusServico Status { get; set; } = StatusServico.Pendente;
    public int? ResponsavelId { get; set; }
    public Usuario? Responsavel { get; set; }
    public string? Observacao { get; set; }
    public DateTime AgendadoPara { get; set; }
    public DateTime? IniciadoEm { get; set; }
    public DateTime? FinalizadoEm { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<RegistroServicoItem> Itens { get; set; } = new List<RegistroServicoItem>();
    public ICollection<FotoServico> Fotos { get; set; } = new List<FotoServico>();
}
