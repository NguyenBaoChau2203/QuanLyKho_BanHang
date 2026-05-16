namespace QuanLyKhoBanHang.DAL.Data;

public sealed class DatabaseOptions
{
    public string ConnectionString { get; init; } =
        @"Server=(localdb)\MSSQLLocalDB;Database=QuanLyKhoBanHang;Trusted_Connection=True;TrustServerCertificate=True";
}

