using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class PurchaseService
{
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

        if (receipt.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Phiếu nhập phải có ít nhất một dòng hàng.");
        }

        return ServiceResult<int>.Ok(0, "Service nhập kho đang chờ Dũ triển khai transaction thật.");
    }

    public ServiceResult<List<PurchaseReceiptDto>> GetReceipts(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<PurchaseReceiptDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        return ServiceResult<List<PurchaseReceiptDto>>.Ok(new List<PurchaseReceiptDto>(), "Chưa có dữ liệu phiếu nhập.");
    }

    public ServiceResult<PurchaseReceiptDto> GetReceiptById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<PurchaseReceiptDto>.Fail("Id phiếu nhập không hợp lệ.");
        }

        return ServiceResult<PurchaseReceiptDto>.Fail("Chưa có dữ liệu phiếu nhập theo id.");
    }
}
