using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Admin;
using System.Data;

namespace QuanLyKhoBanHang.DAL.Auth;

public sealed class AuditLogRepository : RepositoryBase
{
    public AuditLogRepository(DatabaseOptions options) : base(options) { }

    public List<AuditLogDto> Query(DateTime fromDate, DateTime toDate, string? keyword)
    {
        var list = new List<AuditLogDto>();
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();

        var sql = @"
SELECT a.Id, a.CreatedAt, ISNULL(u.Username, N'Hệ thống'), ISNULL(u.FullName, N'Hệ thống'),
       a.Action, a.EntityName, ISNULL(a.Description, N'')
FROM AuditLogs a
LEFT JOIN Users u ON u.Id = a.UserId
WHERE a.CreatedAt >= @From AND a.CreatedAt <= @To";

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            sql += @" AND (u.Username LIKE @Kw OR u.FullName LIKE @Kw OR a.Action LIKE @Kw OR a.EntityName LIKE @Kw OR a.Description LIKE @Kw)";
        }

        sql += " ORDER BY a.CreatedAt DESC";

        var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@From", fromDate);
        cmd.Parameters.AddWithValue("@To", toDate);
        if (!string.IsNullOrWhiteSpace(keyword))
            cmd.Parameters.AddWithValue("@Kw", "%" + keyword + "%");

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new AuditLogDto
            {
                Id = reader.GetInt32(0),
                CreatedAt = reader.GetDateTime(1),
                Username = reader.GetString(2),
                FullName = reader.GetString(3),
                Action = reader.GetString(4),
                EntityName = reader.GetString(5),
                Description = reader.GetString(6)
            });
        }

        return list;
    }

    public void Write(int? userId, string action, string entityName, int? entityId, string? description)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
INSERT INTO AuditLogs (UserId, Action, EntityName, EntityId, Description)
VALUES (@UserId, @Action, @EntityName, @EntityId, @Description)", conn);
        cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Action", action);
        cmd.Parameters.AddWithValue("@EntityName", entityName);
        cmd.Parameters.AddWithValue("@EntityId", (object?)entityId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Description", (object?)description ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }
}
