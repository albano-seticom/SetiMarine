using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace SetiMarine.Web.Services;

public class UsuarioContextService(AuthenticationStateProvider authProvider)
{
    private ClaimsPrincipal? _user;

    public async Task InitAsync()
    {
        if (_user is not null) return;
        var state = await authProvider.GetAuthenticationStateAsync();
        _user = state.User;
    }

    public int EmpresaId      => int.Parse(_user?.FindFirst("EmpresaId")?.Value ?? "0");
    public int UsuarioId      => int.Parse(_user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    public string Nome        => _user?.FindFirst(ClaimTypes.Name)?.Value ?? "";
    public string EmpresaNome => _user?.FindFirst("EmpresaNome")?.Value ?? "";
    public bool IsAdmin       => _user?.IsInRole("Admin") ?? false;
    public bool IsSuperAdmin  => _user?.IsInRole("SuperAdmin") ?? false;
    public bool IsOperacional => _user?.IsInRole("Operacional") ?? false;
}
