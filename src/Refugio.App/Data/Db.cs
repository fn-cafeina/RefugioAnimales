using System;
using System.IO;
using System.Text.Json;
using MySqlConnector;

namespace Refugio.App.Data;

public sealed class DbSettings
{
    public string ConnectionString { get; set; } = "";
}

public static class Db
{
    public static string ConnectionString { get; }

    static Db()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("No se encontró appsettings.json junto al ejecutable.", path);
        }

        var settings = JsonSerializer.Deserialize<DbSettings>(File.ReadAllText(path))
            ?? throw new InvalidOperationException("appsettings.json inválido.");

        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
        {
            throw new InvalidOperationException("Falta ConnectionString en appsettings.json.");
        }

        ConnectionString = settings.ConnectionString;
    }

    public static MySqlConnection Open()
    {
        var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
