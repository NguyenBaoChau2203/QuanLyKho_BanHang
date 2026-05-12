namespace QuanLyKhoBanHang.DTO.Reports;

public sealed class RevenueSummaryDto
{
    public DateTime Date { get; set; }
    public int InvoiceCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal EstimatedProfit { get; set; }
}
