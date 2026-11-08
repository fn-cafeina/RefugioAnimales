using Refugio.App.Data;
using Refugio.App.Models;

namespace Refugio.App.Services;

public class AuthService
{
    private readonly UsuarioRepository _usuarios = new();

    public Usuario? Login(string email, string password)
    {
        var usuario = _usuarios.GetByEmail(email);
        if (usuario is null || !usuario.Activo) return null;

        return PasswordHasher.Verify(password, usuario.PasswordHash) ? usuario : null;
    }
}
