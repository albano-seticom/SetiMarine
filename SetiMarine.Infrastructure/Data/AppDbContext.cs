using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Empresa>               Empresas               { get; set; }
    public DbSet<Plano>                 Planos                 { get; set; }
    public DbSet<Usuario>               Usuarios               { get; set; }
    public DbSet<SessaoAtiva>           Sessoes                { get; set; }
    public DbSet<Cliente>               Clientes               { get; set; }
    public DbSet<Embarcacao>            Embarcacoes            { get; set; }
    public DbSet<Vaga>                  Vagas                  { get; set; }
    public DbSet<Movimentacao>          Movimentacoes          { get; set; }
    public DbSet<HistoricoMovimentacao> HistoricoMovimentacoes { get; set; }
    public DbSet<OrdemServico>          OrdensServico          { get; set; }
    public DbSet<Contrato>              Contratos              { get; set; }
    public DbSet<Configuracao>          Configuracoes          { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Empresa>().ToTable("sis_empresas");
        mb.Entity<Plano>().ToTable("cfg_planos");
        mb.Entity<Usuario>().ToTable("sis_usuarios");
        mb.Entity<SessaoAtiva>().ToTable("sis_sessoes");
        mb.Entity<Cliente>().ToTable("mar_clientes");
        mb.Entity<Embarcacao>().ToTable("mar_embarcacoes");
        mb.Entity<Vaga>().ToTable("mar_vagas");
        mb.Entity<Movimentacao>().ToTable("mar_movimentacoes");
        mb.Entity<HistoricoMovimentacao>().ToTable("mar_historico_movimentacoes");
        mb.Entity<OrdemServico>().ToTable("mar_ordens_servico");
        mb.Entity<Contrato>().ToTable("mar_contratos");
        mb.Entity<Configuracao>().ToTable("cfg_configuracoes");

        mb.Entity<Usuario>()
            .HasIndex(u => new { u.EmpresaId, u.Email })
            .IsUnique();

        mb.Entity<Vaga>()
            .HasIndex(v => new { v.EmpresaId, v.Codigo })
            .IsUnique();

        mb.Entity<Embarcacao>()
            .HasOne(e => e.VagaFixa)
            .WithMany()
            .HasForeignKey(e => e.VagaFixaId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Vaga>()
            .HasOne(v => v.EmbarcacaoAtual)
            .WithMany()
            .HasForeignKey(v => v.EmbarcacaoAtualId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<Movimentacao>()
            .HasOne(m => m.Responsavel)
            .WithMany()
            .HasForeignKey(m => m.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
