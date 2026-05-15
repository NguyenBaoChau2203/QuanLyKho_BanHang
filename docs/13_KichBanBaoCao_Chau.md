# Kịch Bản Báo Cáo - Phần Châu Phụ Trách

Tài liệu này dùng để Châu tập trình bày với vai trò nhóm trưởng. Trọng tâm: giới thiệu nhóm và đề tài, database, đăng nhập/đăng ký/quên mật khẩu, phân quyền cơ bản, và chatbot AI agent. Các phần kho, bán hàng, báo cáo chỉ nhắc ở mức luồng liên quan, không đi sâu thay phần của Dũ/Hùng.

## 1. Mục Tiêu Khi Trình Bày

- Nói rõ vai trò nhóm trưởng: chốt kiến trúc, database, UI tổng thể, auth, phân quyền, tích hợp service, chatbot AI assistant.
- Demo trước, giải thích luồng sau.
- Nhấn mạnh mô hình 3 lớp: WinForms -> BLL -> DAL -> SQL Server.
- Với chatbot AI agent, phải nhấn mạnh: AI không truy cập database trực tiếp, không sinh SQL để chạy, dữ liệu thật lấy từ BLL service.
- Với database, nói rõ: database hiện không dùng trigger và không dùng stored procedure; nghiệp vụ nằm ở BLL/DAL để dễ kiểm soát trong đồ án.

## 2. Phân Công Của Châu Cần Nói

Châu có thể nói:

> Em là Châu, nhóm trưởng của nhóm. Trong đề tài này em phụ trách kiến trúc tổng thể, OpenSpec, thiết kế database tổng thể, viết và quản lý SQL script, giao diện WinForms, tích hợp service của các bạn vào UI, phần đăng nhập - tài khoản - phân quyền cơ bản, và chatbot AI assistant. Các bạn còn lại phụ trách sâu các nghiệp vụ kho, bán hàng và báo cáo.

Không nên nhận sâu:

- Không nói mình làm toàn bộ nghiệp vụ nhập kho chi tiết nếu phần backend thuộc Dũ.
- Không nói mình làm toàn bộ nghiệp vụ bán hàng/báo cáo chi tiết nếu phần backend thuộc Hùng.
- Khi demo các module này, chỉ nói: "Em demo nhanh để thấy dữ liệu đi qua database và chatbot sử dụng lại service này".

## 3. Timeline Trình Bày Gợi Ý

| Phần | Thời lượng | Cách nói |
| --- | ---: | --- |
| Mở đầu nhóm, đề tài, phân công | 2 phút | Nói vai trò nhóm trưởng, mục tiêu phần mềm |
| Kiến trúc 3 lớp | 2 phút | Nói WinForms -> BLL -> DAL -> SQL Server |
| Database | 5 phút | Mở schema/SSMS, giải thích nhóm bảng và quan hệ |
| Auth: đăng nhập, đăng ký, quên mật khẩu | 4 phút | Demo rồi giải thích service/repository/database |
| Chatbot AI agent | 8-10 phút | Demo nhiều câu, giải thích luồng agent thật kỹ |
| Các phần còn lại | 2 phút | Nói nhanh danh mục/kho/bán hàng/báo cáo |
| Kết luận | 1 phút | Nêu hướng phát triển |

## 4. Mở Đầu Báo Cáo

Nói:

> Kính chào thầy và các bạn. Em là Châu, nhóm trưởng nhóm thực hiện đề tài Quản lý kho và bán hàng bằng WinForms. Mục tiêu của nhóm em là xây dựng một phần mềm có thể demo được quy trình cơ bản của một cửa hàng: quản lý hàng hóa, nhà cung cấp, khách hàng, nhập kho, bán hàng, kiểm kê, báo cáo doanh thu và có thêm trợ lý AI hỗ trợ người quản lý tra cứu nhanh.
>
> Nhóm em tổ chức project theo mô hình 3 lớp. Giao diện WinForms chỉ gọi service ở tầng BLL. BLL xử lý validation, nghiệp vụ và điều phối. DAL dùng ADO.NET để truy cập SQL Server. DTO dùng để truyền dữ liệu giữa các tầng.
>
> Trong phần trình bày của em, em sẽ tập trung vào những phần em phụ trách chính: database, xác thực người dùng, phân quyền cơ bản và chatbot AI assistant. Các phần còn lại em demo ở mức tổng quan để thấy hệ thống chạy đồng bộ.

