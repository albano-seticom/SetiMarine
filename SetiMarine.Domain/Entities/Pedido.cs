using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Pedido
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }

    public TipoPedido Tipo { get; set; }
    public StatusPedido Status { get; set; } = StatusPedido.Aberto;

    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int? EmbarcacaoId { get; set; }
    public Embarcacao? Embarcacao { get; set; }

    public int? ResponsavelId { get; set; }
    public Usuario? Responsavel { get; set; }

    public string? Descricao { get; set; }
    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? PrevisaoConclusao { get; set; }
    public DateTime? ConcluidoEm { get; set; }

    public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
    public ICollection<PedidoServico> Servicos { get; set; } = new List<PedidoServico>();
}
