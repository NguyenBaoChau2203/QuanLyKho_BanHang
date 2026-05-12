using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class StocktakeService
{
    public ServiceResult<int> CreateStocktake(StocktakeDto stocktake)
    {
        if (stocktake.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Phiếu kiểm kê phải có ít nhất một dòng hàng.");
        }

        return ServiceResult<int>.Ok(0, "Service kiểm kê đang chờ Dũ triển khai transaction thật.");
    }
}
