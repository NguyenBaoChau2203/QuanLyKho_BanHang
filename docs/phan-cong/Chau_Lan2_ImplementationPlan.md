# Kế hoạch triển khai lần 2 - Châu

Ngày lập: 2026-05-15

Phạm vi tài liệu: planning only cho branch `feature/project-integration-chau-v2`. Tài liệu này không xác nhận đã implement code.

## 1. Current confirmed state

### Git / workspace

- Branch hiện tại: `feature/project-integration-chau-v2`.
- `git status --short --branch` đang có các file phân công/audit chưa tracked:
  - `docs/phan-cong/Chau_Lan2_SauAudit.md`
  - `docs/phan-cong/Du_Lan2_SauAudit.md`
  - `docs/phan-cong/Hung_Lan2_SauAudit.md`
  - `docs/phan-cong/Lan2_SauAuditMergePR.md`
- Không được revert hoặc ghi đè các file trên nếu không có yêu cầu riêng.

### Build status

- Đã chạy `dotnet build QuanLyKhoBanHang.sln`.
- Lần chạy trong sandbox fail vì không đọc được `C:\Users\chau1\AppData\Roaming\NuGet\NuGet.Config`; đã chạy lại ngoài sandbox để xác nhận blocker thật.
- Kết quả confirmed: build fail ở project DAL, trước khi tới lỗi WinForms/BLL.
- Blocker chính: các repository kho/master data đang dùng `System.Data.SqlClient`, trong khi `src/QuanLyKhoBanHang.DAL/QuanLyKhoBanHang.DAL.csproj` chỉ reference `Microsoft.Data.SqlClient`.
- Build hiện báo nhiều lỗi `CS1069` cho `SqlConnection` / `SqlCommand` trong:
  - `src/QuanLyKhoBanHang.DAL/MasterData/CategoryRepository.cs`
  - `src/QuanLyKhoBanHang.DAL/MasterData/ProductRepository.cs`
  - `src/QuanLyKhoBanHang.DAL/MasterData/SupplierRepository.cs`
  - `src/QuanLyKhoBanHang.DAL/Inventory/PurchaseRepository.cs`
  - `src/QuanLyKhoBanHang.DAL/Inventory/StocktakeRepository.cs`
  - `src/QuanLyKhoBanHang.DAL/Inventory/StockTransactionRepository.cs`

### Known blockers from source inspection

- Sau khi sửa SQL provider, khả năng cao sẽ lộ tiếp lỗi constructor:
  - WinForms đang gọi `new ProductService()`, `new CategoryService()`, `new SupplierService()`, `new InventoryService()`, `new PurchaseService()`, `new StocktakeService()`.
  - Các service này hiện chỉ có constructor nhận `DatabaseOptions`.
  - `AssistantService` cũng tạo `new InventoryService()` và `new StocktakeService()`.
  - Test hiện có cũng gọi `new InventoryService()`.
- `StockTransactions.TransactionType` trong `database/schema.sql` là `NVARCHAR(30)`, nhưng `StockTransactionRepository` đọc/ghi kiểu `int` theo enum `StockTransactionType`. Đây là rủi ro runtime/DB contract cần chốt với Dũ trước khi demo.
- Một số repository bán hàng/báo cáo/khách hàng của Hùng đang hard-code connection string `Server=BaoChau2203;Database=QuanLyKhoBanHang;...` thay vì dùng `DatabaseOptions`.
- `SalesService.CreateInvoice` gọi repository thật nhưng trả cứng invoice id `1`.
- `FrmPurchaseReceipt` và `FrmSalesInvoice` chưa truyền `CreatedByUserId` từ user đang đăng nhập.
- `FrmStocktake.Save()` vẫn chỉ hiển thị thông báo demo, chưa gọi `StocktakeService.CreateStocktake`.
- `FrmMain.LoadDefaultView()` lấy default feature nhưng hiện đang mở `PermissionService.FeatureSupplier` cố định.

