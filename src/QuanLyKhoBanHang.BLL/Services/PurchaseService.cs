using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Inventory;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class PurchaseService
{
    public ServiceResult<int> CreateReceipt(PurchaseReceiptDto receipt)
    {
        if (receipt.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Phiếu nhập phải có ít nhất một dòng hàng.");
        }

        return ServiceResult<int>.Ok(0, "Service nhập kho đang chờ Dũ triển khai transaction thật.");
    }
}
