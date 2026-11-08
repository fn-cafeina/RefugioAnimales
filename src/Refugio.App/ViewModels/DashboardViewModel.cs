using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refugio.App.Data;
using Refugio.App.Models;

namespace Refugio.App.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private static readonly string[] MesesCortos =
        { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

    private readonly ReporteRepository _reportes = new();

    [ObservableProperty]
    private ResumenDashboard resumen = new();

    [ObservableProperty]
    private string? errorMessage;

    public ObservableCollection<BarraItem> AdopcionesBarras { get; } = new();
    public ObservableCollection<BarraItem> DonacionesBarras { get; } = new();
    public ObservableCollection<TopDonante> TopDonantes { get; } = new();

    public DashboardViewModel() => Load();

    [RelayCommand]
    private void Load()
    {
        try
        {
            ErrorMessage = null;
            Resumen = _reportes.GetResumen();

            var adopciones = _reportes.GetAdopcionesPorMes(DateTime.Today.Year);
            var maxAdopciones = Math.Max(1, adopciones.Count == 0 ? 0 : adopciones.Max(x => x.Adopciones));

            AdopcionesBarras.Clear();
            for (var mes = 1; mes <= 12; mes++)
            {
                var valor = adopciones.FirstOrDefault(x => x.Mes == mes).Adopciones;
                AdopcionesBarras.Add(new BarraItem
                {
                    Etiqueta = MesesCortos[mes - 1],
                    ValorTexto = valor.ToString(),
                    Altura = valor == 0 ? 2 : 12 + 90.0 * valor / maxAdopciones
                });
            }

            var donaciones = _reportes.GetDonacionesMensuales(6);
            var maxDonacion = Math.Max(1m, donaciones.Count == 0 ? 0 : donaciones.Max(x => x.Total));

            DonacionesBarras.Clear();
            foreach (var donacion in donaciones)
            {
                DonacionesBarras.Add(new BarraItem
                {
                    Etiqueta = FormatearMes(donacion.Mes),
                    ValorTexto = "$" + donacion.Total.ToString("N0"),
                    Altura = donacion.Total == 0 ? 2 : 12 + 90.0 * (double)(donacion.Total / maxDonacion)
                });
            }

            TopDonantes.Clear();
            foreach (var donante in _reportes.GetTopDonantes(5))
            {
                TopDonantes.Add(donante);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "No se pudieron cargar los indicadores: " + ex.Message;
        }
    }

    private static string FormatearMes(string mes)
    {
        if (mes.Length < 7) return mes;
        return int.TryParse(mes[5..7], out var numero) && numero is >= 1 and <= 12
            ? MesesCortos[numero - 1]
            : mes;
    }
}
