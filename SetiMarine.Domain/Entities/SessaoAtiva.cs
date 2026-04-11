namespace SetiMarine.Domain.Entities;

public class SessaoAtiva
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public DateTime UltimaAtividadeEm { get; set; } = DateTime.UtcNow;
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
}
