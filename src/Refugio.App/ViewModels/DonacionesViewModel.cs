using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Data;
using Refugio.App.Models;
using Refugio.App.Services;

namespace Refugio.App.ViewModels;

public partial class DonacionesViewModel : ViewModelBase
{
    private readonly DonacionRepository _repo = new();

    public ObservableCollection<Donacion> Donaciones { get; } = new();
    public IReadOnlyList<string> Metodos { get; } =
        new[] { "efectivo", "transferencia", "tarjeta", "otro" };

    [ObservableProperty]
    private string editDonante = "";

    [ObservableProperty]
    private decimal? editMonto = 0;

    [ObservableProperty]
    private DateTimeOffset? editFecha = DateTimeOffset.Now;

    [ObservableProperty]
    private string editMetodo = "efectivo";

    [ObservableProperty]
    private string editNota = "";

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private string? infoMessage;

    public DonacionesViewModel() => Load();

    [RelayCommand]
    private void Load()
    {
        try
        {
            ErrorMessage = null;
            var lista = _repo.GetAll();
            Donaciones.Clear();
            foreach (var donacion in lista) Donaciones.Add(donacion);
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron cargar las donaciones: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(EditDonante))
        {
            ErrorMessage = "El nombre del donante es obligatorio.";
            return;
        }

        if (EditMonto is null or <= 0)
        {
            ErrorMessage = "El monto debe ser mayor que cero.";
            return;
        }

        try
        {
            var donacion = new Donacion
            {
                Donante = EditDonante.Trim(),
                Monto = EditMonto.Value,
                Fecha = (EditFecha ?? DateTimeOffset.Now).Date,
                Metodo = EditMetodo,
                Nota = string.IsNullOrWhiteSpace(EditNota) ? null : EditNota.Trim()
            };

            _repo.Insert(donacion, Session.CurrentUser?.Id);
            InfoMessage = "Donación registrada correctamente.";
            EditDonante = "";
            EditMonto = 0;
            EditFecha = DateTimeOffset.Now;
            EditMetodo = "efectivo";
            EditNota = "";
            Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudo registrar la donación: " + ex.Message;
        }
    }
}
