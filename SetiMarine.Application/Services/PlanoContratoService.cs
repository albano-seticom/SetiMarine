using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public class PlanoContratoService(ISetiMarineDbContext ctx)
{
    public async Task<List<PlanoContrato>> ListarAsync(int empresaId)
        => await ctx.PlanosContrato
            .Include(p => p.Servicos)
                .ThenInclude(s => s.TipoServico)
            .Where(p => p.EmpresaId == empresaId)
            .OrderBy(p => p.Nome)
            .ToListAsync();

    public async Task<PlanoContrato?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.PlanosContrato
            .Include(p => p.Servicos)
                .ThenInclude(s => s.TipoServico)
            .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId);

    public async Task CriarAsync(PlanoContrato plano, List<PlanoContratoServico> servicos)
    {
        plano.CriadoEm = DateTime.UtcNow;
        plano.Servicos = servicos;
        ctx.PlanosContrato.Add(plano);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(PlanoContrato plano, List<PlanoContratoServico> servicos)
    {
        var existente = await ctx.PlanosContrato
            .Include(p => p.Servicos)
            .FirstOrDefaultAsync(p => p.Id == plano.Id && p.EmpresaId == plano.EmpresaId)
            ?? throw new InvalidOperationException("Plano não encontrado.");

        existente.Nome        = plano.Nome;
        existente.Descricao   = plano.Descricao;
        existente.Mensalidade = plano.Mensalidade;
        existente.Ativo       = plano.Ativo;

        ctx.PlanoContratoServicos.RemoveRange(existente.Servicos);
        existente.Servicos = servicos;

        await ctx.SaveChangesAsync();
    }

    public async Task ExcluirAsync(int id, int empresaId)
    {
        var plano = await ctx.PlanosContrato
            .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Plano não encontrado.");
        ctx.PlanosContrato.Remove(plano);
        await ctx.SaveChangesAsync();
    }

    public async Task<List<TipoServicoConfig>> ListarTiposServicoAsync(int empresaId)
        => await ctx.TiposServicoConfig
            .Where(t => t.EmpresaId == empresaId && t.Ativo)
            .OrderBy(t => t.Nome)
            .ToListAsync();

    public async Task<int> SeedDemoAsync(int empresaId)
    {
        if (await ctx.PlanosContrato.AnyAsync(p => p.EmpresaId == empresaId))
            return 0;

        var servicos = await ctx.TiposServicoConfig
            .Where(s => s.EmpresaId == empresaId && s.Ativo)
            .ToListAsync();

        int SvcId(string nome) =>
            servicos.FirstOrDefault(s => s.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;

        var limpezaId  = SvcId("Limpeza Quinzenal");
        var posPasseioId = SvcId("Pós-Passeio");
        var vistoriaId = SvcId("Vistoria");
        var checkSaidaId = SvcId("Saída");
        var checkRetornoId = SvcId("Retorno");

        var planos = new List<PlanoContrato>
        {
            new()
            {
                EmpresaId   = empresaId,
                Nome        = "Plano Básico",
                Descricao   = "Mensalidade + 1 limpeza quinzenal por mês",
                Mensalidade = 450m,
                Ativo       = true,
                CriadoEm   = DateTime.UtcNow,
                Servicos    = new List<PlanoContratoServico>(
                    limpezaId > 0
                        ? new[] { new PlanoContratoServico { TipoServicoConfigId = limpezaId, Quantidade = 1, Observacao = "por mês" } }
                        : Array.Empty<PlanoContratoServico>()),
            },
            new()
            {
                EmpresaId   = empresaId,
                Nome        = "Plano Intermediário",
                Descricao   = "Mensalidade + 2 limpezas + checklist de saída e retorno",
                Mensalidade = 750m,
                Ativo       = true,
                CriadoEm   = DateTime.UtcNow,
                Servicos    = new List<PlanoContratoServico>(
                    new[] {
                        limpezaId     > 0 ? new PlanoContratoServico { TipoServicoConfigId = limpezaId,     Quantidade = 2, Observacao = "por mês" }       : null,
                        checkSaidaId  > 0 ? new PlanoContratoServico { TipoServicoConfigId = checkSaidaId,  Quantidade = 4, Observacao = "até 4 saídas/mês" } : null,
                        checkRetornoId> 0 ? new PlanoContratoServico { TipoServicoConfigId = checkRetornoId,Quantidade = 4, Observacao = "até 4 retornos/mês"} : null,
                    }.Where(s => s != null).Select(s => s!)),
            },
            new()
            {
                EmpresaId   = empresaId,
                Nome        = "Plano Premium",
                Descricao   = "Mensalidade + limpeza pós-passeio ilimitada + vistoria mensal",
                Mensalidade = 1200m,
                Ativo       = true,
                CriadoEm   = DateTime.UtcNow,
                Servicos    = new List<PlanoContratoServico>(
                    new[] {
                        posPasseioId > 0 ? new PlanoContratoServico { TipoServicoConfigId = posPasseioId, Quantidade = 99, Observacao = "ilimitado"       } : null,
                        limpezaId    > 0 ? new PlanoContratoServico { TipoServicoConfigId = limpezaId,   Quantidade = 2,  Observacao = "por mês"           } : null,
                        vistoriaId   > 0 ? new PlanoContratoServico { TipoServicoConfigId = vistoriaId,  Quantidade = 1,  Observacao = "vistoria mensal"   } : null,
                    }.Where(s => s != null).Select(s => s!)),
            },
            new()
            {
                EmpresaId   = empresaId,
                Nome        = "Plano Visitante",
                Descricao   = "Sem mensalidade fixa — cobrança por diária ou uso avulso",
                Mensalidade = null,
                Ativo       = true,
                CriadoEm   = DateTime.UtcNow,
                Servicos    = new List<PlanoContratoServico>(),
            },
        };

        ctx.PlanosContrato.AddRange(planos);
        await ctx.SaveChangesAsync();
        return planos.Count;
    }
}
