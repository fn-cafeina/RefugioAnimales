using System;
using System.Collections.Generic;
using MySqlConnector;
using Refugio.App.Models;

namespace Refugio.App.Data;

public class TratamientoRepository
{
    public List<Tratamiento> GetAll()
    {
        var tratamientos = new List<Tratamiento>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT t.id, t.animal_id, a.nombre AS animal, t.veterinario_id,
                   u.nombre AS veterinario, t.tipo, t.descripcion, t.fecha, t.costo, t.estado
            FROM tratamientos t
            JOIN animales a ON a.id = t.animal_id
            LEFT JOIN usuarios u ON u.id = t.veterinario_id
            ORDER BY t.fecha DESC, t.id DESC
            """;

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) tratamientos.Add(Map(reader));

        return tratamientos;
    }

    public int Insert(Tratamiento tratamiento)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO tratamientos
                (animal_id, veterinario_id, tipo, descripcion, fecha, costo, estado)
            VALUES
                (@animalId, @veterinarioId, @tipo, @descripcion, @fecha, @costo, @estado)
            """;
        cmd.Parameters.AddWithValue("@animalId", tratamiento.AnimalId);
        cmd.Parameters.AddWithValue("@veterinarioId", (object?)tratamiento.VeterinarioId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@tipo", tratamiento.Tipo);
        cmd.Parameters.AddWithValue("@descripcion", tratamiento.Descripcion);
        cmd.Parameters.AddWithValue("@fecha", tratamiento.Fecha);
        cmd.Parameters.AddWithValue("@costo", tratamiento.Costo);
        cmd.Parameters.AddWithValue("@estado", tratamiento.Estado);
        cmd.ExecuteNonQuery();

        using var idCmd = conn.CreateCommand();
        idCmd.CommandText = "SELECT LAST_INSERT_ID()";
        return Convert.ToInt32(idCmd.ExecuteScalar());
    }

    public void UpdateEstado(int tratamientoId, string estado)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE tratamientos SET estado = @estado WHERE id = @id";
        cmd.Parameters.AddWithValue("@estado", estado);
        cmd.Parameters.AddWithValue("@id", tratamientoId);
        cmd.ExecuteNonQuery();
    }

    private static Tratamiento Map(MySqlDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["id"]),
        AnimalId = Convert.ToInt32(reader["animal_id"]),
        Animal = Convert.ToString(reader["animal"]) ?? "",
        VeterinarioId = reader["veterinario_id"] == DBNull.Value
            ? null
            : Convert.ToInt32(reader["veterinario_id"]),
        Veterinario = reader["veterinario"] == DBNull.Value ? null : Convert.ToString(reader["veterinario"]),
        Tipo = Convert.ToString(reader["tipo"]) ?? "otro",
        Descripcion = Convert.ToString(reader["descripcion"]) ?? "",
        Fecha = Convert.ToDateTime(reader["fecha"]),
        Costo = Convert.ToDecimal(reader["costo"]),
        Estado = Convert.ToString(reader["estado"]) ?? "pendiente"
    };
}
