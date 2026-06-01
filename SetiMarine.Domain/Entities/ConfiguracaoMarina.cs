namespace SetiMarine.Domain.Entities;

public class ConfiguracaoMarina
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public bool CaixaAutomatico { get; set; } = true;

    // Integração SetiFiscal
    public string? SetiFiscalBaseUrl { get; set; }
    public string? SetiFiscalApiKey { get; set; }
    public Guid? SetiFiscalEmpresaId { get; set; }
    public string? CodigoServicoDefault { get; set; }
    public string? CodigoNbsDefault { get; set; }

    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
