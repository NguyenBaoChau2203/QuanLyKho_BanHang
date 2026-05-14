using System.Data;
using System.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.DAL.MasterData;

public sealed class SupplierRepository : RepositoryBase
{
    public SupplierRepository(DatabaseOptions options) : base(options) { }

    public List<SupplierDto> GetAll()
    {
        var suppliers = new List<SupplierDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand("SELECT Id, Code, Name, Phone, Email, Address, IsActive FROM Suppliers WHERE IsActive = 1 ORDER BY Name", connection);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            suppliers.Add(new SupplierDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }
        
        return suppliers;
    }

    public List<SupplierDto> Search(string keyword)
    {
        var suppliers = new List<SupplierDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT Id, Code, Name, Phone, Email, Address, IsActive FROM Suppliers WHERE IsActive = 1 AND (Code LIKE @Keyword OR Name LIKE @Keyword OR Phone LIKE @Keyword) ORDER BY Name", 
            connection);
        
        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            suppliers.Add(new SupplierDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }
        
        return suppliers;
    }

    public SupplierDto? GetById(int id)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand("SELECT Id, Code, Name, Phone, Email, Address, IsActive FROM Suppliers WHERE Id = @Id", connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        if (reader.Read())
        {
            return new SupplierDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
        
        return null;
    }

    public int Create(SupplierDto supplier)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "INSERT INTO Suppliers (Code, Name, Phone, Email, Address, IsActive) OUTPUT INSERTED.Id VALUES (@Code, @Name, @Phone, @Email, @Address, @IsActive)", 
            connection);
        
        command.Parameters.AddWithValue("@Code", supplier.Code);
        command.Parameters.AddWithValue("@Name", supplier.Name);
        command.Parameters.AddWithValue("@Phone", (object?)supplier.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Email", (object?)supplier.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@Address", (object?)supplier.Address ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", supplier.IsActive);
        
        connection.Open();
        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    public bool Update(SupplierDto supplier)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "UPDATE Suppliers SET Code = @Code, Name = @Name, Phone = @Phone, Email = @Email, Address = @Address, IsActive = @IsActive WHERE Id = @Id", 
            connection);
        
        command.Parameters.AddWithValue("@Id", supplier.Id);
        command.Parameters.AddWithValue("@Code", supplier.Code);
        command.Parameters.AddWithValue("@Name", supplier.Name);
        command.Parameters.AddWithValue("@Phone", (object?)supplier.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@Email", (object?)supplier.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@Address", (object?)supplier.Address ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", supplier.IsActive);
        
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Deactivate(int id)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand("UPDATE Suppliers SET IsActive = 0 WHERE Id = @Id", connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool CodeExists(string code, int? excludeId = null)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        string query = excludeId.HasValue
            ? "SELECT COUNT(*) FROM Suppliers WHERE Code = @Code AND Id <> @ExcludeId"
            : "SELECT COUNT(*) FROM Suppliers WHERE Code = @Code";
        
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Code", code);
        
        if (excludeId.HasValue)
        {
            command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
        }
        
        connection.Open();
        var count = (int)command.ExecuteScalar();
        return count > 0;
    }
}
