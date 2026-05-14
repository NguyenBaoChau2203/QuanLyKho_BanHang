using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Common;
using System.Data;

namespace QuanLyKhoBanHang.DAL.Auth;

public sealed class PermissionRepository : RepositoryBase
{
    public PermissionRepository(DatabaseOptions options) : base(options) { }

    public List<(int Id, string FeatureKey, string FeatureName, string GroupName, string Note)> GetAllPermissions()
    {
        var list = new List<(int Id, string FeatureKey, string FeatureName, string GroupName, string Note)>();
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand("SELECT Id, FeatureKey, FeatureName, GroupName, Note FROM Permissions ORDER BY Id", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add((
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            ));
        return list;
    }

    public HashSet<string> GetFeatureKeysForRole(int roleId)
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
SELECT p.FeatureKey
FROM RolePermissions rp
INNER JOIN Permissions p ON p.Id = rp.PermissionId
WHERE rp.RoleId = @RoleId", conn);
        cmd.Parameters.AddWithValue("@RoleId", roleId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            keys.Add(reader.GetString(0));
        return keys;
    }
}
