using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Sales;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class SalesService
{
    public ServiceResult<int> CreateInvoice(SalesInvoiceDto invoice)
    {
        if (invoice.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Hóa đơn phải có ít nhất một dòng hàng.");
        }

        return ServiceResult<int>.Ok(0, "Service bán hàng đang chờ Hùng triển khai transaction thật.");
    }
}
