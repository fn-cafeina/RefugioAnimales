using System;
using System.Collections.Generic;
using MySqlConnector;
using Refugio.App.Models;

namespace Refugio.App.Data;

public class AnimalRepository
{
    public List<Animal> GetAll(string? busqueda, string estado, string especie)
    {
        var animales = new List<Animal>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, nombre, especie, raza, sexo, fecha_nacimiento, descripcion,
                   foto_url, estado, fecha_ingreso, solicitudes_pendientes
            FROM v_animales_catalogo
            WHERE (@busqueda IS NULL
                   OR nombre LIKE CONCAT('%', @busqueda, '%')
                   OR COALESCE(raza, '') LIKE CONCAT('%', @busqueda, '%'))
              AND (@estado = 'todos' OR estado = @estado)
              AND (@especie = 'todas' OR especie = @especie)
            ORDER BY nombre
            """;
        cmd.Parameters.AddWithValue("@busqueda",
            string.IsNullOrWhiteSpace(busqueda) ? DBNull.Value : busqueda.Trim());
        cmd.Parameters.AddWithValue("@estado", estado);
        cmd.Parameters.AddWithValue("@especie", especie);

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) animales.Add(Map(reader));

        return animales;
    }

    public List<Animal> GetDisponibles()
    {
        var animales = new List<Animal>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, nombre, especie, raza, sexo, fecha_nacimiento, descripcion,
                   foto_url, estado, fecha_ingreso, solicitudes_pendientes
            FROM v_animales_catalogo
            WHERE estado = 'disponible'
            ORDER BY nombre
            """;

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) animales.Add(Map(reader));

        return animales;
    }

    public int Insert(Animal animal)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO animales
                (nombre, especie, raza, sexo, fecha_nacimiento, descripcion, foto_url, estado, fecha_ingreso)
            VALUES
                (@nombre, @especie, @raza, @sexo, @fechaNacimiento, @descripcion, @fotoUrl, 'disponible', @fechaIngreso)
            """;
        AddParams(cmd, animal);
        cmd.ExecuteNonQuery();

        using var idCmd = conn.CreateCommand();
        idCmd.CommandText = "SELECT LAST_INSERT_ID()";
        return Convert.ToInt32(idCmd.ExecuteScalar());
    }

    public void Update(Animal animal)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE animales
            SET nombre = @nombre,
                especie = @especie,
                raza = @raza,
                sexo = @sexo,
                fecha_nacimiento = @fechaNacimiento,
                descripcion = @descripcion,
                foto_url = @fotoUrl,
                estado = @estado,
                fecha_ingreso = @fechaIngreso
            WHERE id = @id
            """;
        AddParams(cmd, animal);
        cmd.Parameters.AddWithValue("@estado", animal.Estado);
        cmd.Parameters.AddWithValue("@id", animal.Id);
        cmd.ExecuteNonQuery();
    }

    public void UpdateEstado(int animalId, string estado)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE animales SET estado = @estado WHERE id = @id";
        cmd.Parameters.AddWithValue("@estado", estado);
        cmd.Parameters.AddWithValue("@id", animalId);
        cmd.ExecuteNonQuery();
    }

    private static void AddParams(MySqlCommand cmd, Animal animal)
    {
        cmd.Parameters.AddWithValue("@nombre", animal.Nombre);
        cmd.Parameters.AddWithValue("@especie", animal.Especie);
        cmd.Parameters.AddWithValue("@raza", (object?)animal.Raza ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@sexo", animal.Sexo);
        cmd.Parameters.AddWithValue("@fechaNacimiento", (object?)animal.FechaNacimiento ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@descripcion", (object?)animal.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@fotoUrl", (object?)animal.FotoUrl ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@fechaIngreso", animal.FechaIngreso);
    }

    private static Animal Map(MySqlDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["id"]),
        Nombre = Convert.ToString(reader["nombre"]) ?? "",
        Especie = Convert.ToString(reader["especie"]) ?? "otro",
        Raza = reader["raza"] == DBNull.Value ? null : Convert.ToString(reader["raza"]),
        Sexo = Convert.ToString(reader["sexo"]) ?? "macho",
        FechaNacimiento = reader["fecha_nacimiento"] == DBNull.Value
            ? null
            : Convert.ToDateTime(reader["fecha_nacimiento"]),
        Descripcion = reader["descripcion"] == DBNull.Value ? null : Convert.ToString(reader["descripcion"]),
        FotoUrl = reader["foto_url"] == DBNull.Value ? null : Convert.ToString(reader["foto_url"]),
        Estado = Convert.ToString(reader["estado"]) ?? "disponible",
        FechaIngreso = Convert.ToDateTime(reader["fecha_ingreso"]),
        SolicitudesPendientes = Convert.ToInt32(reader["solicitudes_pendientes"])
    };
}
