using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SetiMarine.Application.Services;

namespace SetiMarine.Web.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // POST /auth/login â€” recebe form, valida, seta cookie e redireciona
        app.MapPost("/auth/login", async (HttpContext ctx, AuthService authService) =>
        {
            var form = await ctx.Request.ReadFormAsync();
            var email = form["email"].ToString();
            var senha = form["senha"].ToString();
            var returnUrl = form["returnUrl"].ToString();

            if (string.IsNullOrWhiteSpace(returnUrl) || !returnUrl.StartsWith("/"))
                returnUrl = "/vagas";

            var (principal, erro) = await authService.LoginAsync(email, senha);

            if (erro != null)
            {
                ctx.Response.Redirect($"/login?erro={Uri.EscapeDataString(erro)}");
                return;
            }

            await ctx.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal!,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
                });

            ctx.Response.Redirect(returnUrl);
        });

        // GET /auth/logout
        app.MapGet("/auth/logout", async (HttpContext ctx) =>
        {
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ctx.Response.Redirect("/login");
        });
    }
}

