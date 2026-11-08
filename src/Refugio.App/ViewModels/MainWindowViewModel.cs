using CommunityToolkit.Mvvm.ComponentModel;
using Refugio.App.Models;
using Refugio.App.Services;

namespace Refugio.App.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private object? currentPage;

    public MainWindowViewModel() => ShowLogin();

    private void ShowLogin()
    {
        Session.CurrentUser = null;
        CurrentPage = new LoginViewModel(OnLoginSuccess);
    }

    private void OnLoginSuccess(Usuario usuario)
    {
        Session.CurrentUser = usuario;
        CurrentPage = new ShellViewModel(usuario, ShowLogin);
    }
}
