using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Common;
using System.Data;

namespace QuanLyKhoBanHang.DAL.Auth;

public sealed class RoleRepository : RepositoryBase
{
    public RoleRepository(DatabaseOptions options) : base(options) { }

    public List<(int Id, string Name)> GetAll()
    {
        var list = new List<(int Id, string Name)>();
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand("SELECT Id, Name FROM Roles ORDER BY Id", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add((reader.GetInt32(0), reader.GetString(1)));
        return list;
    }

    public string? GetNameById(int id)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand("SELECT Name FROM Roles WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        var result = cmd.ExecuteScalar();
        return result as string;
    }

    public int? GetIdByName(string name)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand("SELECT Id FROM Roles WHERE Name = @Name", conn);
        cmd.Parameters.AddWithValue("@Name", name);
        var result = cmd.ExecuteScalar();
        return result as int?;
    }
}
