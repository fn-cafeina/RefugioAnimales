using System;

namespace Refugio.App.Models;

public class Solicitud
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public string Animal { get; set; } = "";
    public int AdoptanteId { get; set; }
    public string Adoptante { get; set; } = "";
    public string Estado { get; set; } = "pendiente";
    public string? Motivo { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? ResueltaEn { get; set; }
    public string? ResueltaPor { get; set; }

    public string FechaSolicitudTexto => FechaSolicitud.ToString("dd/MM/yyyy");

    public string ResueltaEnTexto => ResueltaEn?.ToString("dd/MM/yyyy") ?? "";

    public string EstadoTexto => Estado switch
    {
        "pendiente" => "Pendiente",
        "aprobada" => "Aprobada",
        "rechazada" => "Rechazada",
        "cancelada" => "Cancelada",
        _ => Estado
    };
}
