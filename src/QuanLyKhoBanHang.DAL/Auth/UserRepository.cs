using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Auth;
using QuanLyKhoBanHang.DTO.Common;
using System.Data;

namespace QuanLyKhoBanHang.DAL.Auth;

public sealed class UserRepository : RepositoryBase
{
    public UserRepository(DatabaseOptions options) : base(options) { }

    public UserDto? GetByUsername(string username)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
SELECT Id, Username, PasswordHash, FullName, RoleId, IsActive, MustChangePassword, CreatedAt, UpdatedAt, LastLoginAt
FROM Users
WHERE Username = @Username", conn);
        cmd.Parameters.AddWithValue("@Username", username);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapUser(reader);
        return null;
    }

    public UserDto? GetById(int id)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
SELECT Id, Username, PasswordHash, FullName, RoleId, IsActive, MustChangePassword, CreatedAt, UpdatedAt, LastLoginAt
FROM Users
WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapUser(reader);
        return null;
    }

    public List<UserDto> GetAll()
    {
        var list = new List<UserDto>();
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
SELECT Id, Username, PasswordHash, FullName, RoleId, IsActive, MustChangePassword, CreatedAt, UpdatedAt, LastLoginAt
FROM Users
ORDER BY Id", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapUser(reader));
        return list;
    }

    public int Create(string username, string passwordHash, string fullName, int roleId, bool isActive)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
INSERT INTO Users (Username, PasswordHash, FullName, RoleId, IsActive, MustChangePassword)
OUTPUT INSERTED.Id
VALUES (@Username, @PasswordHash, @FullName, @RoleId, @IsActive, @MustChangePassword)", conn);
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
        cmd.Parameters.AddWithValue("@FullName", fullName);
        cmd.Parameters.AddWithValue("@RoleId", roleId);
        cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
        cmd.Parameters.AddWithValue("@MustChangePassword", 0);
        return (int)cmd.ExecuteScalar()!;
    }

    public void Update(int id, string username, string fullName, int roleId, bool isActive)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
UPDATE Users
SET Username = @Username, FullName = @FullName, RoleId = @RoleId, IsActive = @IsActive, UpdatedAt = SYSDATETIME()
WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@FullName", fullName);
        cmd.Parameters.AddWithValue("@RoleId", roleId);
        cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
        cmd.ExecuteNonQuery();
    }

    public void UpdatePasswordHash(int id, string passwordHash, bool mustChangePassword)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
UPDATE Users
SET PasswordHash = @PasswordHash, MustChangePassword = @MustChangePassword, UpdatedAt = SYSDATETIME()
WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
        cmd.Parameters.AddWithValue("@MustChangePassword", mustChangePassword ? 1 : 0);
        cmd.ExecuteNonQuery();
    }

    public void UpdateLastLogin(int id)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
UPDATE Users SET LastLoginAt = SYSDATETIME() WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    public void ClearMustChangePassword(int id)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
UPDATE Users SET MustChangePassword = 0, UpdatedAt = SYSDATETIME() WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    public void Deactivate(int id)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand(@"
UPDATE Users SET IsActive = 0, UpdatedAt = SYSDATETIME() WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.ExecuteNonQuery();
    }

    public bool IsUsernameTaken(string username, int? excludeId = null)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
        if (excludeId.HasValue)
        {
            sql += " AND Id <> @ExcludeId";
        }

        var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Username", username);
        if (excludeId.HasValue)
            cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    public string? GetPasswordHash(int id)
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand("SELECT PasswordHash FROM Users WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        var result = cmd.ExecuteScalar();
        return result as string;
    }

    public int CountActiveAdmins()
    {
        using var conn = new SqlConnection(Options.ConnectionString);
        conn.Open();
        var cmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE RoleId = @RoleId AND IsActive = 1", conn);
        cmd.Parameters.AddWithValue("@RoleId", (int)UserRole.Admin);
        return (int)cmd.ExecuteScalar()!;
    }

    private static UserDto MapUser(IDataRecord reader)
    {
        return new UserDto
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            FullName = reader.GetString(3),
            Role = (UserRole)reader.GetInt32(4),
            IsActive = reader.GetBoolean(5),
            MustChangePassword = reader.GetBoolean(6)
        };
    }
}
