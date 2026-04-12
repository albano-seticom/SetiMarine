# SetiMarine — Prompt de Continuidade para IA

Cole este arquivo no início de uma nova conversa para retomar o desenvolvimento do SetiMarine exatamente de onde parou.

---

## Contexto

Você é um assistente técnico especializado em sistemas desenvolvidos pela SETICOM. Estamos desenvolvendo o **SetiMarine** — um sistema SaaS completo para gestão de marinas, focado em operação em tempo real, organização de fluxo de embarcações, mapa visual de vagas e integração com WhatsApp.

---

## Stack Tecnológica

- **Frontend/Backend:** Blazor Server (.NET 8, C#)
- **Banco de dados:** PostgreSQL
- **ORM:** Entity Framework Core 8 com Migrations
- **Tempo real:** SignalR
- **WhatsApp API:** CodeChat (mesmo padrão SetiDesk)
- **IDE:** Visual Studio
- **Versionamento:** GitHub — https://github.com/albano-seticom/SetiMarine
- **Servidor:** Ubuntu 192.168.0.177
- **Reverse Proxy:** Traefik 192.168.0.250
- **URL produção:** https://setimarine.seticom.com.br
- **Deploy:** push no GitHub → git pull no servidor → docker compose up --build
- **Container Blazor:** `setimarine_blazor` porta 9092 (interno 8080)
- **Container Banco:** `postgres_setimarine` porta 5436

---

## Estrutura da Solução

```
C:\Projetos\SetiMarine\
├── SetiMarine.sln
├── SetiMarine.Domain/
│   ├── Entities/
│   │   ├── Empresa.cs
│   │   ├── Plano.cs
│   │   ├── Usuario.cs
│   │   ├── SessaoAtiva.cs
│   │   ├── Cliente.cs
│   │   ├── Embarcacao.cs
│   │   ├── Vaga.cs
│   │   ├── Movimentacao.cs
│   │   ├── HistoricoMovimentacao.cs
│   │   ├── OrdemServico.cs
│   │   ├── Contrato.cs
│   │   └── Configuracao.cs
│   └── Enums/Enums.cs
├── SetiMarine.Application/            (vazio — regras futuras)
├── SetiMarine.Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── AppDbContextFactory.cs    (IDesignTimeDbContextFactory)
│   ├── Migrations/                    (migration "Inicial" aplicada)
│   └── Services/
│       └── LanguageService.cs        (PT/EN/ES)
└── SetiMarine.Web/
    ├── Components/
    │   ├── App.razor
    │   ├── Routes.razor
    │   ├── _Imports.razor
    │   ├── Layout/
    │   │   ├── AppLayout.razor       (sidebar — TODO: implementar completo)
    │   │   └── EmptyLayout.razor
    │   └── Pages/
    │       ├── Public/
    │       │   ├── Home.razor        ✅ landing page completa
    │       │   ├── Login.razor       ⚠️ placeholder
    │       │   └── Cadastro.razor    ⚠️ placeholder
    │       └── App/
    │           ├── Dashboard.razor   ⚠️ placeholder
    │           └── SuperAdmin.razor  ⚠️ placeholder
    ├── Hubs/                         (vazio — SignalR futuro)
    ├── wwwroot/
    │   ├── css/app.css               ✅ tema claro/azul completo + Outfit font
    │   ├── js/site.js
    │   └── images/
    │       ├── boats/
    │       │   ├── boat-main.jpg     ✅ iate grande (fundo transparente)
    │       │   ├── boat-small.jpg    ✅ lancha pequena (fundo transparente)
    │       │   ├── boat-sail.png     ✅ barcos ilustração
    │       │   └── top/
    │       │       ├── lancha-top.svg  ✅ vista superior para mapa
    │       │       └── veleiro-top.svg ✅ vista superior para mapa
    │       ├── marina/
    │       │   ├── marina-hero.jpg   ✅ Monaco ao pôr do sol (hero bg)
    │       │   └── marina-aerial.jpg ✅ marina aérea (mapa bg)
    │       └── flags/                ⚠️ br.png, us.png, es.png (adicionar ainda)
    ├── Program.cs                    ✅ auth cookies + DbFactory + SVG MIME
    ├── appsettings.json
    ├── appsettings.Development.json  (gitignore — conexão local)
    └── Dockerfile
```

---

## Banco de Dados

- **String de conexão produção:** `Host=postgres_setimarine;Port=5432;Database=setimarine;Username=setimarine;Password=SetiMarine@2026`
- **String de conexão local:** `Host=localhost;Port=5436;...`
- **AppDbContextFactory:** aponta para 192.168.0.177:5436 (para migrations locais)

### Tabelas

| Entidade | Tabela |
|---|---|
| Empresa | sis_empresas |
| Plano | cfg_planos |
| Usuario | sis_usuarios |
| SessaoAtiva | sis_sessoes |
| Cliente | mar_clientes |
| Embarcacao | mar_embarcacoes |
| Vaga | mar_vagas |
| Movimentacao | mar_movimentacoes |
| HistoricoMovimentacao | mar_historico_movimentacoes |
| OrdemServico | mar_ordens_servico |
| Contrato | mar_contratos |
| Configuracao | cfg_configuracoes |

### Dados já inseridos no banco

**Empresa:** SETICOM Tecnologia (Id=1)
**Usuário SuperAdmin:** `setimarine@seticomtecnologia.com.br` / senha hash SHA256 de `trpiee`
**Planos cadastrados:**
- Starter — R$ 197 — 30 embarcações / 3 usuários
- Professional — R$ 497 — 100 embarcações / 10 usuários (Destaque=true)
- Enterprise — Sob consulta — ilimitado

---

## Enums

```csharp
public enum PerfilUsuario  { SuperAdmin = 0, Admin = 1, Operacional = 2 }
public enum TipoVaga       { Agua = 0, Seco = 1 }
public enum StatusVaga     { Livre = 0, Ocupada = 1, EmMovimentacao = 2, Manutencao = 3 }
public enum TipoEmbarcacao { Lancha = 0, Veleiro = 1, Escuna = 2, JetSki = 3, Outro = 4 }
public enum StatusMovimentacao
{
    Agendado = 0, AguardandoPreparo = 1, EmPreparo = 2, Pronto = 3,
    Descendo = 4, NaAgua = 5, Retornando = 6, Subindo = 7, Finalizado = 8
}
public enum TipoContrato { Mensalista = 0, Diaria = 1, Visitante = 2 }
public enum StatusOS     { Pendente = 0, EmExecucao = 1, Finalizado = 2 }
```

---

## Páginas Implementadas

| Página | Rota | Status | Observações |
|---|---|---|---|
| Landing page | / | ✅ | Multilíngue PT/EN/ES, hero + mapa + planos |
| Login | /login | ⚠️ | Placeholder — próxima a implementar |
| Cadastro | /cadastro | ⚠️ | Placeholder — wizard 3 etapas |
| Dashboard | /dashboard | ⚠️ | Placeholder |
| SuperAdmin | /superadmin | ⚠️ | Placeholder — gestão de empresas e planos |

---

## Home.razor — Funcionalidades Implementadas

- Navbar com logo âncora + seletor de idioma (PT/EN/ES) com dropdown de bandeiras
- Hero com foto de marina Monaco como background (opacidade .18)
- 3 barcos flutuando com animação CSS
- Barra de stats (100% tempo real / PT-EN-ES / WhatsApp / PWA)
- Mapa visual da marina:
  - Vagas na ÁGUA em pontões horizontais (Pontão A e B)
  - Vagas no SECO em grid de dois setores com lista
  - Cores: livre (azul escuro) / ocupada (azul) / movimentação (laranja) / manutenção (vermelho)
  - Imagens vista superior dos barcos nas vagas
- Seção de planos dinâmica — busca do banco via DbFactory
- CTA final com dois cards (nova marina / já tenho acesso)
- Footer SETICOM

---

## Visual / Design

- **Fonte títulos:** Outfit (700/800)
- **Fonte corpo:** DM Sans (300/400/500)
- **Paleta:** `--navy: #03122b` / `--mid: #1565c0` / `--accent: #29b6f6` / `--foam: #e8f6fd`
- **Fundo hero:** azul claro `#f0f7ff` com foto marina Monaco semi-transparente
- **Fundo mapa/planos:** dark `#030d1a`
- Bandeiras do seletor de idioma: `/images/flags/br.png`, `us.png`, `es.png` — **AINDA NÃO ADICIONADAS**

---

## Infra e Deploy

```bash
# No servidor (192.168.0.177)
cd /opt/apps/SetiMarine
git pull
cd ~/docker
docker compose up -d --build setimarine_blazor
docker logs setimarine_blazor -f
```

```powershell
# No Windows (desenvolvimento)
cd C:\Projetos\SetiMarine
git add .
git commit -m "descricao"
git push
```

### Migrations (rodar local apontando pro servidor)
```powershell
cd C:\Projetos\SetiMarine\SetiMarine.Web
dotnet ef migrations add NomeDaMigration --project ../SetiMarine.Infrastructure
dotnet ef database update --project ../SetiMarine.Infrastructure
```

### Traefik (já configurado)
```toml
[http.routers.setimarine]
  rule = "Host(`setimarine.seticom.com.br`)"
  service = "setimarine"
  entryPoints = ["websecure"]
  tls = { certResolver = "lets-encrypt" }
  middlewares = ["dotnet-proxy"]
[http.services.setimarine.loadBalancer]
  [[http.services.setimarine.loadBalancer.servers]]
    url = "http://192.168.0.177:9092"
```

---

## Padrões Obrigatórios (CRÍTICOS — igual SetiDesk/SetiTrib)

1. **NUNCA** `@bind` + `@oninput` — usar `value=` + `@oninput`
2. **NUNCA** `@bind` + `@onchange` — usar `@bind:after`
3. **`SignInAsync`** somente em endpoints HTTP POST no `Program.cs`
4. **DbContext:** sempre `await using var db = await DbFactory.CreateDbContextAsync()`
5. **Multi-empresa:** todo dado filtrado por `EmpresaId`
6. **Migrations:** usar `AppDbContextFactory` — NÃO depender do DI do Program.cs
7. **Nome de variável:** NUNCA usar `$home` no PowerShell (variável reservada) — usar `$conteudo`
8. **Caminhos PowerShell:** sempre usar caminho completo com `C:\Projetos\...`
9. **Script Criar-SetiMarine.ps1:** NÃO rodar novamente — sobrescreve arquivos já modificados!
10. **SVG:** configurar MIME type no Program.cs para servir arquivos `.svg`
11. **Datas PostgreSQL:** sempre `DateTime.SpecifyKind(d, DateTimeKind.Utc)`

---

## O que Falta Implementar (Checklist)

### Alta prioridade
- [ ] **Imagens de bandeiras** — adicionar `br.png`, `us.png`, `es.png` em `wwwroot/images/flags/`
- [ ] **Login.razor** — tela completa com validação, hash de senha (definir BCrypt ou SHA256), sessão única
- [ ] **Cadastro.razor** — wizard 3 etapas (dados da marina → usuário admin → escolha de plano)
- [ ] **AppLayout.razor** — sidebar completa com menu, avatar, tema escuro, perfil
- [ ] **Dashboard.razor** — cards operacionais em tempo real (vagas livres, movimentações abertas, OSs)
- [ ] **SuperAdmin.razor** — gestão de empresas, planos, configurações (igual SetiTrib)

### Média prioridade
- [ ] **Mapa visual real** — página `/mapa` com vagas do banco em tempo real via SignalR
- [ ] **Kanban de movimentações** — página `/movimentacoes` com fluxo completo
- [ ] **Cadastro de embarcações** — CRUD com cliente vinculado
- [ ] **Cadastro de vagas** — com editor visual (posição X/Y no mapa)
- [ ] **Ordens de serviço** — lavagem, abastecimento, manutenção
- [ ] **Integração WhatsApp** — webhook CodeChat → criação automática de movimentação

### Fase 2
- [ ] Financeiro (contratos, faturas, PIX, inadimplência)
- [ ] Relatórios e exportação CSV/PDF
- [ ] App PWA para clientes

### Fase 3
- [ ] Multilíngue completo nas telas internas (hoje só na Home)
- [ ] Integrações avançadas

---

## Sobre a SETICOM

- **Site:** https://seticom.com.br
- **Email:** contato@seticomtecnologia.com.br
- **Telefone:** (14) 3732-7510 · (14) 9 9804-9477
- **Endereço:** R. Rio de Janeiro, 1831 – Centro, Avaré – SP

---

## Outros Sistemas SETICOM (para referência de padrões)

| Sistema | Repo | URL | Porta Blazor | Porta PG |
|---|---|---|---|---|
| SetiDesk | albano-seticom/SetiDesk | setidesk.seticom.com.br | 9090 | 5434 |
| SetiTrib | albano-seticom/SetiTrib | setitrib.seticom.com.br | 9091 | 5435 |
| SetiMarine | albano-seticom/SetiMarine | setimarine.seticom.com.br | 9092 | 5436 |
