using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class PedidoUso
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int EmbarcacaoId { get; set; }
    public Embarcacao? Embarcacao { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public DateTime DataPrevista { get; set; }
    public string? Observacao { get; set; }
    public OrigemPedido Origem { get; set; } = OrigemPedido.Manual;
    public string? MensagemOriginal { get; set; }
    public StatusPedidoUso Status { get; set; } = StatusPedidoUso.Recebido;
    public int? MovimentacaoId { get; set; }
    public Movimentacao? Movimentacao { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
