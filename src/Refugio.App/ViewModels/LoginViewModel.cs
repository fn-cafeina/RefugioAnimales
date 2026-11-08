using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Models;
using Refugio.App.Services;

namespace Refugio.App.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly Action<Usuario> _onSuccess;
    private readonly AuthService _authService = new();

    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isBusy;

    public LoginViewModel(Action<Usuario> onSuccess) => _onSuccess = onSuccess;

    [RelayCommand]
    private void Login()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Ingresá tu email y contraseña.";
            return;
        }

        try
        {
            IsBusy = true;
            var usuario = _authService.Login(Email.Trim().ToLowerInvariant(), Password);
            if (usuario is null)
            {
                ErrorMessage = "Credenciales inválidas o usuario inactivo.";
                return;
            }

            _onSuccess(usuario);
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo conectar con la base de datos: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
