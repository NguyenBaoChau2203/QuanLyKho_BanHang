using System.Data;
using System.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.DAL.MasterData;

public sealed class CategoryRepository : RepositoryBase
{
    public CategoryRepository(DatabaseOptions options) : base(options) { }

    public List<CategoryDto> GetAll()
    {
        var categories = new List<CategoryDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand("SELECT Id, Code, Name, Description, IsActive FROM Categories WHERE IsActive = 1 ORDER BY Name", connection);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            categories.Add(new CategoryDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }
        
        return categories;
    }

    public CategoryDto? GetById(int id)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand("SELECT Id, Code, Name, Description, IsActive FROM Categories WHERE Id = @Id", connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        if (reader.Read())
        {
            return new CategoryDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
        
        return null;
    }

    public int Create(CategoryDto category)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "INSERT INTO Categories (Code, Name, Description, IsActive) OUTPUT INSERTED.Id VALUES (@Code, @Name, @Description, @IsActive)", 
            connection);
        
        command.Parameters.AddWithValue("@Code", category.Code);
        command.Parameters.AddWithValue("@Name", category.Name);
        command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", category.IsActive);
        
        connection.Open();
        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    public bool Update(CategoryDto category)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "UPDATE Categories SET Code = @Code, Name = @Name, Description = @Description, IsActive = @IsActive WHERE Id = @Id", 
            connection);
        
        command.Parameters.AddWithValue("@Id", category.Id);
        command.Parameters.AddWithValue("@Code", category.Code);
        command.Parameters.AddWithValue("@Name", category.Name);
        command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@IsActive", category.IsActive);
        
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Deactivate(int id)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand("UPDATE Categories SET IsActive = 0 WHERE Id = @Id", connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool CodeExists(string code, int? excludeId = null)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        string query = excludeId.HasValue
            ? "SELECT COUNT(*) FROM Categories WHERE Code = @Code AND Id <> @ExcludeId"
            : "SELECT COUNT(*) FROM Categories WHERE Code = @Code";
        
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
