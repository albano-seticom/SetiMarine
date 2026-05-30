using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class MovimentoEstoque
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public TipoMovimentoEstoque Tipo { get; set; }
    public int Quantidade { get; set; }
    public int EstoqueAntes { get; set; }
    public int EstoqueDepois { get; set; }
    public string? Motivo { get; set; }
    public int? PedidoId { get; set; }
    public Pedido? Pedido { get; set; }
    public int? VendaProdutoId { get; set; }
    public VendaProduto? VendaProduto { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
