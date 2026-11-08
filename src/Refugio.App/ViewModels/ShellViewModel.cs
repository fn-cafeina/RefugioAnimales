using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Models;

namespace Refugio.App.ViewModels;

public partial class ShellViewModel : ViewModelBase
{
    private readonly Action _onLogout;

    public Usuario CurrentUser { get; }
    public string UserName => CurrentUser.Nombre;
    public string UserRole => CurrentUser.RolTexto;
    public bool IsAdmin => CurrentUser.Rol == "admin";

    [ObservableProperty]
    private object? currentPage;

    [ObservableProperty]
    private string pageTitle = "Panel";

    public ShellViewModel(Usuario usuario, Action onLogout)
    {
        CurrentUser = usuario;
        _onLogout = onLogout;
        Navigate("dashboard");
    }

    [RelayCommand]
    private void Navigate(string? page)
    {
        switch (page)
        {
            case "animales":
                CurrentPage = new AnimalesViewModel();
                PageTitle = "Animales";
                break;
            case "adoptantes":
                CurrentPage = new AdoptantesViewModel();
                PageTitle = "Adoptantes";
                break;
            case "solicitudes":
                CurrentPage = new SolicitudesViewModel();
                PageTitle = "Solicitudes de adopción";
                break;
            case "tratamientos":
                CurrentPage = new TratamientosViewModel();
                PageTitle = "Tratamientos";
                break;
            case "donaciones":
                CurrentPage = new DonacionesViewModel();
                PageTitle = "Donaciones";
                break;
            case "usuarios":
                CurrentPage = new UsuariosViewModel();
                PageTitle = "Usuarios";
                break;
            default:
                CurrentPage = new DashboardViewModel();
                PageTitle = "Panel";
                break;
        }
    }

    [RelayCommand]
    private void Logout() => _onLogout();
}
