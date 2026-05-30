using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Domain.Data
{
    public interface ISetiMarineDbContext
    {
        DbSet<Empresa> Empresas { get; set; }
        DbSet<Plano> Planos { get; set; }
        DbSet<Usuario> Usuarios { get; set; }
        DbSet<SessaoAtiva> SessoesAtivas { get; set; }
        DbSet<Configuracao> Configuracoes { get; set; }
        DbSet<Cliente> Clientes { get; set; }
        DbSet<Embarcacao> Embarcacoes { get; set; }
        DbSet<Contrato> Contratos { get; set; }
        DbSet<Corredor> Corredores { get; set; }
        DbSet<Vaga> Vagas { get; set; }
        DbSet<VagaEmbarcacao> VagaEmbarcacoes { get; set; }
        DbSet<Movimentacao> Movimentacoes { get; set; }
        DbSet<HistoricoMovimentacao> HistoricoMovimentacoes { get; set; }
        DbSet<PedidoUso> PedidosUso { get; set; }
        DbSet<TipoServicoConfig> TiposServicoConfig { get; set; }
        DbSet<ChecklistTemplate> ChecklistTemplates { get; set; }
        DbSet<ChecklistTemplateItem> ChecklistTemplateItens { get; set; }
        DbSet<RegistroServico> RegistrosServico { get; set; }
        DbSet<RegistroServicoItem> RegistroServicoItens { get; set; }
        DbSet<FotoServico> FotosServico { get; set; }
        DbSet<OrdemServico> OrdensServico { get; set; }
        DbSet<Produto> Produtos { get; set; }
        DbSet<VendaProduto> VendasProduto { get; set; }
        DbSet<Pedido> Pedidos { get; set; }
        DbSet<PedidoItem> PedidoItens { get; set; }
        DbSet<PedidoServico> PedidoServicos { get; set; }
        DbSet<PlanoContrato> PlanosContrato { get; set; }
        DbSet<PlanoContratoServico> PlanoContratoServicos { get; set; }
        DbSet<AgendamentoUso> AgendamentosUso { get; set; }
        DbSet<ConsumoPlano> ConsumoPlano { get; set; }
        DbSet<MovimentoEstoque> MovimentosEstoque { get; set; }
        DbSet<SessaoCaixa> SessoesCaixa { get; set; }
        DbSet<MovimentoCaixa> MovimentosCaixa { get; set; }
        DbSet<ConfiguracaoMarina> ConfiguracoesMarina { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
