using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.DTO.Inventory;

public sealed class StockTransactionDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public StockTransactionType TransactionType { get; set; }
    public int QuantityChange { get; set; }
    public int QuantityAfter { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public string? Note { get; set; }
}
