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

    // ============================================================
    // MARINA - Produtos e Vendas
    // ============================================================
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<VendaProduto> VendasProduto { get; set; }

    // ============================================================
    // MARINA - Pedidos
    // ============================================================
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItens { get; set; }
    public DbSet<PedidoServico> PedidoServicos { get; set; }

    // ============================================================
    // MARINA - Planos de Contrato
    // ============================================================
    public DbSet<PlanoContrato> PlanosContrato { get; set; }
    public DbSet<PlanoContratoServico> PlanoContratoServicos { get; set; }

    // ============================================================
    // MARINA - Agenda de Uso
    // ============================================================
    public DbSet<AgendamentoUso> AgendamentosUso { get; set; }
    public DbSet<ConsumoPlano> ConsumoPlano { get; set; }

    // ============================================================
    // MARINA - Estoque
    // ============================================================
    public DbSet<MovimentoEstoque> MovimentosEstoque { get; set; }

    // ============================================================
    // MARINA - Caixa
    // ============================================================
    public DbSet<SessaoCaixa> SessoesCaixa { get; set; }
    public DbSet<MovimentoCaixa> MovimentosCaixa { get; set; }

    // ============================================================
    // MARINA - Configuração
    // ============================================================
    public DbSet<ConfiguracaoMarina> ConfiguracoesMarina { get; set; }

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

        mb.Entity<Produto>().ToTable("mar_produtos");
        mb.Entity<VendaProduto>().ToTable("mar_vendas_produto");

        mb.Entity<VendaProduto>()
            .Ignore(v => v.ValorTotal);

        mb.Entity<Pedido>().ToTable("mar_pedidos");
        mb.Entity<PedidoItem>().ToTable("mar_pedido_itens");
        mb.Entity<PedidoServico>().ToTable("mar_pedido_servicos");

        mb.Entity<PlanoContrato>().ToTable("cfg_planos_contrato");
        mb.Entity<PlanoContratoServico>().ToTable("cfg_plano_contrato_servicos");

        mb.Entity<AgendamentoUso>().ToTable("mar_agendamentos_uso");
        mb.Entity<ConsumoPlano>().ToTable("cfg_consumo_plano");
        mb.Entity<MovimentoEstoque>().ToTable("mar_movimentos_estoque");
        mb.Entity<SessaoCaixa>().ToTable("mar_sessoes_caixa");
        mb.Entity<MovimentoCaixa>().ToTable("mar_movimentos_caixa");
        mb.Entity<ConfiguracaoMarina>().ToTable("cfg_configuracao_marina");

        mb.Entity<ConfiguracaoMarina>()
            .HasIndex(c => c.EmpresaId)
            .IsUnique();

        mb.Entity<PedidoItem>().Ignore(i => i.ValorTotal);

        mb.Entity<PedidoServico>()
            .HasOne(s => s.Responsavel)
            .WithMany()
            .HasForeignKey(s => s.ResponsavelId)
            .OnDelete(DeleteBehavior.SetNull);

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

        // ============================================================
        // Relacionamentos - Pedido
        // ============================================================
        mb.Entity<Pedido>()
            .HasMany(p => p.Itens)
            .WithOne(i => i.Pedido)
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Pedido>()
            .HasMany(p => p.Servicos)
            .WithOne(s => s.Pedido)
            .HasForeignKey(s => s.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============================================================
        // Relacionamentos - PlanoContrato
        // ============================================================
        mb.Entity<PlanoContrato>()
            .HasMany(p => p.Servicos)
            .WithOne(s => s.PlanoContrato)
            .HasForeignKey(s => s.PlanoContratoId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<Contrato>()
            .HasOne(c => c.PlanoContrato)
            .WithMany()
            .HasForeignKey(c => c.PlanoContratoId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<PlanoContratoServico>()
            .HasOne(s => s.TipoServico)
            .WithMany()
            .HasForeignKey(s => s.TipoServicoConfigId)
            .OnDelete(DeleteBehavior.Restrict);

        // ============================================================
        // Relacionamentos - ConsumoPlano
        // ============================================================
        mb.Entity<ConsumoPlano>()
            .HasOne(cp => cp.Contrato)
            .WithMany()
            .HasForeignKey(cp => cp.ContratoId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<ConsumoPlano>()
            .HasOne(cp => cp.PlanoContratoServico)
            .WithMany()
            .HasForeignKey(cp => cp.PlanoContratoServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<ConsumoPlano>()
            .HasIndex(cp => new { cp.ContratoId, cp.PlanoContratoServicoId, cp.Periodo })
            .IsUnique();

        // ============================================================
        // Relacionamentos - AgendamentoUso (plano opcional)
        // ============================================================
        mb.Entity<AgendamentoUso>()
            .HasOne(a => a.Contrato)
            .WithMany()
            .HasForeignKey(a => a.ContratoId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<AgendamentoUso>()
            .HasOne(a => a.PlanoContratoServicoUsado)
            .WithMany()
            .HasForeignKey(a => a.PlanoContratoServicoId)
            .OnDelete(DeleteBehavior.SetNull);

        // ============================================================
        // Relacionamentos - MovimentoEstoque
        // ============================================================
        mb.Entity<MovimentoEstoque>()
            .HasOne(m => m.Produto)
            .WithMany()
            .HasForeignKey(m => m.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        mb.Entity<MovimentoEstoque>()
            .HasOne(m => m.Usuario)
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<MovimentoEstoque>()
            .HasOne(m => m.Pedido)
            .WithMany()
            .HasForeignKey(m => m.PedidoId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<MovimentoEstoque>()
            .HasOne(m => m.VendaProduto)
            .WithMany()
            .HasForeignKey(m => m.VendaProdutoId)
            .OnDelete(DeleteBehavior.SetNull);

        // ============================================================
        // Relacionamentos - Caixa
        // ============================================================
        mb.Entity<SessaoCaixa>()
            .HasOne(s => s.Operador)
            .WithMany()
            .HasForeignKey(s => s.OperadorId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<MovimentoCaixa>()
            .HasOne(m => m.SessaoCaixa)
            .WithMany(s => s.Movimentos)
            .HasForeignKey(m => m.SessaoCaixaId)
            .OnDelete(DeleteBehavior.Cascade);

        mb.Entity<MovimentoCaixa>()
            .HasOne(m => m.Pedido)
            .WithMany()
            .HasForeignKey(m => m.PedidoId)
            .OnDelete(DeleteBehavior.SetNull);

        mb.Entity<MovimentoCaixa>()
            .HasOne(m => m.VendaProduto)
            .WithMany()
            .HasForeignKey(m => m.VendaProdutoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
