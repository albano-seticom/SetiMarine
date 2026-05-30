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

    public int?      HorasAluguel          { get; set; }
    public DateTime? DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoReal     { get; set; }
    public StatusAluguel? StatusEmprestimo { get; set; }

    public string? Observacao { get; set; }

    // Snapshot fiscal no momento do pedido
    public decimal? PrecoCusto      { get; set; }
    public decimal? AliquotaICMS   { get; set; }
    public decimal? AliquotaIPI    { get; set; }
    public decimal? AliquotaPIS    { get; set; }
    public decimal? AliquotaCOFINS { get; set; }
    public decimal? AliquotaISS    { get; set; }
    public string?  CstIcms        { get; set; }
    public string?  Cfop           { get; set; }
    public string?  Ncm            { get; set; }
}