## 5. Kiến Trúc Tổng Quan Cần Nói

Sơ đồ nói miệng:

```text
WinForms UI
    -> BLL Service
        -> DAL Repository
            -> SQL Server
        <- DTO / ServiceResult<T>
    <- ServiceResult<T>
```

Nói:

> Em cố gắng giữ nguyên tắc là form không gọi DAL trực tiếp và không chứa SQL. Form chỉ lấy dữ liệu người dùng nhập, gọi service, rồi hiển thị kết quả. Service public trả về ServiceResult gồm Success, Message và Data, nhờ vậy UI xử lý thành công/thất bại thống nhất.

File liên quan:

| Thành phần | File chính |
| --- | --- |
| ServiceResult chung | `src/QuanLyKhoBanHang.BLL/Common/ServiceResult.cs` |
| DTO người dùng | `src/QuanLyKhoBanHang.DTO/Auth/UserDto.cs` |
| DTO assistant | `src/QuanLyKhoBanHang.DTO/Assistant/AssistantResponseDto.cs` |
| Cấu hình chuỗi kết nối | `src/QuanLyKhoBanHang.DAL/Data/DatabaseOptions.cs` |
| Main shell điều hướng | `src/QuanLyKhoBanHang.WinForms/Forms/Main/FrmMain.cs` |

## 6. Phần Database

### 6.1. File Cần Mở Khi Trình Bày

| Nội dung | File |
| --- | --- |
| Tạo database, bảng, khóa ngoại, constraint, index | `database/schema.sql` |
| Dữ liệu demo: role, user, sản phẩm, nhà cung cấp, khách hàng, phiếu nhập, hóa đơn, kiểm kê, phân quyền | `database/seed.sql` |
| Ghi chú cách chạy database | `database/README.md` |

### 6.2. Câu Nói Mở Đầu Database

> Phần database là nền tảng của toàn bộ hệ thống. Em thiết kế database theo các nhóm nghiệp vụ: người dùng và phân quyền, danh mục, nhập kho, bán hàng, giao dịch kho, kiểm kê, audit log và yêu cầu khôi phục mật khẩu.
>
> Database hiện không dùng trigger và không dùng stored procedure. Lý do là nhóm em muốn giữ nghiệp vụ ở tầng BLL, còn DAL chịu trách nhiệm query và mapping. Cách này giúp thầy/các bạn đọc code dễ hơn, thấy rõ luồng WinForms -> BLL -> DAL, đồng thời tránh nghiệp vụ bị ẩn dưới database.

Nếu thầy hỏi "vậy có stored procedure nào không?":

> Dạ hiện tại không có stored procedure. Hệ thống dùng ADO.NET với câu SQL có parameter trong repository. Ví dụ khi đăng nhập, `UserRepository.GetByUsername` dùng `@Username`, không nối chuỗi từ input người dùng.

Nếu thầy hỏi "có trigger không?":

> Dạ không có trigger. Việc tăng/giảm tồn kho hoặc ghi stock transaction được thiết kế để xử lý ở service/repository, để luồng nghiệp vụ rõ ràng trong code và phù hợp mô hình 3 lớp của đồ án.

### 6.3. Nhóm Bảng Cần Giải Thích

| Nhóm | Bảng | Ý nghĩa |
| --- | --- | --- |
| Người dùng | `Roles`, `Users` | Lưu vai trò, tài khoản, hash mật khẩu, trạng thái active, yêu cầu đổi mật khẩu |
| Phân quyền | `Permissions`, `RolePermissions` | Xác định màn hình nào role nào được truy cập |
| Danh mục | `Categories`, `Products`, `Suppliers`, `Customers` | Nền tảng cho kho và bán hàng |
| Nhập kho | `PurchaseReceipts`, `PurchaseReceiptDetails` | Phiếu nhập và dòng sản phẩm nhập |
| Bán hàng | `SalesInvoices`, `SalesInvoiceDetails` | Hóa đơn bán và dòng sản phẩm bán |
| Tồn kho | `StockTransactions` | Lịch sử tăng/giảm tồn theo từng sản phẩm |
| Kiểm kê | `Stocktakes`, `StocktakeDetails` | Phiếu kiểm kê, số hệ thống và số thực tế |
| Audit/Auth | `AuditLogs`, `PasswordRecoveryRequests` | Nhật ký thao tác và yêu cầu quên mật khẩu |

### 6.4. Điểm Thiết Kế Nên Nhấn Mạnh

