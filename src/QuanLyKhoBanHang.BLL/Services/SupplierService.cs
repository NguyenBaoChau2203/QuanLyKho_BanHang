using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class SupplierService
{
    public ServiceResult<List<SupplierDto>> GetAllSuppliers()
    {
        return ServiceResult<List<SupplierDto>>.Ok(new List<SupplierDto>(), "Chưa có dữ liệu nhà cung cấp.");
    }
}
