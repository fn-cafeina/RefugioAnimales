using System;

namespace Refugio.App.Models;

public class Tratamiento
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public string Animal { get; set; } = "";
    public int? VeterinarioId { get; set; }
    public string? Veterinario { get; set; }
    public string Tipo { get; set; } = "consulta";
    public string Descripcion { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.Today;
    public decimal Costo { get; set; }
    public string Estado { get; set; } = "pendiente";

    public string FechaTexto => Fecha.ToString("dd/MM/yyyy");

    public string CostoTexto => "$" + Costo.ToString("N0");

    public string EstadoTexto => Estado == "pendiente" ? "Pendiente" : "Completado";
}
