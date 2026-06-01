namespace SetiMarine.Domain.Entities;

public class NotaItem
{
    public int Id { get; set; }
    public int NotaId { get; set; }
    public Nota? Nota { get; set; }

    public int? ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }

    public string? Cfop { get; set; }
    public string? Ncm { get; set; }
    public string? CstIcms { get; set; }

    public decimal AliquotaICMS { get; set; }
    public decimal ValorICMS { get; set; }
    public decimal AliquotaIPI { get; set; }
    public decimal ValorIPI { get; set; }
    public decimal AliquotaPIS { get; set; }
    public decimal ValorPIS { get; set; }
    public decimal AliquotaCOFINS { get; set; }
    public decimal ValorCOFINS { get; set; }
    public decimal AliquotaISS { get; set; }
    public decimal ValorISS { get; set; }

    // Códigos para NFS-e
    public string? CodigoServico { get; set; }
    public string? CodigoNbs { get; set; }
}
