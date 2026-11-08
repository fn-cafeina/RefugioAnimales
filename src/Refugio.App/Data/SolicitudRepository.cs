using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Refugio.App.Models;

namespace Refugio.App.Data;

public class SolicitudRepository
{
    public List<Solicitud> GetAll(string estado)
    {
        var solicitudes = new List<Solicitud>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT id, animal_id, animal, adoptante_id, adoptante, estado, motivo,
                   fecha_solicitud, resuelta_en, resuelta_por_nombre
            FROM v_solicitudes_detalle
            WHERE (@estado = 'todas' OR estado = @estado)
            ORDER BY fecha_solicitud DESC
            """;
        cmd.Parameters.AddWithValue("@estado", estado);

        using var reader = cmd.ExecuteReader();
        while (reader.Read()) solicitudes.Add(Map(reader));

        return solicitudes;
    }

    public int Insert(int animalId, int adoptanteId)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO solicitudes_adopcion (animal_id, adoptante_id, estado)
            VALUES (@animalId, @adoptanteId, 'pendiente')
            """;
        cmd.Parameters.AddWithValue("@animalId", animalId);
        cmd.Parameters.AddWithValue("@adoptanteId", adoptanteId);
        cmd.ExecuteNonQuery();

        using var idCmd = conn.CreateCommand();
        idCmd.CommandText = "SELECT LAST_INSERT_ID()";
        return Convert.ToInt32(idCmd.ExecuteScalar());
    }

    public void Aprobar(int solicitudId, int usuarioId)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_aprobar_solicitud";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@p_solicitud_id", solicitudId);
        cmd.Parameters.AddWithValue("@p_usuario_id", usuarioId);
        cmd.ExecuteNonQuery();
    }

    public void Resolver(int solicitudId, string estado, string? motivo, int usuarioId)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            UPDATE solicitudes_adopcion
            SET estado = @estado,
                motivo = @motivo,
                resuelta_por = @usuarioId
            WHERE id = @id
            """;
        cmd.Parameters.AddWithValue("@estado", estado);
        cmd.Parameters.AddWithValue("@motivo", (object?)motivo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
        cmd.Parameters.AddWithValue("@id", solicitudId);
        cmd.ExecuteNonQuery();
    }

    private static Solicitud Map(MySqlDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["id"]),
        AnimalId = Convert.ToInt32(reader["animal_id"]),
        Animal = Convert.ToString(reader["animal"]) ?? "",
        AdoptanteId = Convert.ToInt32(reader["adoptante_id"]),
        Adoptante = Convert.ToString(reader["adoptante"]) ?? "",
        Estado = Convert.ToString(reader["estado"]) ?? "pendiente",
        Motivo = reader["motivo"] == DBNull.Value ? null : Convert.ToString(reader["motivo"]),
        FechaSolicitud = Convert.ToDateTime(reader["fecha_solicitud"]),
        ResueltaEn = reader["resuelta_en"] == DBNull.Value ? null : Convert.ToDateTime(reader["resuelta_en"]),
        ResueltaPor = reader["resuelta_por_nombre"] == DBNull.Value ? null : Convert.ToString(reader["resuelta_por_nombre"])
    };
}
