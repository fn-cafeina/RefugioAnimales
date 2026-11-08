namespace Refugio.App.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Rol { get; set; } = "voluntario";
    public bool Activo { get; set; } = true;

    public string RolTexto => Rol switch
    {
        "admin" => "Administrador",
        "veterinario" => "Veterinario",
        "voluntario" => "Voluntario",
        _ => Rol
    };

    public string ActivoTexto => Activo ? "Sí" : "No";
}
