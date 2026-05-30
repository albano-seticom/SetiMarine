using Microsoft.EntityFrameworkCore;
using SetiMarine.Domain.Data;
using SetiMarine.Domain.Entities;
using SetiMarine.Domain.Enums;

namespace SetiMarine.Application.Services;

public class SeederService(ISetiMarineDbContext ctx)
{
    public async Task SeedSuperAdminAsync()
    {
        if (await ctx.Usuarios.AnyAsync(u => u.Perfil == PerfilUsuario.SuperAdmin))
            return;

        ctx.Usuarios.Add(new Usuario
        {
            EmpresaId = null,
            Nome      = "SuperAdmin",
            Email     = "superadmin@seticom.com.br",
            SenhaHash = AuthService.HashSenha("SetiAdmin@2026"),
            Perfil    = PerfilUsuario.SuperAdmin,
            Ativo     = true,
            CriadoEm  = DateTime.UtcNow,
        });
        await ctx.SaveChangesAsync();
    }

    public async Task<int> SeedProdutosAsync(int empresaId)
    {
        if (await ctx.Produtos.AnyAsync(p => p.EmpresaId == empresaId))
            return 0;

        var itens = new List<(Produto Produto, ProdutoAux Aux)>
        {
            // ── Combustíveis ──────────────────────────────────────────
            (new() { Nome = "Gasolina Comum",         Categoria = CategoriaProduto.Combustivel,  Unidade = "L"  },
             new() { PrecoVenda = 6.50m,  ControlaEstoque = true, Estoque = 5000, EstoqueMinimo = 500  }),
            (new() { Nome = "Gasolina Aditivada",      Categoria = CategoriaProduto.Combustivel,  Unidade = "L"  },
             new() { PrecoVenda = 7.20m,  ControlaEstoque = true, Estoque = 3000, EstoqueMinimo = 300  }),
            (new() { Nome = "Diesel Marítimo",         Categoria = CategoriaProduto.Combustivel,  Unidade = "L"  },
             new() { PrecoVenda = 5.80m,  ControlaEstoque = true, Estoque = 2000, EstoqueMinimo = 300  }),

            // ── Lubrificantes ────────────────────────────────────────
            (new() { Nome = "Óleo Motor 4T 1L",        Categoria = CategoriaProduto.Lubrificante, Unidade = "un" },
             new() { PrecoVenda = 35.00m, ControlaEstoque = true, Estoque = 50,   EstoqueMinimo = 10   }),
            (new() { Nome = "Óleo Motor 2T 1L",        Categoria = CategoriaProduto.Lubrificante, Unidade = "un" },
             new() { PrecoVenda = 28.00m, ControlaEstoque = true, Estoque = 30,   EstoqueMinimo = 8    }),
            (new() { Nome = "Óleo de Câmbio 1L",       Categoria = CategoriaProduto.Lubrificante, Unidade = "un" },
             new() { PrecoVenda = 42.00m, ControlaEstoque = true, Estoque = 20,   EstoqueMinimo = 5    }),
            (new() { Nome = "Fluido de Freio 500ml",   Categoria = CategoriaProduto.Lubrificante, Unidade = "un" },
             new() { PrecoVenda = 25.00m, ControlaEstoque = true, Estoque = 15,   EstoqueMinimo = 4    }),

            // ── Equipamentos para aluguel ────────────────────────────
            (new() { Nome = "Colete Salva-Vidas Adulto",   Categoria = CategoriaProduto.Equipamento, Unidade = "un", Alugavel = true,
                     Descricao = "Colete tipo I homologado NORMAM-01, capacidade 70kg+." },
             new() { PrecoVenda = 180.00m, PrecoAluguelDia = 30.00m, ControlaEstoque = true, Estoque = 10, EstoqueMinimo = 3 }),
            (new() { Nome = "Colete Salva-Vidas Infantil", Categoria = CategoriaProduto.Equipamento, Unidade = "un", Alugavel = true,
                     Descricao = "Colete infantil homologado, capacidade até 40kg." },
             new() { PrecoVenda = 140.00m, PrecoAluguelDia = 20.00m, ControlaEstoque = true, Estoque = 6,  EstoqueMinimo = 2 }),
            (new() { Nome = "Âncora Temporária 4kg",       Categoria = CategoriaProduto.Equipamento, Unidade = "un", Alugavel = true },
             new() { PrecoVenda = 290.00m, PrecoAluguelDia = 50.00m, ControlaEstoque = true, Estoque = 4,  EstoqueMinimo = 1 }),
            (new() { Nome = "Remo de Emergência",           Categoria = CategoriaProduto.Equipamento, Unidade = "un", Alugavel = true },
             new() { PrecoVenda = 95.00m,  PrecoAluguelDia = 15.00m, ControlaEstoque = true, Estoque = 6,  EstoqueMinimo = 2 }),
            (new() { Nome = "Corda de Amarração 10m",       Categoria = CategoriaProduto.Equipamento, Unidade = "un", Alugavel = true },
             new() { PrecoVenda = 45.00m,  PrecoAluguelDia = 10.00m, ControlaEstoque = true, Estoque = 8,  EstoqueMinimo = 2 }),
            (new() { Nome = "Boia de Sinalização",          Categoria = CategoriaProduto.Equipamento, Unidade = "un", Alugavel = true },
             new() { PrecoVenda = 75.00m,  PrecoAluguelDia = 15.00m, ControlaEstoque = true, Estoque = 4,  EstoqueMinimo = 1 }),
            (new() { Nome = "Extensão Elétrica 25m",        Categoria = CategoriaProduto.Equipamento, Unidade = "un", Alugavel = true },
             new() { PrecoAluguelDia = 35.00m, ControlaEstoque = true, Estoque = 3, EstoqueMinimo = 1 }),
            (new() { Nome = "Kit Primeiros Socorros Náutico", Categoria = CategoriaProduto.Equipamento, Unidade = "un", Alugavel = true,
                     Descricao = "Kit com curativos, atadura, tesoura, luvas e manual." },
             new() { PrecoVenda = 120.00m, PrecoAluguelDia = 20.00m, ControlaEstoque = true, Estoque = 4, EstoqueMinimo = 1 }),

            // ── Acessórios ────────────────────────────────────────────
            (new() { Nome = "Extintor 1kg ABC",          Categoria = CategoriaProduto.Acessorio,    Unidade = "un" },
             new() { PrecoVenda = 89.00m,  ControlaEstoque = true, Estoque = 20, EstoqueMinimo = 5 }),
            (new() { Nome = "Sinalizador Manual (kit 3)", Categoria = CategoriaProduto.Acessorio,    Unidade = "un" },
             new() { PrecoVenda = 45.00m,  ControlaEstoque = true, Estoque = 15, EstoqueMinimo = 4 }),
            (new() { Nome = "Protetor Solar FPS50 200ml", Categoria = CategoriaProduto.Acessorio,    Unidade = "un" },
             new() { PrecoVenda = 35.00m,  ControlaEstoque = true, Estoque = 20, EstoqueMinimo = 5 }),
            (new() { Nome = "Silicone Marítimo 280g",    Categoria = CategoriaProduto.Acessorio,    Unidade = "un" },
             new() { PrecoVenda = 28.00m,  ControlaEstoque = true, Estoque = 20, EstoqueMinimo = 5 }),
            (new() { Nome = "Fita Veda Rosca 50m",       Categoria = CategoriaProduto.Acessorio,    Unidade = "un" },
             new() { PrecoVenda = 12.00m,  ControlaEstoque = true, Estoque = 30, EstoqueMinimo = 8 }),
            (new() { Nome = "Repelente Spray 100ml",     Categoria = CategoriaProduto.Acessorio,    Unidade = "un" },
             new() { PrecoVenda = 22.00m,  ControlaEstoque = true, Estoque = 15, EstoqueMinimo = 4 }),

            // ── Peças ─────────────────────────────────────────────────
            (new() { Nome = "Filtro de Combustível Universal", Categoria = CategoriaProduto.Peca, Unidade = "un" },
             new() { PrecoVenda = 35.00m, ControlaEstoque = true, Estoque = 10, EstoqueMinimo = 3 }),
            (new() { Nome = "Impeller de Bomba d'Água",        Categoria = CategoriaProduto.Peca, Unidade = "un" },
             new() { PrecoVenda = 85.00m, ControlaEstoque = true, Estoque = 8,  EstoqueMinimo = 2 }),
            (new() { Nome = "Correia Poly-V",                  Categoria = CategoriaProduto.Peca, Unidade = "un" },
             new() { PrecoVenda = 55.00m, ControlaEstoque = true, Estoque = 6,  EstoqueMinimo = 2 }),
            (new() { Nome = "Vela de Ignição (par)",           Categoria = CategoriaProduto.Peca, Unidade = "un" },
             new() { PrecoVenda = 48.00m, ControlaEstoque = true, Estoque = 12, EstoqueMinimo = 4 }),

            // ── Alimentos / Conveniência ──────────────────────────────
            (new() { Nome = "Água Mineral 500ml",      Categoria = CategoriaProduto.Alimento, Unidade = "un" },
             new() { PrecoVenda = 4.00m,  ControlaEstoque = true, Estoque = 96,  EstoqueMinimo = 24 }),
            (new() { Nome = "Água Mineral 1,5L",       Categoria = CategoriaProduto.Alimento, Unidade = "un" },
             new() { PrecoVenda = 6.00m,  ControlaEstoque = true, Estoque = 48,  EstoqueMinimo = 12 }),
            (new() { Nome = "Refrigerante Lata 350ml", Categoria = CategoriaProduto.Alimento, Unidade = "un" },
             new() { PrecoVenda = 6.50m,  ControlaEstoque = true, Estoque = 72,  EstoqueMinimo = 18 }),
            (new() { Nome = "Cerveja Long Neck 355ml", Categoria = CategoriaProduto.Alimento, Unidade = "un" },
             new() { PrecoVenda = 9.00m,  ControlaEstoque = true, Estoque = 96,  EstoqueMinimo = 24 }),
            (new() { Nome = "Snack Mix Salgado",       Categoria = CategoriaProduto.Alimento, Unidade = "un" },
             new() { PrecoVenda = 8.50m,  ControlaEstoque = true, Estoque = 36,  EstoqueMinimo = 10 }),
            (new() { Nome = "Isotônico 500ml",         Categoria = CategoriaProduto.Alimento, Unidade = "un" },
             new() { PrecoVenda = 7.00m,  ControlaEstoque = true, Estoque = 48,  EstoqueMinimo = 12 }),
        };

        foreach (var (prod, aux) in itens)
        {
            prod.EmpresaId   = empresaId;
            prod.Ativo       = true;
            prod.CriadoEm    = DateTime.UtcNow;
            aux.EmpresaId    = empresaId;
            aux.AtualizadoEm = DateTime.UtcNow;
            prod.Aux         = aux;
            ctx.Produtos.Add(prod);
        }

        await ctx.SaveChangesAsync();
        return itens.Count;
    }

