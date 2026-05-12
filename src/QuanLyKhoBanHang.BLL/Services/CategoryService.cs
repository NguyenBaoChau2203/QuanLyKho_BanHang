using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class CategoryService
{
    public ServiceResult<List<CategoryDto>> GetAllCategories()
    {
        return ServiceResult<List<CategoryDto>>.Ok(new List<CategoryDto>(), "Chưa có dữ liệu loại hàng.");
    }
}
