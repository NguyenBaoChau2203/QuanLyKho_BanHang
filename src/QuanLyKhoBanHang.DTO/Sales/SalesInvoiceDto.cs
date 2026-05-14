namespace QuanLyKhoBanHang.DTO.Sales;

public sealed class SalesInvoiceDto
{
    public int Id { get; set; }
    public string InvoiceCode { get; set; }
    public int? CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int CreatedByUserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Note { get; set; }

    // Danh sách các món hàng trong hóa đơn
    public List<SalesInvoiceLineDto> Details { get; set; } = new List<SalesInvoiceLineDto>();
    public List<SalesInvoiceLineDto> Lines { get; set; } = new List<SalesInvoiceLineDto>();
}
