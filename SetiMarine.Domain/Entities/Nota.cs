using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Nota
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }

    public int? PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int Numero { get; set; }
    public string Serie { get; set; } = "1";
    public string? ChaveNFe { get; set; }
    public TipoNota   Tipo   { get; set; } = TipoNota.NFe;
    public StatusNota Status { get; set; } = StatusNota.Rascunho;

    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public DateTime? DataSaida { get; set; }

    public decimal ValorProdutos { get; set; }
    public decimal ValorICMS { get; set; }
    public decimal ValorIPI { get; set; }
    public decimal ValorPIS { get; set; }
    public decimal ValorCOFINS { get; set; }
    public decimal ValorISS { get; set; }
    public decimal ValorTotal { get; set; }

    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<NotaItem> Itens { get; set; } = new List<NotaItem>();
}
