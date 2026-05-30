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
        var q = ctx.Produtos
            .Include(p => p.Aux)
            .Where(p => p.EmpresaId == empresaId);
        if (apenasAtivos.HasValue) q = q.Where(p => p.Ativo == apenasAtivos.Value);
        if (categoria.HasValue)    q = q.Where(p => p.Categoria == categoria.Value);
        return await q.OrderBy(p => p.Nome).ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.Produtos
            .Include(p => p.Aux)
            .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId);

    public async Task CriarAsync(Produto produto, ProdutoAux aux)
    {
        produto.CriadoEm = DateTime.UtcNow;
        aux.EmpresaId    = produto.EmpresaId;
        aux.AtualizadoEm = DateTime.UtcNow;
        produto.Aux      = aux;
        ctx.Produtos.Add(produto);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(Produto produto, ProdutoAux aux)
    {
        var ex = await ctx.Produtos
            .Include(p => p.Aux)
            .FirstOrDefaultAsync(p => p.Id == produto.Id && p.EmpresaId == produto.EmpresaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        ex.Nome     = produto.Nome;
        ex.Descricao = produto.Descricao;
        ex.Categoria = produto.Categoria;
        ex.Unidade  = produto.Unidade;
        ex.Alugavel = produto.Alugavel;
        ex.Ativo    = produto.Ativo;

        if (ex.Aux == null)
        {
            ex.Aux = new ProdutoAux { ProdutoId = ex.Id, EmpresaId = ex.EmpresaId };
            ctx.ProdutoAux.Add(ex.Aux);
        }

        ex.Aux.PrecoCusto       = aux.PrecoCusto;
        ex.Aux.PrecoVenda       = aux.PrecoVenda;
        ex.Aux.PrecoAluguelHora = aux.PrecoAluguelHora;
        ex.Aux.PrecoAluguelDia  = aux.PrecoAluguelDia;
        ex.Aux.MargemLucro      = aux.MargemLucro;
        ex.Aux.ControlaEstoque  = aux.ControlaEstoque;
        ex.Aux.EstoqueMinimo    = aux.EstoqueMinimo;
        ex.Aux.AliquotaICMS    = aux.AliquotaICMS;
        ex.Aux.AliquotaIPI     = aux.AliquotaIPI;
        ex.Aux.AliquotaPIS     = aux.AliquotaPIS;
        ex.Aux.AliquotaCOFINS  = aux.AliquotaCOFINS;
        ex.Aux.AliquotaISS     = aux.AliquotaISS;
        ex.Aux.CstIcms         = aux.CstIcms;
        ex.Aux.CstPisCofins    = aux.CstPisCofins;
        ex.Aux.Cfop            = aux.Cfop;
        ex.Aux.Ncm             = aux.Ncm;
        ex.Aux.AtualizadoEm    = DateTime.UtcNow;

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
            .Include(v => v.Produto).ThenInclude(p => p!.Aux)
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id && v.EmpresaId == empresaId);

    public async Task RegistrarVendaAsync(VendaProduto venda)
    {
        var produto = await ctx.Produtos
            .Include(p => p.Aux)
            .FirstOrDefaultAsync(p => p.Id == venda.ProdutoId && p.EmpresaId == venda.EmpresaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        var aux = produto.Aux;
        if (aux != null && aux.ControlaEstoque)
        {
            if (aux.Estoque < venda.Quantidade)
                throw new InvalidOperationException($"Estoque insuficiente. Disponível: {aux.Estoque} {produto.Unidade}.");
            aux.Estoque -= venda.Quantidade;
            aux.AtualizadoEm = DateTime.UtcNow;
        }

        venda.DataTransacao = DateTime.UtcNow;
        venda.Tipo          = TipoTransacao.Venda;
        ctx.VendasProduto.Add(venda);
        await ctx.SaveChangesAsync();
    }

    public async Task RegistrarAluguelAsync(VendaProduto aluguel)
    {
        var produto = await ctx.Produtos
            .Include(p => p.Aux)
            .FirstOrDefaultAsync(p => p.Id == aluguel.ProdutoId && p.EmpresaId == aluguel.EmpresaId)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        if (!produto.Alugavel)
            throw new InvalidOperationException("Este produto não está disponível para aluguel.");

        var aux = produto.Aux;
        if (aux != null && aux.ControlaEstoque)
        {
            if (aux.Estoque < aluguel.Quantidade)
                throw new InvalidOperationException($"Estoque insuficiente. Disponível: {aux.Estoque} {produto.Unidade}.");
            aux.Estoque -= aluguel.Quantidade;
            aux.AtualizadoEm = DateTime.UtcNow;
        }

        aluguel.DataTransacao = DateTime.UtcNow;
        aluguel.Tipo          = TipoTransacao.Aluguel;
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

        var aux = t.Produto?.Aux;
        if (aux != null && aux.ControlaEstoque)
        {
            aux.Estoque      += t.Quantidade;
            aux.AtualizadoEm  = DateTime.UtcNow;
        }

        await ctx.SaveChangesAsync();
    }

    // ── Auxiliares para dropdowns ────────────────────────────────

    public async Task<List<Cliente>> ListarClientesAsync(int empresaId)
        => await ctx.Clientes.Where(c => c.EmpresaId == empresaId).OrderBy(c => c.Nome).ToListAsync();

    public async Task<List<Usuario>> ListarColaboradoresAsync(int empresaId)
        => await ctx.Usuarios.Where(u => u.EmpresaId == empresaId && u.Ativo).OrderBy(u => u.Nome).ToListAsync();
}
