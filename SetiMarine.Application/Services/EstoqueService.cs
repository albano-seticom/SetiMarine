using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class EstoqueService(ISetiMarineDbContext ctx)
{
    public async Task<List<Produto>> ListarProdutosComEstoqueAsync(int empresaId)
        => await ctx.Produtos
            .Include(p => p.Aux)
            .Where(p => p.EmpresaId == empresaId && p.Ativo && p.Aux != null && p.Aux.ControlaEstoque)
            .OrderBy(p => p.Nome)
            .ToListAsync();

    public async Task<List<Produto>> ListarTodosProdutosAsync(int empresaId)
        => await ctx.Produtos
            .Include(p => p.Aux)
            .Where(p => p.EmpresaId == empresaId && p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();

    public async Task<List<MovimentoEstoque>> ListarMovimentosAsync(int empresaId, int? produtoId = null, int take = 100)
    {
        var q = ctx.MovimentosEstoque
            .Include(m => m.Produto)
            .Include(m => m.Usuario)
            .Where(m => m.EmpresaId == empresaId);

        if (produtoId.HasValue)
            q = q.Where(m => m.ProdutoId == produtoId.Value);

        return await q.OrderByDescending(m => m.CriadoEm).Take(take).ToListAsync();
    }

    public async Task<MovimentoEstoque> RegistrarEntradaAsync(
        int empresaId, int produtoId, int quantidade, string? motivo, int? usuarioId)
        => await RegistrarAsync(empresaId, produtoId, quantidade, TipoMovimentoEstoque.Entrada, motivo, usuarioId);

    public async Task<MovimentoEstoque> RegistrarSaidaAsync(
        int empresaId, int produtoId, int quantidade, string? motivo, int? usuarioId,
        int? pedidoId = null, int? vendaProdutoId = null)
        => await RegistrarAsync(empresaId, produtoId, quantidade, TipoMovimentoEstoque.Saida, motivo, usuarioId, pedidoId, vendaProdutoId);

    public async Task<MovimentoEstoque> AjustarAsync(
        int empresaId, int produtoId, int novoEstoque, string? motivo, int? usuarioId)
    {
        var produto = await ctx.Produtos
            .Include(p => p.Aux)
            .FirstOrDefaultAsync(p => p.Id == produtoId && p.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        var aux = produto.Aux ?? throw new InvalidOperationException("Produto sem dados auxiliares.");

        if (!aux.ControlaEstoque)
            throw new InvalidOperationException("Este produto não tem controle de estoque ativo.");

        var antes       = aux.Estoque;
        var diff        = novoEstoque - antes;
        aux.Estoque     = novoEstoque;
        aux.AtualizadoEm = DateTime.UtcNow;

        var mov = new MovimentoEstoque
        {
            EmpresaId     = empresaId,
            ProdutoId     = produtoId,
            UsuarioId     = usuarioId,
            Tipo          = TipoMovimentoEstoque.Ajuste,
            Quantidade    = Math.Abs(diff),
            EstoqueAntes  = antes,
            EstoqueDepois = novoEstoque,
            Motivo        = motivo ?? "Ajuste manual",
            CriadoEm     = DateTime.UtcNow,
        };
        ctx.MovimentosEstoque.Add(mov);
        await ctx.SaveChangesAsync();
        return mov;
    }

    private async Task<MovimentoEstoque> RegistrarAsync(
        int empresaId, int produtoId, int quantidade, TipoMovimentoEstoque tipo,
        string? motivo, int? usuarioId, int? pedidoId = null, int? vendaProdutoId = null)
    {
        var produto = await ctx.Produtos
            .Include(p => p.Aux)
            .FirstOrDefaultAsync(p => p.Id == produtoId && p.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        var aux = produto.Aux;
        if (aux == null || !aux.ControlaEstoque) return null!;

        var antes = aux.Estoque;
        aux.Estoque = tipo == TipoMovimentoEstoque.Entrada
            ? antes + quantidade
            : antes - quantidade;
        aux.AtualizadoEm = DateTime.UtcNow;

        var mov = new MovimentoEstoque
        {
            EmpresaId      = empresaId,
            ProdutoId      = produtoId,
            UsuarioId      = usuarioId,
            Tipo           = tipo,
            Quantidade     = quantidade,
            EstoqueAntes   = antes,
            EstoqueDepois  = aux.Estoque,
            Motivo         = motivo,
            PedidoId       = pedidoId,
            VendaProdutoId = vendaProdutoId,
            CriadoEm       = DateTime.UtcNow,
        };
        ctx.MovimentosEstoque.Add(mov);
        await ctx.SaveChangesAsync();
        return mov;
    }
}
