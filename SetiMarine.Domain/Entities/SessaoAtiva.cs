namespace SetiMarine.Domain.Entities;

public class SessaoAtiva
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int EmpresaId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime IniciadaEm { get; set; } = DateTime.UtcNow;
    public DateTime ExpiraEm { get; set; } = DateTime.UtcNow.AddHours(12);
    public DateTime UltimaAtividadeEm { get; set; } = DateTime.UtcNow;
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
}
