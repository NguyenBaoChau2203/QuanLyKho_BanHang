using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Sales;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class CustomerService
{
    public ServiceResult<List<CustomerDto>> GetAllCustomers()
    {
        return ServiceResult<List<CustomerDto>>.Ok(new List<CustomerDto>(), "Chưa có dữ liệu khách hàng.");
    }

    public ServiceResult<List<CustomerDto>> SearchCustomers(string keyword)
    {
        return ServiceResult<List<CustomerDto>>.Ok(new List<CustomerDto>(), "Chưa có dữ liệu tìm kiếm khách hàng.");
    }

    public ServiceResult<CustomerDto> GetCustomerById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<CustomerDto>.Fail("Id khách hàng không hợp lệ.");
        }

        return ServiceResult<CustomerDto>.Fail("Chưa có dữ liệu khách hàng theo id.");
    }

    public ServiceResult<int> CreateCustomer(CustomerDto customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Code) || string.IsNullOrWhiteSpace(customer.Name))
        {
            return ServiceResult<int>.Fail("Mã khách hàng và tên khách hàng là bắt buộc.");
        }

        return ServiceResult<int>.Ok(0, "Service khách hàng đang ở chế độ stub.");
    }

    public ServiceResult<bool> UpdateCustomer(CustomerDto customer)
    {
        if (customer.Id <= 0)
        {
            return ServiceResult<bool>.Fail("Id khách hàng không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(customer.Code) || string.IsNullOrWhiteSpace(customer.Name))
        {
            return ServiceResult<bool>.Fail("Mã khách hàng và tên khách hàng là bắt buộc.");
        }

        return ServiceResult<bool>.Ok(true, "Service khách hàng đang ở chế độ stub.");
    }

    public ServiceResult<bool> DeactivateCustomer(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<bool>.Fail("Id khách hàng không hợp lệ.");
        }

        return ServiceResult<bool>.Ok(true, "Service khách hàng đang ở chế độ stub.");
    }
}
