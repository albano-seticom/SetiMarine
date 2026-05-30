using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Produto
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public CategoriaProduto Categoria { get; set; } = CategoriaProduto.Outro;
    public string Unidade { get; set; } = "un";

    public decimal? PrecoVenda      { get; set; }
    public decimal? PrecoAluguelHora { get; set; }
    public decimal? PrecoAluguelDia  { get; set; }

    public bool ControlaEstoque { get; set; } = false;
    public int Estoque { get; set; } = 0;
    public int EstoqueMinimo { get; set; } = 0;

    public bool Alugavel { get; set; } = false;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<VendaProduto> Transacoes { get; set; } = new List<VendaProduto>();
}
