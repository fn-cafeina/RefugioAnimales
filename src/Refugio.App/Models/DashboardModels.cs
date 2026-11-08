namespace Refugio.App.Models;

public class ResumenDashboard
{
    public int AnimalesDisponibles { get; set; }
    public int AnimalesEnTratamiento { get; set; }
    public int SolicitudesPendientes { get; set; }
    public int AdopcionesMes { get; set; }
    public decimal DonacionesMes { get; set; }
    public decimal PromedioDiasAdopcion { get; set; }
}

public class BarraItem
{
    public string Etiqueta { get; set; } = "";
    public string ValorTexto { get; set; } = "";
    public double Altura { get; set; }
}

public class TopDonante
{
    public string Donante { get; set; } = "";
    public decimal Total { get; set; }
    public int Donaciones { get; set; }
    public int Posicion { get; set; }
}
