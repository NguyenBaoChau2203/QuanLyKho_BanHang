using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL; // Để gọi SalesRepository
using QuanLyKhoBanHang.DTO.Sales;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class SalesService
{
    private readonly SalesRepository _repo;

    public SalesService()
    {
        _repo = new SalesRepository();
    }

    // 1. Hàm tạo hóa đơn (Bạn đã làm xong - Giữ nguyên)
    public ServiceResult<int> CreateInvoice(SalesInvoiceDto invoice)
    {
        if (string.IsNullOrWhiteSpace(invoice.InvoiceCode))
        {
            return ServiceResult<int>.Fail("Mã hóa đơn là bắt buộc.");
        }

        if (invoice.Lines == null || invoice.Lines.Count == 0)
        {
            return ServiceResult<int>.Fail("Hóa đơn phải có ít nhất một dòng hàng.");
        }

        decimal total = 0;
        foreach (var line in invoice.Lines)
        {
            if (line.Quantity <= 0)
            {
                return ServiceResult<int>.Fail($"Sản phẩm '{line.ProductName}' có số lượng không hợp lệ.");
            }

            line.LineTotal = line.Quantity * line.UnitPrice;
            total += line.LineTotal;
        }
        invoice.TotalAmount = total;

        if (invoice.DiscountAmount < 0 || invoice.DiscountAmount > invoice.TotalAmount)
        {
            return ServiceResult<int>.Fail("Số tiền giảm giá không hợp lệ.");
        }

        try
        {
            _repo.CreateSalesInvoice(invoice);
            return ServiceResult<int>.Ok(1, "Lưu hóa đơn và cập nhật kho thành công!");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail("Lỗi hệ thống: " + ex.Message);
        }
    }

    // 2. Hàm lấy danh sách hóa đơn (Đã sửa lại để gọi DAL)
    public ServiceResult<List<SalesInvoiceDto>> GetInvoices(DateTime fromDate, DateTime toDate)
    {
        if (fromDate.Date > toDate.Date)
        {
            return ServiceResult<List<SalesInvoiceDto>>.Fail("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
        }

        try
        {
            var data = _repo.GetInvoices(fromDate, toDate);
            return ServiceResult<List<SalesInvoiceDto>>.Ok(data, $"Tìm thấy {data.Count} hóa đơn.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SalesInvoiceDto>>.Fail("Lỗi khi lấy danh sách hóa đơn: " + ex.Message);
        }
    }

    // 3. Hàm lấy chi tiết một hóa đơn (Đã sửa lại để gọi DAL)
    public ServiceResult<SalesInvoiceDto> GetInvoiceById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<SalesInvoiceDto>.Fail("Id hóa đơn không hợp lệ.");
        }

        try
        {
            var invoice = _repo.GetInvoiceById(id);
            if (invoice == null)
            {
                return ServiceResult<SalesInvoiceDto>.Fail("Không tìm thấy hóa đơn có ID này.");
            }
            return ServiceResult<SalesInvoiceDto>.Ok(invoice, "Lấy thông tin hóa đơn thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<SalesInvoiceDto>.Fail("Lỗi hệ thống: " + ex.Message);
        }
    }
}
