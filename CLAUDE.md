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
- `ValorTotal` computed properties on entities must be `.Ignore()`d in `OnModelCreating`
- **Multi-tenancy:** every query must filter by `EmpresaId` — all entities have this field

## Key Patterns

### Services
- All services receive `ISetiMarineDbContext` via primary constructor (e.g., `public class FooService(ISetiMarineDbContext ctx)`)
- Register every new service as `AddScoped<FooService>()` in `Program.cs`
- Adding a new entity requires: entity class → `ISetiMarineDbContext` → `AppDbContext` (DbSet + `ToTable()`) → migration

### Razor Pages
- All authenticated pages: `@layout AppLayout`, `@rendermode @(new InteractiveServerRenderMode(prerender: false))`
- Public pages (login, home): `@layout EmptyLayout`
- Pages live under `SetiMarine.Web/Components/Pages/App/<Feature>/` with `Index.razor` + `Form.razor`
- Every page calls `await UserCtx.InitAsync()` in `OnInitializedAsync()` before using `UserCtx.EmpresaId`

### Authentication & Authorization
- Cookie-based auth (`CookieAuthenticationDefaults`)
- Login/logout handled by minimal API endpoints in `AuthEndpoints.cs` (`POST /auth/login`, `GET /auth/logout`), not Blazor pages
- Claims available via `UsuarioContextService` (scoped): `EmpresaId`, `UsuarioId`, `Nome`, `EmpresaNome`, roles
- Roles: `SuperAdmin`, `Admin`, `Operacional` — use `<AuthorizeView Roles="Admin,SuperAdmin">` in Razor

### Enums
All enums are in a single file: `SetiMarine.Domain/Enums/Enums.cs`

### Frontend / CSS
- Two CSS files: `app.css` (public/global base) and `app-layout.css` (authenticated app shell)
- Design tokens are CSS variables: `--navy-700`, `--ink-*`, `--line`, `--paper`, `--cream-*`, `--s-free`, `--s-maint`, `--s-mov`
- Badge classes: `badge`, `badge-free`, `badge-occ`, `badge-mov`, `badge-maint`
- Form layout classes: `form-section`, `form-section-title`, `form-row`, `field`, `form-actions`
- Table wrapper: `table-wrap` + standard `<table>`
- Buttons: `btn`, `btn-primary`, `btn-ghost`, `btn-danger`, `btn-sm`
- Page header: `page-head` with nested `page-head-actions`

### Canvas Maps
`mapa.js` and `dashboard.js` render the marina layout on HTML Canvas — these are not Blazor components. The pages load them via `<script>` tags and pass data as JSON attributes on the canvas element.

## Business Domain

Marina management SaaS. Core concepts:
- **Vagas** (berths): molhadas/secas, organized in **Corredores**, can hold multiple vessels
- **Embarcações** (vessels): belong to **Clientes**, have a fixed berth (`VagaFixaId`)
- **VagaEmbarcacao**: junction for the current occupancy (active vessels in a berth)
- **Movimentações**: lifecycle of a vessel leaving and returning (multi-step status machine)
- **Pedidos**: unified order entity for `Venda`, `Emprestimo`, and `OrdemServico` — contains `PedidoItem` (products) and `PedidoServico` (service steps)
- **Contratos**: rental agreements linking clients to berths with plan type
- **RegistroServico**: checklist-based service records with photos
