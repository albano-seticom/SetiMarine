using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class AgendamentoUsoService(ISetiMarineDbContext ctx)
{
    public async Task<List<AgendamentoUso>> ListarPeriodoAsync(int empresaId, DateTime inicio, DateTime fim)
        => await ctx.AgendamentosUso
            .Include(a => a.Cliente)
            .Include(a => a.Embarcacao)
            .Where(a => a.EmpresaId == empresaId
                     && a.DataHoraUso >= inicio
                     && a.DataHoraUso < fim)
            .OrderBy(a => a.DataHoraUso)
            .ToListAsync();

    public async Task<AgendamentoUso?> ObterPorIdAsync(int id, int empresaId)
        => await ctx.AgendamentosUso
            .Include(a => a.Cliente)
            .Include(a => a.Embarcacao)
            .FirstOrDefaultAsync(a => a.Id == id && a.EmpresaId == empresaId);

    public async Task CriarAsync(AgendamentoUso ag)
    {
        ag.CriadoEm           = DateTime.UtcNow;
        ag.DataHoraPreparacao = ag.DataHoraUso.AddMinutes(-ag.MinutosAntecedencia);
        ctx.AgendamentosUso.Add(ag);
        await ctx.SaveChangesAsync();
    }

    public async Task EditarAsync(AgendamentoUso ag)
    {
        var ex = await ctx.AgendamentosUso
            .FirstOrDefaultAsync(a => a.Id == ag.Id && a.EmpresaId == ag.EmpresaId)
            ?? throw new InvalidOperationException("Agendamento não encontrado.");

        ex.ClienteId                 = ag.ClienteId;
        ex.EmbarcacaoId              = ag.EmbarcacaoId;
        ex.DataHoraUso               = ag.DataHoraUso;
        ex.MinutosAntecedencia       = ag.MinutosAntecedencia;
        ex.DataHoraPreparacao        = ag.DataHoraUso.AddMinutes(-ag.MinutosAntecedencia);
        ex.TipoUso                   = ag.TipoUso;
        ex.Status                    = ag.Status;
        ex.Observacoes               = ag.Observacoes;
        ex.ContratoId                = ag.ContratoId;
        ex.PlanoContratoServicoId    = ag.PlanoContratoServicoId;
        ex.CobrancaExtra             = ag.CobrancaExtra;
        await ctx.SaveChangesAsync();
    }

    public async Task AlterarStatusAsync(int id, int empresaId, StatusAgendamento status)
    {
        var ag = await ctx.AgendamentosUso
            .FirstOrDefaultAsync(a => a.Id == id && a.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Agendamento não encontrado.");
        ag.Status = status;
        await ctx.SaveChangesAsync();
    }

    public async Task ExcluirAsync(int id, int empresaId)
    {
        var ag = await ctx.AgendamentosUso
            .FirstOrDefaultAsync(a => a.Id == id && a.EmpresaId == empresaId)
            ?? throw new InvalidOperationException("Agendamento não encontrado.");
        ctx.AgendamentosUso.Remove(ag);
        await ctx.SaveChangesAsync();
    }

    public async Task<List<Cliente>> ListarClientesAsync(int empresaId)
        => await ctx.Clientes
            .Where(c => c.EmpresaId == empresaId && c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();

    public async Task<List<Embarcacao>> ListarEmbarcacoesAsync(int empresaId, int? clienteId = null)
    {
        var q = ctx.Embarcacoes
            .Where(e => e.EmpresaId == empresaId && e.Ativa);
        if (clienteId.HasValue && clienteId > 0)
            q = q.Where(e => e.ClienteId == clienteId);
        return await q.OrderBy(e => e.Nome).ToListAsync();
    }

    // ── Seeder de demonstração ─────────────────────────────────────────────
    public async Task<int> SeedDemoAsync(int empresaId)
    {
        if (await ctx.AgendamentosUso.AnyAsync(a => a.EmpresaId == empresaId))
            return 0;

        var clientes    = await ctx.Clientes.Where(c => c.EmpresaId == empresaId && c.Ativo).Take(4).ToListAsync();
        var embarcacoes = await ctx.Embarcacoes.Where(e => e.EmpresaId == empresaId && e.Ativa).Take(6).ToListAsync();

        if (!clientes.Any() || !embarcacoes.Any()) return 0;

        var hoje  = DateTime.UtcNow.Date;
        var seg   = hoje.AddDays(-(int)hoje.DayOfWeek + 1); // segunda desta semana
        var random = new Random(42);

        var demos = new List<AgendamentoUso>();

        void Add(int diasOffset, int hora, int clienteIdx, int embarcacaoIdx,
                 TipoUsoEmbarcacao tipo, StatusAgendamento status, string? obs = null)
        {
            var c   = clientes[clienteIdx % clientes.Count];
            var emb = embarcacoes[embarcacaoIdx % embarcacoes.Count];
            var dt  = DateTime.SpecifyKind(seg.AddDays(diasOffset).AddHours(hora), DateTimeKind.Utc);
            demos.Add(new AgendamentoUso
            {
                EmpresaId          = empresaId,
                ClienteId          = c.Id,
                EmbarcacaoId       = emb.Id,
                DataHoraUso        = dt,
                DataHoraPreparacao = dt.AddMinutes(-60),
                MinutosAntecedencia = 60,
                TipoUso            = tipo,
                Status             = status,
                Observacoes        = obs,
                CriadoEm           = DateTime.UtcNow,
            });
        }

        // Segunda — 2 itens
        Add(0,  8, 0, 0, TipoUsoEmbarcacao.DescidaNaAgua, StatusAgendamento.Concluido, "Passeio manhã — família");
        Add(0, 14, 1, 1, TipoUsoEmbarcacao.Passeio,       StatusAgendamento.Concluido);
        // Terça
        Add(1, 10, 2, 2, TipoUsoEmbarcacao.UsoPier,       StatusAgendamento.Concluido, "Abastecimento + revisão rápida");
        // Quarta
        Add(2,  9, 0, 0, TipoUsoEmbarcacao.DescidaNaAgua, StatusAgendamento.Concluido);
        Add(2, 15, 3, 3, TipoUsoEmbarcacao.DescidaNaAgua, StatusAgendamento.Confirmado, "Pesca — sair às 15h em ponto");
        // Quinta
        Add(3, 11, 1, 1, TipoUsoEmbarcacao.Passeio,       StatusAgendamento.Confirmado, "Aniversário da filha");
        Add(3, 16, 2, 4, TipoUsoEmbarcacao.UsoPier,       StatusAgendamento.Pendente);
        // Sexta
        Add(4,  8, 3, 3, TipoUsoEmbarcacao.DescidaNaAgua, StatusAgendamento.Confirmado);
        Add(4, 14, 0, 0, TipoUsoEmbarcacao.DescidaNaAgua, StatusAgendamento.Pendente,   "Cliente pediu limpeza após retorno");
        // Sábado
        Add(5,  7, 1, 2, TipoUsoEmbarcacao.DescidaNaAgua, StatusAgendamento.Confirmado, "Saída às 7h — pesca esportiva");
        Add(5,  9, 2, 1, TipoUsoEmbarcacao.Passeio,       StatusAgendamento.Confirmado, "Passeio ilha dos Lobos");
        Add(5, 13, 3, 5, TipoUsoEmbarcacao.DescidaNaAgua, StatusAgendamento.Pendente);
        // Domingo
        Add(6,  8, 0, 4, TipoUsoEmbarcacao.DescidaNaAgua, StatusAgendamento.Confirmado, "Passeio família");
        Add(6, 10, 2, 0, TipoUsoEmbarcacao.Passeio,       StatusAgendamento.Pendente);

        ctx.AgendamentosUso.AddRange(demos);
        await ctx.SaveChangesAsync();
        return demos.Count;
    }
}
