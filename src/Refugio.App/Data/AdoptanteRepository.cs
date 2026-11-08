using System;
using System.Collections.Generic;
using MySqlConnector;
using Refugio.App.Models;

namespace Refugio.App.Data;

public class AdoptanteRepository
{
    public List<Adoptante> GetAll(string? busqueda)
    {
        var adoptantes = new List<Adoptante>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, nombre, telefono, email, direccion, notas
            FROM adoptantes
            WHERE (@busqueda IS NULL
                   OR nombre LIKE CONCAT('%', @busqueda, '%')
                   OR COALESCE(email, '') LIKE CONCAT('%', @busqueda, '%'))
            ORDER BY nombre
            """;
        cmd.Parameters.AddWithValue("@busqueda",
            string.IsNullOrWhiteSpace(busqueda) ? DBNull.Value : busqueda.Trim());

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) adoptantes.Add(Map(reader));

        return adoptantes;
    }

    public int Insert(Adoptante adoptante)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO adoptantes (nombre, telefono, email, direccion, notas)
            VALUES (@nombre, @telefono, @email, @direccion, @notas)
            """;
        AddParams(cmd, adoptante);
        cmd.ExecuteNonQuery();

        using var idCmd = conn.CreateCommand();
        idCmd.CommandText = "SELECT LAST_INSERT_ID()";
        return Convert.ToInt32(idCmd.ExecuteScalar());
    }

    public void Update(Adoptante adoptante)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE adoptantes
            SET nombre = @nombre,
                telefono = @telefono,
                email = @email,
                direccion = @direccion,
                notas = @notas
            WHERE id = @id
            """;
        AddParams(cmd, adoptante);
        cmd.Parameters.AddWithValue("@id", adoptante.Id);
        cmd.ExecuteNonQuery();
    }

    private static void AddParams(MySqlCommand cmd, Adoptante adoptante)
    {
        cmd.Parameters.AddWithValue("@nombre", adoptante.Nombre);
        cmd.Parameters.AddWithValue("@telefono", (object?)adoptante.Telefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@email", (object?)adoptante.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@direccion", (object?)adoptante.Direccion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@notas", (object?)adoptante.Notas ?? DBNull.Value);
    }

    private static Adoptante Map(MySqlDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["id"]),
        Nombre = Convert.ToString(reader["nombre"]) ?? "",
        Telefono = reader["telefono"] == DBNull.Value ? null : Convert.ToString(reader["telefono"]),
        Email = reader["email"] == DBNull.Value ? null : Convert.ToString(reader["email"]),
        Direccion = reader["direccion"] == DBNull.Value ? null : Convert.ToString(reader["direccion"]),
        Notas = reader["notas"] == DBNull.Value ? null : Convert.ToString(reader["notas"])
    };
}
