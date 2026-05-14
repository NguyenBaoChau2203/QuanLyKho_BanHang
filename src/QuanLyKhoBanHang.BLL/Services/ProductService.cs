using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DAL.MasterData;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(DatabaseOptions options)
    {
        _productRepository = new ProductRepository(options);
    }

    public ServiceResult<List<ProductDto>> GetAllProducts()
    {
        try
        {
            var products = _productRepository.GetAll();
            return ServiceResult<List<ProductDto>>.Ok(products, products.Count > 0 ? "Đã tải danh sách sản phẩm." : "Chưa có dữ liệu sản phẩm.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ProductDto>>.Fail($"Lỗi khi tải danh sách sản phẩm: {ex.Message}");
        }
    }

    public ServiceResult<List<ProductDto>> SearchProducts(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return ServiceResult<List<ProductDto>>.Fail("Từ khóa tìm kiếm không được để trống.");
        }

        try
        {
            var products = _productRepository.Search(keyword);
            return ServiceResult<List<ProductDto>>.Ok(products, products.Count > 0 ? "Đã tìm thấy sản phẩm." : "Không tìm thấy sản phẩm nào.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<ProductDto>>.Fail($"Lỗi khi tìm kiếm sản phẩm: {ex.Message}");
        }
    }

    public ServiceResult<ProductDto> GetProductById(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<ProductDto>.Fail("Id sản phẩm không hợp lệ.");
        }

        try
        {
            var product = _productRepository.GetById(id);
            return product != null
                ? ServiceResult<ProductDto>.Ok(product, "Đã tải sản phẩm.")
                : ServiceResult<ProductDto>.Fail("Không tìm thấy sản phẩm.");
        }
        catch (Exception ex)
        {
            return ServiceResult<ProductDto>.Fail($"Lỗi khi tải sản phẩm: {ex.Message}");
        }
    }

    public ServiceResult<int> CreateProduct(ProductDto product)
    {
        if (string.IsNullOrWhiteSpace(product.Code) || string.IsNullOrWhiteSpace(product.Name))
        {
            return ServiceResult<int>.Fail("Mã sản phẩm và tên sản phẩm là bắt buộc.");
        }

        if (product.CategoryId <= 0)
        {
            return ServiceResult<int>.Fail("Loại hàng không hợp lệ.");
        }

        try
        {
            if (_productRepository.CodeExists(product.Code))
            {
                return ServiceResult<int>.Fail("Mã sản phẩm đã tồn tại.");
            }

            var id = _productRepository.Create(product);
            return id > 0
                ? ServiceResult<int>.Ok(id, "Đã tạo sản phẩm thành công.")
                : ServiceResult<int>.Fail("Không thể tạo sản phẩm.");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail($"Lỗi khi tạo sản phẩm: {ex.Message}");
        }
    }

    public ServiceResult<bool> UpdateProduct(ProductDto product)
    {
        if (product.Id <= 0)
        {
            return ServiceResult<bool>.Fail("Id sản phẩm không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(product.Code) || string.IsNullOrWhiteSpace(product.Name))
        {
            return ServiceResult<bool>.Fail("Mã sản phẩm và tên sản phẩm là bắt buộc.");
        }

        if (product.CategoryId <= 0)
        {
            return ServiceResult<bool>.Fail("Loại hàng không hợp lệ.");
        }

        try
        {
            if (_productRepository.CodeExists(product.Code, product.Id))
            {
                return ServiceResult<bool>.Fail("Mã sản phẩm đã tồn tại.");
            }

            var result = _productRepository.Update(product);
            return result
                ? ServiceResult<bool>.Ok(true, "Đã cập nhật sản phẩm thành công.")
                : ServiceResult<bool>.Fail("Không thể cập nhật sản phẩm.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
        }
    }

    public ServiceResult<bool> DeactivateProduct(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<bool>.Fail("Id sản phẩm không hợp lệ.");
        }

        try
        {
            var result = _productRepository.Deactivate(id);
            return result
                ? ServiceResult<bool>.Ok(true, "Đã vô hiệu hóa sản phẩm thành công.")
                : ServiceResult<bool>.Fail("Không thể vô hiệu hóa sản phẩm.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail($"Lỗi khi vô hiệu hóa sản phẩm: {ex.Message}");
        }
    }
}
