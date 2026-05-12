namespace QuanLyKhoBanHang.DTO.Inventory;

public sealed class PurchaseReceiptLineDto
{
    public int Id { get; set; }
    public int PurchaseReceiptId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal => Quantity * UnitCost;
}
