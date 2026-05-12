using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.MasterData;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class ProductService
{
    public ServiceResult<List<ProductDto>> GetAllProducts()
    {
        return ServiceResult<List<ProductDto>>.Ok(new List<ProductDto>(), "Chưa có dữ liệu sản phẩm.");
    }

    public ServiceResult<List<ProductDto>> SearchProducts(string keyword)
    {
        return ServiceResult<List<ProductDto>>.Ok(new List<ProductDto>(), "Chưa có dữ liệu tìm kiếm sản phẩm.");
    }

    public ServiceResult<int> CreateProduct(ProductDto product)
    {
        if (string.IsNullOrWhiteSpace(product.Code) || string.IsNullOrWhiteSpace(product.Name))
        {
            return ServiceResult<int>.Fail("Mã sản phẩm và tên sản phẩm là bắt buộc.");
        }

        return ServiceResult<int>.Ok(0, "Service sản phẩm đang chờ Dũ triển khai DAL/BLL thật.");
    }
}
