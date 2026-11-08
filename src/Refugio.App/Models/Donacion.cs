using System;

namespace Refugio.App.Models;

public class Donacion
{
    public int Id { get; set; }
    public string Donante { get; set; } = "";
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public string Metodo { get; set; } = "efectivo";
    public string? Nota { get; set; }
    public string? RegistradoPor { get; set; }

    public string FechaTexto => Fecha.ToString("dd/MM/yyyy");

    public string MontoTexto => "$" + Monto.ToString("N0");

    public string MetodoTexto => Metodo switch
    {
        "efectivo" => "Efectivo",
        "transferencia" => "Transferencia",
        "tarjeta" => "Tarjeta",
        _ => "Otro"
    };
}
