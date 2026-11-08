using System;

namespace Refugio.App.Models;

public class Animal
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Especie { get; set; } = "perro";
    public string? Raza { get; set; }
    public string Sexo { get; set; } = "macho";
    public DateTime? FechaNacimiento { get; set; }
    public string? Descripcion { get; set; }
    public string? FotoUrl { get; set; }
    public string Estado { get; set; } = "disponible";
    public DateTime FechaIngreso { get; set; } = DateTime.Today;
    public int SolicitudesPendientes { get; set; }

    public string EdadTexto
    {
        get
        {
            if (FechaNacimiento is null) return "—";
            var years = (int)((DateTime.Today - FechaNacimiento.Value).TotalDays / 365.25);
            return years <= 0 ? "Menos de 1 año" : $"{years} años";
        }
    }

    public string FechaIngresoTexto => FechaIngreso.ToString("dd/MM/yyyy");

    public string EstadoTexto => Estado switch
    {
        "disponible" => "Disponible",
        "en_tratamiento" => "En tratamiento",
        "adoptado" => "Adoptado",
        _ => Estado
    };
}
