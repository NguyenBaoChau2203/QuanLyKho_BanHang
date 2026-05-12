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
- Username: `du`
- Password: `123456`
- Username: `hung`
- Password: `123456`

Demo readiness:

- `seed.sql` có dữ liệu cho đăng nhập, dashboard, master data, tồn kho đầu kỳ và một số giao dịch bán/nhập/kiểm kê mẫu.
- `PasswordHash` trong seed vẫn là text demo để dễ trình diễn; khi backend thật sẵn sàng cần chuyển sang hash + salt.
