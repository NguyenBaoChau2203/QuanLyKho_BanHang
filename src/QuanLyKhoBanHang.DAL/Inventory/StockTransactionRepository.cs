using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.DAL.Inventory;

public sealed class StockTransactionRepository : RepositoryBase
{
    public StockTransactionRepository(DatabaseOptions options) : base(options) { }

    public List<StockTransactionDto> GetTransactions(DateTime fromDate, DateTime toDate)
    {
        var transactions = new List<StockTransactionDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT st.Id, st.ProductId, p.Name AS ProductName, st.TransactionType, st.QuantityChange, st.QuantityAfter, " +
            "st.ReferenceCode, st.CreatedAt, st.CreatedByUserId, st.Note " +
            "FROM StockTransactions st " +
            "INNER JOIN Products p ON st.ProductId = p.Id " +
            "WHERE CAST(st.CreatedAt AS DATE) >= @FromDate AND CAST(st.CreatedAt AS DATE) <= @ToDate " +
            "ORDER BY st.CreatedAt DESC", connection);
        
        command.Parameters.AddWithValue("@FromDate", fromDate.Date);
        command.Parameters.AddWithValue("@ToDate", toDate.Date);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            transactions.Add(new StockTransactionDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                TransactionType = (StockTransactionType)reader.GetInt32(reader.GetOrdinal("TransactionType")),
                QuantityChange = reader.GetInt32(reader.GetOrdinal("QuantityChange")),
                QuantityAfter = reader.GetInt32(reader.GetOrdinal("QuantityAfter")),
                ReferenceCode = reader.GetString(reader.GetOrdinal("ReferenceCode")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note"))
            });
        }
        
        return transactions;
    }

    public int CreateTransaction(StockTransactionDto transaction)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "INSERT INTO StockTransactions (ProductId, TransactionType, QuantityChange, QuantityAfter, ReferenceCode, CreatedByUserId, Note) " +
            "OUTPUT INSERTED.Id VALUES (@ProductId, @TransactionType, @QuantityChange, @QuantityAfter, @ReferenceCode, @CreatedByUserId, @Note)", 
            connection);
        
        command.Parameters.AddWithValue("@ProductId", transaction.ProductId);
        command.Parameters.AddWithValue("@TransactionType", (int)transaction.TransactionType);
        command.Parameters.AddWithValue("@QuantityChange", transaction.QuantityChange);
        command.Parameters.AddWithValue("@QuantityAfter", transaction.QuantityAfter);
        command.Parameters.AddWithValue("@ReferenceCode", transaction.ReferenceCode);
        command.Parameters.AddWithValue("@CreatedByUserId", transaction.CreatedByUserId);
        command.Parameters.AddWithValue("@Note", (object?)transaction.Note ?? DBNull.Value);
        
        connection.Open();
        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    public List<StockTransactionDto> GetTransactionsByProduct(int productId, DateTime fromDate, DateTime toDate)
    {
        var transactions = new List<StockTransactionDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT st.Id, st.ProductId, p.Name AS ProductName, st.TransactionType, st.QuantityChange, st.QuantityAfter, " +
            "st.ReferenceCode, st.CreatedAt, st.CreatedByUserId, st.Note " +
            "FROM StockTransactions st " +
            "INNER JOIN Products p ON st.ProductId = p.Id " +
            "WHERE st.ProductId = @ProductId AND CAST(st.CreatedAt AS DATE) >= @FromDate AND CAST(st.CreatedAt AS DATE) <= @ToDate " +
            "ORDER BY st.CreatedAt DESC", connection);
        
        command.Parameters.AddWithValue("@ProductId", productId);
        command.Parameters.AddWithValue("@FromDate", fromDate.Date);
        command.Parameters.AddWithValue("@ToDate", toDate.Date);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            transactions.Add(new StockTransactionDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                TransactionType = (StockTransactionType)reader.GetInt32(reader.GetOrdinal("TransactionType")),
                QuantityChange = reader.GetInt32(reader.GetOrdinal("QuantityChange")),
                QuantityAfter = reader.GetInt32(reader.GetOrdinal("QuantityAfter")),
                ReferenceCode = reader.GetString(reader.GetOrdinal("ReferenceCode")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note"))
            });
        }
        
        return transactions;
    }
}
