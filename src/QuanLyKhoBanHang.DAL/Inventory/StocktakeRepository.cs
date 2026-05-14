using System.Data;
using System.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.DAL.Inventory;

public sealed class StocktakeRepository : RepositoryBase
{
    public StocktakeRepository(DatabaseOptions options) : base(options) { }

    public int CreateStocktake(StocktakeDto stocktake)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        
        try
        {
            int stocktakeId;
            
            using (var command = new SqlCommand(
                "INSERT INTO Stocktakes (StocktakeCode, StocktakeDate, CreatedByUserId, Note) " +
                "OUTPUT INSERTED.Id VALUES (@StocktakeCode, @StocktakeDate, @CreatedByUserId, @Note)", 
                connection, transaction))
            {
                command.Parameters.AddWithValue("@StocktakeCode", stocktake.StocktakeCode);
                command.Parameters.AddWithValue("@StocktakeDate", stocktake.StocktakeDate);
                command.Parameters.AddWithValue("@CreatedByUserId", stocktake.CreatedByUserId);
                command.Parameters.AddWithValue("@Note", (object?)stocktake.Note ?? DBNull.Value);
                
                var result = command.ExecuteScalar();
                stocktakeId = result != null ? Convert.ToInt32(result) : 0;
            }
            
            if (stocktakeId <= 0)
            {
                transaction.Rollback();
                return 0;
            }
            
            foreach (var line in stocktake.Lines)
            {
                using var lineCommand = new SqlCommand(
                    "INSERT INTO StocktakeDetails (StocktakeId, ProductId, SystemQuantity, ActualQuantity) " +
                    "VALUES (@StocktakeId, @ProductId, @SystemQuantity, @ActualQuantity)", 
                    connection, transaction);
                
                lineCommand.Parameters.AddWithValue("@StocktakeId", stocktakeId);
                lineCommand.Parameters.AddWithValue("@ProductId", line.ProductId);
                lineCommand.Parameters.AddWithValue("@SystemQuantity", line.SystemQuantity);
                lineCommand.Parameters.AddWithValue("@ActualQuantity", line.ActualQuantity);
                
                lineCommand.ExecuteNonQuery();
            }
            
            transaction.Commit();
            return stocktakeId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<StocktakeDto> GetStocktakes(DateTime fromDate, DateTime toDate)
    {
        var stocktakes = new List<StocktakeDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT Id, StocktakeCode, StocktakeDate, CreatedByUserId, Note " +
            "FROM Stocktakes " +
            "WHERE CAST(StocktakeDate AS DATE) >= @FromDate AND CAST(StocktakeDate AS DATE) <= @ToDate " +
            "ORDER BY StocktakeDate DESC", connection);
        
        command.Parameters.AddWithValue("@FromDate", fromDate.Date);
        command.Parameters.AddWithValue("@ToDate", toDate.Date);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var stocktake = new StocktakeDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                StocktakeCode = reader.GetString(reader.GetOrdinal("StocktakeCode")),
                StocktakeDate = reader.GetDateTime(reader.GetOrdinal("StocktakeDate")),
                CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note")),
                Lines = new List<StocktakeLineDto>()
            };
            
            stocktakes.Add(stocktake);
        }
        
        foreach (var stocktake in stocktakes)
        {
            stocktake.Lines = GetStocktakeLines(stocktake.Id);
        }
        
        return stocktakes;
    }

    public StocktakeDto? GetStocktakeById(int id)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT Id, StocktakeCode, StocktakeDate, CreatedByUserId, Note " +
            "FROM Stocktakes WHERE Id = @Id", connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        if (reader.Read())
        {
            var stocktake = new StocktakeDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                StocktakeCode = reader.GetString(reader.GetOrdinal("StocktakeCode")),
                StocktakeDate = reader.GetDateTime(reader.GetOrdinal("StocktakeDate")),
                CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note")),
                Lines = new List<StocktakeLineDto>()
            };
            
            stocktake.Lines = GetStocktakeLines(stocktake.Id);
            return stocktake;
        }
        
        return null;
    }

    private List<StocktakeLineDto> GetStocktakeLines(int stocktakeId)
    {
        var lines = new List<StocktakeLineDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT sd.Id, sd.StocktakeId, sd.ProductId, p.Name AS ProductName, sd.SystemQuantity, sd.ActualQuantity " +
            "FROM StocktakeDetails sd " +
            "INNER JOIN Products p ON sd.ProductId = p.Id " +
            "WHERE sd.StocktakeId = @StocktakeId", connection);
        
        command.Parameters.AddWithValue("@StocktakeId", stocktakeId);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            lines.Add(new StocktakeLineDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                StocktakeId = reader.GetInt32(reader.GetOrdinal("StocktakeId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                SystemQuantity = reader.GetInt32(reader.GetOrdinal("SystemQuantity")),
                ActualQuantity = reader.GetInt32(reader.GetOrdinal("ActualQuantity"))
            });
        }
        
        return lines;
    }

    public bool StocktakeCodeExists(string code, int? excludeId = null)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        string query = excludeId.HasValue
            ? "SELECT COUNT(*) FROM Stocktakes WHERE StocktakeCode = @Code AND Id <> @ExcludeId"
            : "SELECT COUNT(*) FROM Stocktakes WHERE StocktakeCode = @Code";
        
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
