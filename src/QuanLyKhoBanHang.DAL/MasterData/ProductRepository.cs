using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.DAL.MasterData;

public sealed class ProductRepository : RepositoryBase
{
    public ProductRepository(DatabaseOptions options) : base(options) { }

    public List<ProductDto> GetAll()
    {
        var products = new List<ProductDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT p.Id, p.Code, p.Name, p.CategoryId, c.Name AS CategoryName, p.Unit, p.CostPrice, p.SellingPrice, p.QuantityOnHand, p.MinStockLevel, p.IsActive " +
            "FROM Products p " +
            "INNER JOIN Categories c ON p.CategoryId = c.Id " +
            "WHERE p.IsActive = 1 ORDER BY p.Name", connection);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            products.Add(new ProductDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                SellingPrice = reader.GetDecimal(reader.GetOrdinal("SellingPrice")),
                QuantityOnHand = reader.GetInt32(reader.GetOrdinal("QuantityOnHand")),
                MinStockLevel = reader.GetInt32(reader.GetOrdinal("MinStockLevel")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }
        
        return products;
    }

    public List<ProductDto> Search(string keyword)
    {
        var products = new List<ProductDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT p.Id, p.Code, p.Name, p.CategoryId, c.Name AS CategoryName, p.Unit, p.CostPrice, p.SellingPrice, p.QuantityOnHand, p.MinStockLevel, p.IsActive " +
            "FROM Products p " +
            "INNER JOIN Categories c ON p.CategoryId = c.Id " +
            "WHERE p.IsActive = 1 AND (p.Code LIKE @Keyword OR p.Name LIKE @Keyword) ORDER BY p.Name", connection);
        
        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            products.Add(new ProductDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                SellingPrice = reader.GetDecimal(reader.GetOrdinal("SellingPrice")),
                QuantityOnHand = reader.GetInt32(reader.GetOrdinal("QuantityOnHand")),
                MinStockLevel = reader.GetInt32(reader.GetOrdinal("MinStockLevel")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }
        
        return products;
    }

    public ProductDto? GetById(int id)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT p.Id, p.Code, p.Name, p.CategoryId, c.Name AS CategoryName, p.Unit, p.CostPrice, p.SellingPrice, p.QuantityOnHand, p.MinStockLevel, p.IsActive " +
            "FROM Products p " +
            "INNER JOIN Categories c ON p.CategoryId = c.Id " +
            "WHERE p.Id = @Id", connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        if (reader.Read())
        {
            return new ProductDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                SellingPrice = reader.GetDecimal(reader.GetOrdinal("SellingPrice")),
                QuantityOnHand = reader.GetInt32(reader.GetOrdinal("QuantityOnHand")),
                MinStockLevel = reader.GetInt32(reader.GetOrdinal("MinStockLevel")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
        
        return null;
    }

    public int Create(ProductDto product)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "INSERT INTO Products (Code, Name, CategoryId, Unit, CostPrice, SellingPrice, QuantityOnHand, MinStockLevel, IsActive) " +
            "OUTPUT INSERTED.Id VALUES (@Code, @Name, @CategoryId, @Unit, @CostPrice, @SellingPrice, @QuantityOnHand, @MinStockLevel, @IsActive)", 
            connection);
        
        command.Parameters.AddWithValue("@Code", product.Code);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
        command.Parameters.AddWithValue("@Unit", product.Unit);
        command.Parameters.AddWithValue("@CostPrice", product.CostPrice);
        command.Parameters.AddWithValue("@SellingPrice", product.SellingPrice);
        command.Parameters.AddWithValue("@QuantityOnHand", product.QuantityOnHand);
        command.Parameters.AddWithValue("@MinStockLevel", product.MinStockLevel);
        command.Parameters.AddWithValue("@IsActive", product.IsActive);
        
        connection.Open();
        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    public bool Update(ProductDto product)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "UPDATE Products SET Code = @Code, Name = @Name, CategoryId = @CategoryId, Unit = @Unit, CostPrice = @CostPrice, " +
            "SellingPrice = @SellingPrice, QuantityOnHand = @QuantityOnHand, MinStockLevel = @MinStockLevel, IsActive = @IsActive WHERE Id = @Id", 
            connection);
        
        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Code", product.Code);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
        command.Parameters.AddWithValue("@Unit", product.Unit);
        command.Parameters.AddWithValue("@CostPrice", product.CostPrice);
        command.Parameters.AddWithValue("@SellingPrice", product.SellingPrice);
        command.Parameters.AddWithValue("@QuantityOnHand", product.QuantityOnHand);
        command.Parameters.AddWithValue("@MinStockLevel", product.MinStockLevel);
        command.Parameters.AddWithValue("@IsActive", product.IsActive);
        
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool Deactivate(int id)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand("UPDATE Products SET IsActive = 0 WHERE Id = @Id", connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool UpdateQuantity(int productId, int quantityChange)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand("UPDATE Products SET QuantityOnHand = QuantityOnHand + @QuantityChange WHERE Id = @ProductId", connection);
        
        command.Parameters.AddWithValue("@ProductId", productId);
        command.Parameters.AddWithValue("@QuantityChange", quantityChange);
        
        connection.Open();
        return command.ExecuteNonQuery() > 0;
    }

    public bool CodeExists(string code, int? excludeId = null)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        string query = excludeId.HasValue
            ? "SELECT COUNT(*) FROM Products WHERE Code = @Code AND Id <> @ExcludeId"
            : "SELECT COUNT(*) FROM Products WHERE Code = @Code";
        
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

    public List<ProductDto> GetLowStockProducts()
    {
        var products = new List<ProductDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT p.Id, p.Code, p.Name, p.CategoryId, c.Name AS CategoryName, p.Unit, p.CostPrice, p.SellingPrice, p.QuantityOnHand, p.MinStockLevel, p.IsActive " +
            "FROM Products p " +
            "INNER JOIN Categories c ON p.CategoryId = c.Id " +
            "WHERE p.IsActive = 1 AND p.QuantityOnHand <= p.MinStockLevel ORDER BY p.Name", connection);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            products.Add(new ProductDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                CostPrice = reader.GetDecimal(reader.GetOrdinal("CostPrice")),
                SellingPrice = reader.GetDecimal(reader.GetOrdinal("SellingPrice")),
                QuantityOnHand = reader.GetInt32(reader.GetOrdinal("QuantityOnHand")),
                MinStockLevel = reader.GetInt32(reader.GetOrdinal("MinStockLevel")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            });
        }
        
        return products;
    }
}
