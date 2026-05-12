using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class StocktakeService
{
    public ServiceResult<int> CreateStocktake(StocktakeDto stocktake)
    {
        if (string.IsNullOrWhiteSpace(stocktake.StocktakeCode))
        {
            return ServiceResult<int>.Fail("Mã phiếu kiểm kê là bắt buộc.");
        }

        if (stocktake.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Phiếu kiểm kê phải có ít nhất một dòng hàng.");
        }

        return ServiceResult<int>.Ok(0, "Service kiểm kê đang chờ Dũ triển khai transaction thật.");
    }

    public ServiceResult<List<StocktakeDto>> GetStocktakes(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<StocktakeDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        return ServiceResult<List<StocktakeDto>>.Ok(new List<StocktakeDto>(), "Chưa có dữ liệu phiếu kiểm kê.");
    }

    public ServiceResult<StocktakeDto> GetStocktakeById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<StocktakeDto>.Fail("Id phiếu kiểm kê không hợp lệ.");
        }

        return ServiceResult<StocktakeDto>.Fail("Chưa có dữ liệu phiếu kiểm kê theo id.");
    }
}
