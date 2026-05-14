using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.DAL.Inventory;

public sealed class PurchaseRepository : RepositoryBase
{
    public PurchaseRepository(DatabaseOptions options) : base(options) { }

    public int CreateReceipt(PurchaseReceiptDto receipt)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        
        try
        {
            int receiptId;
            
            using (var command = new SqlCommand(
                "INSERT INTO PurchaseReceipts (ReceiptCode, SupplierId, ReceiptDate, CreatedByUserId, TotalAmount, Note) " +
                "OUTPUT INSERTED.Id VALUES (@ReceiptCode, @SupplierId, @ReceiptDate, @CreatedByUserId, @TotalAmount, @Note)", 
                connection, transaction))
            {
                command.Parameters.AddWithValue("@ReceiptCode", receipt.ReceiptCode);
                command.Parameters.AddWithValue("@SupplierId", receipt.SupplierId);
                command.Parameters.AddWithValue("@ReceiptDate", receipt.ReceiptDate);
                command.Parameters.AddWithValue("@CreatedByUserId", receipt.CreatedByUserId);
                command.Parameters.AddWithValue("@TotalAmount", receipt.TotalAmount);
                command.Parameters.AddWithValue("@Note", (object?)receipt.Note ?? DBNull.Value);
                
                var result = command.ExecuteScalar();
                receiptId = result != null ? Convert.ToInt32(result) : 0;
            }
            
            if (receiptId <= 0)
            {
                transaction.Rollback();
                return 0;
            }
            
            foreach (var line in receipt.Lines)
            {
                using var lineCommand = new SqlCommand(
                    "INSERT INTO PurchaseReceiptDetails (PurchaseReceiptId, ProductId, Quantity, UnitCost) " +
                    "VALUES (@PurchaseReceiptId, @ProductId, @Quantity, @UnitCost)", 
                    connection, transaction);
                
                lineCommand.Parameters.AddWithValue("@PurchaseReceiptId", receiptId);
                lineCommand.Parameters.AddWithValue("@ProductId", line.ProductId);
                lineCommand.Parameters.AddWithValue("@Quantity", line.Quantity);
                lineCommand.Parameters.AddWithValue("@UnitCost", line.UnitCost);
                
                lineCommand.ExecuteNonQuery();
            }
            
            transaction.Commit();
            return receiptId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<PurchaseReceiptDto> GetReceipts(DateTime fromDate, DateTime toDate)
    {
        var receipts = new List<PurchaseReceiptDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT Id, ReceiptCode, SupplierId, ReceiptDate, CreatedByUserId, TotalAmount, Note " +
            "FROM PurchaseReceipts " +
            "WHERE CAST(ReceiptDate AS DATE) >= @FromDate AND CAST(ReceiptDate AS DATE) <= @ToDate " +
            "ORDER BY ReceiptDate DESC", connection);
        
        command.Parameters.AddWithValue("@FromDate", fromDate.Date);
        command.Parameters.AddWithValue("@ToDate", toDate.Date);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var receipt = new PurchaseReceiptDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ReceiptCode = reader.GetString(reader.GetOrdinal("ReceiptCode")),
                SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId")),
                ReceiptDate = reader.GetDateTime(reader.GetOrdinal("ReceiptDate")),
                CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note")),
                Lines = new List<PurchaseReceiptLineDto>()
            };
            
            receipts.Add(receipt);
        }
        
        foreach (var receipt in receipts)
        {
            receipt.Lines = GetReceiptLines(receipt.Id);
        }
        
        return receipts;
    }

    public PurchaseReceiptDto? GetReceiptById(int id)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT Id, ReceiptCode, SupplierId, ReceiptDate, CreatedByUserId, TotalAmount, Note " +
            "FROM PurchaseReceipts WHERE Id = @Id", connection);
        
        command.Parameters.AddWithValue("@Id", id);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        if (reader.Read())
        {
            var receipt = new PurchaseReceiptDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ReceiptCode = reader.GetString(reader.GetOrdinal("ReceiptCode")),
                SupplierId = reader.GetInt32(reader.GetOrdinal("SupplierId")),
                ReceiptDate = reader.GetDateTime(reader.GetOrdinal("ReceiptDate")),
                CreatedByUserId = reader.GetInt32(reader.GetOrdinal("CreatedByUserId")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? null : reader.GetString(reader.GetOrdinal("Note")),
                Lines = new List<PurchaseReceiptLineDto>()
            };
            
            receipt.Lines = GetReceiptLines(receipt.Id);
            return receipt;
        }
        
        return null;
    }

    private List<PurchaseReceiptLineDto> GetReceiptLines(int receiptId)
    {
        var lines = new List<PurchaseReceiptLineDto>();
        
        using var connection = new SqlConnection(Options.ConnectionString);
        using var command = new SqlCommand(
            "SELECT prd.Id, prd.PurchaseReceiptId, prd.ProductId, p.Name AS ProductName, prd.Quantity, prd.UnitCost " +
            "FROM PurchaseReceiptDetails prd " +
            "INNER JOIN Products p ON prd.ProductId = p.Id " +
            "WHERE prd.PurchaseReceiptId = @PurchaseReceiptId", connection);
        
        command.Parameters.AddWithValue("@PurchaseReceiptId", receiptId);
        
        connection.Open();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            lines.Add(new PurchaseReceiptLineDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                PurchaseReceiptId = reader.GetInt32(reader.GetOrdinal("PurchaseReceiptId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitCost = reader.GetDecimal(reader.GetOrdinal("UnitCost"))
            });
        }
        
        return lines;
    }

    public bool ReceiptCodeExists(string code, int? excludeId = null)
    {
        using var connection = new SqlConnection(Options.ConnectionString);
        string query = excludeId.HasValue
            ? "SELECT COUNT(*) FROM PurchaseReceipts WHERE ReceiptCode = @Code AND Id <> @ExcludeId"
            : "SELECT COUNT(*) FROM PurchaseReceipts WHERE ReceiptCode = @Code";
        
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
