using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;

namespace QuanLyKhoBanHang.DAL.Auth;

public sealed class PasswordRecoveryRepository : RepositoryBase
{
    public PasswordRecoveryRepository(DatabaseOptions options) : base(options) { }

    public void Create(int userId, string requestCode)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
INSERT INTO PasswordRecoveryRequests (UserId, RequestCode)
VALUES (@UserId, @RequestCode)", conn);
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@RequestCode", requestCode);
        cmd.ExecuteNonQuery();
    }

    public (int Id, int UserId)? GetPendingByCode(string requestCode)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
SELECT Id, UserId FROM PasswordRecoveryRequests
WHERE RequestCode = @RequestCode AND IsResolved = 0", conn);
        cmd.Parameters.AddWithValue("@RequestCode", requestCode);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return (reader.GetInt32(0), reader.GetInt32(1));
        return null;
    }

    public List<(int Id, int UserId, string Username, DateTime CreatedAt)> GetPending()
    {
        var list = new List<(int Id, int UserId, string Username, DateTime CreatedAt)>();
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
SELECT pr.Id, pr.UserId, u.Username, pr.CreatedAt
FROM PasswordRecoveryRequests pr
INNER JOIN Users u ON u.Id = pr.UserId
WHERE pr.IsResolved = 0
ORDER BY pr.CreatedAt DESC", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add((reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetDateTime(3)));
        return list;
    }

    public void Resolve(int id)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
UPDATE PasswordRecoveryRequests
SET IsResolved = 1, ResolvedAt = SYSDATETIME()
WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }
}
