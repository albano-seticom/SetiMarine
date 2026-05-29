# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run the application (from repo root)
dotnet run --project SetiMarine.Web

# Build entire solution
dotnet build SetiMarine.sln

# Add a migration (always run from repo root)
dotnet ef migrations add <Name> --project SetiMarine.Infrastructure --startup-project SetiMarine.Web

# Remove last migration
dotnet ef migrations remove --project SetiMarine.Infrastructure --startup-project SetiMarine.Web

# Apply migrations manually (normally auto-applied on startup)
dotnet ef database update --project SetiMarine.Infrastructure --startup-project SetiMarine.Web
```

There are no automated tests in this project.

## Architecture

Four-project clean architecture solution:

```
SetiMarine.Domain        → Entities, Enums, ISetiMarineDbContext interface
SetiMarine.Application   → Service classes (business logic + data access)
SetiMarine.Infrastructure → AppDbContext (EF Core + Npgsql), Migrations
SetiMarine.Web           → Blazor Server app (pages, layouts, static assets)
```

**Dependency direction:** Web → Application → Domain ← Infrastructure

## Database

- PostgreSQL via Npgsql EF Core
- `appsettings.Development.json` overrides `DefaultConnection` to `192.168.0.177:5436` for local dev
- Migrations run **automatically** on startup via `MigrateAsync()` in `Program.cs` — no manual apply needed after adding a migration
- All tables use Portuguese prefix naming: `sis_*` (sistema), `mar_*` (marina), `cfg_*` (configuração)
- `ValorTotal` computed properties on entities must be `.Ignore()`d in `OnModelCreating` — affects `PedidoItem` and `VendaProduto`
- **Multi-tenancy:** every query must filter by `EmpresaId` — all entities have this field
- Unique constraints: `(EmpresaId, Email)` on Usuario; `(EmpresaId, Codigo)` on Vaga
- Adding a new entity requires: entity class → `ISetiMarineDbContext` → `AppDbContext` (DbSet + `ToTable()` + relationships) → migration

## Key Patterns

### Services
- All services receive `ISetiMarineDbContext` via primary constructor (e.g., `public class FooService(ISetiMarineDbContext ctx)`)
- Register every new service as `AddScoped<FooService>()` in `Program.cs`
- `SeederService` populates default marina catalog (produtos + TipoServicoConfig) — idempotent via `AnyAsync` guard

### Razor Pages
- All authenticated pages: `@layout AppLayout`, `@rendermode @(new InteractiveServerRenderMode(prerender: false))`
- Public pages (login, home, cadastro): `@layout EmptyLayout`
- Pages live under `SetiMarine.Web/Components/Pages/App/<Feature>/` with `Index.razor` + `Form.razor`
- Every page calls `await UserCtx.InitAsync()` in `OnInitializedAsync()` before using `UserCtx.EmpresaId`
- **Print pages**: use `@layout EmptyLayout` with inline print CSS — see `OrdensServico/Imprimir.razor`
- **PDV form** (`Pedidos/Form.razor`): two-column grid layout (300px catalog + `1fr` panel), both columns fixed height via `height: min(540px, calc(100vh - 220px))` on the grid wrapper with `align-items: stretch`. Items area scrolls; footer (totals + actions) stays pinned. Select fields use `value="@_var" @onchange="..."` (not `@bind:after`) to avoid Blazor rendering bugs.

### Select Binding
Use `value="@_var" @onchange="e => _var = e.Value?.ToString() ?? \"\""` instead of `@bind:after` for selects that trigger side-effects. `@bind:after` with void methods causes the selected value to visually reset in some Blazor Server scenarios.

### Authentication & Authorization
- Cookie-based auth (`CookieAuthenticationDefaults`)
- Login/logout handled by minimal API endpoints in `AuthEndpoints.cs` (`POST /auth/login`, `GET /auth/logout`), not Blazor pages
- Claims available via `UsuarioContextService` (scoped): `EmpresaId`, `UsuarioId`, `Nome`, `EmpresaNome`, roles
- Roles: `SuperAdmin`, `Admin`, `Operacional` — use `<AuthorizeView Roles="Admin,SuperAdmin">` in Razor
- `SessaoAtiva` entity tracks active sessions with token, IP, UserAgent (12-hour TTL)

### Enums
All enums are in a single file: `SetiMarine.Domain/Enums/Enums.cs`. Key enums:
- `TipoPedido`: Venda, Emprestimo, OrdemServico
- `StatusPedido`: Rascunho, Aberto, EmAndamento, Concluido, Cancelado
- `TipoItemPedido`: Venda, Emprestimo (item-level, different from order-level TipoPedido)
- `CategoriaProduto`: Acessorio, Combustivel, Lubrificante, Peca, Equipamento, Alimento, Outro
- `TipoServico`: LimpezaQuinzenal, LimpezaPosPasseio, VistoriaPeriodica, ChecklistSaida, ChecklistRetorno, Abastecimento, Outro
- `StatusItemChecklist`: Pendente, Ok, Problema, NaoAplica
- `StatusAluguel`: Ativo, Devolvido, Atrasado
- `StatusAluguel` / `TipoTransacao` (Venda, Aluguel) used in `VendaProduto`

### Frontend / CSS
- Two CSS files: `app.css` (public/global base) and `app-layout.css` (authenticated app shell)
- App shell: CSS grid with `grid-template-columns: 310px 1fr` (sidebar + content) and `grid-template-rows: 78px 1fr` (topbar + main)
- Design tokens: `--navy-700`, `--ink-*`, `--line`, `--paper`, `--cream-*`, `--s-free`, `--s-maint`, `--s-mov`
- Badge classes: `badge`, `badge-free`, `badge-occ`, `badge-mov`, `badge-maint`
- Form layout: `form-section`, `form-section-title`, `form-row`, `field`, `form-actions`
- Table wrapper: `table-wrap` + standard `<table>`
- Buttons: `btn`, `btn-primary`, `btn-ghost`, `btn-danger`, `btn-sm`
- Page header: `page-head` with nested `page-head-actions`; breadcrumbs use flex row with `align-items: baseline`
- For flex overflow/scroll to work inside a flex container, child must have `min-height: 0`

### Canvas Maps
`mapa.js` and `dashboard.js` render the marina layout on HTML Canvas — these are not Blazor components. Pages load them via `<script>` tags and pass data as JSON attributes on the canvas element. `Vagas/Layout.razor` is the interactive version with berth selection.

## Business Domain

Marina management SaaS. Core concepts:

- **Vagas** (berths): molhadas/secas, organized in **Corredores**, identified by `Codigo`; `VagaEmbarcacao` junction tracks current occupancy
- **Embarcações** (vessels): belong to **Clientes**, have an optional fixed berth (`VagaFixaId`)
- **Movimentações**: vessel departure/return lifecycle (multi-step status machine); optionally linked to a `RegistroServico`
- **Pedidos**: unified order for `Venda`, `Emprestimo`, `OrdemServico` — child collections `PedidoItem` (products with qty/price/loan dates) and `PedidoServico` (service steps with optional valor). `EditarAsync` removes and re-adds both collections to replace them cleanly.
- **Contratos**: rental agreements linking Clientes to Vagas with plan type
- **RegistroServico**: checklist-based service records with `FotoServico` (photos, `EhAvaria` flag for damage), `RegistroServicoItem` (checklist items per `ChecklistTemplate`), linked optionally to Embarcacao, Vaga, TipoServicoConfig, and Movimentacao
- **TipoServicoConfig**: per-empresa service type configuration (frequency in days, linked `ChecklistTemplate`)
- **ChecklistTemplate / ChecklistTemplateItem**: reusable checklist definitions attached to service types
- **VendaProduto**: product sale/rental transactions (`TipoTransacao`, `StatusAluguel` for active loans)
- **Empresa**: has `InstanciaWhatsApp` for future WhatsApp integration
- **Configuracao**: key-value store per empresa for arbitrary settings
