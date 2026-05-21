using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class PedidoService(ISetiMarineDbContext ctx)
{
    public async Task<List<Pedido>> ListarPorEmpresaAsync(
        int empresaId, TipoPedido? tipo = null, StatusPedido? status = null)
    {
        var q = ctx.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Embarcacao)
            .Include(p => p.Responsavel)
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .Include(p => p.Servicos)
            .Where(p => p.EmpresaId == empresaId);

        if (tipo.HasValue)
            q = q.Where(p => p.Tipo == tipo.Value);
        if (status.HasValue)
            q = q.Where(p => p.Status == status.Value);

        return await q.OrderByDescending(p => p.CriadoEm).ToListAsync();
    }

    public async Task<Pedido?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Embarcacao)
            .Include(p => p.Responsavel)
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .Include(p => p.Servicos)
            .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId);

    public async Task CriarAsync(Pedido pedido, List<PedidoItem> itens, List<PedidoServico> servicos)
    {
        pedido.CriadoEm = DateTime.UtcNow;
        pedido.Status   = StatusPedido.Aberto;
        pedido.Itens    = itens;
        pedido.Servicos = servicos;
        ctx.Pedidos.Add(pedido);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(Pedido pedido, List<PedidoItem> itens, List<PedidoServico> servicos)
    {
        var existente = await ctx.Pedidos
            .Include(p => p.Itens)
            .Include(p => p.Servicos)
            .FirstOrDefaultAsync(p => p.Id == pedido.Id && p.EmpresaId == pedido.EmpresaId)
            ?? throw new InvalidOperationException("Pedido não encontrado.");

        ctx.PedidoItens.RemoveRange(existente.Itens);
        ctx.PedidoServicos.RemoveRange(existente.Servicos);

        existente.Tipo             = pedido.Tipo;
        existente.Status           = pedido.Status;
        existente.ClienteId        = pedido.ClienteId;
        existente.EmbarcacaoId     = pedido.EmbarcacaoId;
        existente.ResponsavelId    = pedido.ResponsavelId;
        existente.Descricao        = pedido.Descricao;
        existente.Observacoes      = pedido.Observacoes;
        existente.PrevisaoConclusao = pedido.PrevisaoConclusao;

        if (pedido.Status == StatusPedido.Concluido && existente.ConcluidoEm == null)
            existente.ConcluidoEm = DateTime.UtcNow;

        existente.Itens    = itens;
        existente.Servicos = servicos;

        await ctx.SaveChangesAsync();
    }

    public async Task AvancarStatusAsync(int id, int empresaId)
    {
        var p = await ctx.Pedidos.FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Pedido não encontrado.");

        p.Status = p.Status switch
        {
            StatusPedido.Rascunho    => StatusPedido.Aberto,
            StatusPedido.Aberto      => StatusPedido.EmAndamento,
            StatusPedido.EmAndamento => StatusPedido.Concluido,
            _                        => p.Status
        };

        if (p.Status == StatusPedido.Concluido)
            p.ConcluidoEm = DateTime.UtcNow;

        await ctx.SaveChangesAsync();
    }

    public async Task<List<Cliente>> ListarClientesAsync(int empresaId)
        => await ctx.Clientes
            .Where(c => c.EmpresaId == empresaId && c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();

    public async Task<List<Embarcacao>> ListarEmbarcacoesAsync(int empresaId)
        => await ctx.Embarcacoes
            .Include(e => e.Cliente)
            .Where(e => e.EmpresaId == empresaId && e.Ativa)
            .OrderBy(e => e.Nome)
            .ToListAsync();

    public async Task<List<Produto>> ListarProdutosAsync(int empresaId)
        => await ctx.Produtos
            .Where(p => p.EmpresaId == empresaId && p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();

    public async Task<List<Usuario>> ListarColaboradoresAsync(int empresaId)
        => await ctx.Usuarios
            .Where(u => u.EmpresaId == empresaId && u.Ativo)
            .OrderBy(u => u.Nome)
            .ToListAsync();

    public async Task<List<TipoServicoConfig>> ListarTiposServicoAsync(int empresaId)
        => await ctx.TiposServicoConfig
            .Where(s => s.EmpresaId == empresaId && s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync();
}
