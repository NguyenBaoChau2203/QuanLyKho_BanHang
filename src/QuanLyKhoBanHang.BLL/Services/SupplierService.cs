using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class SupplierService
{
    public ServiceResult<List<SupplierDto>> GetAllSuppliers()
    {
        return ServiceResult<List<SupplierDto>>.Ok(new List<SupplierDto>(), "Chưa có dữ liệu nhà cung cấp.");
    }

    public ServiceResult<List<SupplierDto>> SearchSuppliers(string keyword)
    {
        return ServiceResult<List<SupplierDto>>.Ok(new List<SupplierDto>(), "Chưa có dữ liệu tìm kiếm nhà cung cấp.");
    }

    public ServiceResult<int> CreateSupplier(SupplierDto supplier)
    {
        if (string.IsNullOrWhiteSpace(supplier.Code) || string.IsNullOrWhiteSpace(supplier.Name))
        {
            return ServiceResult<int>.Fail("Mã nhà cung cấp và tên nhà cung cấp là bắt buộc.");
        }

        return ServiceResult<int>.Ok(0, "Service nhà cung cấp đang ở chế độ stub.");
    }

    public ServiceResult<bool> UpdateSupplier(SupplierDto supplier)
    {
        if (supplier.Id <= 0)
        {
            return ServiceResult<bool>.Fail("Id nhà cung cấp không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(supplier.Code) || string.IsNullOrWhiteSpace(supplier.Name))
        {
            return ServiceResult<bool>.Fail("Mã nhà cung cấp và tên nhà cung cấp là bắt buộc.");
        }

        return ServiceResult<bool>.Ok(true, "Service nhà cung cấp đang ở chế độ stub.");
    }

    public ServiceResult<bool> DeactivateSupplier(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<bool>.Fail("Id nhà cung cấp không hợp lệ.");
        }

        return ServiceResult<bool>.Ok(true, "Service nhà cung cấp đang ở chế độ stub.");
    }
}