- Khóa chính dùng `Id INT IDENTITY`.
- Các bảng nghiệp vụ có khóa ngoại rõ ràng, ví dụ:
  - `Users.RoleId -> Roles.Id`
  - `Products.CategoryId -> Categories.Id`
  - `PurchaseReceipts.SupplierId -> Suppliers.Id`
  - `SalesInvoices.CustomerId -> Customers.Id`
  - `StockTransactions.ProductId -> Products.Id`
- Số tiền dùng `DECIMAL(18,2)`.
- Ngày giờ dùng `DATETIME2`.
- Các danh mục dùng `IsActive` để ngưng sử dụng thay vì xóa cứng.
- Có constraint kiểm tra giá và số lượng không âm:
  - `CK_Products_Prices`
  - `CK_Products_Quantity`
  - `CK_PurchaseReceiptDetails_Quantity`
  - `CK_SalesInvoiceDetails_Quantity`
- Có index hỗ trợ tra cứu:
  - `IX_Products_Name`
  - `IX_StockTransactions_ProductId`
  - `IX_SalesInvoices_InvoiceDate`
  - `IX_PurchaseReceipts_ReceiptDate`

### 6.5. Luồng Dữ Liệu Database Nên Trình Bày

#### Luồng nhập kho

```text
Suppliers + Products
    -> PurchaseReceipts
    -> PurchaseReceiptDetails
    -> StockTransactions
    -> Products.QuantityOnHand tăng
```

Nói ngắn:

> Khi nhập hàng, hệ thống tạo phiếu nhập, tạo chi tiết phiếu nhập, sau đó tồn kho của sản phẩm tăng và có một dòng lịch sử trong StockTransactions.

#### Luồng bán hàng

```text
Customers + Products
    -> SalesInvoices
    -> SalesInvoiceDetails
    -> StockTransactions
    -> Products.QuantityOnHand giảm
```

Nói ngắn:

> Khi bán hàng, hệ thống tạo hóa đơn và chi tiết hóa đơn. Về nguyên tắc nghiệp vụ, service phải kiểm tra tồn kho trước, sau đó giảm tồn và ghi lại giao dịch xuất kho.

#### Luồng kiểm kê

```text
Products
    -> Stocktakes
    -> StocktakeDetails
    -> so sánh SystemQuantity và ActualQuantity
```

Nói ngắn:

> Kiểm kê dùng để so sánh số lượng trên hệ thống với số lượng thực tế. Phần này giúp phát hiện lệch kho.

#### Luồng chatbot đọc dữ liệu

```text
FrmAssistant
    -> AssistantService
        -> ReportService / InventoryService / StocktakeService
            -> DAL Repository
                -> SQL Server
```

Nói:

> Chatbot không đọc database trực tiếp. Chatbot đi qua service giống các màn hình khác. Đây là điểm an toàn quan trọng.

## 7. Phần Đăng Nhập, Đăng Ký, Quên Mật Khẩu

### 7.1. Trang/Màn Hình Liên Quan

| Trang/màn hình | File UI | Service | Repository | Bảng DB |
| --- | --- | --- | --- | --- |
| Đăng nhập | `src/QuanLyKhoBanHang.WinForms/Forms/Auth/FrmLogin.cs` | `src/QuanLyKhoBanHang.BLL/Services/AuthService.cs` | `src/QuanLyKhoBanHang.DAL/Auth/UserRepository.cs`, `src/QuanLyKhoBanHang.DAL/Auth/AuditLogRepository.cs` | `Users`, `Roles`, `AuditLogs` |
| Đăng ký trên màn login | `FrmLogin.cs` | Hiện là luồng thông báo cần admin phê duyệt | Không tạo trực tiếp từ form login | Không ghi user mới từ màn này |
| Quên mật khẩu | `FrmLogin.cs` | `src/QuanLyKhoBanHang.BLL/Services/PasswordRecoveryService.cs` | `PasswordRecoveryRepository.cs`, `UserRepository.cs`, `AuditLogRepository.cs` | `PasswordRecoveryRequests`, `Users`, `AuditLogs` |
| Đổi mật khẩu bắt buộc | `src/QuanLyKhoBanHang.WinForms/Forms/Auth/FrmChangePassword.cs` | `UserAccountService.ChangePassword` | `UserRepository`, `AuditLogRepository` | `Users`, `AuditLogs` |
| Quản lý tài khoản admin | `src/QuanLyKhoBanHang.WinForms/Forms/Admin/FrmUserManagement.cs` | `UserAccountService` | `UserRepository`, `RoleRepository`, `AuditLogRepository` | `Users`, `Roles`, `AuditLogs` |
| Phân quyền menu | `FrmMain.cs` | `PermissionService` | `PermissionRepository` | `Permissions`, `RolePermissions` |

