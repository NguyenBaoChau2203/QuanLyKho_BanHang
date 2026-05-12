using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Assistant;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AssistantService
{
    public ServiceResult<AssistantResponseDto> Ask(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return ServiceResult<AssistantResponseDto>.Fail("Vui lòng nhập câu hỏi hoặc câu lệnh.");
        }

        var normalized = question.Trim().ToLowerInvariant();
        var response = new AssistantResponseDto
        {
            Intent = "unknown",
            Handled = true
        };

        if (normalized.Contains("doanh thu"))
        {
            response.Intent = "revenue";
            response.Answer = "Đã nhận câu hỏi doanh thu. Hùng sẽ nối ReportService để trả dữ liệu thật.";
        }
        else if (normalized.Contains("sắp hết") || normalized.Contains("tồn thấp"))
        {
            response.Intent = "low-stock";
            response.Answer = "Đã nhận câu hỏi tồn thấp. Dũ sẽ nối InventoryService để trả dữ liệu thật.";
        }
        else if (normalized.Contains("top") || normalized.Contains("bán chạy"))
        {
            response.Intent = "top-products";
            response.Answer = "Đã nhận câu hỏi top sản phẩm bán chạy. Hùng sẽ nối ReportService để trả dữ liệu thật.";
        }
        else if ((normalized.Contains("khách") || normalized.Contains("khach")) && (normalized.Contains("mua") || normalized.Contains("nhiều") || normalized.Contains("nhieu")))
        {
            response.Intent = "top-customers";
            response.Answer = "Đã nhận câu hỏi khách hàng mua nhiều nhất. Hùng sẽ nối ReportService để trả dữ liệu thật.";
        }
        else if (normalized.Contains("kiểm kê") || normalized.Contains("kiem ke"))
        {
            response.Intent = "stocktake-today";
            response.Answer = "Đã nhận câu hỏi kiểm kê hôm nay. Dũ sẽ nối StocktakeService để trả dữ liệu thật.";
        }
        else
        {
            response.Handled = false;
            response.Answer = "Trợ lý chưa hiểu câu này. Hãy thử: doanh thu hôm nay, hàng sắp hết, top sản phẩm bán chạy, khách hàng mua nhiều nhất, kiểm kê hôm nay.";
        }

        return ServiceResult<AssistantResponseDto>.Ok(response);
    }
}