### UI screens still using stub/demo

- Demo/in-memory:
  - `FrmLogin`: đăng nhập demo account; đăng ký và quên mật khẩu là stub/mô phỏng.
  - `FrmUserManagement`, `FrmRolePermission`, `FrmAuditLog`: admin demo/in-memory.
  - `DashboardService` / `FrmDashboard`: KPI hardcode và fallback stub.
- Stub CRUD buttons:
  - `FrmProduct`: save/deactivate còn UI stub.
  - `FrmCategory`: save/deactivate còn UI stub.
  - `FrmSupplier`: save/deactivate còn UI stub.
  - `FrmCustomer`: save/deactivate còn UI stub.
- Demo/fallback operational screens:
  - `FrmInventory`: dùng service nếu có data, fallback `CreateStubProducts`, xuất Excel demo.
  - `FrmPurchaseReceipt`: lấy product từ service/fallback, supplier đang nhập text và `SupplierId = 1`, in phiếu/lưu tạm demo.
  - `FrmStocktake`: load product từ service/fallback, xác nhận kiểm kê demo.
  - `FrmSalesInvoice`: load product/customer từ service/fallback, chưa set `CreatedByUserId`.
  - `FrmReport`: dùng `ReportService`, fallback stub và export demo.
  - `FrmAssistant`: UI gọi `AssistantService`; rule-based hiện chưa có đủ nhánh trực tiếp cho `hàng sắp hết` và `kiểm kê hôm nay` theo source hiện tại, dù test đang kỳ vọng đủ.

### Backend parts waiting on Dũ/Hùng

- Chờ Dũ hoàn thiện và bàn giao backend kho/master data:
  - SQL provider thống nhất.
  - Constructor/service wiring cho kho.
  - CRUD thật cho product/category/supplier.
  - Purchase transaction atomically lưu phiếu, tăng tồn, ghi stock transaction.
  - Inventory current stock / low stock / transaction history.
  - Stocktake transaction atomically lưu kiểm kê, cập nhật tồn, ghi stock transaction.
- Chờ Hùng hoàn thiện và bàn giao backend khách hàng/bán hàng/báo cáo/assistant:
  - Customer CRUD thật.
  - Sales transaction atomically lưu hóa đơn, giảm tồn, ghi stock transaction, không bán vượt tồn.
  - Report doanh thu/top sản phẩm/top khách hàng đọc DB thật.
  - Assistant trả lời đủ command demo.
  - Repository không hard-code connection string riêng.

## 2. Implementation phases

### Phase 0: build stabilization and contract check

Mục tiêu: build xanh trước khi Châu wire UI sâu.

Việc cần làm:

- Dũ sửa SQL provider trong repository kho/master data, ưu tiên dùng `Microsoft.Data.SqlClient` vì DAL project đã reference package này.
- Dũ thêm hoặc thống nhất constructor không tham số cho các service kho nếu UI/test vẫn dùng `new()`, nhưng phải giữ constructor nhận `DatabaseOptions`.
- Hùng chuẩn hóa repository customer/sales/report dùng `DatabaseOptions` hoặc thống nhất rõ vì sao chưa làm trong PR.
- Châu review contract sau khi build qua:
  - WinForms chỉ reference BLL/DTO.
  - WinForms không có SQL hoặc reference DAL.
  - DTO/service public method không đổi ngầm.
  - `CreatedByUserId`, `TransactionType`, connection string và return id của sales/purchase/stocktake khớp schema.
- Chỉ chuyển sang Phase 1 khi `dotnet build QuanLyKhoBanHang.sln` pass.

### Phase 1: UI-to-service wiring for ready services

Mục tiêu: chỉ wire các màn hình có service đã sẵn sàng, không tự viết lại backend của Dũ/Hùng.

