using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class ProdutoService(ISetiMarineDbContext ctx)
{
    // ── Catálogo ─────────────────────────────────────────────────

    public async Task<List<Produto>> ListarAsync(int empresaId, bool? apenasAtivos = true, CategoriaProduto? categoria = null)
    {
        var q = ctx.Produtos.Where(p => p.EmpresaId == empresaId);
        if (apenasAtivos.HasValue) q = q.Where(p => p.Ativo == apenasAtivos.Value);
        if (categoria.HasValue)    q = q.Where(p => p.Categoria == categoria.Value);
        return await q.OrderBy(p => p.Nome).ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Produtos.FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId);

    public async Task CriarAsync(Produto produto)
    {
        produto.CriadoEm = DateTime.UtcNow;
        ctx.Produtos.Add(produto);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(Produto produto)
    {
        ctx.Produtos.Update(produto);
        await ctx.SaveChangesAsync();
    }

    public async Task SalvarAsync(Produto produto)
    {
        if (produto.Id == 0)
        {
            produto.CriadoEm = DateTime.UtcNow;
            ctx.Produtos.Add(produto);
        }
        else
        {
            var ex = await ctx.Produtos.FindAsync(produto.Id)
                ?? throw new InvalidOperationException("Produto não encontrado.");
            ex.Nome            = produto.Nome;
            ex.Categoria       = produto.Categoria;
            ex.Unidade         = produto.Unidade;
            ex.PrecoVenda      = produto.PrecoVenda;
            ex.PrecoAluguelDia = produto.PrecoAluguelDia;
            ex.Estoque         = produto.Estoque;
            ex.EstoqueMinimo   = produto.EstoqueMinimo;
            ex.Alugavel        = produto.Alugavel;
            ex.Ativo           = produto.Ativo;
        }
        await ctx.SaveChangesAsync();
    }

    public async Task AlternarAtivoAsync(int id, int empresaId)
    {
        var p = await ObterPorIdAsync(id, empresaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");
        p.Ativo = !p.Ativo;
        await ctx.SaveChangesAsync();
    }

    // ── Transações ───────────────────────────────────────────────

    public async Task<List<VendaProduto>> ListarTransacoesAsync(int empresaId, TipoTransacao? tipo = null, int? produtoId = null)
    {
        var q = ctx.VendasProduto
            .Include(v => v.Produto)
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Where(v => v.EmpresaId == empresaId);

        if (tipo.HasValue)      q = q.Where(v => v.Tipo == tipo.Value);
        if (produtoId.HasValue) q = q.Where(v => v.ProdutoId == produtoId.Value);

        return await q.OrderByDescending(v => v.DataTransacao).ToListAsync();
    }

    public async Task<VendaProduto?> ObterTransacaoPorIdAsync(int id, int empresaId)
        => await ctx.VendasProduto
            .Include(v => v.Produto)
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id && v.EmpresaId == empresaId);

    public async Task RegistrarVendaAsync(VendaProduto venda)
    {
        var produto = await ObterPorIdAsync(venda.ProdutoId, venda.EmpresaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        if (produto.Estoque < venda.Quantidade)
            throw new InvalidOperationException($"Estoque insuficiente. Disponível: {produto.Estoque} {produto.Unidade}.");

        produto.Estoque -= venda.Quantidade;
        venda.DataTransacao = DateTime.UtcNow;
        venda.Tipo = TipoTransacao.Venda;

        ctx.VendasProduto.Add(venda);
        await ctx.SaveChangesAsync();
    }

    public async Task RegistrarAluguelAsync(VendaProduto aluguel)
    {
        var produto = await ObterPorIdAsync(aluguel.ProdutoId, aluguel.EmpresaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        if (!produto.Alugavel)
            throw new InvalidOperationException("Este produto não está disponível para aluguel.");

        if (produto.Estoque < aluguel.Quantidade)
            throw new InvalidOperationException($"Estoque insuficiente. Disponível: {produto.Estoque} {produto.Unidade}.");

        produto.Estoque -= aluguel.Quantidade;
        aluguel.DataTransacao = DateTime.UtcNow;
        aluguel.Tipo = TipoTransacao.Aluguel;
        aluguel.StatusAluguel = SetiMarine.Domain.Enums.StatusAluguel.Ativo;

        ctx.VendasProduto.Add(aluguel);
        await ctx.SaveChangesAsync();
    }

    public async Task RegistrarDevolucaoAsync(int transacaoId, int empresaId)
    {
        var t = await ObterTransacaoPorIdAsync(transacaoId, empresaId)
            ?? throw new InvalidOperationException("Transação não encontrada.");

        if (t.Tipo != TipoTransacao.Aluguel)
            throw new InvalidOperationException("Somente aluguéis podem ser devolvidos.");

        t.DataDevolucaoReal = DateTime.UtcNow;
        t.StatusAluguel     = SetiMarine.Domain.Enums.StatusAluguel.Devolvido;

        var produto = await ObterPorIdAsync(t.ProdutoId, empresaId);
        if (produto != null) produto.Estoque += t.Quantidade;

        await ctx.SaveChangesAsync();
    }

    // ── Auxiliares para dropdowns ────────────────────────────────

    public async Task<List<Cliente>> ListarClientesAsync(int empresaId)
        => await ctx.Clientes.Where(c => c.EmpresaId == empresaId).OrderBy(c => c.Nome).ToListAsync();

    public async Task<List<Usuario>> ListarColaboradoresAsync(int empresaId)
        => await ctx.Usuarios.Where(u => u.EmpresaId == empresaId && u.Ativo).OrderBy(u => u.Nome).ToListAsync();
}
