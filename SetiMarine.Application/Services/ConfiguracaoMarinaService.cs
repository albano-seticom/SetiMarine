using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;

namespace SetiMarine.Application.Services;

public class ConfiguracaoMarinaService(ISetiMarineDbContext ctx)
{
    public async Task<ConfiguracaoMarina> ObterAsync(int empresaId)
    {
        var cfg = await ctx.ConfiguracoesMarina
            .FirstOrDefaultAsync(c => c.EmpresaId == empresaId);

        if (cfg == null)
        {
            cfg = new ConfiguracaoMarina { EmpresaId = empresaId };
            ctx.ConfiguracoesMarina.Add(cfg);
            await ctx.SaveChangesAsync();
        }

        return cfg;
    }

    public async Task SalvarAsync(ConfiguracaoMarina cfg)
    {
        var existente = await ctx.ConfiguracoesMarina
            .FirstOrDefaultAsync(c => c.EmpresaId == cfg.EmpresaId);

        if (existente == null)
        {
            ctx.ConfiguracoesMarina.Add(cfg);
        }
        else
        {
            existente.CaixaAutomatico     = cfg.CaixaAutomatico;
            existente.SetiFiscalBaseUrl   = cfg.SetiFiscalBaseUrl;
            existente.SetiFiscalApiKey    = cfg.SetiFiscalApiKey;
            existente.SetiFiscalEmpresaId = cfg.SetiFiscalEmpresaId;
            existente.CodigoServicoDefault = cfg.CodigoServicoDefault;
            existente.CodigoNbsDefault    = cfg.CodigoNbsDefault;
            existente.AtualizadoEm        = DateTime.UtcNow;
        }

        await ctx.SaveChangesAsync();
    }
}
