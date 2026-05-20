using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Data;
using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class AlocacaoService
{
    private readonly ISetiMarineDbContext _context;

    public AlocacaoService(ISetiMarineDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Aloca uma embarcação em uma vaga.
    /// Se a embarcação já estiver em outra vaga, desaloca automaticamente.
    /// </summary>
    public async Task AlocarEmbarcacaoAsync(int vagaId, int embarcacaoId, int empresaId)
    {
        // Valida se a vaga pertence à empresa
        var vaga = await _context.Vagas
            .Include(v => v.EmbarcacoesNaVaga.Where(e => e.Ativa))
            .FirstOrDefaultAsync(v => v.Id == vagaId && v.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Vaga não encontrada.");

        if (vaga.Status == StatusVaga.Manutencao)
            throw new InvalidOperationException("Esta vaga está em manutenção e não pode receber embarcações.");

        // Verifica se a embarcação já está nesta vaga
        if (vaga.EmbarcacoesNaVaga.Any(e => e.EmbarcacaoId == embarcacaoId))
            throw new InvalidOperationException("Esta embarcação já está alocada nesta vaga.");

        // Desaloca da vaga anterior (se houver)
        var alocacaoAnterior = await _context.VagaEmbarcacoes
            .Include(ve => ve.Vaga)
            .FirstOrDefaultAsync(ve => ve.EmbarcacaoId == embarcacaoId && ve.Ativa);

        if (alocacaoAnterior != null)
        {
            alocacaoAnterior.Ativa = false;
            alocacaoAnterior.SaidaEm = DateTime.UtcNow;

            // Atualiza status da vaga anterior se não tiver mais embarcações
            var vagaAnterior = alocacaoAnterior.Vaga!;
            var temOutrasEmbarcacoes = await _context.VagaEmbarcacoes
                .AnyAsync(ve => ve.VagaId == vagaAnterior.Id && ve.Ativa && ve.Id != alocacaoAnterior.Id);

            if (!temOutrasEmbarcacoes && vagaAnterior.Status == StatusVaga.Ocupada)
                vagaAnterior.Status = StatusVaga.Livre;
        }

        // Cria nova alocação
        var novaAlocacao = new VagaEmbarcacao
        {
            VagaId = vagaId,
            EmbarcacaoId = embarcacaoId,
            EntradaEm = DateTime.UtcNow,
            Ativa = true
        };

        _context.VagaEmbarcacoes.Add(novaAlocacao);
        vaga.Status = StatusVaga.Ocupada;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Desaloca uma embarcação de uma vaga pelo ID do registro VagaEmbarcacao.
    /// </summary>
    public async Task DesalocarEmbarcacaoAsync(int vagaEmbarcacaoId, int empresaId)
    {
        var alocacao = await _context.VagaEmbarcacoes
            .Include(ve => ve.Vaga)
            .FirstOrDefaultAsync(ve => ve.Id == vagaEmbarcacaoId && ve.Vaga!.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Alocação não encontrada.");

        alocacao.Ativa = false;
        alocacao.SaidaEm = DateTime.UtcNow;

        // Verifica se ainda há embarcações na vaga
        var temOutrasEmbarcacoes = await _context.VagaEmbarcacoes
            .AnyAsync(ve => ve.VagaId == alocacao.VagaId && ve.Ativa && ve.Id != vagaEmbarcacaoId);

        if (!temOutrasEmbarcacoes)
            alocacao.Vaga!.Status = StatusVaga.Livre;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retorna a vaga atual de uma embarcação (alocação ativa).
    /// </summary>
    public async Task<VagaEmbarcacao?> ObterVagaAtualDaEmbarcacaoAsync(int embarcacaoId)
    {
        return await _context.VagaEmbarcacoes
            .Include(ve => ve.Vaga)
            .FirstOrDefaultAsync(ve => ve.EmbarcacaoId == embarcacaoId && ve.Ativa);
    }

    /// <summary>
    /// Retorna histórico completo de alocações de uma embarcação.
    /// </summary>
    public async Task<List<VagaEmbarcacao>> ObterHistoricoEmbarcacaoAsync(int embarcacaoId)
    {
        return await _context.VagaEmbarcacoes
            .Include(ve => ve.Vaga)
            .Where(ve => ve.EmbarcacaoId == embarcacaoId)
            .OrderByDescending(ve => ve.EntradaEm)
            .ToListAsync();
    }
}