- Wire nhóm Dũ sau khi backend kho pass build/test:
  - `FrmProduct` gọi `ProductService.CreateProduct`, `UpdateProduct`, `DeactivateProduct`.
  - `FrmCategory` gọi `CategoryService.CreateCategory`, `UpdateCategory`, `DeactivateCategory`.
  - `FrmSupplier` gọi `SupplierService.CreateSupplier`, `UpdateSupplier`, `DeactivateSupplier`.
  - `FrmInventory` hiển thị rõ data thật hoặc fallback demo.
  - `FrmPurchaseReceipt` dùng product/supplier thật nếu service sẵn sàng; save gọi `PurchaseService.CreateReceipt`.
  - `FrmStocktake` save gọi `StocktakeService.CreateStocktake`.
- Wire nhóm Hùng sau khi backend sales/report pass build/test:
  - `FrmCustomer` gọi `CustomerService.CreateCustomer`, `UpdateCustomer`, `DeactivateCustomer`.
  - `FrmSalesInvoice` save gọi `SalesService.CreateInvoice` với data đầy đủ.
  - `FrmReport` giữ gọi `ReportService`, chỉ chỉnh fallback/messaging nếu cần.
  - `FrmAssistant` giữ gọi `AssistantService`, xác minh đủ command checklist.

### Phase 2: CreatedByUserId/user context integration

Mục tiêu: các nghiệp vụ tạo chứng từ ghi đúng user đăng nhập.

- `FrmMain` cần truyền `UserDto currentUser` vào các form cần user context.
- Các form nên nhận `UserDto` hoặc `currentUserId` qua constructor; không hard-code `1`.
- Cần set `CreatedByUserId` cho:
  - `PurchaseReceiptDto` trong `FrmPurchaseReceipt`.
  - `SalesInvoiceDto` trong `FrmSalesInvoice`.
  - `StocktakeDto` trong `FrmStocktake`.
- Không đổi DTO nếu property đã có sẵn; chỉ dùng property hiện có.

### Phase 3: demo fallback cleanup and messaging

Mục tiêu: demo không gây hiểu nhầm giữa dữ liệu thật và dữ liệu mẫu.

- Màn hình đã nối service thật: thông báo success/fail lấy từ `ServiceResult.Message`, refresh lại list sau khi save/deactivate.
- Màn hình vẫn fallback: message phải nói rõ `dữ liệu demo`, `chưa hỗ trợ`, hoặc `backend chưa sẵn sàng`.
- Không xóa fallback an toàn nếu backend chưa ổn định; chỉ làm rõ trạng thái.
- Đăng ký và quên mật khẩu tiếp tục là demo/mô phỏng trong MVP, trừ khi có OpenSpec change mới.

### Phase 4: checklist validation

Mục tiêu: xác nhận demo checklist có thể chạy.

- Chạy OpenSpec validate cho change liên quan nếu có thay đổi OpenSpec:
  - `npx --yes --package @fission-ai/openspec openspec validate bootstrap-inventory-sales-mvp`
- Chạy:
  - `dotnet build QuanLyKhoBanHang.sln`
  - `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`
- Chạy thủ công `docs/06_ChecklistDemo.md`:
  - login 4 account demo.
  - mở tất cả menu chính không exception.
  - kiểm tra role menu nhanh.
  - nhập kho, tồn kho, kiểm kê.
  - bán hàng, báo cáo doanh thu.
  - assistant 5 câu demo.
  - admin section demo.
- Ghi rõ màn hình nào còn stub/demo trong PR.

### Phase 5: optional extensions only if MVP is stable

Chỉ làm nếu Phase 0-4 pass và có OpenSpec change riêng được approve:

- Đăng ký tài khoản thật.
- Quên mật khẩu/đặt lại mật khẩu thật.
- Hash password/auth security thật.
- Admin account/role/audit DAL thật.
- In hóa đơn thật.
- Xuất Excel thật.
- AI online nâng cao ngoài rule-based.

