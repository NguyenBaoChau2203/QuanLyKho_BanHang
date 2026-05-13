using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Sales;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class SalesService
{
    public ServiceResult<int> CreateInvoice(SalesInvoiceDto invoice)
    {
        if (string.IsNullOrWhiteSpace(invoice.InvoiceCode))
        {
            return ServiceResult<int>.Fail("Mã hóa đơn là bắt buộc.");
        }

        if (invoice.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Hóa đơn phải có ít nhất một dòng hàng.");
        }

        return ServiceResult<int>.Ok(0, "Service bán hàng đang chờ Hùng triển khai transaction thật.");
    }

    public ServiceResult<List<SalesInvoiceDto>> GetInvoices(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<SalesInvoiceDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        return ServiceResult<List<SalesInvoiceDto>>.Ok(new List<SalesInvoiceDto>(), "Chưa có dữ liệu hóa đơn.");
    }

    public ServiceResult<SalesInvoiceDto> GetInvoiceById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<SalesInvoiceDto>.Fail("Id hóa đơn không hợp lệ.");
        }

        return ServiceResult<SalesInvoiceDto>.Fail("Chưa có dữ liệu hóa đơn theo id.");
    }
}