### 7.2. Demo Đăng Nhập

Demo:

1. Mở app.
2. Nhập sai user/password để thấy thông báo.
3. Nhập đúng tài khoản demo.
4. Sau khi đăng nhập, vào màn hình chính.

Tài khoản seed có trong `database/seed.sql`:

| Username | Vai trò |
| --- | --- |
| `admin` | Admin |
| `manager` | Quản lý |
| `du` | Thủ kho |
| `hung` | Nhân viên bán hàng |

Nói:

> Khi đăng nhập, form `FrmLogin` không kiểm tra database trực tiếp mà gọi `AuthService.Authenticate`. Service kiểm tra username/password rỗng, tìm user theo username, kiểm tra tài khoản còn active không, sau đó lấy hash mật khẩu và gọi `PasswordHasher.Verify`. Nếu đúng, service cập nhật `LastLoginAt`, ghi audit log và trả về `ServiceResult<UserDto>`.

Luồng:

```text
FrmLogin.HandleLogin
    -> AuthService.Authenticate(username, password)
        -> UserRepository.GetByUsername(username)
        -> UserRepository.GetPasswordHash(user.Id)
        -> PasswordHasher.Verify(password, storedHash)
        -> UserRepository.UpdateLastLogin(user.Id)
        -> AuditLogRepository.Write(...)
    -> nếu Success: mở FrmMain(currentUser)
```

Điểm bảo mật nên nói:

- Mật khẩu không so sánh plain text trực tiếp.
- `PasswordHasher` dùng PBKDF2 (`Rfc2898DeriveBytes`) với SHA256, salt và 100000 iterations.
- Repository dùng parameter `@Username`, `@Id`, không nối chuỗi input người dùng.
- Login fail trả thông báo chung để không lộ tài khoản nào tồn tại.

### 7.3. Demo Đăng Ký

Demo:

1. Bấm link "Đăng ký tài khoản".
2. Nhập thông tin hoặc chỉ mở màn hình.
3. Bấm tạo tài khoản.

Nói đúng hiện trạng:

> Với bản demo hiện tại, đăng ký trực tiếp từ màn login chưa tự tạo tài khoản ngay. Hệ thống hiển thị thông báo tài khoản mới cần quản trị viên phê duyệt và phân quyền. Cách này phù hợp với phần mềm quản lý nội bộ, vì không phải ai cũng được tự tạo tài khoản vào hệ thống kho/bán hàng.

Nếu thầy hỏi tạo tài khoản thật ở đâu:

> Tạo tài khoản thật nằm ở phần quản trị tài khoản, thông qua `UserAccountService.CreateAccount`, có kiểm tra trùng username, hash mật khẩu rồi lưu vào bảng `Users`.

Luồng admin tạo tài khoản:

```text
FrmUserManagement
    -> UserAccountService.CreateAccount(account, createdByUserId)
        -> ValidateAccount
        -> UserRepository.IsUsernameTaken
        -> PasswordHasher.Hash
        -> UserRepository.Create
        -> AuditLogRepository.Write
```

### 7.4. Demo Quên Mật Khẩu

Demo:

1. Bấm "Quên mật khẩu".
2. Nhập username.
3. Bấm gửi yêu cầu khôi phục.
4. Hệ thống báo yêu cầu đã được ghi nhận.

Nói:

> Luồng quên mật khẩu của nhóm em được thiết kế theo hướng an toàn cho phần mềm nội bộ. Người dùng gửi yêu cầu khôi phục, hệ thống ghi nhận vào bảng `PasswordRecoveryRequests`, sau đó quản trị viên hỗ trợ đặt lại mật khẩu. Hệ thống trả thông báo chung kể cả username không tồn tại để tránh lộ thông tin tài khoản.

Luồng:

```text
FrmLogin.HandleForgotStub
    -> PasswordRecoveryService.SubmitForgotPasswordRequest(username)
        -> UserRepository.GetByUsername(username)
        -> nếu user active:
            -> GenerateRequestCode
            -> PasswordRecoveryRepository.Create(user.Id, requestCode)
            -> AuditLogRepository.Write
        -> luôn trả message chung cho UI
```

Điểm nên nói:

