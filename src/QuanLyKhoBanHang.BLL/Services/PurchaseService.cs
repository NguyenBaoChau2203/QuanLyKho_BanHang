using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DAL.Inventory;
using QuanLyKhoBanHang.DAL.MasterData;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class PurchaseService
{
    private readonly PurchaseRepository _purchaseRepository;
    private readonly ProductRepository _productRepository;
    private readonly StockTransactionRepository _stockTransactionRepository;

    public PurchaseService() : this(new DatabaseOptions())
    {
    }

    public PurchaseService(DatabaseOptions options)
    {
        _purchaseRepository = new PurchaseRepository(options);
        _productRepository = new ProductRepository(options);
        _stockTransactionRepository = new StockTransactionRepository(options);
    }

    public ServiceResult<int> CreateReceipt(PurchaseReceiptDto receipt)
    {
        if (string.IsNullOrWhiteSpace(receipt.ReceiptCode))
        {
            return ServiceResult<int>.Fail("Mã phiếu nhập là bắt buộc.");
        }

        if (receipt.SupplierId <= 0)
        {
            return ServiceResult<int>.Fail("Nhà cung cấp không hợp lệ.");
        }

        if (receipt.CreatedByUserId <= 0)
        {
            return ServiceResult<int>.Fail("Người tạo không hợp lệ.");
        }

        if (receipt.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Phiếu nhập phải có ít nhất một dòng hàng.");
        }

        try
        {
            if (_purchaseRepository.ReceiptCodeExists(receipt.ReceiptCode))
            {
                return ServiceResult<int>.Fail("Mã phiếu nhập đã tồn tại.");
            }

            foreach (var line in receipt.Lines)
            {
                if (line.ProductId <= 0)
                {
                    return ServiceResult<int>.Fail($"Sản phẩm không hợp lệ.");
                }

                if (line.Quantity <= 0)
                {
                    return ServiceResult<int>.Fail($"Số lượng phải lớn hơn 0.");
                }

                if (line.UnitCost < 0)
                {
                    return ServiceResult<int>.Fail($"Đơn giá không được âm.");
                }
            }

            decimal totalAmount = receipt.Lines.Sum(l => l.Quantity * l.UnitCost);
            receipt.TotalAmount = totalAmount;

            var productUpdates = new List<ProductQuantityUpdateDto>();
            var stockTransactions = new List<StockTransactionDto>();

            foreach (var line in receipt.Lines)
            {
                var product = _productRepository.GetById(line.ProductId);
                if (product == null)
                {
                    return ServiceResult<int>.Fail($"Không tìm thấy sản phẩm với ID {line.ProductId}.");
                }

                int newQuantity = product.QuantityOnHand + line.Quantity;

                productUpdates.Add(new ProductQuantityUpdateDto
                {
                    ProductId = line.ProductId,
                    QuantityChange = line.Quantity
                });

                stockTransactions.Add(new StockTransactionDto
                {
                    ProductId = line.ProductId,
                    TransactionType = StockTransactionType.Purchase,
                    QuantityChange = line.Quantity,
                    QuantityAfter = newQuantity,
                    ReferenceCode = receipt.ReceiptCode,
                    CreatedAt = DateTime.Now,
                    CreatedByUserId = receipt.CreatedByUserId,
                    Note = $"Nhập kho từ phiếu {receipt.ReceiptCode}"
                });
            }

            int receiptId = _purchaseRepository.CreateReceiptWithTransaction(receipt, productUpdates, stockTransactions);

            if (receiptId <= 0)
            {
                return ServiceResult<int>.Fail("Không thể tạo phiếu nhập.");
            }

            return ServiceResult<int>.Ok(receiptId, "Đã tạo phiếu nhập thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail($"Lỗi khi tạo phiếu nhập: {ex.Message}");
        }
    }

    public ServiceResult<List<PurchaseReceiptDto>> GetReceipts(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<PurchaseReceiptDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var receipts = _purchaseRepository.GetReceipts(fromDate, toDate);
            return ServiceResult<List<PurchaseReceiptDto>>.Ok(receipts, receipts.Count > 0 ? "Đã tải danh sách phiếu nhập." : "Chưa có dữ liệu phiếu nhập.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<PurchaseReceiptDto>>.Fail($"Lỗi khi tải danh sách phiếu nhập: {ex.Message}");
        }
    }

    public ServiceResult<PurchaseReceiptDto> GetReceiptById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<PurchaseReceiptDto>.Fail("Id phiếu nhập không hợp lệ.");
        }

        try
        {
            var receipt = _purchaseRepository.GetReceiptById(id);
            return receipt != null
                ? ServiceResult<PurchaseReceiptDto>.Ok(receipt, "Đã tải phiếu nhập.")
                : ServiceResult<PurchaseReceiptDto>.Fail("Không tìm thấy phiếu nhập.");
        }
        catch (Exception ex)
        {
            return ServiceResult<PurchaseReceiptDto>.Fail($"Lỗi khi tải phiếu nhập: {ex.Message}");
        }
    }
}
