namespace SetiMarine.Domain.Entities;

public class FotoServico
{
    public int Id { get; set; }
    public int RegistroServicoId { get; set; }
    public RegistroServico? RegistroServico { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool EhAvaria { get; set; } = false;
    public DateTime TiradaEm { get; set; } = DateTime.UtcNow;
    public int? UsuarioId { get; set; }
}
