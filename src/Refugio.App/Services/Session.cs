namespace Refugio.App.Services;

using Refugio.App.Models;

public static class Session
{
    public static Usuario? CurrentUser { get; set; }

    public static bool IsAdmin => CurrentUser?.Rol == "admin";
    public static bool IsVeterinario => CurrentUser?.Rol == "veterinario";
}
