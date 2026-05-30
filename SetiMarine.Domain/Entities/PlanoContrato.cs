namespace SetiMarine.Domain.Entities;

public class PlanoContrato
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal? Mensalidade { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public List<PlanoContratoServico> Servicos { get; set; } = new();
}
