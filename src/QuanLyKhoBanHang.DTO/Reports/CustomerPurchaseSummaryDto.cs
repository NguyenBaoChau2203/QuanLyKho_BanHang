namespace QuanLyKhoBanHang.DTO.Reports;

public sealed class CustomerPurchaseSummaryDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal TotalAmount { get; set; }
}