    public async Task<int> SeedServicosAsync(int empresaId)
    {
        if (await ctx.TiposServicoConfig.AnyAsync(s => s.EmpresaId == empresaId))
            return 0;

        var servicos = new List<TipoServicoConfig>
        {
            new() { Nome = "Limpeza Quinzenal",        Tipo = TipoServico.LimpezaQuinzenal,  FrequenciaDias = 15 },
            new() { Nome = "Limpeza Pós-Passeio",      Tipo = TipoServico.LimpezaPosPasseio, FrequenciaDias = 1  },
            new() { Nome = "Vistoria de Segurança",    Tipo = TipoServico.VistoriaPeriodica,  FrequenciaDias = 30 },
            new() { Nome = "Checklist de Saída",       Tipo = TipoServico.ChecklistSaida,    FrequenciaDias = 1  },
            new() { Nome = "Checklist de Retorno",     Tipo = TipoServico.ChecklistRetorno,  FrequenciaDias = 1  },
            new() { Nome = "Abastecimento",            Tipo = TipoServico.Abastecimento,     FrequenciaDias = 1  },
        };

        foreach (var s in servicos)
        {
            s.EmpresaId = empresaId;
            s.Ativo     = true;
            s.CriadoEm  = DateTime.UtcNow;
        }

        ctx.TiposServicoConfig.AddRange(servicos);
        await ctx.SaveChangesAsync();
        return servicos.Count;
    }
}