## 3. File-level plan

| File / area | Vì sao có thể cần sửa | Nên thay đổi | Không được thay đổi |
| --- | --- | --- | --- |
| `src/QuanLyKhoBanHang.DAL/QuanLyKhoBanHang.DAL.csproj` | Đang reference `Microsoft.Data.SqlClient`; build fail do file khác dùng `System.Data.SqlClient`. | Chỉ thay package nếu team chọn provider khác; khuyến nghị giữ `Microsoft.Data.SqlClient`. | Không thêm package mới tùy tiện nếu chỉ cần đổi namespace. |
| `src/QuanLyKhoBanHang.DAL/MasterData/CategoryRepository.cs` | Build fail do `System.Data.SqlClient`; thuộc Dũ. | Đổi sang provider thống nhất; giữ SQL parameter; kiểm tra CRUD thật. | Châu không viết lại toàn bộ repository nếu Dũ chưa bàn giao. |
| `src/QuanLyKhoBanHang.DAL/MasterData/ProductRepository.cs` | Build fail do provider; dùng cho product/inventory/purchase/sales. | Đổi provider; kiểm tra `GetLowStockProducts`, `UpdateQuantity`, transaction compatibility. | Không đổi schema/DTO ngầm; không cho phép quantity âm trái constraint. |
| `src/QuanLyKhoBanHang.DAL/MasterData/SupplierRepository.cs` | Build fail do provider; supplier cần cho purchase. | Đổi provider; giữ parameterized SQL; bảo đảm search/get/create/update/deactivate. | Không hard-code supplier id trong UI thay cho service thật. |
| `src/QuanLyKhoBanHang.DAL/Inventory/PurchaseRepository.cs` | Build fail; purchase hiện lưu receipt/detail nhưng orchestration transaction đang ở service không atomic toàn flow. | Dũ quyết định transaction boundary; tốt nhất atomic receipt + details + stock + transaction. | Châu không sửa sâu nghiệp vụ kho nếu không phải minimal build fix. |
| `src/QuanLyKhoBanHang.DAL/Inventory/StocktakeRepository.cs` | Build fail; stocktake cần save thật. | Dũ sửa provider và bảo đảm save header/detail. | Không để UI chỉ báo demo sau khi service thật đã sẵn sàng. |
| `src/QuanLyKhoBanHang.DAL/Inventory/StockTransactionRepository.cs` | Build fail; có mismatch `TransactionType` enum int vs schema NVARCHAR. | Dũ chốt contract: đổi mapping sang string hoặc cập nhật schema/OpenSpec có kiểm soát. | Không đổi enum/schema âm thầm vì ảnh hưởng purchase/sales/report. |
| `src/QuanLyKhoBanHang.DAL/Data/DatabaseOptions.cs` | Nhiều repo cần connection string chung. | Dùng làm nguồn cấu hình thống nhất; có thể sau này đọc config/env nếu cần. | Không hard-code secret/API key; không đổi public contract bất ngờ. |
| `src/QuanLyKhoBanHang.DAL/Data/CustomerRepository.cs` | Hùng repo đang hard-code connection string. | Hùng chuyển sang `DatabaseOptions` hoặc adapter tương thích service. | Châu không rewrite backend customer nếu chưa cần build/integration. |
| `src/QuanLyKhoBanHang.DAL/Data/SalesRepository.cs` | Hùng repo đang hard-code connection string; transaction sales đã có nhưng cần return id. | Hùng trả invoice id thật và dùng config chung; giữ transaction. | Không bán vượt tồn; không bỏ ghi stock transaction. |
| `src/QuanLyKhoBanHang.DAL/Data/ReportRepository.cs` | Hùng repo đang hard-code connection string; dashboard/report phụ thuộc data thật. | Hùng thống nhất config và verify queries theo schema. | Không để SQL query nối chuỗi từ input. |
| `src/QuanLyKhoBanHang.BLL/Services/CategoryService.cs` | UI gọi constructor không tham số; service hiện chỉ nhận `DatabaseOptions`. | Dũ thêm constructor mặc định hoặc Châu inject options thống nhất sau khi quyết định. | Không đổi public method names/return types. |
| `src/QuanLyKhoBanHang.BLL/Services/ProductService.cs` | Tương tự; dùng rộng trong UI. | Giữ `ServiceResult<T>`; validate giá/tồn âm theo phân công Dũ. | Không đưa SQL vào BLL. |
| `src/QuanLyKhoBanHang.BLL/Services/SupplierService.cs` | Tương tự; cần cho purchase. | Giữ CRUD contract hiện có. | Không đổi DTO nếu không cập nhật OpenSpec/docs. |
| `src/QuanLyKhoBanHang.BLL/Services/InventoryService.cs` | UI/test/assistant gọi `new()`; hiện chỉ có `DatabaseOptions`. | Dũ thêm constructor/config; trả current stock/low stock/history thật. | Không hard-code dashboard/demo trong service thật. |
| `src/QuanLyKhoBanHang.BLL/Services/PurchaseService.cs` | Cần atomic business flow và `CreatedByUserId`. | Dũ đảm bảo transaction và message rõ; Châu chỉ gọi service đúng contract. | Không để receipt lưu thành công nhưng tăng tồn/transaction fail. |
| `src/QuanLyKhoBanHang.BLL/Services/StocktakeService.cs` | Cần save thật và assistant kiểm kê. | Dũ đảm bảo create stocktake cập nhật tồn/transaction. | Không dùng demo-only save nếu backend đã sẵn sàng. |
| `src/QuanLyKhoBanHang.BLL/Services/CustomerService.cs` | Customer CRUD đã có service nhưng repo hard-code config. | Hùng hoàn thiện validation/check trùng nếu cần; Châu wire UI. | Không yêu cầu Châu viết lại toàn bộ customer backend. |
| `src/QuanLyKhoBanHang.BLL/Services/SalesService.cs` | Save invoice cần `CreatedByUserId`, return id thật, no oversell. | Hùng trả id thật từ repository và giữ validation. | Không trả cứng `1`; không đổi method signature ngầm. |
| `src/QuanLyKhoBanHang.BLL/Services/ReportService.cs` | Report UI/assistant phụ thuộc service. | Hùng verify revenue/top product/top customer thật. | Không để UI gọi repository/report SQL trực tiếp. |
| `src/QuanLyKhoBanHang.BLL/Services/DashboardService.cs` | Hiện hardcode KPI demo. | Nếu backend report/inventory sẵn sàng, có thể orchestration qua BLL services trong MVP; nếu chưa, label rõ demo. | Không bắt buộc real dashboard nếu MVP chưa ổn định. |
| `src/QuanLyKhoBanHang.BLL/Services/AssistantService.cs` và `Services/Assistant/*` | AssistantService đang tạo service kho bằng constructor không tồn tại; rule-based thiếu/không rõ command checklist. | Sau Phase 0, đảm bảo 5 command demo handled; dùng BLL services, không gọi DAL. | Không thêm AI online nếu rule-based chưa ổn; không hard-code API key. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Main/FrmMain.cs` | Cần truyền current user vào form nghiệp vụ; default view đang mở supplier cố định. | Truyền `_currentUser` hoặc `_currentUser.Id` cho purchase/sales/stocktake; dùng default feature thật. | Không sửa lớn layout/sidebar nếu không cần. |
| `src/QuanLyKhoBanHang.WinForms/Forms/MasterData/FrmProduct.cs` | Save/deactivate đang stub. | Khi ProductService sẵn sàng, map input -> DTO -> service -> refresh. | Không gọi DAL hoặc SQL trực tiếp. |
| `src/QuanLyKhoBanHang.WinForms/Forms/MasterData/FrmCategory.cs` | Save/deactivate đang stub. | Gọi create/update/deactivate thật, giữ fallback list khi service fail. | Không đổi UI chính ngoài wiring tối thiểu. |
| `src/QuanLyKhoBanHang.WinForms/Forms/MasterData/FrmSupplier.cs` | Save/deactivate đang stub. | Gọi SupplierService thật, refresh sau success. | Không hard-code supplier behavior ngoài demo fallback. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Sales/FrmCustomer.cs` | Save/deactivate đang stub. | Sau Hùng bàn giao, gọi CustomerService thật. | Không rewrite customer repository từ UI. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Inventory/FrmInventory.cs` | Đã gọi service/fallback; cần messaging sạch. | Hiển thị trạng thái thật/demo rõ; refresh sau purchase/stocktake nếu cần. | Không thêm Excel thật trong MVP nếu chưa ổn. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Inventory/FrmPurchaseReceipt.cs` | Save gọi service nhưng `SupplierId = 1`, chưa set `CreatedByUserId`, supplier text demo. | Truyền user id; chỉ dùng supplier thật khi supplier service ready; message rõ nếu fallback. | Không hard-code user/supplier nếu có dữ liệu thật. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Inventory/FrmStocktake.cs` | Save vẫn demo. | Build `StocktakeDto`, set user id, gọi `CreateStocktake`, refresh inventory. | Không giả lập thành công khi service trả fail. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Sales/FrmSalesInvoice.cs` | Save gọi service nhưng chưa set `CreatedByUserId`. | Truyền user id; giữ no-oversell message từ service; refresh products sau success. | Không tự trừ tồn ở UI. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Reports/FrmReport.cs` | Report fallback demo. | Dùng service thật khi Hùng xong; fallback chỉ khi service fail/empty có chủ đích. | Không gọi DAL trực tiếp; export thật để Phase 5. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Assistant/FrmAssistant.cs` | UI cần đủ command demo. | Chỉ chỉnh messaging/UI nếu BLL trả đủ; không đưa business logic vào form. | Không gọi DAL/API trực tiếp từ WinForms. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Dashboard/FrmDashboard.cs` | KPI hardcode/fallback. | Nếu DashboardService chưa thật, ghi rõ demo; nếu thật, refresh từ service. | Không làm dashboard real trước build stabilization. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Auth/FrmLogin.cs` | Register/forgot là stub. | Giữ mô phỏng và message rõ trong MVP. | Không làm real registration/forgot password nếu chưa có OpenSpec mới. |
| `src/QuanLyKhoBanHang.WinForms/Forms/Admin/*.cs` | Admin account/permission/audit đang demo. | Giữ demo/in-memory trong MVP, chỉ polish message nếu cần. | Không làm Admin DAL thật trong scope integration MVP. |
| `tests/QuanLyKhoBanHang.Tests/*` | Một số test đang kỳ vọng demo service hoặc constructor không tham số. | Sau Phase 0, cập nhật/giữ test theo contract mới; thêm test nghiệp vụ Dũ/Hùng nếu backend sẵn sàng. | Không xóa test chỉ để build xanh. |
| `docs/06_ChecklistDemo.md` | Là checklist nghiệm thu cuối. | Chỉ cập nhật nếu behavior demo chính thức đổi. | Không sửa checklist để né lỗi demo. |
| `openspec/changes/bootstrap-inventory-sales-mvp/*` | Change hiện có còn task Dũ/Hùng/Châu chưa tick. | Chỉ tick task sau validate/build/test/demo pass. | Không implement ngoài scope mô tả. |

