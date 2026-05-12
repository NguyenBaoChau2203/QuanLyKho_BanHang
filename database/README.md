# Database

Database chính dùng SQL Server LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=QuanLyKhoBanHang;Trusted_Connection=True;TrustServerCertificate=True
```

Thứ tự chạy:

1. `schema.sql`
2. `seed.sql`

Người phụ trách chính database scripts: Châu.

Dũ và Hùng có thể đề xuất thay đổi bảng/cột phục vụ service của mình, nhưng cần báo Châu hoặc ghi rõ trong pull request trước khi sửa trực tiếp `schema.sql`/`seed.sql`.

Tài khoản demo:

- Username: `admin`
- Password: `admin123`

Ghi chú: `seed.sql` lưu password dạng text demo để tiện thuyết trình ban đầu. Khi triển khai thật, DAL/BLL phải chuyển sang hash + salt.
