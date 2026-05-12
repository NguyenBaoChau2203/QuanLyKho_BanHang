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

        return ServiceResult<int>.Ok(1, "Phiếu kiểm kê demo đã sẵn sàng theo contract hiện tại.");
    }

    public ServiceResult<List<StocktakeDto>> GetStocktakes(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<StocktakeDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        var rows = new List<StocktakeDto>
        {
            new()
            {
                Id = 1,
                StocktakeCode = "KK0001",
                StocktakeDate = DateTime.Today.AddDays(-1),
                CreatedByUserId = 4,
                Note = "Kiểm kê demo quầy trưng bày",
                Lines =
                [
                    new StocktakeLineDto { Id = 1, StocktakeId = 1, ProductId = 4, ProductName = "Nước rửa chén 750ml", SystemQuantity = 32, ActualQuantity = 30 },
                    new StocktakeLineDto { Id = 2, StocktakeId = 1, ProductId = 5, ProductName = "Kem đánh răng 110g", SystemQuantity = 30, ActualQuantity = 30 },
                    new StocktakeLineDto { Id = 3, StocktakeId = 1, ProductId = 6, ProductName = "Khăn giấy 100 tờ", SystemQuantity = 118, ActualQuantity = 118 }
                ]
            }
        };

        return ServiceResult<List<StocktakeDto>>.Ok(rows.Where(x => x.StocktakeDate.Date >= fromDate.Date && x.StocktakeDate.Date <= toDate.Date).ToList(), "Danh sách kiểm kê demo từ seed.");
    }

    public ServiceResult<StocktakeDto> GetStocktakeById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<StocktakeDto>.Fail("Id phiếu kiểm kê không hợp lệ.");
        }

        var result = GetStocktakes(DateTime.Today.AddDays(-30), DateTime.Today);
        var stocktake = result.Data?.FirstOrDefault(x => x.Id == id);
        return stocktake is null
            ? ServiceResult<StocktakeDto>.Fail("Chưa có dữ liệu phiếu kiểm kê theo id.")
            : ServiceResult<StocktakeDto>.Ok(stocktake, "Đã tải phiếu kiểm kê demo.");
    }
}
