using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SetiMarine.Infrastructure.Data;
using SetiMarine.Infrastructure.Services;
using SetiMarine.Application.Services;
using SetiMarine.Domain.Data;
using SetiMarine.Web.Components;
using SetiMarine.Web.Endpoints;
using SetiMarine.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISetiMarineDbContext, AppDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath      = "/login";
        o.LogoutPath     = "/logout";
        o.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<UsuarioContextService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SecaoService>();
builder.Services.AddScoped<CorredorService>();
builder.Services.AddScoped<VagaService>();
builder.Services.AddScoped<AlocacaoService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<EmbarcacaoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ContratoService>();
builder.Services.AddScoped<OrdemServicoService>();
builder.Services.AddScoped<MovimentacaoService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<SeederService>();
builder.Services.AddScoped<SuperAdminService>();
builder.Services.AddScoped<PlanoContratoService>();
builder.Services.AddScoped<AgendamentoUsoService>();
builder.Services.AddScoped<ConsumoPlanoService>();
builder.Services.AddScoped<EstoqueService>();
builder.Services.AddScoped<CaixaService>();
builder.Services.AddScoped<ConfiguracaoMarinaService>();
builder.Services.AddScoped<NotaService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var dbContext = dbContextFactory.CreateDbContext();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<SeederService>();
    await seeder.SeedSuperAdminAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