## 4. OpenSpec decision

- MVP integration hiện tại có thể tiếp tục dưới OpenSpec change hiện có: `bootstrap-inventory-sales-mvp`.
- Lý do: `tasks.md` của change này còn rõ các task chưa xong:
  - Dũ triển khai DAL/BLL thật cho kho.
  - Hùng triển khai DAL/BLL thật cho bán hàng và báo cáo.
  - Châu tích hợp service thật vào UI.
- Châu không cần tạo OpenSpec change mới chỉ để wire UI vào service MVP đã có contract.
- Nếu phát hiện cần đổi public method, DTO property, `TransactionType` schema, hoặc flow nghiệp vụ lớn, phải cập nhật OpenSpec/docs trước khi code tiếp.
- Các extension sau cần OpenSpec change riêng, không nằm trong MVP integration:
  - đăng ký tài khoản thật,
  - quên mật khẩu/đặt lại mật khẩu thật,
  - hash password/auth security thật,
  - Admin account/role/audit DAL thật,
  - in hóa đơn/xuất Excel thật nếu vượt demo.
- Proposed OpenSpec change name nếu làm auth/admin extension:

```text
auth-admin-real-accounts-recovery
```

Scope đề xuất cho change này: real account registration decision, admin reset/forgot password flow, password hashing, account/role/audit persistence, security acceptance criteria. Không gộp change này vào vòng MVP integration nếu build/demo chưa ổn.