- Không gửi email/OTP trong MVP.
- Có bảng `PasswordRecoveryRequests` để lưu yêu cầu.
- Có `AuditLogs` để lưu thao tác.
- Admin có thể reset mật khẩu bằng `UserAccountService.ResetPassword`.

### 7.5. Phân Quyền Sau Đăng Nhập

Nói:

> Sau khi đăng nhập thành công, `FrmMain` nhận `UserDto currentUser`. Main shell gọi `PermissionService.GetAccessibleFeatures(role)` để lấy danh sách màn hình mà vai trò được truy cập. Vì vậy mỗi role nhìn thấy menu khác nhau.

Luồng:

```text
FrmMain(currentUser)
    -> PermissionService.GetAccessibleFeatures(currentUser.Role)
        -> PermissionRepository.GetFeatureKeysForRole(roleId)
            -> Permissions + RolePermissions
    -> build sidebar/menu theo quyền
```

File:

- `src/QuanLyKhoBanHang.WinForms/Forms/Main/FrmMain.cs`
- `src/QuanLyKhoBanHang.BLL/Services/PermissionService.cs`
- `src/QuanLyKhoBanHang.DAL/Auth/PermissionRepository.cs`
- `database/schema.sql`: `Permissions`, `RolePermissions`
- `database/seed.sql`: seed quyền cho Admin/Manager/SalesStaff/WarehouseStaff

## 8. Phần Chatbot AI Agent

Đây là phần cần nói kỹ nhất.

### 8.1. Trang/Màn Hình Và File Liên Quan

| Thành phần | File |
| --- | --- |
| Màn hình chatbot | `src/QuanLyKhoBanHang.WinForms/Forms/Assistant/FrmAssistant.cs` |
| Service chính UI gọi | `src/QuanLyKhoBanHang.BLL/Services/AssistantService.cs` |
| Rule-based fallback | `src/QuanLyKhoBanHang.BLL/Services/Assistant/RuleBasedAssistantProvider.cs` |
| Intent hợp lệ | `src/QuanLyKhoBanHang.BLL/Services/Assistant/AssistantIntentCatalog.cs` |
| Mode AI/offline/fallback | `src/QuanLyKhoBanHang.BLL/Services/Assistant/AssistantModes.cs` |
| Context an toàn gửi cho AI | `src/QuanLyKhoBanHang.BLL/Services/Assistant/AssistantSafeContext.cs` |
| Provider DeepSeek | `src/QuanLyKhoBanHang.BLL/Services/Assistant/DeepSeekAssistantProvider.cs` |
| Cấu hình DeepSeek | `src/QuanLyKhoBanHang.BLL/Services/Assistant/DeepSeekOptions.cs` |
| DTO trả về UI | `src/QuanLyKhoBanHang.DTO/Assistant/AssistantResponseDto.cs` |
| Test chatbot | `tests/QuanLyKhoBanHang.Tests/AssistantServiceTests.cs` |

Service chatbot dùng lại:

| Câu hỏi | Service BLL |
| --- | --- |
| `doanh thu hôm nay` | `ReportService.GetRevenue(...)` |
| `hàng sắp hết`, `hang sap het` | `InventoryService.GetLowStockProducts()` |
| `top sản phẩm bán chạy` | `ReportService.GetTopSellingProducts(...)` |
| `khách hàng mua nhiều nhất` | `ReportService.GetTopCustomers(...)` |
| `kiểm kê hôm nay` | `StocktakeService.GetStocktakes(...)` |

### 8.2. Demo Chatbot

Câu nên demo:

```text
doanh thu hôm nay
hàng sắp hết
hang sap het
sản phẩm nào sắp hết hàng?
top sản phẩm bán chạy
khách hàng mua nhiều nhất
kiểm kê hôm nay
```

Khi demo, nói:

> Em sẽ demo chatbot theo kiểu người quản lý hỏi nhanh bằng tiếng Việt. Thay vì vào từng màn hình báo cáo hoặc tồn kho, người dùng có thể hỏi trực tiếp: doanh thu hôm nay, hàng sắp hết, top sản phẩm bán chạy.

### 8.3. Giải Thích Agent Là Gì

Nói:

> Trong phạm vi đồ án, agent ở đây là một trợ lý có khả năng nhận câu hỏi tự nhiên, phân loại ý định của người dùng, gọi đúng service nghiệp vụ nội bộ, lấy dữ liệu an toàn rồi trả lời lại. Nó không phải agent tự hành phức tạp, nhưng đã có orchestration: nhận yêu cầu, xác định intent, gọi công cụ nội bộ là các BLL service, xử lý fallback và trả kết quả.

