using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DAL.MasterData;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class SupplierService
{
    private readonly SupplierRepository _supplierRepository;

    public SupplierService() : this(new DatabaseOptions())
    {
    }

    public SupplierService(DatabaseOptions options)
    {
        _supplierRepository = new SupplierRepository(options);
    }

    public ServiceResult<List<SupplierDto>> GetAllSuppliers()
    {
        try
        {
            var suppliers = _supplierRepository.GetAll();
            return ServiceResult<List<SupplierDto>>.Ok(suppliers, suppliers.Count > 0 ? "Đã tải danh sách nhà cung cấp." : "Chưa có dữ liệu nhà cung cấp.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SupplierDto>>.Fail($"Lỗi khi tải danh sách nhà cung cấp: {ex.Message}");
        }
    }

    public ServiceResult<List<SupplierDto>> SearchSuppliers(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return ServiceResult<List<SupplierDto>>.Fail("Từ khóa tìm kiếm không được để trống.");
        }

        try
        {
            var suppliers = _supplierRepository.Search(keyword);
            return ServiceResult<List<SupplierDto>>.Ok(suppliers, suppliers.Count > 0 ? "Đã tìm thấy nhà cung cấp." : "Không tìm thấy nhà cung cấp nào.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<SupplierDto>>.Fail($"Lỗi khi tìm kiếm nhà cung cấp: {ex.Message}");
        }
    }

    public ServiceResult<int> CreateSupplier(SupplierDto supplier)
    {
        if (string.IsNullOrWhiteSpace(supplier.Code) || string.IsNullOrWhiteSpace(supplier.Name))
        {
            return ServiceResult<int>.Fail("Mã nhà cung cấp và tên nhà cung cấp là bắt buộc.");
        }

        try
        {
            if (_supplierRepository.CodeExists(supplier.Code))
            {
                return ServiceResult<int>.Fail("Mã nhà cung cấp đã tồn tại.");
            }

            var id = _supplierRepository.Create(supplier);
            return id > 0
                ? ServiceResult<int>.Ok(id, "Đã tạo nhà cung cấp thành công.")
                : ServiceResult<int>.Fail("Không thể tạo nhà cung cấp.");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail($"Lỗi khi tạo nhà cung cấp: {ex.Message}");
        }
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

        try
        {
            if (_supplierRepository.CodeExists(supplier.Code, supplier.Id))
            {
                return ServiceResult<bool>.Fail("Mã nhà cung cấp đã tồn tại.");
            }

            var result = _supplierRepository.Update(supplier);
            return result
                ? ServiceResult<bool>.Ok(true, "Đã cập nhật nhà cung cấp thành công.")
                : ServiceResult<bool>.Fail("Không thể cập nhật nhà cung cấp.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail($"Lỗi khi cập nhật nhà cung cấp: {ex.Message}");
        }
    }

    public ServiceResult<bool> DeactivateSupplier(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<bool>.Fail("Id nhà cung cấp không hợp lệ.");
        }

        try
        {
            var result = _supplierRepository.Deactivate(id);
            return result
                ? ServiceResult<bool>.Ok(true, "Đã vô hiệu hóa nhà cung cấp thành công.")
                : ServiceResult<bool>.Fail("Không thể vô hiệu hóa nhà cung cấp.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail($"Lỗi khi vô hiệu hóa nhà cung cấp: {ex.Message}");
        }
    }
}
