using QuanLyKhoBanHang.DTO.Assistant;
using QuanLyKhoBanHang.BLL.Services;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace QuanLyKhoBanHang.BLL.Services.Assistant;

public class RuleBasedAssistantProvider
{
    private readonly ReportService _reportService;
    private readonly InventoryService _inventoryService;
    private readonly StocktakeService _stocktakeService;

    public RuleBasedAssistantProvider(ReportService reportService, InventoryService inventoryService, StocktakeService stocktakeService)
    {
        _reportService = reportService;
        _inventoryService = inventoryService;
        _stocktakeService = stocktakeService;
    }

    /// <summary>
    /// Xây dựng ngữ cảnh an toàn cho trợ lý.
    /// Trả về IReadOnlyList để khớp hoàn toàn với AssistantService.
    /// </summary>
    public IReadOnlyList<AssistantSafeContext> BuildSafeContexts()
    {
        return new List<AssistantSafeContext>
        {
            new AssistantSafeContext
            {
                Key = "SystemStatus",
                Value = "Cửa hàng đang hoạt động bình thường.",
                Title = "Trạng thái",
                Intent = "System.Status",
                Answer = "Hệ thống đã sẵn sàng."
            }
        };
    }

    public AssistantResponseDto Ask(string question, string mode, string statusMessage, bool isFallback)
    {
        var response = new AssistantResponseDto
        {
            Intent = "Unknown",
            Handled = false,
            Mode = mode,
            StatusMessage = statusMessage,
            IsFallback = isFallback,
            Answer = "Xin lỗi, tôi chưa hiểu ý bạn. Hiện tại tôi có thể giúp bạn xem 'doanh thu hôm nay', 'doanh thu tháng này', 'top sản phẩm bán chạy', hoặc 'khách hàng mua nhiều nhất'."
        };

        if (string.IsNullOrWhiteSpace(question)) return response;

        string cmd = question.ToLower().Trim();
        DateTime today = DateTime.Today;
        DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        // 1. Lệnh: DOANH THU HÔM NAY
        if (cmd.Contains("doanh thu hôm nay"))
        {
            var res = _reportService.GetRevenue(today, today);
            // Nếu code báo đỏ chữ Success, hãy đổi thành IsSuccess theo đúng ServiceResult của bạn
            if (res != null && res.Success && res.Data != null && res.Data.Any())
            {
                var data = res.Data.First();
                response.Answer = $"Doanh thu hôm nay là {data.Revenue:N0} VNĐ với tổng cộng {data.InvoiceCount} hóa đơn.";
            }
            else response.Answer = "Hôm nay cửa hàng chưa có doanh thu nào.";

            response.Handled = true;
            response.Intent = "Report.RevenueToday";
            return response;
        }

        // 2. Lệnh: DOANH THU THÁNG NÀY
        if (cmd.Contains("doanh thu tháng này"))
        {
            var res = _reportService.GetRevenue(firstDayOfMonth, today);
            if (res != null && res.Success && res.Data != null && res.Data.Any())
            {
                decimal totalRev = res.Data.Sum(x => x.Revenue);
                int totalInv = res.Data.Sum(x => x.InvoiceCount);
                response.Answer = $"Doanh thu tháng này đạt {totalRev:N0} VNĐ với tổng cộng {totalInv} hóa đơn.";
            }
            else response.Answer = "Tháng này cửa hàng chưa có doanh thu nào.";

            response.Handled = true;
            response.Intent = "Report.RevenueMonth";
            return response;
        }

        // 3. Lệnh: TOP SẢN PHẨM BÁN CHẠY
        if (cmd.Contains("top sản phẩm") || cmd.Contains("bán chạy"))
        {
            var res = _reportService.GetTopSellingProducts(firstDayOfMonth, today, 5);
            if (res != null && res.Success && res.Data != null && res.Data.Any())
            {
                StringBuilder sb = new StringBuilder("Top sản phẩm bán chạy nhất tháng này gồm có:\n");
                foreach (var item in res.Data)
                {
                    sb.AppendLine($"- {item.ProductName}: Đã bán được {item.QuantitySold} cái.");
                }
                response.Answer = sb.ToString();
            }
            else response.Answer = "Chưa có dữ liệu sản phẩm bán chạy trong tháng này.";

            response.Handled = true;
            response.Intent = "Report.TopProducts";
            return response;
        }

        // 4. Lệnh: KHÁCH HÀNG MUA NHIỀU NHẤT
        if (cmd.Contains("khách hàng") || cmd.Contains("mua nhiều"))
        {
            var res = _reportService.GetTopCustomers(firstDayOfMonth, today, 5);
            if (res != null && res.Success && res.Data != null && res.Data.Any())
            {
                StringBuilder sb = new StringBuilder("Top những khách hàng mua nhiều nhất tháng này là:\n");
                foreach (var item in res.Data)
                {
                    sb.AppendLine($"- {item.CustomerName}: Đã chi tiêu {item.TotalAmount:N0} VNĐ.");
                }
                response.Answer = sb.ToString();
            }
            else response.Answer = "Chưa có dữ liệu khách hàng mua hàng trong tháng này.";

            response.Handled = true;
            response.Intent = "Report.TopCustomers";
            return response;
        }

        return response;
    }
}
