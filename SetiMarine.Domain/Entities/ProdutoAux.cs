namespace SetiMarine.Domain.Entities;

public class ProdutoAux
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public int EmpresaId { get; set; }

    // Preços
    public decimal? PrecoCusto { get; set; }
    public decimal? PrecoVenda { get; set; }
    public decimal? PrecoAluguelHora { get; set; }
    public decimal? PrecoAluguelDia { get; set; }
    public decimal? MargemLucro { get; set; }

    // Estoque
    public bool ControlaEstoque { get; set; } = false;
    public int Estoque { get; set; } = 0;
    public int EstoqueMinimo { get; set; } = 0;

    // Tributação
    public decimal AliquotaICMS { get; set; } = 0;
    public decimal AliquotaIPI { get; set; } = 0;
    public decimal AliquotaPIS { get; set; } = 0;
    public decimal AliquotaCOFINS { get; set; } = 0;
    public decimal AliquotaISS { get; set; } = 0;
    public string? CstIcms { get; set; }
    public string? CstPisCofins { get; set; }
    public string? Cfop { get; set; }
    public string? Ncm { get; set; }

    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
