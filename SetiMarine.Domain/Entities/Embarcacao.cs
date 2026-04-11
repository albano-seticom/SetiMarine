using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Embarcacao
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoEmbarcacao Tipo { get; set; }
    public string? Fabricante { get; set; }
    public string? Modelo { get; set; }
    public int? AnoFabricacao { get; set; }
    public decimal ComprimentoMetros { get; set; }
    public decimal? BocaMetros { get; set; }
    public decimal? CaladoMetros { get; set; }
    public string? Cor { get; set; }
    public string? NumeroRegistro { get; set; }
    public string? FotoUrl { get; set; }
    public bool Ativa { get; set; } = true;
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public int? VagaFixaId { get; set; }
    public Vaga? VagaFixa { get; set; }
}
