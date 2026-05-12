namespace QuanLyKhoBanHang.DTO.Sales;

public sealed class SalesInvoiceDto
{
    public int Id { get; set; }
    public string InvoiceCode { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Today;
    public int CreatedByUserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount => TotalAmount - DiscountAmount;
    public string? Note { get; set; }
    public List<SalesInvoiceLineDto> Lines { get; set; } = new();
}
