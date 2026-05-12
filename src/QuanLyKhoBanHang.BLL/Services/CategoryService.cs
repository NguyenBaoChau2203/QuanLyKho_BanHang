using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class CategoryService
{
    public ServiceResult<List<CategoryDto>> GetAllCategories()
    {
        return ServiceResult<List<CategoryDto>>.Ok(new List<CategoryDto>(), "Chưa có dữ liệu loại hàng.");
    }

    public ServiceResult<int> CreateCategory(CategoryDto category)
    {
        if (string.IsNullOrWhiteSpace(category.Code) || string.IsNullOrWhiteSpace(category.Name))
        {
            return ServiceResult<int>.Fail("Mã loại hàng và tên loại hàng là bắt buộc.");
        }

        return ServiceResult<int>.Ok(0, "Service loại hàng đang ở chế độ stub.");
    }

    public ServiceResult<bool> UpdateCategory(CategoryDto category)
    {
        if (category.Id <= 0)
        {
            return ServiceResult<bool>.Fail("Id loại hàng không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(category.Code) || string.IsNullOrWhiteSpace(category.Name))
        {
            return ServiceResult<bool>.Fail("Mã loại hàng và tên loại hàng là bắt buộc.");
        }

        return ServiceResult<bool>.Ok(true, "Service loại hàng đang ở chế độ stub.");
    }

    public ServiceResult<bool> DeactivateCategory(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<bool>.Fail("Id loại hàng không hợp lệ.");
        }

        return ServiceResult<bool>.Ok(true, "Service loại hàng đang ở chế độ stub.");
    }
}
