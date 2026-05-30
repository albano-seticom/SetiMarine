using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public record CotaInfo(
    int PlanoContratoServicoId,
    string ServicoNome,
    int Limite,
    int Usado,
    int Disponivel,
    bool Excedido);

public class ConsumoPlanoService(ISetiMarineDbContext ctx)
{
    public async Task<List<CotaInfo>> ObterCotasMesAsync(int contratoId, string periodo)
    {
        var contrato = await ctx.Contratos
            .Include(c => c.PlanoContrato)
                .ThenInclude(p => p!.Servicos)
                    .ThenInclude(s => s.TipoServico)
            .FirstOrDefaultAsync(c => c.Id == contratoId);

        if (contrato?.PlanoContrato == null || !contrato.PlanoContrato.Servicos.Any())
            return new();

        var consumos = await ctx.ConsumoPlano
            .Where(cp => cp.ContratoId == contratoId && cp.Periodo == periodo)
            .ToListAsync();

        return contrato.PlanoContrato.Servicos.Select(sv =>
        {
            var usado = consumos.FirstOrDefault(cp => cp.PlanoContratoServicoId == sv.Id)?.QuantidadeUsada ?? 0;
            return new CotaInfo(
                sv.Id,
                sv.TipoServico?.Nome ?? "—",
                sv.Quantidade,
                usado,
                Math.Max(0, sv.Quantidade - usado),
                usado >= sv.Quantidade);
        }).ToList();
    }

    public async Task<ConsumoPlano> IncrementarAsync(int contratoId, int planoContratoServicoId, string periodo)
    {
        var consumo = await ctx.ConsumoPlano
            .FirstOrDefaultAsync(cp => cp.ContratoId == contratoId
                                    && cp.PlanoContratoServicoId == planoContratoServicoId
                                    && cp.Periodo == periodo);

        if (consumo == null)
        {
            consumo = new ConsumoPlano
            {
                ContratoId              = contratoId,
                PlanoContratoServicoId  = planoContratoServicoId,
                Periodo                 = periodo,
                QuantidadeUsada         = 1,
                AtualizadoEm            = DateTime.UtcNow,
            };
            ctx.ConsumoPlano.Add(consumo);
        }
        else
        {
            consumo.QuantidadeUsada++;
            consumo.AtualizadoEm = DateTime.UtcNow;
        }

        await ctx.SaveChangesAsync();
        return consumo;
    }

    public async Task<Contrato?> ObterContratoComPlanoAsync(int? clienteId, int empresaId)
    {
        if (clienteId == null || clienteId == 0) return null;

        return await ctx.Contratos
            .Include(c => c.PlanoContrato)
                .ThenInclude(p => p!.Servicos)
                    .ThenInclude(s => s.TipoServico)
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId
                                   && c.ClienteId == clienteId
                                   && c.Ativo
                                   && c.PlanoContratoId != null);
    }
}
