namespace SetiMarine.Domain.Entities;

public class ConfiguracaoMarina
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public bool CaixaAutomatico { get; set; } = true;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
