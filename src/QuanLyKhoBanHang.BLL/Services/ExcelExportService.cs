using ClosedXML.Excel;
using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.DTO.Reports;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class ExcelExportService
{
    /// <summary>
    /// Xuất danh sách tồn kho ra file Excel.
    /// </summary>
    public ServiceResult<string> ExportInventory(List<ProductDto> products, string filePath)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Tồn kho");

            // Header
            ws.Cell(1, 1).Value = "BÁO CÁO TỒN KHO";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, 8).Merge();

            ws.Cell(2, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range(2, 1, 2, 8).Merge();

            // Column headers
            int row = 4;
            string[] headers = ["STT", "Mã SP", "Tên sản phẩm", "Loại hàng", "ĐVT", "Tồn kho", "Mức tối thiểu", "Giá trị tồn"];
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(row, i + 1).Value = headers[i];
            }

            var headerRange = ws.Range(row, 1, row, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data
            for (int i = 0; i < products.Count; i++)
            {
                var p = products[i];
                row = 5 + i;
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = p.Code;
                ws.Cell(row, 3).Value = p.Name;
                ws.Cell(row, 4).Value = string.IsNullOrWhiteSpace(p.CategoryName) ? "Chưa phân loại" : p.CategoryName;
                ws.Cell(row, 5).Value = string.IsNullOrWhiteSpace(p.Unit) ? "Cái" : p.Unit;
                ws.Cell(row, 6).Value = p.QuantityOnHand;
                ws.Cell(row, 7).Value = p.MinStockLevel;
                var costPrice = p.CostPrice > 0 ? p.CostPrice : Math.Round(p.SellingPrice * 0.72M, 0);
                ws.Cell(row, 8).Value = costPrice * p.QuantityOnHand;
                ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0";

                // Highlight low stock
                if (p.QuantityOnHand <= p.MinStockLevel)
                {
                    ws.Range(row, 1, row, 8).Style.Font.FontColor = XLColor.Red;
                }
            }

            // Summary row
            row++;
            ws.Cell(row, 5).Value = "Tổng:";
            ws.Cell(row, 5).Style.Font.Bold = true;
            ws.Cell(row, 6).Value = products.Sum(p => p.QuantityOnHand);
            ws.Cell(row, 6).Style.Font.Bold = true;

            // Auto-fit
            ws.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
            return ServiceResult<string>.Ok(filePath, $"Đã xuất {products.Count} sản phẩm ra file Excel.");
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Fail($"Lỗi khi xuất Excel tồn kho: {ex.Message}");
        }
    }

    /// <summary>
    /// Xuất phiếu kiểm kê ra file Excel.
    /// </summary>
    public ServiceResult<string> ExportStocktake(List<StocktakeExportRow> lines, DateTime stocktakeDate, string filePath)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Kiểm kê");

            ws.Cell(1, 1).Value = "PHIẾU KIỂM KÊ KHO";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, 7).Merge();

            ws.Cell(2, 1).Value = $"Ngày kiểm kê: {stocktakeDate:dd/MM/yyyy}";
            ws.Cell(2, 5).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range(2, 1, 2, 4).Merge();

            int row = 4;
            string[] headers = ["STT", "Mã SP", "Tên sản phẩm", "Loại hàng", "Số hệ thống", "Số thực tế", "Chênh lệch"];
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(row, i + 1).Value = headers[i];
            }

            var headerRange = ws.Range(row, 1, row, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                row = 5 + i;
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = line.ProductCode;
                ws.Cell(row, 3).Value = line.ProductName;
                ws.Cell(row, 4).Value = line.CategoryName;
                ws.Cell(row, 5).Value = line.SystemQuantity;
                ws.Cell(row, 6).Value = line.ActualQuantity;
                ws.Cell(row, 7).Value = line.Difference;

                if (line.Difference < 0)
                    ws.Cell(row, 7).Style.Font.FontColor = XLColor.Red;
                else if (line.Difference > 0)
                    ws.Cell(row, 7).Style.Font.FontColor = XLColor.FromHtml("#D97706");
            }

            ws.Columns().AdjustToContents();
            workbook.SaveAs(filePath);
            return ServiceResult<string>.Ok(filePath, $"Đã xuất phiếu kiểm kê {lines.Count} sản phẩm ra Excel.");
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Fail($"Lỗi khi xuất Excel kiểm kê: {ex.Message}");
        }
    }

    /// <summary>
    /// Xuất báo cáo doanh thu ra file Excel.
    /// </summary>
    public ServiceResult<string> ExportReport(
        List<RevenueSummaryDto> revenue,
        List<ProductSalesSummaryDto> topProducts,
        List<CustomerPurchaseSummaryDto> topCustomers,
        DateTime fromDate, DateTime toDate, string filePath)
    {
        try
        {
            using var workbook = new XLWorkbook();

            // Sheet 1: Doanh thu
            var wsRevenue = workbook.Worksheets.Add("Doanh thu");
            wsRevenue.Cell(1, 1).Value = "BÁO CÁO DOANH THU";
            wsRevenue.Cell(1, 1).Style.Font.Bold = true;
            wsRevenue.Cell(1, 1).Style.Font.FontSize = 14;
            wsRevenue.Range(1, 1, 1, 4).Merge();
            wsRevenue.Cell(2, 1).Value = $"Từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}";
            wsRevenue.Range(2, 1, 2, 4).Merge();

            string[] revHeaders = ["Ngày", "Số hóa đơn", "Doanh thu", "Lợi nhuận ước tính"];
            for (int i = 0; i < revHeaders.Length; i++)
                wsRevenue.Cell(4, i + 1).Value = revHeaders[i];
            var rh = wsRevenue.Range(4, 1, 4, revHeaders.Length);
            rh.Style.Font.Bold = true;
            rh.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
            rh.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < revenue.Count; i++)
            {
                wsRevenue.Cell(5 + i, 1).Value = revenue[i].Date.ToString("dd/MM/yyyy");
                wsRevenue.Cell(5 + i, 2).Value = revenue[i].InvoiceCount;
                wsRevenue.Cell(5 + i, 3).Value = revenue[i].Revenue;
                wsRevenue.Cell(5 + i, 3).Style.NumberFormat.Format = "#,##0";
                wsRevenue.Cell(5 + i, 4).Value = revenue[i].EstimatedProfit;
                wsRevenue.Cell(5 + i, 4).Style.NumberFormat.Format = "#,##0";
            }

            int totalRow = 5 + revenue.Count;
            wsRevenue.Cell(totalRow, 1).Value = "Tổng cộng";
            wsRevenue.Cell(totalRow, 1).Style.Font.Bold = true;
            wsRevenue.Cell(totalRow, 2).Value = revenue.Sum(r => r.InvoiceCount);
            wsRevenue.Cell(totalRow, 3).Value = revenue.Sum(r => r.Revenue);
            wsRevenue.Cell(totalRow, 3).Style.NumberFormat.Format = "#,##0";
            wsRevenue.Cell(totalRow, 4).Value = revenue.Sum(r => r.EstimatedProfit);
            wsRevenue.Cell(totalRow, 4).Style.NumberFormat.Format = "#,##0";
            wsRevenue.Range(totalRow, 1, totalRow, 4).Style.Font.Bold = true;
            wsRevenue.Columns().AdjustToContents();

            // Sheet 2: Top sản phẩm
            var wsProducts = workbook.Worksheets.Add("Top sản phẩm");
            wsProducts.Cell(1, 1).Value = "TOP SẢN PHẨM BÁN CHẠY";
            wsProducts.Cell(1, 1).Style.Font.Bold = true;
            wsProducts.Cell(1, 1).Style.Font.FontSize = 14;
            wsProducts.Range(1, 1, 1, 5).Merge();

            string[] prodHeaders = ["#", "Mã SP", "Tên sản phẩm", "SL bán", "Doanh thu"];
            for (int i = 0; i < prodHeaders.Length; i++)
                wsProducts.Cell(3, i + 1).Value = prodHeaders[i];
            var ph = wsProducts.Range(3, 1, 3, prodHeaders.Length);
            ph.Style.Font.Bold = true;
            ph.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
            ph.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < topProducts.Count; i++)
            {
                wsProducts.Cell(4 + i, 1).Value = i + 1;
                wsProducts.Cell(4 + i, 2).Value = topProducts[i].ProductCode;
                wsProducts.Cell(4 + i, 3).Value = topProducts[i].ProductName;
                wsProducts.Cell(4 + i, 4).Value = topProducts[i].QuantitySold;
                wsProducts.Cell(4 + i, 5).Value = topProducts[i].Revenue;
                wsProducts.Cell(4 + i, 5).Style.NumberFormat.Format = "#,##0";
            }
            wsProducts.Columns().AdjustToContents();

            // Sheet 3: Top khách hàng
            var wsCustomers = workbook.Worksheets.Add("Top khách hàng");
            wsCustomers.Cell(1, 1).Value = "TOP KHÁCH HÀNG";
            wsCustomers.Cell(1, 1).Style.Font.Bold = true;
            wsCustomers.Cell(1, 1).Style.Font.FontSize = 14;
            wsCustomers.Range(1, 1, 1, 4).Merge();

            string[] custHeaders = ["#", "Khách hàng", "Số hóa đơn", "Tổng mua"];
            for (int i = 0; i < custHeaders.Length; i++)
                wsCustomers.Cell(3, i + 1).Value = custHeaders[i];
            var ch = wsCustomers.Range(3, 1, 3, custHeaders.Length);
            ch.Style.Font.Bold = true;
            ch.Style.Fill.BackgroundColor = XLColor.FromHtml("#4F46E5");
            ch.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < topCustomers.Count; i++)
            {
                wsCustomers.Cell(4 + i, 1).Value = i + 1;
                wsCustomers.Cell(4 + i, 2).Value = topCustomers[i].CustomerName;
                wsCustomers.Cell(4 + i, 3).Value = topCustomers[i].InvoiceCount;
                wsCustomers.Cell(4 + i, 4).Value = topCustomers[i].TotalAmount;
                wsCustomers.Cell(4 + i, 4).Style.NumberFormat.Format = "#,##0";
            }
            wsCustomers.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
            return ServiceResult<string>.Ok(filePath, "Đã xuất báo cáo ra file Excel.");
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Fail($"Lỗi khi xuất Excel báo cáo: {ex.Message}");
        }
    }
}

/// <summary>
/// DTO dùng cho xuất Excel kiểm kê.
/// </summary>
public sealed class StocktakeExportRow
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int SystemQuantity { get; set; }
    public int ActualQuantity { get; set; }
    public int Difference => ActualQuantity - SystemQuantity;
}
