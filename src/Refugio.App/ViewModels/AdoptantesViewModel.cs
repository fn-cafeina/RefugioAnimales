using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Data;
using Refugio.App.Models;

namespace Refugio.App.ViewModels;

public partial class AdoptantesViewModel : ViewModelBase
{
    private readonly AdoptanteRepository _repo = new();
    private int? _editingId;

    public ObservableCollection<Adoptante> Adoptantes { get; } = new();

    [ObservableProperty]
    private string? busqueda;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private Adoptante? selectedAdoptante;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? infoMessage;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private string editNombre = "";

    [ObservableProperty]
    private string editTelefono = "";

    [ObservableProperty]
    private string editEmail = "";

    [ObservableProperty]
    private string editDireccion = "";

    [ObservableProperty]
    private string editNotas = "";

    public bool HasSelection => SelectedAdoptante is not null;

    public AdoptantesViewModel() => Load();

    [RelayCommand]
    private void Load()
    {
        try
        {
            ErrorMessage = null;
            var lista = _repo.GetAll(Busqueda);
            Adoptantes.Clear();
            foreach (var adoptante in lista) Adoptantes.Add(adoptante);
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron cargar los adoptantes: " + ex.Message;
        }
    }

    [RelayCommand]
    private void New()
    {
        _editingId = null;
        SelectedAdoptante = null;
        IsEditing = true;
        ErrorMessage = null;
        InfoMessage = null;
        EditNombre = "";
        EditTelefono = "";
        EditEmail = "";
        EditDireccion = "";
        EditNotas = "";
    }

    [RelayCommand]
    private void EditSelected()
    {
        if (SelectedAdoptante is null) return;

        _editingId = SelectedAdoptante.Id;
        IsEditing = true;
        ErrorMessage = null;
        InfoMessage = null;
        EditNombre = SelectedAdoptante.Nombre;
        EditTelefono = SelectedAdoptante.Telefono ?? "";
        EditEmail = SelectedAdoptante.Email ?? "";
        EditDireccion = SelectedAdoptante.Direccion ?? "";
        EditNotas = SelectedAdoptante.Notas ?? "";
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(EditNombre))
        {
            ErrorMessage = "El nombre del adoptante es obligatorio.";
            return;
        }

        try
        {
            var adoptante = new Adoptante
            {
                Id = _editingId ?? 0,
                Nombre = EditNombre.Trim(),
                Telefono = string.IsNullOrWhiteSpace(EditTelefono) ? null : EditTelefono.Trim(),
                Email = string.IsNullOrWhiteSpace(EditEmail) ? null : EditEmail.Trim().ToLowerInvariant(),
                Direccion = string.IsNullOrWhiteSpace(EditDireccion) ? null : EditDireccion.Trim(),
                Notas = string.IsNullOrWhiteSpace(EditNotas) ? null : EditNotas.Trim()
            };

            if (_editingId is null)
            {
                _repo.Insert(adoptante);
                InfoMessage = "Adoptante registrado correctamente.";
            }
            else
            {
                _repo.Update(adoptante);
                InfoMessage = "Adoptante actualizado correctamente.";
            }

            IsEditing = false;
            _editingId = null;
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo guardar el adoptante: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsEditing = false;
        _editingId = null;
        ErrorMessage = null;
    }
}
