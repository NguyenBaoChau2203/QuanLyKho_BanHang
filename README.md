# QuanLyKhoBanHang

Đồ án cuối kỳ môn lập trình Windows Forms: phần mềm quản lý kho và bán hàng theo mô hình 3 lớp.

## Stack

- C# WinForms .NET 8
- SQL Server LocalDB
- ADO.NET thuần ở tầng DAL
- Kiến trúc 3 lớp: WinForms -> BLL -> DAL -> DTO
- OpenSpec để quản lý yêu cầu, thiết kế và task theo từng thay đổi

## Cách chạy nhanh

1. Mở `QuanLyKhoBanHang.sln` bằng Visual Studio.
2. Chạy script `database/schema.sql` trên SQL Server LocalDB.
3. Chạy script `database/seed.sql` để có dữ liệu demo.
4. Set `QuanLyKhoBanHang.WinForms` làm startup project.
5. Chạy app, dùng tài khoản demo:
   - Username: `admin`
   - Password: `admin123`

## Tài liệu nhóm

- [Nhiệm vụ tổng thể](docs/01_NhiemVuTongThe.md)
- [Phân công công việc](docs/02_PhanCongCongViec.md)
- [Workflow làm việc](docs/03_WorkflowLamViec.md)
- [Quy chuẩn chung](docs/04_QuyChuanChung.md)
- [OpenSpec workflow](docs/05_OpenSpecWorkflow.md)
- [Checklist demo](docs/06_ChecklistDemo.md)

## Quy tắc quan trọng

- WinForms chỉ gọi BLL, không gọi DAL.
- BLL xử lý nghiệp vụ và validation.
- DAL chứa SQL, mapping dữ liệu và transaction.
- DTO chỉ chứa dữ liệu.
- Mọi thay đổi lớn phải cập nhật docs hoặc OpenSpec change trước khi code sâu.
