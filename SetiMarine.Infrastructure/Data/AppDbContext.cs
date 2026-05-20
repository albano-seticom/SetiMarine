using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Data;

namespace SetiMarine.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), ISetiMarineDbContext
{
    // ============================================================
    // SISTEMA
    // ============================================================
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Plano> Planos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<SessaoAtiva> SessoesAtivas { get; set; }
    public DbSet<Configuracao> Configuracoes { get; set; }

    // ============================================================
    // MARINA - Clientes e Embarcacoes
    // ============================================================
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Embarcacao> Embarcacoes { get; set; }
    public DbSet<Contrato> Contratos { get; set; }

    // ============================================================
    // MARINA - Vagas e Corredores
    // ============================================================
    public DbSet<Corredor> Corredores { get; set; }
    public DbSet<Vaga> Vagas { get; set; }
    public DbSet<VagaEmbarcacao> VagaEmbarcacoes { get; set; }

    // ============================================================
    // MARINA - Movimentacoes
    // ============================================================
    public DbSet<Movimentacao> Movimentacoes { get; set; }
    public DbSet<HistoricoMovimentacao> HistoricoMovimentacoes { get; set; }
    public DbSet<PedidoUso> PedidosUso { get; set; }

    // ============================================================
    // MARINA - Servicos e Checklists
    // ============================================================
    public DbSet<TipoServicoConfig> TiposServicoConfig { get; set; }
    public DbSet<ChecklistTemplate> ChecklistTemplates { get; set; }
    public DbSet<ChecklistTemplateItem> ChecklistTemplateItens { get; set; }
    public DbSet<RegistroServico> RegistrosServico { get; set; }
    public DbSet<RegistroServicoItem> RegistroServicoItens { get; set; }
    public DbSet<FotoServico> FotosServico { get; set; }

    // ============================================================
    // MARINA - Ordens de Servico
    // ============================================================
    public DbSet<OrdemServico> OrdensServico { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // ============================================================
        // Prefixos de tabela
        // ============================================================
        mb.Entity<Empresa>().ToTable("sis_empresas");
        mb.Entity<Plano>().ToTable("cfg_planos");
        mb.Entity<Usuario>().ToTable("sis_usuarios");
        mb.Entity<SessaoAtiva>().ToTable("sis_sessoes");
        mb.Entity<Configuracao>().ToTable("cfg_configuracoes");

        mb.Entity<Cliente>().ToTable("mar_clientes");
        mb.Entity<Embarcacao>().ToTable("mar_embarcacoes");
        mb.Entity<Contrato>().ToTable("mar_contratos");

        mb.Entity<Corredor>().ToTable("mar_corredores");
        mb.Entity<Vaga>().ToTable("mar_vagas");
        mb.Entity<VagaEmbarcacao>().ToTable("mar_vaga_embarcacoes");

        mb.Entity<Movimentacao>().ToTable("mar_movimentacoes");
        mb.Entity<HistoricoMovimentacao>().ToTable("mar_historico_movimentacoes");
        mb.Entity<PedidoUso>().ToTable("mar_pedidos_uso");

        mb.Entity<TipoServicoConfig>().ToTable("mar_tipos_servico_config");
        mb.Entity<ChecklistTemplate>().ToTable("mar_checklist_templates");
        mb.Entity<ChecklistTemplateItem>().ToTable("mar_checklist_template_itens");
        mb.Entity<RegistroServico>().ToTable("mar_registros_servico");
        mb.Entity<RegistroServicoItem>().ToTable("mar_registro_servico_itens");
        mb.Entity<FotoServico>().ToTable("mar_fotos_servico");

        mb.Entity<OrdemServico>().ToTable("mar_ordens_servico");

        // ============================================================
        // Indices unicos
        // ============================================================
        mb.Entity<Usuario>()
            .HasIndex(u => new { u.EmpresaId, u.Email })
            .IsUnique();

        mb.Entity<Vaga>()
            .HasIndex(v => new { v.EmpresaId, v.Codigo })
            .IsUnique();

        // ============================================================
        // Relacionamentos - Embarcacao/Vaga
        // ============================================================
        mb.Entity<Embarcacao>()
            .HasOne(e => e.VagaFixa)
            .WithOne(v => v.EmbarcacaoAtual)
            .HasForeignKey<Embarcacao>(e => e.VagaFixaId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<Vaga>()
            .HasOne(v => v.Corredor)
            .WithMany(c => c.Vagas)
            .HasForeignKey(v => v.CorredorId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<VagaEmbarcacao>()
            .HasOne(ve => ve.Vaga)
            .WithMany(v => v.EmbarcacoesNaVaga)
            .HasForeignKey(ve => ve.VagaId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<VagaEmbarcacao>()
            .HasOne(ve => ve.Embarcacao)
            .WithMany()
            .HasForeignKey(ve => ve.EmbarcacaoId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============================================================
        // Relacionamentos - RegistroServico
        // ============================================================
        mb.Entity<RegistroServico>()
            .HasMany(r => r.Itens)
            .WithOne(i => i.RegistroServico)
            .HasForeignKey(i => i.RegistroServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<RegistroServico>()
            .HasMany(r => r.Fotos)
            .WithOne(f => f.RegistroServico)
            .HasForeignKey(f => f.RegistroServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============================================================
        // Relacionamentos - ChecklistTemplate
        // ============================================================
        mb.Entity<ChecklistTemplate>()
            .HasMany(t => t.Itens)
            .WithOne(i => i.ChecklistTemplate)
            .HasForeignKey(i => i.ChecklistTemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
