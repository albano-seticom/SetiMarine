using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class VendaProduto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }

    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public TipoTransacao Tipo { get; set; } = TipoTransacao.Venda;
    public int Quantidade { get; set; } = 1;
    public decimal PrecoUnitario { get; set; }
    public decimal ValorTotal => Quantidade * PrecoUnitario;

    public DateTime DataTransacao { get; set; } = DateTime.UtcNow;

    // campos de aluguel
    public DateTime? DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoReal { get; set; }
    public StatusAluguel? StatusAluguel { get; set; }

    public string? Observacao { get; set; }
}
