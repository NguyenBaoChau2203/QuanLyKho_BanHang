# 📦 QuanLyKhoBanHang — Hệ Thống Quản Lý Kho & Bán Hàng Chuyên Nghiệp

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4.svg?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/download)
[![WinForms](https://img.shields.io/badge/UI-Windows%20Forms-blue.svg?style=flat-square&logo=windows)](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)
[![Architecture](https://img.shields.io/badge/Architecture-3--Layer-green.svg?style=flat-square)](https://github.com/NguyenBaoChau2203/QuanLyKho_BanHang)
[![Security](https://img.shields.io/badge/Security-PBKDF2%20%2F%20RBAC-red.svg?style=flat-square)](https://github.com/NguyenBaoChau2203/QuanLyKho_BanHang)
[![Database](https://img.shields.io/badge/Database-SQL%20Server-lightgrey.svg?style=flat-square&logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![AI Assistant](https://img.shields.io/badge/AI%20Assistant-DeepSeek-blue?style=flat-square)](https://github.com/NguyenBaoChau2203/QuanLyKho_BanHang)

> **Đồ án cuối kỳ môn Lập trình Windows Forms**
> Một phần mềm quản lý kho, nhập xuất vật tư, bán hàng và báo cáo thống kê chuyên nghiệp, hiện đại, tuân thủ nghiêm ngặt mô hình kiến trúc 3 lớp (3-Layer Architecture) và các tiêu chuẩn kiểm thử khắt khe.

---

## 🌟 Tổng Quan Dự Án

**QuanLyKhoBanHang** được thiết kế nhằm giải quyết bài toán vận hành thực tế cho các doanh nghiệp, cửa hàng bán lẻ vừa và nhỏ. Dự án tập trung vào tính toàn vẹn dữ liệu thông qua quản lý Transaction chặt chẽ, bảo mật hệ thống bằng phân quyền động và mã hóa, trải nghiệm người dùng hiện đại (Fonts, Icons, Color Theme thống nhất) và tích hợp Trợ lý Trí tuệ Nhân tạo (AI Hybrid Assistant) giúp tối ưu hóa năng suất quản lý.

---

## 🏗️ Kiến Trúc Hệ Thống (3-Layer Architecture)

Dự án tuân thủ nghiêm ngặt mô hình kiến trúc **3 lớp decoupled** nhằm tách biệt hoàn toàn trách nhiệm giữa các thành phần, tăng khả năng bảo trì, mở rộng và kiểm thử:

```text
       ┌────────────────────────────────────────────────────────┐
       │             QuanLyKhoBanHang.WinForms                  │ (Presentation Layer)
       │     - Giao diện người dùng hiện đại, an toàn           │
       │     - Tuyệt đối KHÔNG chứa SQL & Logic nghiệp vụ       │
       └─────────────────────────┬──────────────────────────────┘
                                 │ (Chỉ giao tiếp qua BLL Service)
                                 ▼
       ┌────────────────────────────────────────────────────────┐
       │               QuanLyKhoBanHang.BLL                     │ (Business Logic Layer)
       │     - Xử lý nghiệp vụ chính & Orchestration            │
       │     - Đảm bảo Validation toàn vẹn dữ liệu              │
       └─────────────────────────┬──────────────────────────────┘
                                 │ (Chỉ gọi các DAL Repositories)
                                 ▼
       ┌────────────────────────────────────────────────────────┐
       │               QuanLyKhoBanHang.DAL                     │ (Data Access Layer)
       │     - Truy vấn SQL Server qua ADO.NET thuần            │
       │     - Quản lý SQL Transactions & Parameterized Query   │
       └─────────────────────────┬──────────────────────────────┘
                                 │
                                 ▼
       ┌────────────────────────────────────────────────────────┐
       │               QuanLyKhoBanHang.DTO                     │ (Data Transfer Objects)
       │     - Các Class định nghĩa cấu trúc dữ liệu thuần      │
       │     - Truyền tải xuyên suốt giữa 3 lớp                │
       └────────────────────────────────────────────────────────┘
```

> [!IMPORTANT]
> **Quy tắc vàng của kiến trúc (Architecture Guardrails):**
> 1. Lớp giao diện (WinForms) **không được** tham chiếu trực tiếp đến lớp dữ liệu (DAL).
> 2. Mọi truy vấn cơ sở dữ liệu phải được thực hiện bằng tham số hóa (Parameterized Query) trong DAL để phòng chống hoàn toàn tấn công SQL Injection.
> 3. Các nghiệp vụ ghi đa bảng (như tạo hóa đơn bán hàng hay phiếu nhập kho) bắt buộc phải sử dụng chung một SQL Transaction để đảm bảo tính toàn vẹn (ACID).

---

## 👥 Bảng Phân Công Công Việc & Đóng Góp Chi Tiết

Dự án được xây dựng bởi nhóm 3 thành viên, phân công trách nhiệm rõ ràng theo chức năng và phân tầng. Để giúp nhà tuyển dụng và hội đồng đánh giá dễ dàng nhận diện đóng góp cụ thể của từng người, dưới đây là bảng chi tiết:

### 👑 1. Nguyễn Bảo Châu (@NguyenBaoChau2203) — Nhóm Trưởng, Kiến Trúc Sư & Phát Triển UI/Hệ Thống

Châu giữ vai trò **Lead Architect**, phụ trách thiết kế nền tảng, cơ sở dữ liệu, phát triển toàn bộ giao diện người dùng và tích hợp hệ thống.

*   **Thiết kế Kiến trúc & Setup Hệ thống:**
    *   Thiết kế cấu trúc Solution 3 lớp chuẩn chỉ, khai báo DTO mẫu và thống nhất các Service Contract (`ServiceResult<T>`) ở Phase 0 để cả nhóm làm việc song song không bị block nhau.
    *   Thiết lập và quản lý quy trình **OpenSpec** (Specs, Proposal, Tasks) phục vụ quản lý chất lượng và phát triển phần mềm chuẩn chuyên nghiệp.
*   **Thiết kế Cơ sở Dữ liệu (Database Architect):**
    *   Thiết kế toàn bộ mô hình cơ sở dữ liệu (ERD) trên SQL Server qua SSMS.
    *   Viết, cập nhật và chịu trách nhiệm chính về kịch bản khởi tạo DB `database/schema.sql` và nạp dữ liệu mẫu `database/seed.sql` (bảo đảm tính nhất quán của khóa ngoại, index và các ràng buộc dữ liệu).
*   **Phát triển 100% Giao diện WinForms UI (Modern WinForms UX):**
    *   Xây dựng giao diện hiện đại sử dụng bộ Icon chuyên nghiệp từ **FontAwesome.Sharp**, ứng dụng bộ Fonts Inter/Roboto rõ ràng, thiết kế Color Theme thống nhất qua `AppTheme` và `UiFactory` giúp giao diện phẳng, đẹp, chuẩn tỷ lệ hiển thị tiếng Việt không bị cắt chữ.
    *   Hoàn thiện toàn bộ các Form chức năng:
        *   `FrmLogin`: Màn hình đăng nhập bảo mật phân quyền.
        *   `FrmMain`: Giao diện làm việc chính với Sidebar điều hướng linh hoạt, đổi màu và hiển thị thông tin phiên làm việc.
        *   `FrmDashboard`: Biểu đồ thống kê trực quan doanh thu, cảnh báo tồn kho thấp và top sản phẩm chạy bán chạy.
        *   `FrmProduct`, `FrmCategory`, `FrmSupplier`, `FrmCustomer`: Các màn hình danh mục dữ liệu được thiết kế chuyên nghiệp, hỗ trợ tìm kiếm và hiển thị dữ liệu lưới (DataGridView) mượt mà.
        *   `FrmPurchaseReceipt`, `FrmInventory`, `FrmStocktake`: Màn hình lập phiếu nhập kho, theo dõi tồn và lập phiếu kiểm kê kho.
        *   `FrmSalesInvoice`: Giao diện bán hàng chuyên nghiệp, hỗ trợ quét sản phẩm, tính tiền tự động, áp chiết khấu và tạo hóa đơn xuất kho.
        *   `FrmReport`: Báo cáo doanh thu, sản lượng bán hàng linh hoạt theo thời gian bằng biểu đồ trực quan.
        *   `FrmAssistant`: Giao diện Trợ lý thông minh AI Agent.
        *   `FrmAccount`, `FrmPermission`, `FrmAuditLog`: Các màn hình quản trị hệ thống dành riêng cho tài khoản Quản trị viên (Admin).
*   **Phát triển Bảo mật & Quản trị Hệ thống (Security & Administration):**
    *   Triển khai cơ chế phân quyền dựa trên vai trò (Role-Based Access Control - RBAC) điều hướng SideBar động theo quyền hạn của tài khoản đăng nhập (Admin, Manager, WarehouseStaff, SalesStaff).
    *   Viết module mã hóa mật khẩu an toàn sử dụng thuật toán mã hóa PBKDF2 thay vì lưu trữ dạng văn bản thuần (clear-text), bảo mật thông tin tối đa.
    *   Thiết kế hệ thống và giao diện hiển thị **Nhật ký hệ thống (Audit Log Viewer)** theo dõi lịch sử hoạt động thời gian thực của người dùng.
*   **Tích hợp & Kết nối Nghiệp vụ (Integration & Orchestration):**
    *   Nối và truyền tải thành công dữ liệu từ tầng WinForms UI xuống các dịch vụ thật (`ProductService`, `SalesService`, `PurchaseService`, `ReportService`,...) do Dũ và Hùng phát triển, loại bỏ hoàn toàn các Mock/Stub.
    *   Tối ưu hóa khả năng bẫy lỗi ở tầng UI, giúp ứng dụng hoạt động mượt mà, thân thiện, hiển thị thông báo tiếng Việt trực quan khi backend gặp lỗi thay vì bị crash.
*   **Tích hợp Trợ lý AI (Hybrid AI Assistant):**
    *   Thiết kế và phát triển `AssistantService` kết hợp sử dụng DeepSeek API khi online và bộ xử lý ngôn ngữ tự nhiên offline cục bộ (Offline NLP Rule-based Fallback) hỗ trợ người quản lý hỏi đáp nhanh về doanh thu, hàng tồn kho, và thống kê cửa hàng.

---

### 📦 2. Trần Minh Dũ — Nhà Phát Triển Backend Kho & Nghiệp Vụ Tồn Kho

Dũ phụ trách thiết kế các lớp dữ liệu và nghiệp vụ (DAL & BLL) liên quan đến chuỗi cung ứng, sản phẩm và kho hàng.

*   **Phát triển Lớp Dữ liệu (DAL - ADO.NET):**
    *   Viết repository truy vấn dữ liệu an toàn bằng ADO.NET thuần cho các thực thể: `CategoryRepository`, `ProductRepository`, `SupplierRepository`, `PurchaseRepository`, `StocktakeRepository`, `StockTransactionRepository`.
*   **Thiết kế Nghiệp vụ Kho & Toàn vẹn Dữ liệu (BLL):**
    *   Hoàn thiện các service: `CategoryService`, `ProductService`, `SupplierService` với đầy đủ các nghiệp vụ CRUD, kiểm tra trùng mã sản phẩm/loại hàng/nhà cung cấp.
    *   Triển khai nghiệp vụ **Nhập kho (`PurchaseService.CreateReceipt`)**: Đảm bảo thực hiện tuần tự các bước: kiểm tra nhà cung cấp hợp lệ -> thêm dữ liệu phiếu nhập chi tiết -> cộng dồn số lượng tồn kho sản phẩm -> ghi nhận giao dịch kho (`StockTransactions`). Toàn bộ luồng được bảo vệ bởi một **SQL Transaction** duy nhất.
    *   Xây dựng chức năng **Kiểm kê kho (`StocktakeService.CreateStocktake`)**: Tính toán chênh lệch tồn hệ thống và thực tế, tự động tạo giao dịch điều chỉnh kho khi có chênh lệch số lượng.
    *   Viết logic **Cảnh báo tồn kho thấp (`InventoryService.GetLowStockProducts`)** lọc tự động các sản phẩm có số lượng dưới định mức tối thiểu.
*   **Kiểm thử Đơn vị (Unit Testing):**
    *   Viết các ca kiểm thử bảo đảm tính toàn vẹn nghiệp vụ kho: Tạo sản phẩm thiếu mã/tên phải thất bại, tạo phiếu nhập trống/âm giá phải thất bại, nhập kho thành công phải cộng dồn tồn và ghi nhật ký kho.

---

### 💵 3. Nguyễn Việt Hùng — Nhà Phát Triển Backend Bán Hàng & Báo Cáo Doanh Thu

Hùng chịu trách nhiệm thiết kế DAL & BLL của luồng bán hàng, chăm sóc khách hàng và lập báo cáo tài chính cửa hàng.

*   **Phát triển Lớp Dữ liệu (DAL - ADO.NET):**
    *   Hoàn thiện truy vấn ADO.NET cho: `CustomerRepository`, `SalesRepository` (bán lẻ & hóa đơn), `ReportRepository` (doanh thu và thống kê nâng cao).
*   **Thiết kế Nghiệp vụ Bán hàng & Khách hàng (BLL):**
    *   Hoàn thiện `CustomerService`: Quản lý danh sách khách hàng thân thiết, validate tên, số điện thoại và trùng mã.
    *   Triển khai nghiệp vụ **Bán hàng & Xuất hóa đơn (`SalesService.CreateInvoice`)**:
        *   Validate chiết khấu, giảm giá hợp lệ.
        *   Kiểm tra số lượng tồn kho thời gian thực; **chặn tuyệt đối** bán vượt tồn kho.
        *   Tự động giảm tồn kho sản phẩm tương ứng, ghi nhận giao dịch xuất kho và lưu hóa đơn chi tiết.
        *   Áp dụng **SQL Transaction** bao bọc toàn bộ tiến trình để đảm bảo không bị thất thoát hàng hóa hay sai lệch doanh thu.
*   **Phát triển Nghiệp vụ Báo cáo & Thống kê (Business Intelligence):**
    *   Viết các câu lệnh truy vấn phức tạp của `ReportService` để kết xuất báo cáo doanh thu theo khoảng thời gian, thống kê Top sản phẩm bán chạy nhất (theo sản lượng và doanh số) và Top khách hàng mua nhiều nhất.
*   **Đóng góp Trợ lý AI (AI Assistant Rule Engine):**
    *   Viết các command rules phục vụ cho chatbot trợ lý quản lý: Kết nối trực tiếp vào DB để lấy dữ liệu thực tế cho các câu hỏi `"doanh thu hôm nay"`, `"doanh thu tháng này"`, `"top sản phẩm bán chạy"`,...
*   **Kiểm thử Đơn vị (Unit Testing):**
    *   Viết các bài test xác thực nghiệp vụ hóa đơn: Chặn tạo hóa đơn trống, chặn bán vượt tồn kho, kiểm tra tính toán tổng tiền chính xác sau khi chiết khấu, kiểm tra kết xuất báo cáo doanh thu khớp với DB.

---

### 📊 Ma Trận Đóng Góp Công Việc (Contribution Matrix)

| Thành phần chức năng | Phân tầng | Nguyễn Bảo Châu (@NguyenBaoChau2203) | Trần Minh Dũ | Nguyễn Việt Hùng |
| :--- | :--- | :---: | :---: | :---: |
| **Kiến trúc & Setup** | Project, Specs, Contracts | 👑 **Chính (100%)** | — | — |
| **Cơ sở Dữ liệu** | Schema, Seed, Triggers, Constraints | 👑 **Chính (80%)** | Phối hợp (10%) | Phối hợp (10%) |
| **Giao diện người dùng** | WinForms UI, Themes, Fonts, Helpers | 👑 **Chính (100%)** | — | — |
| **Bảo mật & Auth** | Hashing (PBKDF2), RBAC, Admin UI | 👑 **Chính (100%)** | — | — |
| **Quản trị hệ thống** | Quyền hạn Matrix, Audit Log, User CRUD | 👑 **Chính (100%)** | — | — |
| **Danh mục kho** | Product, Category, Supplier DAL/BLL | Tích hợp UI (30%) | 👑 **Chính (70%)** | — |
| **Nghiệp vụ kho** | Nhập kho, Kiểm kê, Tồn kho DAL/BLL | Tích hợp UI (30%) | 👑 **Chính (70%)** | — |
| **Bán hàng & Khách hàng**| Hóa đơn, Giảm tồn, Khách hàng DAL/BLL | Tích hợp UI (30%) | — | 👑 **Chính (70%)** |
| **Báo cáo & Thống kê** | Doanh thu, Top bán chạy, Top mua nhiều | Tích hợp UI (30%) | — | 👑 **Chính (70%)** |
| **Trợ lý thông minh** | Assistant UI, LLM API, Local rule BLL | 👑 **Chính (85%)** | — | Phối hợp (15%) |
| **Kiểm thử hệ thống** | Unit Tests & Integration Tests | Chạy Validate (20%) | 👑 **Chính (40%)** | 👑 **Chính (40%)** |

---

## 🛠️ Công Nghệ Sử Dụng

*   **Ngôn ngữ lập trình:** C# 12
*   **Framework chính:** .NET 8.0 (Windows Forms & .NET Core Class Library)
*   **Hệ quản trị CSDL:** Microsoft SQL Server
*   **Thư viện đồ họa UI:** `FontAwesome.Sharp` (Phiên bản phẳng hiện đại)
*   **Thư viện kết nối CSDL:** `Microsoft.Data.SqlClient` (ADO.NET Thuần)
*   **Quy trình quản lý chất lượng:** OpenSpec (Fission AI validator)
*   **Thư viện kiểm thử:** xUnit / NUnit

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Dự Án

### 📋 Yêu cầu môi trường
*   **Visual Studio 2022** (Đã cài đặt workload `.NET Desktop Development`).
*   **.NET 8.0 SDK** hoặc mới hơn.
*   **SQL Server** (Developer hoặc Express Edition) cùng công cụ **SSMS (SQL Server Management Studio)**.

### 🏃 Các bước khởi chạy ứng dụng
1.  **Tải mã nguồn về máy (Clone project):**
    ```bash
    git clone https://github.com/NguyenBaoChau2203/QuanLyKho_BanHang.git
    ```
2.  **Khởi tạo Cơ sở dữ liệu:**
    *   Mở SQL Server Management Studio (SSMS) và kết nối vào SQL Server.
    *   Mở và chạy file [database/schema.sql](database/schema.sql) để khởi tạo cấu trúc cơ sở dữ liệu `QuanLyKhoBanHang`.
    *   Tiếp theo, mở và chạy file [database/seed.sql](database/seed.sql) để nạp các danh mục ban đầu cùng các tài khoản demo được phân quyền sẵn.
3.  **Cấu hình kết nối cơ sở dữ liệu:**
    *   Mở dự án bằng cách nhấp đúp vào file `QuanLyKhoBanHang.sln` trong Visual Studio 2022.
    *   Mở file cấu hình kết nối trong dự án `QuanLyKhoBanHang.WinForms` (chỉnh sửa thông tin `Connection String` trỏ đúng về server cơ sở dữ liệu trên máy bạn).
4.  **Chạy dự án:**
    *   Chọn project khởi chạy mặc định (Set as Startup Project) là **`QuanLyKhoBanHang.WinForms`**.
    *   Nhấn `F5` hoặc nút `Start` trong Visual Studio để biên dịch và khởi chạy chương trình.

### 🔑 Danh Sách Tài Khoản Thử Nghiệm (Demo Accounts)

Hệ thống đã được seed sẵn các tài khoản tương ứng với các phân hệ phân quyền (RBAC) khác nhau phục vụ quá trình trình diễn và đánh giá:

| Tài khoản | Mật khẩu | Quyền hạn (Role) | Chức năng Sidebar tương ứng |
| :--- | :--- | :--- | :--- |
| **`admin`** | `admin123` | **Administrator** | Xem được tất cả các tab chức năng, bao gồm các tab cấu hình hệ thống: Quản lý tài khoản, Phân quyền và Nhật ký hệ thống (Audit Log). |
| **`manager`** | `123456` | **Manager** | Toàn quyền kiểm soát nghiệp vụ: Kho hàng, Bán hàng, Danh mục và xem Báo cáo doanh thu chuyên sâu. Ẩn các tab quản trị hệ thống của Admin. |
| **`du`** | `123456` | **WarehouseStaff** | Nhân viên kho: Chỉ hiển thị các phân hệ Danh mục sản phẩm/nhà cung cấp, Nhập kho, Tồn kho và Kiểm kê. Ẩn hoàn toàn Bán hàng, Báo cáo và Admin. |
| **`hung`** | `123456` | **SalesStaff** | Nhân viên bán hàng: Chỉ hiển thị phân hệ Danh mục khách hàng, Bán hàng (lập hóa đơn). Ẩn hoàn toàn Nhập kho, Kiểm kê, Báo cáo và Admin. |

---

## 🛡️ Kiểm Tra Độ An Toàn & Chuẩn Kiến Trúc (Quality Guardrails)

Dự án cung cấp sẵn các công cụ tự động giúp xác thực tính toàn vẹn của mã nguồn:

### 1. Build & Chạy Unit Tests tự động:
```powershell
dotnet build QuanLyKhoBanHang.sln
dotnet test QuanLyKhoBanHang.sln --no-build --no-restore
```

### 2. Kiểm tra tính độc lập của tầng giao diện (WinForms tuyệt đối không gọi DAL trực tiếp):
Chạy câu lệnh PowerShell sau để kiểm tra xem có dòng code nào trong project WinForms vi phạm gọi trực tiếp SQL hoặc DAL không:
```powershell
Get-ChildItem -Path src\QuanLyKhoBanHang.WinForms -Recurse -File -Filter *.cs | Select-String -Pattern 'QuanLyKhoBanHang.DAL|SqlConnection|SqlCommand|SELECT |INSERT |UPDATE |DELETE |FROM ' -CaseSensitive
```
*(Nếu kết quả trả về rỗng nghĩa là tầng WinForms hoàn toàn sạch bóng SQL, tuân thủ đúng kiến trúc 3 lớp decoupled).*

---

## ⚖️ Giấy Phép (License)

Dự án được phân phối theo giấy phép MIT. Xem chi tiết tại file [LICENSE](LICENSE).

---
*Hy vọng các nhà tuyển dụng tìm thấy thông tin hữu ích về năng lực phát triển phần mềm, tư duy kiến trúc hệ thống và khả năng làm việc nhóm chuyên nghiệp của chúng tôi thông qua dự án này!*
