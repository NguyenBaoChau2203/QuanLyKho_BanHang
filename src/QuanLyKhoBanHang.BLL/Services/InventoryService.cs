using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DAL.Inventory;
using QuanLyKhoBanHang.DAL.MasterData;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.DTO.Inventory;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class InventoryService
{
    private readonly ProductRepository _productRepository;
    private readonly StockTransactionRepository _stockTransactionRepository;

    public InventoryService(DatabaseOptions options)
    {
        _productRepository = new ProductRepository(options);
        _stockTransactionRepository = new StockTransactionRepository(options);
    }

    public ServiceResult<List<ProductDto>> GetCurrentStock()
    {
        try
        {
            var products = _productRepository.GetAll();
            return ServiceResult<List<ProductDto>>.Ok(products, products.Count > 0 ? "Đã tải tồn kho hiện tại." : "Chưa có dữ liệu tồn kho.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ProductDto>>.Fail($"Lỗi khi tải tồn kho: {ex.Message}");
        }
    }

    public ServiceResult<List<ProductDto>> GetLowStockProducts()
    {
        try
        {
            var products = _productRepository.GetLowStockProducts();
            return ServiceResult<List<ProductDto>>.Ok(products, products.Count > 0 ? $"Đã tìm thấy {products.Count} sản phẩm tồn thấp." : "Không có sản phẩm nào dưới mức tồn tối thiểu.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ProductDto>>.Fail($"Lỗi khi tải danh sách tồn thấp: {ex.Message}");
        }
    }

    public ServiceResult<List<StockTransactionDto>> GetStockTransactions(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<StockTransactionDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var transactions = _stockTransactionRepository.GetTransactions(fromDate, toDate);
            return ServiceResult<List<StockTransactionDto>>.Ok(transactions, transactions.Count > 0 ? "Đã tải lịch sử giao dịch kho." : "Không có giao dịch kho nào trong khoảng thời gian này.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<StockTransactionDto>>.Fail($"Lỗi khi tải lịch sử giao dịch kho: {ex.Message}");
        }
    }

    public ServiceResult<List<StockTransactionDto>> GetStockTransactionsByProduct(int productId, DateTime fromDate, DateTime toDate)
    {
        if (productId <= 0)
        {
            return ServiceResult<List<StockTransactionDto>>.Fail("Id sản phẩm không hợp lệ.");
        }

        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<StockTransactionDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var transactions = _stockTransactionRepository.GetTransactionsByProduct(productId, fromDate, toDate);
            return ServiceResult<List<StockTransactionDto>>.Ok(transactions, transactions.Count > 0 ? "Đã tải lịch sử giao dịch kho của sản phẩm." : "Không có giao dịch kho nào cho sản phẩm này trong khoảng thời gian này.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<StockTransactionDto>>.Fail($"Lỗi khi tải lịch sử giao dịch kho của sản phẩm: {ex.Message}");
        }
    }
}
