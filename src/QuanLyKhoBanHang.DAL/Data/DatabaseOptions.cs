namespace QuanLyKhoBanHang.DAL.Data;

public sealed class DatabaseOptions
{
    public string ConnectionString { get; init; } =
        @"Server=BaoChau2203;Database=QuanLyKhoBanHang;Trusted_Connection=True;TrustServerCertificate=True";
}