### 8.4. Luồng Chatbot Offline Rule-Based

```text
FrmAssistant
    -> AssistantService.Ask(question)
        -> nếu không có DEEPSEEK_API_KEY:
            -> RuleBasedAssistantProvider.Ask(question, offline-rule-based, ...)
                -> normalize câu hỏi tiếng Việt/câu không dấu
                -> xác định intent
                -> gọi ReportService / InventoryService / StocktakeService
                -> tạo AssistantResponseDto
    -> UI hiển thị câu trả lời
```

Nói:

> Nếu máy không có API key hoặc không có mạng, chatbot vẫn chạy bằng rule-based offline. Đây là yêu cầu quan trọng để demo ổn định trên lớp.

### 8.5. Luồng Chatbot AI Online

```text
FrmAssistant
    -> AssistantService.Ask(question)
        -> RuleBasedAssistantProvider.BuildSafeContexts()
            -> lấy sẵn dữ liệu an toàn từ BLL
        -> DeepSeekAssistantProvider.Ask(question, safeContexts)
            -> AI chọn intent và trả JSON
        -> AssistantService ground lại câu trả lời bằng dữ liệu BLL
        -> trả AssistantResponseDto mode ai-online
```

Nói:

> Nếu có `DEEPSEEK_API_KEY`, hệ thống thử dùng AI online. Nhưng AI chỉ nhận context an toàn đã được BLL chuẩn bị sẵn. AI không nhận connection string, không nhận DAL object, không được sinh SQL để hệ thống chạy. Sau khi AI trả về intent, `AssistantService` vẫn ground lại câu trả lời bằng dữ liệu từ BLL để tránh AI bịa số liệu.

### 8.6. Luồng Khi AI Lỗi

```text
DeepSeek lỗi / timeout / API key sai / response sai JSON
    -> AssistantService catch exception
    -> gọi RuleBasedAssistantProvider.Ask(...)
    -> trả mode ai-failed-fallback
    -> WinForms không crash
```

Nói:

> Nếu API lỗi, mất mạng, hết quota hoặc trả dữ liệu không hợp lệ, service bắt lỗi và chuyển về rule-based fallback. Người dùng vẫn nhận được câu trả lời demo-safe, app không bị crash.

### 8.7. Điểm An Toàn Cần Nhấn Mạnh

Nói rõ từng ý:

- WinForms chỉ gọi `AssistantService`.
- `AssistantService` nằm trong BLL, không để UI gọi DeepSeek trực tiếp.
- DeepSeek provider không gọi DAL.
- AI không được thực thi SQL.
- Dữ liệu thật vẫn lấy từ:
  - `ReportService`
  - `InventoryService`
  - `StocktakeService`
- Nếu dữ liệu trong database không có, chatbot phải nói không có, không tự bịa "Sản phẩm A/B/C".

Câu nói mạnh:

> Điểm quan trọng là nhóm em không để AI bịa số liệu. Ví dụ khi hỏi "sản phẩm nào sắp hết hàng", câu trả lời phải dựa trên `InventoryService.GetLowStockProducts()`. Nếu database không có sản phẩm dưới mức tồn tối thiểu, chatbot phải trả lời là hiện tại không có sản phẩm nào sắp hết, chứ không tự tạo dữ liệu mẫu.

### 8.8. Nếu Thầy Hỏi Vì Sao Cần Chatbot

Trả lời:

> Báo cáo truyền thống yêu cầu người dùng biết phải vào đúng màn hình. Chatbot giúp người quản lý hỏi nhanh bằng ngôn ngữ tự nhiên. Ví dụ thay vì vào báo cáo rồi chọn ngày, người quản lý có thể hỏi "doanh thu hôm nay". Về kỹ thuật, chatbot vẫn đi qua service nên không phá kiến trúc 3 lớp.

### 8.9. Nếu Thầy Hỏi AI Có Thay Thế BLL Không

Trả lời:

> Dạ không. AI không thay thế BLL. BLL vẫn là nơi xử lý nghiệp vụ và lấy dữ liệu. AI chỉ hỗ trợ hiểu câu hỏi tiếng Việt và diễn đạt câu trả lời thân thiện hơn.

## 9. Các Trang/Màn Hình Châu Có Thể Nhắc Nhanh

