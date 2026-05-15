using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DAL.Inventory;
using QuanLyKhoBanHang.DAL.MasterData;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class StocktakeService
{
    private readonly StocktakeRepository _stocktakeRepository;
    private readonly ProductRepository _productRepository;
    private readonly StockTransactionRepository _stockTransactionRepository;

    public StocktakeService() : this(new DatabaseOptions())
    {
    }

    public StocktakeService(DatabaseOptions options)
    {
        _stocktakeRepository = new StocktakeRepository(options);
        _productRepository = new ProductRepository(options);
        _stockTransactionRepository = new StockTransactionRepository(options);
    }

    public ServiceResult<int> CreateStocktake(StocktakeDto stocktake)
    {
        if (string.IsNullOrWhiteSpace(stocktake.StocktakeCode))
        {
            return ServiceResult<int>.Fail("Mã phiếu kiểm kê là bắt buộc.");
        }

        if (stocktake.CreatedByUserId <= 0)
        {
            return ServiceResult<int>.Fail("Người tạo không hợp lệ.");
        }

        if (stocktake.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Phiếu kiểm kê phải có ít nhất một dòng hàng.");
        }

        try
        {
            if (_stocktakeRepository.StocktakeCodeExists(stocktake.StocktakeCode))
            {
                return ServiceResult<int>.Fail("Mã phiếu kiểm kê đã tồn tại.");
            }

            foreach (var line in stocktake.Lines)
            {
                if (line.ProductId <= 0)
                {
                    return ServiceResult<int>.Fail($"Sản phẩm không hợp lệ.");
                }

                if (line.SystemQuantity < 0 || line.ActualQuantity < 0)
                {
                    return ServiceResult<int>.Fail($"Số lượng không được âm.");
                }

                var product = _productRepository.GetById(line.ProductId);
                if (product == null)
                {
                    return ServiceResult<int>.Fail($"Không tìm thấy sản phẩm với ID {line.ProductId}.");
                }

                line.SystemQuantity = product.QuantityOnHand;
            }

            var productUpdates = new List<ProductQuantityUpdateDto>();
            var stockTransactions = new List<StockTransactionDto>();

            foreach (var line in stocktake.Lines)
            {
                int difference = line.ActualQuantity - line.SystemQuantity;

                if (difference != 0)
                {
                    int newQuantity = line.ActualQuantity;

                    productUpdates.Add(new ProductQuantityUpdateDto
                    {
                        ProductId = line.ProductId,
                        QuantityChange = difference
                    });

                    stockTransactions.Add(new StockTransactionDto
                    {
                        ProductId = line.ProductId,
                        TransactionType = StockTransactionType.StocktakeAdjustment,
                        QuantityChange = difference,
                        QuantityAfter = newQuantity,
                        ReferenceCode = stocktake.StocktakeCode,
                        CreatedAt = DateTime.Now,
                        CreatedByUserId = stocktake.CreatedByUserId,
                        Note = $"Điều chỉnh tồn kho từ kiểm kê {stocktake.StocktakeCode}"
                    });
                }
            }

            int stocktakeId = _stocktakeRepository.CreateStocktakeWithTransaction(stocktake, productUpdates, stockTransactions);

            if (stocktakeId <= 0)
            {
                return ServiceResult<int>.Fail("Không thể tạo phiếu kiểm kê.");
            }

            return ServiceResult<int>.Ok(stocktakeId, "Đã tạo phiếu kiểm kê thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail($"Lỗi khi tạo phiếu kiểm kê: {ex.Message}");
        }
    }

    public ServiceResult<List<StocktakeDto>> GetStocktakes(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<StocktakeDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var stocktakes = _stocktakeRepository.GetStocktakes(fromDate, toDate);
            return ServiceResult<List<StocktakeDto>>.Ok(stocktakes, stocktakes.Count > 0 ? "Đã tải danh sách phiếu kiểm kê." : "Chưa có dữ liệu phiếu kiểm kê.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<StocktakeDto>>.Fail($"Lỗi khi tải danh sách phiếu kiểm kê: {ex.Message}");
        }
    }

    public ServiceResult<StocktakeDto> GetStocktakeById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<StocktakeDto>.Fail("Id phiếu kiểm kê không hợp lệ.");
        }

        try
        {
            var stocktake = _stocktakeRepository.GetStocktakeById(id);
            return stocktake != null
                ? ServiceResult<StocktakeDto>.Ok(stocktake, "Đã tải phiếu kiểm kê.")
                : ServiceResult<StocktakeDto>.Fail("Không tìm thấy phiếu kiểm kê.");
        }
        catch (Exception ex)
        {
            return ServiceResult<StocktakeDto>.Fail($"Lỗi khi tải phiếu kiểm kê: {ex.Message}");
        }
    }
}