## 5. Acceptance criteria

### Commands

Build phải pass:

```powershell
dotnet build QuanLyKhoBanHang.sln
```

Test phải pass hoặc có lý do rõ cho test chưa chạy được:

```powershell
dotnet test QuanLyKhoBanHang.sln --no-build --no-restore
```

Nếu có sửa OpenSpec change:

```powershell
npx --yes --package @fission-ai/openspec openspec validate bootstrap-inventory-sales-mvp
```

### Demo checklist items

- Database đã chạy `database/schema.sql` và `database/seed.sql`.
- Login được:
  - `admin/admin123`
  - `manager/123456`
  - `du/123456`
  - `hung/123456`
- Không exception khi mở từng menu chính.
- Role menu đúng:
  - Manager không thấy Admin-only.
  - WarehouseStaff không thấy bán hàng/báo cáo/Admin-only.
  - SalesStaff không thấy nhập kho/kiểm kê/báo cáo/Admin-only.
- Dashboard mở được và nêu rõ thật/demo.
- Product/category/supplier/customer list/search mở được.
- Tạo phiếu nhập kho nếu backend Dũ ready.
- Tồn kho/low stock hiển thị rõ thật/demo.
- Tạo kiểm kê nếu backend Dũ ready.
- Tạo hóa đơn bán hàng nếu backend Hùng ready; không bán vượt tồn.
- Báo cáo doanh thu/top products/top customers nếu backend Hùng ready.
- Assistant trả lời được:
  - `doanh thu hôm nay`
  - `hàng sắp hết`
  - `top sản phẩm bán chạy`
  - `khách hàng mua nhiều nhất`
  - `kiểm kê hôm nay`
