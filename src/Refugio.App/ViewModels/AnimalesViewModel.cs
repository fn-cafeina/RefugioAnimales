using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Data;
using Refugio.App.Models;

namespace Refugio.App.ViewModels;

public partial class AnimalesViewModel : ViewModelBase
{
    private readonly AnimalRepository _repo = new();
    private int? _editingId;

    public ObservableCollection<Animal> Animales { get; } = new();
    public IReadOnlyList<string> EstadosFiltro { get; } = new[] { "todos", "disponible", "en_tratamiento", "adoptado" };
    public IReadOnlyList<string> EspeciesFiltro { get; } = new[] { "todas", "perro", "gato", "conejo", "ave", "otro" };
    public IReadOnlyList<string> Especies { get; } = new[] { "perro", "gato", "conejo", "ave", "otro" };
    public IReadOnlyList<string> Sexos { get; } = new[] { "macho", "hembra" };

    [ObservableProperty]
    private string? busqueda;

    [ObservableProperty]
    private string estadoFiltro = "todos";

    [ObservableProperty]
    private string especieFiltro = "todas";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    [NotifyPropertyChangedFor(nameof(CanMarkTreatment))]
    [NotifyPropertyChangedFor(nameof(CanMarkAvailable))]
    private Animal? selectedAnimal;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? infoMessage;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private string editNombre = "";

    [ObservableProperty]
    private string editEspecie = "perro";

    [ObservableProperty]
    private string editRaza = "";

    [ObservableProperty]
    private string editSexo = "macho";

    [ObservableProperty]
    private DateTimeOffset? editFechaNacimiento;

    [ObservableProperty]
    private string editDescripcion = "";

    [ObservableProperty]
    private string editFotoUrl = "";

    [ObservableProperty]
    private DateTimeOffset? editFechaIngreso = DateTimeOffset.Now;

    public bool HasSelection => SelectedAnimal is not null;
    public bool CanMarkTreatment => SelectedAnimal?.Estado == "disponible";
    public bool CanMarkAvailable => SelectedAnimal?.Estado == "en_tratamiento";

    public AnimalesViewModel() => Load();

    partial void OnEstadoFiltroChanged(string value) => Load();

    partial void OnEspecieFiltroChanged(string value) => Load();

    [RelayCommand]
    private void Load()
    {
        try
        {
            ErrorMessage = null;
            var lista = _repo.GetAll(Busqueda, EstadoFiltro, EspecieFiltro);
            Animales.Clear();
            foreach (var animal in lista) Animales.Add(animal);
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron cargar los animales: " + ex.Message;
        }
    }

    [RelayCommand]
    private void New()
    {
        _editingId = null;
        SelectedAnimal = null;
        IsEditing = true;
        ErrorMessage = null;
        InfoMessage = null;

        EditNombre = "";
        EditEspecie = "perro";
        EditRaza = "";
        EditSexo = "macho";
        EditFechaNacimiento = null;
        EditDescripcion = "";
        EditFotoUrl = "";
        EditFechaIngreso = DateTimeOffset.Now;
    }

    [RelayCommand]
    private void EditSelected()
    {
        if (SelectedAnimal is null) return;

        _editingId = SelectedAnimal.Id;
        IsEditing = true;
        ErrorMessage = null;
        InfoMessage = null;

        EditNombre = SelectedAnimal.Nombre;
        EditEspecie = SelectedAnimal.Especie;
        EditRaza = SelectedAnimal.Raza ?? "";
        EditSexo = SelectedAnimal.Sexo;
        EditFechaNacimiento = SelectedAnimal.FechaNacimiento is null
            ? null
            : new DateTimeOffset(SelectedAnimal.FechaNacimiento.Value);
        EditDescripcion = SelectedAnimal.Descripcion ?? "";
        EditFotoUrl = SelectedAnimal.FotoUrl ?? "";
        EditFechaIngreso = new DateTimeOffset(SelectedAnimal.FechaIngreso);
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(EditNombre))
        {
            ErrorMessage = "El nombre del animal es obligatorio.";
            return;
        }

        if (EditFechaNacimiento is not null && EditFechaNacimiento.Value.Date > DateTime.Today)
        {
            ErrorMessage = "La fecha de nacimiento no puede ser futura.";
            return;
        }

        try
        {
            var animal = new Animal
            {
                Id = _editingId ?? 0,
                Nombre = EditNombre.Trim(),
                Especie = EditEspecie,
                Raza = string.IsNullOrWhiteSpace(EditRaza) ? null : EditRaza.Trim(),
                Sexo = EditSexo,
                FechaNacimiento = EditFechaNacimiento?.Date,
                Descripcion = string.IsNullOrWhiteSpace(EditDescripcion) ? null : EditDescripcion.Trim(),
                FotoUrl = string.IsNullOrWhiteSpace(EditFotoUrl) ? null : EditFotoUrl.Trim(),
                Estado = _editingId is null ? "disponible" : SelectedAnimal?.Estado ?? "disponible",
                FechaIngreso = (EditFechaIngreso ?? DateTimeOffset.Now).Date
            };

            if (_editingId is null)
            {
                _repo.Insert(animal);
                InfoMessage = "Animal registrado correctamente.";
            }
            else
            {
                _repo.Update(animal);
                InfoMessage = "Animal actualizado correctamente.";
            }

            IsEditing = false;
            _editingId = null;
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo guardar el animal: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsEditing = false;
        _editingId = null;
        ErrorMessage = null;
    }

    [RelayCommand]
    private void MarkTreatment() => ChangeEstado("en_tratamiento");

    [RelayCommand]
    private void MarkAvailable() => ChangeEstado("disponible");

    private void ChangeEstado(string estado)
    {
        if (SelectedAnimal is null) return;

        try
        {
            _repo.UpdateEstado(SelectedAnimal.Id, estado);
            InfoMessage = $"Estado de {SelectedAnimal.Nombre} actualizado a {estado.Replace('_', ' ')}.";
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo cambiar el estado: " + ex.Message;
        }
    }
}
