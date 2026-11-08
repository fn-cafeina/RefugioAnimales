using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Data;
using Refugio.App.Models;
using Refugio.App.Services;

namespace Refugio.App.ViewModels;

public partial class TratamientosViewModel : ViewModelBase
{
    private readonly TratamientoRepository _repo = new();
    private readonly AnimalRepository _animalesRepo = new();

    public ObservableCollection<Tratamiento> Tratamientos { get; } = new();
    public ObservableCollection<Animal> Animales { get; } = new();
    public IReadOnlyList<string> Tipos { get; } =
        new[] { "consulta", "vacunacion", "desparasitacion", "cirugia", "otro" };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    [NotifyPropertyChangedFor(nameof(CanComplete))]
    private Tratamiento? selectedTratamiento;

    [ObservableProperty]
    private Animal? selectedAnimalNuevo;

    [ObservableProperty]
    private string editTipo = "consulta";

    [ObservableProperty]
    private string editDescripcion = "";

    [ObservableProperty]
    private DateTimeOffset? editFecha = DateTimeOffset.Now;

    [ObservableProperty]
    private decimal? editCosto = 0;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? infoMessage;

    public bool CanEdit => Session.CurrentUser?.Rol is "admin" or "veterinario";
    public bool HasSelection => SelectedTratamiento is not null;
    public bool CanComplete => CanEdit && SelectedTratamiento?.Estado == "pendiente";

    public TratamientosViewModel() => Load();

    [RelayCommand]
    private void Load()
    {
        try
        {
            ErrorMessage = null;
            var lista = _repo.GetAll();
            Tratamientos.Clear();
            foreach (var tratamiento in lista) Tratamientos.Add(tratamiento);

            Animales.Clear();
            foreach (var animal in _animalesRepo.GetAll(null, "todos", "todas")) Animales.Add(animal);
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron cargar los tratamientos: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Create()
    {
        ErrorMessage = null;
        InfoMessage = null;

        if (!CanEdit)
        {
            ErrorMessage = "Solo veterinarios y administradores pueden registrar tratamientos.";
            return;
        }

        if (SelectedAnimalNuevo is null)
        {
            ErrorMessage = "Seleccioná un animal.";
            return;
        }

        if (string.IsNullOrWhiteSpace(EditDescripcion))
        {
            ErrorMessage = "La descripción del tratamiento es obligatoria.";
            return;
        }

        try
        {
            var tratamiento = new Tratamiento
            {
                AnimalId = SelectedAnimalNuevo.Id,
                VeterinarioId = Session.CurrentUser?.Rol == "veterinario" ? Session.CurrentUser.Id : null,
                Tipo = EditTipo,
                Descripcion = EditDescripcion.Trim(),
                Fecha = (EditFecha ?? DateTimeOffset.Now).Date,
                Costo = EditCosto ?? 0,
                Estado = "pendiente"
            };

            _repo.Insert(tratamiento);
            InfoMessage = "Tratamiento registrado correctamente.";
            SelectedAnimalNuevo = null;
            EditDescripcion = "";
            EditCosto = 0;
            EditFecha = DateTimeOffset.Now;
            EditTipo = "consulta";
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo registrar el tratamiento: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Complete()
    {
        if (SelectedTratamiento is null || !CanEdit) return;

        try
        {
            _repo.UpdateEstado(SelectedTratamiento.Id, "completado");
            InfoMessage = "Tratamiento marcado como completado.";
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo completar el tratamiento: " + ex.Message;
        }
    }
}
