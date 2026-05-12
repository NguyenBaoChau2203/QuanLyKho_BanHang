# Database

Database chính dùng SQL Server LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=QuanLyKhoBanHang;Trusted_Connection=True;TrustServerCertificate=True
```

Thứ tự chạy:

1. `schema.sql`
2. `seed.sql`

Tài khoản demo:

- Username: `admin`
- Password: `admin123`

Ghi chú: `seed.sql` lưu password dạng text demo để tiện thuyết trình ban đầu. Khi triển khai thật, DAL/BLL phải chuyển sang hash + salt.
