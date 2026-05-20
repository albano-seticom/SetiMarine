using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Vaga
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public int? CorredorId { get; set; }
    public Corredor? Corredor { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? Setor { get; set; }
    public TipoVaga Tipo { get; set; }
    public StatusVaga Status { get; set; } = StatusVaga.Livre;
    public decimal TamanhoMaxPes { get; set; }
    public decimal? LarguraMaxPes { get; set; }
    public decimal ComprimentoMaxMetros { get; set; }
    public decimal? BocaMaxMetros { get; set; }
    public decimal PosX { get; set; } = 0;
    public decimal PosY { get; set; } = 0;
    public decimal Largura { get; set; } = 60;
    public decimal Altura { get; set; } = 40;
    public string? Observacao { get; set; }
    public bool Ativa { get; set; } = true;
    public int? EmbarcacaoAtualId { get; set; }
    public Embarcacao? EmbarcacaoAtual { get; set; }
    public ICollection<VagaEmbarcacao> EmbarcacoesNaVaga { get; set; } = new List<VagaEmbarcacao>();
}
