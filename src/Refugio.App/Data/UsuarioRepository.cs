using System;
using System.Collections.Generic;
using MySqlConnector;
using Refugio.App.Models;

namespace Refugio.App.Data;

public class UsuarioRepository
{
    public List<Usuario> GetAll()
    {
        var usuarios = new List<Usuario>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, nombre, email, password_hash, rol, activo
            FROM usuarios
            ORDER BY nombre
            """;

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) usuarios.Add(Map(reader));

        return usuarios;
    }

    public Usuario? GetByEmail(string email)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, nombre, email, password_hash, rol, activo
            FROM usuarios
            WHERE email = @email
            LIMIT 1
            """;
        cmd.Parameters.AddWithValue("@email", email);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public int Insert(Usuario usuario)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO usuarios (nombre, email, password_hash, rol, activo)
            VALUES (@nombre, @email, @password, @rol, @activo)
            """;
        cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
        cmd.Parameters.AddWithValue("@email", usuario.Email);
        cmd.Parameters.AddWithValue("@password", usuario.PasswordHash);
        cmd.Parameters.AddWithValue("@rol", usuario.Rol);
        cmd.Parameters.AddWithValue("@activo", usuario.Activo);
        cmd.ExecuteNonQuery();

        using var idCmd = conn.CreateCommand();
        idCmd.CommandText = "SELECT LAST_INSERT_ID()";
        return Convert.ToInt32(idCmd.ExecuteScalar());
    }

    public void Update(Usuario usuario)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE usuarios
            SET nombre = @nombre,
                email = @email,
                password_hash = @password,
                rol = @rol,
                activo = @activo
            WHERE id = @id
            """;
        cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
        cmd.Parameters.AddWithValue("@email", usuario.Email);
        cmd.Parameters.AddWithValue("@password", usuario.PasswordHash);
        cmd.Parameters.AddWithValue("@rol", usuario.Rol);
        cmd.Parameters.AddWithValue("@activo", usuario.Activo);
        cmd.Parameters.AddWithValue("@id", usuario.Id);
        cmd.ExecuteNonQuery();
    }

    private static Usuario Map(MySqlDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["id"]),
        Nombre = Convert.ToString(reader["nombre"]) ?? "",
        Email = Convert.ToString(reader["email"]) ?? "",
        PasswordHash = Convert.ToString(reader["password_hash"]) ?? "",
        Rol = Convert.ToString(reader["rol"]) ?? "voluntario",
        Activo = Convert.ToBoolean(reader["activo"])
    };
}
