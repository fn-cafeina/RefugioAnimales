using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Data;
using Refugio.App.Models;
using Refugio.App.Services;

namespace Refugio.App.ViewModels;

public partial class SolicitudesViewModel : ViewModelBase
{
    private readonly SolicitudRepository _repo = new();
    private readonly AnimalRepository _animalesRepo = new();
    private readonly AdoptanteRepository _adoptantesRepo = new();

    public ObservableCollection<Solicitud> Solicitudes { get; } = new();
    public ObservableCollection<Animal> AnimalesDisponibles { get; } = new();
    public ObservableCollection<Adoptante> Adoptantes { get; } = new();
    public IReadOnlyList<string> EstadosFiltro { get; } =
        new[] { "todas", "pendiente", "aprobada", "rechazada", "cancelada" };

    [ObservableProperty]
    private string estadoFiltro = "todas";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    [NotifyPropertyChangedFor(nameof(CanResolve))]
    private Solicitud? selectedSolicitud;

    [ObservableProperty]
    private Animal? selectedAnimalNuevo;

    [ObservableProperty]
    private Adoptante? selectedAdoptanteNuevo;

    [ObservableProperty]
    private string motivo = "";

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? infoMessage;

    public bool HasSelection => SelectedSolicitud is not null;
    public bool CanResolve => SelectedSolicitud?.Estado == "pendiente";

    public SolicitudesViewModel() => Load();

    partial void OnEstadoFiltroChanged(string value) => Load();

    [RelayCommand]
    private void Load()
    {
        try
        {
            ErrorMessage = null;
            var lista = _repo.GetAll(EstadoFiltro);
            Solicitudes.Clear();
            foreach (var solicitud in lista) Solicitudes.Add(solicitud);

            AnimalesDisponibles.Clear();
            foreach (var animal in _animalesRepo.GetDisponibles()) AnimalesDisponibles.Add(animal);

            Adoptantes.Clear();
            foreach (var adoptante in _adoptantesRepo.GetAll(null)) Adoptantes.Add(adoptante);
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron cargar las solicitudes: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Create()
    {
        ErrorMessage = null;
        InfoMessage = null;

        if (SelectedAnimalNuevo is null || SelectedAdoptanteNuevo is null)
        {
            ErrorMessage = "Seleccioná un animal disponible y un adoptante.";
            return;
        }

        try
        {
            _repo.Insert(SelectedAnimalNuevo.Id, SelectedAdoptanteNuevo.Id);
            InfoMessage = $"Solicitud registrada para {SelectedAnimalNuevo.Nombre}.";
            SelectedAnimalNuevo = null;
            SelectedAdoptanteNuevo = null;
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo registrar la solicitud: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Approve()
    {
        if (SelectedSolicitud is null || Session.CurrentUser is null) return;

        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            var animal = SelectedSolicitud.Animal;
            _repo.Aprobar(SelectedSolicitud.Id, Session.CurrentUser.Id);
            InfoMessage = $"Adopción de {animal} aprobada. El animal quedó adoptado y las solicitudes rivales fueron rechazadas.";
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo aprobar la solicitud: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Reject()
    {
        if (SelectedSolicitud is null || Session.CurrentUser is null) return;

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(Motivo))
        {
            ErrorMessage = "Indicá un motivo para rechazar la solicitud.";
            return;
        }

        try
        {
            _repo.Resolver(SelectedSolicitud.Id, "rechazada", Motivo.Trim(), Session.CurrentUser.Id);
            InfoMessage = "Solicitud rechazada.";
            Motivo = "";
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo rechazar la solicitud: " + ex.Message;
        }
    }
}
