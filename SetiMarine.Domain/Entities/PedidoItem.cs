using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class PedidoItem
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public TipoItemPedido Tipo { get; set; } = TipoItemPedido.Venda;

    public int Quantidade { get; set; } = 1;
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal => Quantidade * PrecoUnitario;

    public DateTime? DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoReal { get; set; }
    public StatusAluguel? StatusEmprestimo { get; set; }

    public string? Observacao { get; set; }
}
