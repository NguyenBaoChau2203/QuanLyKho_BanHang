using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL;
using QuanLyKhoBanHang.DTO.Sales;
using System;
using System.Collections.Generic;

namespace QuanLyKhoBanHang.BLL.Services
{
    public sealed class CustomerService
    {
        private readonly CustomerRepository _repo;

        public CustomerService()
        {
            _repo = new CustomerRepository();
        }

        public ServiceResult<List<CustomerDto>> GetAllCustomers()
        {
            try { return ServiceResult<List<CustomerDto>>.Ok(_repo.GetAllCustomers(), "Thành công."); }
            catch (Exception ex) { return ServiceResult<List<CustomerDto>>.Fail("Lỗi: " + ex.Message); }
        }

        public ServiceResult<List<CustomerDto>> SearchCustomers(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword)) return GetAllCustomers();
                return ServiceResult<List<CustomerDto>>.Ok(_repo.SearchCustomers(keyword), "Thành công.");
            }
            catch (Exception ex) { return ServiceResult<List<CustomerDto>>.Fail("Lỗi: " + ex.Message); }
        }

        public ServiceResult<CustomerDto> GetCustomerById(int id)
        {
            try
            {
                var cust = _repo.GetCustomerById(id);
                if (cust == null) return ServiceResult<CustomerDto>.Fail("Không tìm thấy khách hàng.");
                return ServiceResult<CustomerDto>.Ok(cust, "Thành công.");
            }
            catch (Exception ex) { return ServiceResult<CustomerDto>.Fail("Lỗi: " + ex.Message); }
        }

        public ServiceResult<int> CreateCustomer(CustomerDto customer)
        {
            // Bài test: Tạo khách hàng thiếu tên thì fail
            if (string.IsNullOrWhiteSpace(customer.Name)) // Đã đổi thành Name
            {
                return ServiceResult<int>.Fail("Lỗi: Tên khách hàng là thông tin bắt buộc!");
            }

            try { return ServiceResult<int>.Ok(_repo.CreateCustomer(customer), "Thêm mới khách hàng thành công."); }
            catch (Exception ex) { return ServiceResult<int>.Fail("Lỗi hệ thống: " + ex.Message); }
        }

        public ServiceResult<bool> UpdateCustomer(CustomerDto customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Name)) return ServiceResult<bool>.Fail("Lỗi: Tên khách hàng là thông tin bắt buộc!");
            if (customer.Id <= 0) return ServiceResult<bool>.Fail("Lỗi: Mã khách hàng không hợp lệ.");

            try
            {
                _repo.UpdateCustomer(customer);
                return ServiceResult<bool>.Ok(true, "Cập nhật thông tin thành công.");
            }
            catch (Exception ex) { return ServiceResult<bool>.Fail("Lỗi hệ thống: " + ex.Message); }
        }

        public ServiceResult<bool> DeactivateCustomer(int id)
        {
            if (id <= 0) return ServiceResult<bool>.Fail("Lỗi: Mã khách hàng không hợp lệ.");

            try
            {
                _repo.DeactivateCustomer(id);
                return ServiceResult<bool>.Ok(true, "Đã vô hiệu hóa khách hàng thành công.");
            }
            catch (Exception ex) { return ServiceResult<bool>.Fail("Lỗi hệ thống: " + ex.Message); }
        }
    }
}
