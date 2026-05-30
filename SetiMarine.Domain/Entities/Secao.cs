using SetiMarine.Domain.Enums;

namespace SetiMarine.Domain.Entities;

public class Secao
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoVaga? TipoPreferencial { get; set; }
    public int Ordem { get; set; } = 0;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public ICollection<Corredor> Corredores { get; set; } = new List<Corredor>();
}
