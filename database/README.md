# Database

Database chính dùng SQL Server Developer qua SSMS:

```text
Server=BaoChau2203;Database=QuanLyKhoBanHang;Trusted_Connection=True;TrustServerCertificate=True
```

Thứ tự chạy:

1. `schema.sql`
2. `seed.sql`

## Cách chạy bằng SSMS

1. Mở SSMS.
2. Connect server `BaoChau2203`.
3. Mở `database/schema.sql` và bấm Execute.
4. Mở `database/seed.sql` và bấm Execute.
5. Refresh thư mục Databases, kiểm tra database `QuanLyKhoBanHang`.

Nếu đã chạy seed bị lỗi hoặc muốn tạo lại từ đầu, chạy đoạn reset này trước:

```sql
USE master;
GO

IF DB_ID(N'QuanLyKhoBanHang') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyKhoBanHang SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyKhoBanHang;
END
GO
```

Sau đó chạy lại `schema.sql` rồi `seed.sql`.

Người phụ trách chính database scripts: Châu.

Dũ và Hùng có thể đề xuất thay đổi bảng/cột phục vụ service của mình, nhưng cần báo Châu hoặc ghi rõ trong pull request trước khi sửa trực tiếp `schema.sql`/`seed.sql`.

Tài khoản demo:

- Username: `admin`
- Password: `admin123`
- Username: `manager`
- Password: `123456`
- Username: `du`
- Password: `123456`
- Username: `hung`
- Password: `123456`

Demo readiness:

- `seed.sql` có dữ liệu cho đăng nhập, dashboard, master data, tồn kho đầu kỳ và một số giao dịch bán/nhập/kiểm kê mẫu.
- `PasswordHash` trong seed vẫn là text demo để dễ trình diễn; khi backend thật sẵn sàng cần chuyển sang hash + salt.