- Admin section mở được ở trạng thái demo/in-memory.

### Expected behavior for real-service screens

- UI chỉ gọi BLL service.
- Save/update/deactivate gọi service thật và hiển thị `ServiceResult.Message`.
- Sau thao tác thành công, màn hình refresh lại data.
- Khi service fail, UI không crash và hiển thị lỗi rõ.
- `CreatedByUserId` ghi đúng user đăng nhập.
- Không có SQL trong WinForms.

### Expected behavior for remaining demo/stub screens

- Message phải nói rõ là demo/stub/mô phỏng.
- Không giả vờ đã ghi DB thật.
- Không dùng demo fallback để che service thật bị lỗi nếu màn hình đã được công bố là real-service.
- Register/forgot password vẫn là mô phỏng cho MVP, trừ khi OpenSpec extension được approve.

## 6. Risk list

### Ownership risks with Dũ/Hùng

- Châu có thể bị kéo vào sửa sâu backend kho/sales nếu build chưa xanh. Giới hạn của Châu là integration/review/minimal safe build fix; backend nghiệp vụ vẫn thuộc Dũ/Hùng.
- Nếu Dũ/Hùng đổi method/DTO/schema không báo, UI của Châu sẽ compile fail hoặc chạy sai demo.
- Nếu PR backend không ghi rõ service nào ready, Châu có thể wire nhầm màn hình vào service chưa ổn định.

