namespace QuanLyKhoBanHang.DTO.Sales;

public sealed class SalesInvoiceLineDto
{
    public int Id { get; set; }
    public int SalesInvoiceId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ProductName { get; set; }
    public decimal LineTotal { get; set; }
}
