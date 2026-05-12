namespace QuanLyKhoBanHang.DTO.Inventory;

public sealed class PurchaseReceiptDto
{
    public int Id { get; set; }
    public string ReceiptCode { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public DateTime ReceiptDate { get; set; } = DateTime.Today;
    public int CreatedByUserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public List<PurchaseReceiptLineDto> Lines { get; set; } = new();
}
