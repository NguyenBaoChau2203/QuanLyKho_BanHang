# 📦 QuanLyKhoBanHang (Warehouse & Sales Management)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4.svg?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/download)
[![WinForms](https://img.shields.io/badge/UI-Windows%20Forms-blue.svg?style=flat-square&logo=windows)](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)
[![Architecture](https://img.shields.io/badge/Architecture-3--Layer-green.svg?style=flat-square)](https://github.com/NguyenBaoChau2203/QuanLyKho_BanHang)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

> **Đồ án cuối kỳ môn Lập trình Windows Forms**
> Một phần mềm quản lý kho và bán hàng chuyên nghiệp, hiện đại, tuân thủ nghiêm ngặt mô hình 3 lớp (3-Layer Architecture).

---

## 🌟 Tổng quan dự án

**QuanLyKhoBanHang** được xây dựng nhằm giải quyết bài toán quản lý kho hàng, nhập xuất vật tư và bán hàng cho các doanh nghiệp vừa và nhỏ. Dự án tập trung vào tính ổn định, bảo mật và trải nghiệm người dùng hiện đại ngay trên nền tảng WinForms cổ điển.

### ✨ Tính năng nổi bật

- **Quản lý danh mục:** Sản phẩm, Loại hàng, Nhà cung cấp, Khách hàng.
- **Nghiệp vụ kho:** Nhập kho, Kiểm kê, Quản lý tồn kho thời gian thực.
- **Nghiệp vụ bán hàng:** Lập hóa đơn, Quản lý khách hàng thân thiết.
- **Báo cáo & Thống kê:** Doanh thu, Top sản phẩm bán chạy, Cảnh báo hàng tồn thấp.
- **Trợ lý AI:** Hỗ trợ truy vấn dữ liệu nhanh bằng ngôn ngữ tự nhiên.
- **Hệ thống & Bảo mật:** Phân quyền chi tiết (RBAC), Nhật ký hệ thống (Audit Log), Mã hóa mật khẩu PBKDF2.

---

## 🛠 Công nghệ sử dụng

- **Ngôn ngữ:** C# 12 / .NET 8.0
- **Giao diện:** Windows Forms (WinForms) với FontAwesome.Sharp.
- **Cơ sở dữ liệu:** SQL Server (ADO.NET thuần cho hiệu năng tối ưu).
- **Quy trình:** OpenSpec (Quản lý yêu cầu & thiết kế).
- **Kiến trúc:** 3-Layer Architecture (WinForms -> BLL -> DAL -> DTO).

---

## 🏗 Kiến trúc hệ thống

Dự án tuân thủ nghiêm ngặt mô hình 3 lớp để đảm bảo tính dễ bảo trì và mở rộng:

1.  **Presentation Layer (`WinForms`):** Chứa giao diện người dùng, chỉ giao tiếp với BLL. Tuyệt đối không chứa SQL hay logic nghiệp vụ.
2.  **Business Logic Layer (`BLL`):** Xử lý logic nghiệp vụ, Validation và điều phối dữ liệu.
3.  **Data Access Layer (`DAL`):** Thực thi các câu lệnh SQL, Mapping dữ liệu và quản lý Transaction.
4.  **Data Transfer Objects (`DTO`):** Các Class định nghĩa cấu trúc dữ liệu truyền tải giữa các lớp.

---

## 🚀 Cách chạy dự án

### 📋 Yêu cầu hệ thống
- Visual Studio 2022 hoặc mới hơn.
- .NET 8.0 SDK.
- SQL Server Developer Edition / SQL Express.

### 🏃 Các bước thực hiện
1. **Clone repository:**
   ```bash
   git clone https://github.com/NguyenBaoChau2203/QuanLyKho_BanHang.git
   ```
2. **Cấu hình Database:**
   - Chạy file `database/schema.sql` trong SSMS để tạo cấu trúc bảng.
   - Chạy file `database/seed.sql` để nạp dữ liệu mẫu và tài khoản Admin.
3. **Mở Solution:** Mở file `QuanLyKhoBanHang.sln`.
4. **Chạy ứng dụng:** Nhấn `F5` hoặc chọn Start project là `QuanLyKhoBanHang.WinForms`.

### 🔑 Tài khoản Demo
| Vai trò | Username | Password |
| :--- | :--- | :--- |
| **Admin** | `admin` | `admin123` |
| **Manager** | `manager` | `123456` |
| **Kho** | `du` | `123456` |
| **Bán hàng** | `hung` | `123456` |

---

## 📚 Tài liệu & Quy trình

- 📝 [Nhiệm vụ tổng thể](docs/01_NhiemVuTongThe.md)
- 🤝 [Phân công công việc](docs/02_PhanCongCongViec.md)
- ⚙️ [Workflow làm việc](docs/03_WorkflowLamViec.md)
- 📏 [Quy chuẩn chung](docs/04_QuyChuanChung.md)
- 🛡️ [Checklist Demo](docs/06_ChecklistDemo.md)

---

## ⚖️ Quy tắc phát triển (Golden Rules)

- ✅ WinForms **CHỈ** gọi BLL.
- ✅ DAL là nơi duy nhất chứa SQL và Parameters.
- ✅ Mọi thay đổi lớn phải bắt đầu bằng **OpenSpec change**.
- ✅ Tuân thủ chuẩn đặt tên PascalCase cho Class/Method và camelCase cho biến cục bộ.

---

## 👥 Thành viên nhóm

- **Nguyễn Bảo Châu** (@NguyenBaoChau2203) - *Lead, Admin, Auth & UI Integration*
- **...** (@...) - *Warehouse & Inventory Services*
- **...** (@...) - *Sales & Reporting Services*

---
*Phát triển bởi đội ngũ đam mê .NET - 2024*
