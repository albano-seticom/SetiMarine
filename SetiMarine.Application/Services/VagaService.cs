using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Data;
using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class VagaService
{
    private readonly ISetiMarineDbContext _context;

    public VagaService(ISetiMarineDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vaga>> ListarPorEmpresaAsync(int empresaId, int? corredorId = null, StatusVaga? status = null)
    {
        var query = _context.Vagas
            .Where(v => v.EmpresaId == empresaId && v.Ativa)
            .Include(v => v.Corredor)
            .Include(v => v.EmbarcacoesNaVaga.Where(e => e.Ativa))
                .ThenInclude(ve => ve.Embarcacao)
            .AsQueryable();

        if (corredorId.HasValue)
            query = query.Where(v => v.CorredorId == corredorId);

        if (status.HasValue)
            query = query.Where(v => v.Status == status);

        return await query.OrderBy(v => v.Corredor != null ? v.Corredor.Ordem : 0).ThenBy(v => v.Codigo).ToListAsync();
    }

    public async Task<Vaga?> ObterPorIdAsync(int id, int empresaId)
    {
        return await _context.Vagas
            .Include(v => v.Corredor)
            .Include(v => v.EmbarcacoesNaVaga.Where(e => e.Ativa))
                .ThenInclude(ve => ve.Embarcacao)
            .FirstOrDefaultAsync(v => v.Id == id && v.EmpresaId == empresaId);
    }

    public async Task<Vaga> CriarAsync(Vaga vaga)
    {
        vaga.Status = StatusVaga.Livre;
        _context.Vagas.Add(vaga);
        await _context.SaveChangesAsync();
        return vaga;
    }

    public async Task<Vaga> EditarAsync(Vaga vaga)
    {
        _context.Vagas.Update(vaga);
        await _context.SaveChangesAsync();
        return vaga;
    }

    public async Task ExcluirAsync(int id, int empresaId)
    {
        var vaga = await _context.Vagas
            .Include(v => v.EmbarcacoesNaVaga)
            .FirstOrDefaultAsync(v => v.Id == id && v.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Vaga não encontrada.");

        if (vaga.EmbarcacoesNaVaga.Any(e => e.Ativa))
            throw new InvalidOperationException("Não é possível excluir uma vaga com embarcações alocadas.");

        vaga.Ativa = false;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza posição X/Y da vaga — chamado pelo drag-and-drop do layout.
    /// </summary>
    public async Task AtualizarPosicaoAsync(int vagaId, int empresaId, decimal posX, decimal posY)
    {
        var vaga = await _context.Vagas
            .FirstOrDefaultAsync(v => v.Id == vagaId && v.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Vaga não encontrada.");

        vaga.PosX = posX;
        vaga.PosY = posY;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza largura/altura da vaga — chamado pelo resize do layout.
    /// </summary>
    public async Task AtualizarDimensoesAsync(int vagaId, int empresaId, decimal largura, decimal altura)
    {
        var vaga = await _context.Vagas
            .FirstOrDefaultAsync(v => v.Id == vagaId && v.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Vaga não encontrada.");

        vaga.Largura = largura;
        vaga.Altura = altura;
        await _context.SaveChangesAsync();
    }
}


