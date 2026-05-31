using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class AlocacaoService(ISetiMarineDbContext ctx)
{
    public async Task AlocarEmbarcacaoAsync(int vagaId, int embarcacaoId, int empresaId)
    {
        var vaga = await ctx.Vagas
            .Include(v => v.EmbarcacoesNaVaga.Where(e => e.Ativa))
            .FirstOrDefaultAsync(v => v.Id == vagaId && v.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Vaga não encontrada.");

        if (vaga.Status == StatusVaga.Manutencao)
            throw new InvalidOperationException("Esta vaga está em manutenção e não pode receber embarcações.");

        if (vaga.EmbarcacoesNaVaga.Any(e => e.EmbarcacaoId == embarcacaoId))
            throw new InvalidOperationException("Esta embarcação já está alocada nesta vaga.");

        var alocacaoAnterior = await ctx.VagaEmbarcacoes
            .Include(ve => ve.Vaga)
            .FirstOrDefaultAsync(ve => ve.EmbarcacaoId == embarcacaoId && ve.Ativa);

        if (alocacaoAnterior != null)
        {
            alocacaoAnterior.Ativa = false;
            alocacaoAnterior.SaidaEm = DateTime.UtcNow;

            var vagaAnterior = alocacaoAnterior.Vaga!;
            var temOutrasEmbarcacoes = await ctx.VagaEmbarcacoes
                .AnyAsync(ve => ve.VagaId == vagaAnterior.Id && ve.Ativa && ve.Id != alocacaoAnterior.Id);

            if (!temOutrasEmbarcacoes && vagaAnterior.Status == StatusVaga.Ocupada)
                vagaAnterior.Status = StatusVaga.Livre;
        }

        ctx.VagaEmbarcacoes.Add(new VagaEmbarcacao
        {
            VagaId       = vagaId,
            EmbarcacaoId = embarcacaoId,
            EntradaEm    = DateTime.UtcNow,
            Ativa        = true
        });
        vaga.Status = StatusVaga.Ocupada;

        await ctx.SaveChangesAsync();
    }

    public async Task DesalocarEmbarcacaoAsync(int vagaEmbarcacaoId, int empresaId)
    {
        var alocacao = await ctx.VagaEmbarcacoes
            .Include(ve => ve.Vaga)
            .FirstOrDefaultAsync(ve => ve.Id == vagaEmbarcacaoId && ve.Vaga!.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Alocação não encontrada.");

        alocacao.Ativa = false;
        alocacao.SaidaEm = DateTime.UtcNow;

        var temOutrasEmbarcacoes = await ctx.VagaEmbarcacoes
            .AnyAsync(ve => ve.VagaId == alocacao.VagaId && ve.Ativa && ve.Id != vagaEmbarcacaoId);

        if (!temOutrasEmbarcacoes)
            alocacao.Vaga!.Status = StatusVaga.Livre;

        await ctx.SaveChangesAsync();
    }

    public async Task<VagaEmbarcacao?> ObterVagaAtualDaEmbarcacaoAsync(int embarcacaoId)
        => await ctx.VagaEmbarcacoes
            .Include(ve => ve.Vaga)
            .FirstOrDefaultAsync(ve => ve.EmbarcacaoId == embarcacaoId && ve.Ativa);

    public async Task<List<VagaEmbarcacao>> ObterHistoricoEmbarcacaoAsync(int embarcacaoId)
        => await ctx.VagaEmbarcacoes
            .Include(ve => ve.Vaga)
            .Where(ve => ve.EmbarcacaoId == embarcacaoId)
            .OrderByDescending(ve => ve.EntradaEm)
            .ToListAsync();
}
