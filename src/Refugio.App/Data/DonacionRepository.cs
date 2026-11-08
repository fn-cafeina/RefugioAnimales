using System;
using System.Collections.Generic;
using MySqlConnector;
using Refugio.App.Models;

namespace Refugio.App.Data;

public class DonacionRepository
{
    public List<Donacion> GetAll()
    {
        var donaciones = new List<Donacion>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT d.id, d.donante, d.monto, d.fecha, d.metodo, d.nota,
                   u.nombre AS registrado_por
            FROM donaciones d
            LEFT JOIN usuarios u ON u.id = d.registrado_por
            ORDER BY d.fecha DESC, d.id DESC
            """;

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) donaciones.Add(Map(reader));

        return donaciones;
    }

    public int Insert(Donacion donacion, int? usuarioId)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO donaciones (donante, monto, fecha, metodo, nota, registrado_por)
            VALUES (@donante, @monto, @fecha, @metodo, @nota, @registradoPor)
            """;
        cmd.Parameters.AddWithValue("@donante", donacion.Donante);
        cmd.Parameters.AddWithValue("@monto", donacion.Monto);
        cmd.Parameters.AddWithValue("@fecha", donacion.Fecha);
        cmd.Parameters.AddWithValue("@metodo", donacion.Metodo);
        cmd.Parameters.AddWithValue("@nota", (object?)donacion.Nota ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@registradoPor", (object?)usuarioId ?? DBNull.Value);
        cmd.ExecuteNonQuery();

        using var idCmd = conn.CreateCommand();
        idCmd.CommandText = "SELECT LAST_INSERT_ID()";
        return Convert.ToInt32(idCmd.ExecuteScalar());
    }

    private static Donacion Map(MySqlDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["id"]),
        Donante = Convert.ToString(reader["donante"]) ?? "",
        Monto = Convert.ToDecimal(reader["monto"]),
        Fecha = Convert.ToDateTime(reader["fecha"]),
        Metodo = Convert.ToString(reader["metodo"]) ?? "efectivo",
        Nota = reader["nota"] == DBNull.Value ? null : Convert.ToString(reader["nota"]),
        RegistradoPor = reader["registrado_por"] == DBNull.Value
            ? null
            : Convert.ToString(reader["registrado_por"])
    };
}