### DTO/public method risks

- Thêm/xóa/đổi property DTO sẽ ảnh hưởng nhiều form.
- Đổi constructor service cần đồng bộ WinForms, tests, AssistantService.
- `SalesService.CreateInvoice` trả id cứng có thể làm UI/print/report sai.
- `StockTransactionType` int enum vs DB NVARCHAR là rủi ro contract lớn.

### DB/schema risks

- Hard-coded connection string `Server=BaoChau2203` làm máy khác khó demo.
- DB seed password đang là plain demo value trong `PasswordHash`; chấp nhận cho MVP demo nhưng không được gọi là auth thật.
- Transaction chưa atomic ở purchase/stocktake nếu orchestration nằm rời giữa nhiều repository call.
- Schema constraint `QuantityOnHand >= 0` có thể throw nếu backend không check oversell/adjustment đúng.

### Demo risks

- Build fail là rủi ro số 1; không làm thêm feature trước khi build xanh.
- Dashboard/Admin/Login demo có thể bị hiểu nhầm là thật nếu message không rõ.
- Assistant checklist có thể fail nếu rule-based thiếu `hàng sắp hết` hoặc `kiểm kê hôm nay`.
- Fallback stub có thể che lỗi DB thật; cần phân biệt bằng message và PR notes.

## 7. Instructions for Gemini 3.1 Pro

Follow this plan exactly.

Before editing:

- Run `git status -sb`.
- Confirm branch is `feature/project-integration-chau-v2`.
- Read:
  - `AGENTS.md`
  - `GEMINI.md`
  - `README.md`
  - `docs/06_ChecklistDemo.md`
  - `docs/phan-cong/Chau_Lan2_SauAudit.md`
  - `docs/phan-cong/Lan2_SauAuditMergePR.md`
  - `docs/10_AntigravityOpenSpecWorkflow.md`
  - `openspec/changes/bootstrap-inventory-sales-mvp/proposal.md`
  - `openspec/changes/bootstrap-inventory-sales-mvp/design.md`
  - `openspec/changes/bootstrap-inventory-sales-mvp/tasks.md`
  - all spec files under `openspec/changes/bootstrap-inventory-sales-mvp/specs/`

Scope rules:

- Do not broaden scope.
- Do not implement optional extensions before MVP is stable.
- Do not implement real registration, forgot password, password hashing, Admin DAL, print invoice, Excel export, or online AI unless a new OpenSpec change is created and approved.
- Do not silently change DTOs, public methods, return types, schema, or service constructors. If a contract change is required, stop and update/report OpenSpec/docs first.
- Do not rewrite Dũ/Hùng backend unless the change is a minimal safe build/integration fix explicitly needed to compile.
- Do not move business logic into WinForms.
- Do not add SQL to WinForms.
- Do not add direct WinForms reference to DAL.
- Do not hard-code API keys or secrets.

Implementation order:

1. Stabilize build provider/constructor blockers first.
2. Run build.
3. Review service contracts and backend readiness.
4. Wire only ready UI screens to BLL services.
5. Add user context for `CreatedByUserId`.
6. Clean fallback/demo messages.
7. Run OpenSpec validation if OpenSpec changed.
8. Run build/test.
9. Run demo checklist manually.
10. Mark OpenSpec tasks complete only after validate/build/test/demo pass.

When blocked:

- Report the blocker with file path, line/context, owner, and proposed next action.
- Do not guess missing DB/schema/DTO behavior.
- Do not fake success by leaving stub messages on screens that should now use real services.

