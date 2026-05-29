namespace SetiMarine.Domain.Entities;

public class PedidoServico
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public decimal? Valor { get; set; }
    public bool Concluido { get; set; } = false;

    public int? ResponsavelId { get; set; }
    public Usuario? Responsavel { get; set; }
}
