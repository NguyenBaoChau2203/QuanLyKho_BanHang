namespace QuanLyKhoBanHang.DAL.Data;

public sealed class DatabaseOptions
{
    public string ConnectionString { get; init; } =
        @"Server=LAPTOP-9B9R17BI\MSSQLSERVER01;Database=QuanLyKhoBanHang;Trusted_Connection=True;TrustServerCertificate=True";
}

