using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class MovimentoCaixa
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int SessaoCaixaId { get; set; }
    public SessaoCaixa? SessaoCaixa { get; set; }
    public TipoMovimentoCaixa Tipo { get; set; }
    public OrigemMovimentoCaixa Origem { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int? PedidoId { get; set; }
    public Pedido? Pedido { get; set; }
    public int? VendaProdutoId { get; set; }
    public VendaProduto? VendaProduto { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