| Màn hình | File UI | Vai trò trong phần Châu |
| --- | --- | --- |
| Login/Register/Forgot | `Forms/Auth/FrmLogin.cs` | Auth UI chính |
| Change password | `Forms/Auth/FrmChangePassword.cs` | Đổi mật khẩu khi bắt buộc |
| Main shell | `Forms/Main/FrmMain.cs` | Điều hướng, phân quyền, truyền current user |
| Assistant | `Forms/Assistant/FrmAssistant.cs` | Chatbot AI agent |
| User management | `Forms/Admin/FrmUserManagement.cs` | Admin tạo/sửa/reset tài khoản |
| Role permission | `Forms/Admin/FrmRolePermission.cs` | Xem ma trận quyền |
| Audit log | `Forms/Admin/FrmAuditLog.cs` | Xem lịch sử thao tác |
| Dashboard | `Forms/Dashboard/FrmDashboard.cs` | Demo tổng quan, có thể nói nhanh |

Các màn hình còn lại do Châu làm UI/tích hợp nhưng không nên giải thích nghiệp vụ sâu:

| Màn hình | File UI | Nói ở mức nào |
| --- | --- | --- |
| Sản phẩm | `Forms/MasterData/FrmProduct.cs` | Nói là danh mục dùng bảng `Products` |
| Loại hàng | `Forms/MasterData/FrmCategory.cs` | Nói là danh mục dùng bảng `Categories` |
| Nhà cung cấp | `Forms/MasterData/FrmSupplier.cs` | Nói là danh mục dùng bảng `Suppliers` |
| Khách hàng | `Forms/Sales/FrmCustomer.cs` | Nói là dữ liệu nền cho bán hàng |
| Nhập kho | `Forms/Inventory/FrmPurchaseReceipt.cs` | Nói luồng database tổng quan, nghiệp vụ sâu thuộc kho |
| Tồn kho | `Forms/Inventory/FrmInventory.cs` | Nói liên quan chatbot `hàng sắp hết` |
| Kiểm kê | `Forms/Inventory/FrmStocktake.cs` | Nói liên quan chatbot `kiểm kê hôm nay` |
| Bán hàng | `Forms/Sales/FrmSalesInvoice.cs` | Nói luồng database tổng quan, nghiệp vụ sâu thuộc bán hàng |
| Báo cáo | `Forms/Reports/FrmReport.cs` | Nói liên quan chatbot và report service |

## 10. Checklist Demo Theo Thứ Tự Nên Chạy

### Bước 1: Mở app và login

Nói:

> Em sẽ đăng nhập bằng tài khoản seed trong database.

Demo:

- Nhập sai để thấy lỗi.
- Nhập đúng `admin`.
- Vào main shell.

### Bước 2: Nói về phân quyền menu

Nói:

> Menu bên trái được dựng từ quyền trong database. `FrmMain` không hard-code cho mọi người thấy tất cả màn hình, mà gọi `PermissionService`.

Nếu có thời gian, logout và login role khác để thấy menu khác.

### Bước 3: Mở database/schema

Nói:

> Em mở nhanh script database để thầy thấy các nhóm bảng chính.

Chỉ vào:

- `Roles`, `Users`
- `Products`
- `PurchaseReceipts`, `PurchaseReceiptDetails`
- `SalesInvoices`, `SalesInvoiceDetails`
- `StockTransactions`
- `Permissions`, `RolePermissions`
- `PasswordRecoveryRequests`
- `AuditLogs`

### Bước 4: Demo quên mật khẩu

Nói:

> Phần quên mật khẩu ghi nhận yêu cầu để admin xử lý, không reset tự do ngay trên màn login.

### Bước 5: Demo chatbot AI agent

Hỏi lần lượt:

```text
doanh thu hôm nay
hàng sắp hết
hang sap het
top sản phẩm bán chạy
khách hàng mua nhiều nhất
kiểm kê hôm nay
```

Sau mỗi câu trả lời, nói:

> Câu trả lời này không lấy từ AI bịa ra. Nó đi qua `AssistantService`, rồi gọi service nghiệp vụ tương ứng.

## 11. Câu Hỏi Dự Phòng Và Cách Trả Lời

### Vì sao không dùng trigger?

> Vì nhóm em muốn nghiệp vụ nằm rõ trong tầng BLL/DAL để đúng mô hình 3 lớp và dễ demo, dễ test. Trigger dễ làm nghiệp vụ bị ẩn dưới database. Với đồ án này, em ưu tiên luồng rõ ràng hơn.

