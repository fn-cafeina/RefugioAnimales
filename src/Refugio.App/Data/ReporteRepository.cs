using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Refugio.App.Models;

namespace Refugio.App.Data;

public class ReporteRepository
{
    public ResumenDashboard GetResumen()
    {
        using var conn = Db.Open();
        ResumenDashboard resumen;

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "sp_resumen_dashboard";
            cmd.CommandType = CommandType.StoredProcedure;

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return new ResumenDashboard();

            resumen = new ResumenDashboard
            {
                AnimalesDisponibles = Convert.ToInt32(reader["animales_disponibles"]),
                AnimalesEnTratamiento = Convert.ToInt32(reader["animales_en_tratamiento"]),
                SolicitudesPendientes = Convert.ToInt32(reader["solicitudes_pendientes"]),
                AdopcionesMes = Convert.ToInt32(reader["adopciones_mes"]),
                DonacionesMes = Convert.ToDecimal(reader["donaciones_mes"])
            };
        }

        resumen.PromedioDiasAdopcion = GetPromedioDiasAdopcion(conn);
        return resumen;
    }

    public List<(int Mes, int Adopciones)> GetAdopcionesPorMes(int anio)
    {
        var resultado = new List<(int, int)>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_adopciones_por_mes";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@p_anio", anio);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            resultado.Add((Convert.ToInt32(reader["mes"]), Convert.ToInt32(reader["adopciones"])));
        }

        return resultado;
    }

    public List<TopDonante> GetTopDonantes(int limite)
    {
        var resultado = new List<TopDonante>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_top_donantes";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@p_limite", limite);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            resultado.Add(new TopDonante
            {
                Donante = Convert.ToString(reader["donante"]) ?? "",
                Total = Convert.ToDecimal(reader["total"]),
                Donaciones = Convert.ToInt32(reader["donaciones"]),
                Posicion = Convert.ToInt32(reader["posicion"])
            });
        }

        return resultado;
    }

    public List<DonacionMensual> GetDonacionesMensuales(int meses)
    {
        var resultado = new List<DonacionMensual>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT mes, cantidad, total
            FROM v_donaciones_mensuales
            ORDER BY mes DESC
            LIMIT @meses
            """;
        cmd.Parameters.AddWithValue("@meses", meses);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            resultado.Add(new DonacionMensual
            {
                Mes = Convert.ToString(reader["mes"]) ?? "",
                Cantidad = Convert.ToInt32(reader["cantidad"]),
                Total = Convert.ToDecimal(reader["total"])
            });
        }

        resultado.Reverse();
        return resultado;
    }

    private static decimal GetPromedioDiasAdopcion(MySqlConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT COALESCE(ROUND(AVG(TIMESTAMPDIFF(DAY, a.fecha_ingreso, s.resuelta_en)), 1), 0) AS promedio
            FROM solicitudes_adopcion s
            JOIN animales a ON a.id = s.animal_id
            WHERE s.estado = 'aprobada' AND s.resuelta_en IS NOT NULL
            """;
        return Convert.ToDecimal(cmd.ExecuteScalar());
    }
}
