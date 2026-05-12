using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Sales;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class CustomerService
{
    public ServiceResult<List<CustomerDto>> GetAllCustomers()
    {
        return ServiceResult<List<CustomerDto>>.Ok(new List<CustomerDto>(), "Chưa có dữ liệu khách hàng.");
    }
}