### Vì sao không dùng stored procedure?

> Hiện tại hệ thống chưa dùng stored procedure. DAL dùng ADO.NET và parameterized SQL. Cách này đủ cho MVP, dễ đọc code và dễ trace từ UI xuống database. Nếu mở rộng thực tế, có thể tách một số báo cáo nặng sang stored procedure sau.

### Chatbot có trực tiếp query database không?

> Không. Chatbot chỉ gọi `AssistantService` ở BLL. Service này gọi các service nghiệp vụ như `ReportService`, `InventoryService`, `StocktakeService`. AI không có quyền truy cập DAL/database.

### Nếu không có mạng thì chatbot có chạy không?

> Có. Không có API key hoặc API lỗi thì hệ thống dùng rule-based offline.

### AI có thể bịa số liệu không?

> Thiết kế hiện tại hạn chế việc đó bằng cách ground lại câu trả lời bằng dữ liệu BLL. AI chỉ phân loại/diễn đạt, số liệu nghiệp vụ vẫn lấy từ service.

### Đăng ký tài khoản có tạo trực tiếp không?

> Trên màn login, đăng ký là luồng yêu cầu quản trị viên phê duyệt. Với phần mềm nội bộ kho/bán hàng, tài khoản nên do admin tạo và phân quyền. Tạo tài khoản thật nằm ở màn quản trị tài khoản.

### Quên mật khẩu có gửi email không?

> Trong MVP chưa gửi email. Hệ thống ghi nhận yêu cầu vào `PasswordRecoveryRequests`, sau đó admin reset mật khẩu. Đây là hướng demo-safe và có thể mở rộng OTP/email sau.

### Mật khẩu có lưu plain text không?

> Không. Mật khẩu dùng `PasswordHasher`, có salt và PBKDF2 SHA256 với 100000 iterations. Seed data cũng lưu dạng hash theo format `v1:iterations:salt:hash`.

## 12. Kết Luận Nên Nói

> Tổng kết lại, phần em phụ trách tập trung vào nền tảng để cả nhóm phát triển đồng bộ: database, kiến trúc 3 lớp, xác thực người dùng, phân quyền cơ bản, giao diện chính và chatbot AI assistant. Điểm em muốn nhấn mạnh nhất là hệ thống giữ được ranh giới kiến trúc: UI không gọi database, AI không gọi database, dữ liệu nghiệp vụ đi qua BLL service. Nếu phát triển tiếp, nhóm em có thể mở rộng thêm email/OTP cho quên mật khẩu, stored procedure cho báo cáo nặng, và nâng cấp chatbot thành trợ lý phân tích sâu hơn nhưng vẫn giữ nguyên nguyên tắc an toàn.

## 13. File Nên Mở Sẵn Trước Khi Báo Cáo

Mở sẵn các file này để demo không mất thời gian:

```text
database/schema.sql
database/seed.sql
src/QuanLyKhoBanHang.WinForms/Forms/Auth/FrmLogin.cs
src/QuanLyKhoBanHang.BLL/Services/AuthService.cs
src/QuanLyKhoBanHang.BLL/Services/PasswordRecoveryService.cs
src/QuanLyKhoBanHang.BLL/Services/AssistantService.cs
src/QuanLyKhoBanHang.BLL/Services/Assistant/RuleBasedAssistantProvider.cs
src/QuanLyKhoBanHang.BLL/Services/Assistant/DeepSeekAssistantProvider.cs
src/QuanLyKhoBanHang.WinForms/Forms/Assistant/FrmAssistant.cs
```

Ưu tiên mở bằng thứ tự:

1. App đang chạy.
2. `database/schema.sql`.
3. `FrmLogin.cs`.
4. `AuthService.cs`.
5. `AssistantService.cs`.
6. `RuleBasedAssistantProvider.cs`.
7. `DeepSeekAssistantProvider.cs`.

## 14. Ghi Nhớ Khi Nói

- Không nói "AI tự lấy dữ liệu từ database".
- Không nói "đăng ký tự tạo tài khoản ngay" nếu đang demo màn login.
- Không nói có trigger/stored procedure vì schema hiện không có.
- Không nói mọi báo cáo/kho/bán hàng đều do Châu làm sâu; Châu phụ trách database/UI/tích hợp, các bạn phụ trách backend nghiệp vụ theo phân công.
- Khi bị hỏi sâu, quay về nguyên tắc: WinForms -> BLL -> DAL -> SQL Server.
