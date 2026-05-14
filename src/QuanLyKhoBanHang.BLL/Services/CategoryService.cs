using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DAL.MasterData;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class CategoryService
{
    private readonly CategoryRepository _categoryRepository;

    public CategoryService(DatabaseOptions options)
    {
        _categoryRepository = new CategoryRepository(options);
    }

    public ServiceResult<List<CategoryDto>> GetAllCategories()
    {
        try
        {
            var categories = _categoryRepository.GetAll();
            return ServiceResult<List<CategoryDto>>.Ok(categories, categories.Count > 0 ? "Đã tải danh sách loại hàng." : "Chưa có dữ liệu loại hàng.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<CategoryDto>>.Fail($"Lỗi khi tải danh sách loại hàng: {ex.Message}");
        }
    }

    public ServiceResult<int> CreateCategory(CategoryDto category)
    {
        if (string.IsNullOrWhiteSpace(category.Code) || string.IsNullOrWhiteSpace(category.Name))
        {
            return ServiceResult<int>.Fail("Mã loại hàng và tên loại hàng là bắt buộc.");
        }

        try
        {
            if (_categoryRepository.CodeExists(category.Code))
            {
                return ServiceResult<int>.Fail("Mã loại hàng đã tồn tại.");
            }

            var id = _categoryRepository.Create(category);
            return id > 0
                ? ServiceResult<int>.Ok(id, "Đã tạo loại hàng thành công.")
                : ServiceResult<int>.Fail("Không thể tạo loại hàng.");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail($"Lỗi khi tạo loại hàng: {ex.Message}");
        }
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

        try
        {
            if (_categoryRepository.CodeExists(category.Code, category.Id))
            {
                return ServiceResult<bool>.Fail("Mã loại hàng đã tồn tại.");
            }

            var result = _categoryRepository.Update(category);
            return result
                ? ServiceResult<bool>.Ok(true, "Đã cập nhật loại hàng thành công.")
                : ServiceResult<bool>.Fail("Không thể cập nhật loại hàng.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail($"Lỗi khi cập nhật loại hàng: {ex.Message}");
        }
    }

    public ServiceResult<bool> DeactivateCategory(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<bool>.Fail("Id loại hàng không hợp lệ.");
        }

        try
        {
            var result = _categoryRepository.Deactivate(id);
            return result
                ? ServiceResult<bool>.Ok(true, "Đã vô hiệu hóa loại hàng thành công.")
                : ServiceResult<bool>.Fail("Không thể vô hiệu hóa loại hàng.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail($"Lỗi khi vô hiệu hóa loại hàng: {ex.Message}");
        }
    }
}
