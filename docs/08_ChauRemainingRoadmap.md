# Châu Remaining Roadmap After Phase 0 Contract Foundation

> Mục tiêu của roadmap này là chia toàn bộ phần việc còn lại của Châu thành các OpenSpec change nhỏ, độc lập, build được, test được, review được và không làm vỡ contract Phase 0.
>
> Nguyên tắc xuyên suốt:
>
> - Chưa implement application code trong roadmap này.
> - Không đổi DTO/service contract trừ khi ghi rõ là Phase 0 gap cần follow-up contract.
> - WinForms luôn đi theo chuỗi `WinForms -> BLL -> DAL -> DTO`.
> - WinForms không gọi DAL trực tiếp, không chứa SQL.
> - UI phải có thể chạy với stub/mock service khi backend thật chưa sẵn sàng.
> - Mỗi phase phải độc lập, có thể commit riêng, review riêng, demo riêng.

## 1. Tổng quan chiến lược

### Mục tiêu lớn

Sau Phase 0, Châu không nên làm một change khổng lồ cho toàn bộ UI. Thay vào đó, nên tách phần còn lại thành các OpenSpec change nhỏ theo thứ tự:

1. xác nhận database/seed đủ demo,
2. dựng shell UI,
3. dựng shared theme/component,
4. làm master data UI,
5. làm inventory UI,
6. làm sales UI,
7. làm dashboard/reports UI,
8. làm assistant UI,
9. tích hợp service thật và polish cuối.

### Tư duy SDD

Mỗi change cần có tối thiểu:

- `proposal.md` mô tả mục tiêu, phạm vi, ngoài phạm vi.
- `design.md` nếu có quyết định kiến trúc hoặc UI layout lớn.
- `tasks.md` chia việc rõ ràng, kiểm tra được.
- tiêu chí hoàn thành gắn với build/test/demo.

### Mô hình cộng tác với Dũ và Hùng

- Dũ có thể tiếp tục backend kho song song.
- Hùng có thể tiếp tục backend bán hàng/báo cáo song song.
- Châu làm UI theo contract đã chốt, dùng mock/stub nếu service thật chưa merge.
- Chỉ đến phase 9 mới tích hợp service thật rộng rãi.

### Những file cần giữ ổn định

- `database/schema.sql`
- `database/seed.sql`
- `src/QuanLyKhoBanHang.DTO/`
- `src/QuanLyKhoBanHang.BLL/Services/` public signatures
- `src/QuanLyKhoBanHang.WinForms/` UI shell và screens

Nếu phát hiện Phase 0 gap, chỉ ghi nhận ở phase tương ứng và tách thành follow-up contract change nhỏ.

---

## 2. Roadmap phases

## Phase 1 — `phase-1-database-demo-readiness`

**OpenSpec change name**

- `phase-1-database-demo-readiness`

**Purpose**

Xác nhận schema và seed đã đủ tốt để demo UI và làm nền cho các phase UI tiếp theo.

**Goal**

Đảm bảo database hiện tại hỗ trợ:

- login demo,
- master data demo,
- nhập kho,
- tồn kho,
- bán hàng,
- dashboard/reports,
- assistant rule-based demo.

**Exact scope**

- Rà soát `schema.sql` và `seed.sql` dưới góc nhìn demo UX.
- Kiểm tra dữ liệu seed có đủ và hợp lý cho các màn hình sau này.
- Xác nhận các bảng, quan hệ, khóa, `IsActive`, kiểu số tiền/ngày giờ.
- Xác nhận dữ liệu demo tạo được:
  - ít nhất một admin login,
  - sản phẩm có tồn,
  - sản phẩm gần hết hàng,
  - khách hàng demo,
  - nhà cung cấp demo,
  - giao dịch nhập/bán mẫu để dashboard và report có dữ liệu.
- Ghi lại mọi gap nếu cần sửa schema/seed ở phase sau.

**Out of scope**

- Không viết UI.
- Không viết BLL/DAL thật.
- Không thêm business logic mới.
- Không đổi public contract nếu không có gap thật sự.

**Files/modules likely affected**

- `database/schema.sql`
- `database/seed.sql`
- `docs/07_ContractFoundation.md` chỉ khi cần ghi nhận gap nhỏ
- `docs/08_ChauRemainingRoadmap.md` nếu cần cập nhật thứ tự

**Service contracts used**

- Chưa gọi trực tiếp service thật.
- Chỉ đối chiếu với contract Phase 0 đã chốt:
  - `AuthService`
  - `CategoryService`
  - `ProductService`
  - `SupplierService`
  - `CustomerService`
  - `PurchaseService`
  - `InventoryService`
  - `StocktakeService`
  - `SalesService`
  - `ReportService`
  - `AssistantService`

**Mock/stub strategy**

- Không cần stub UI mới.
- Dùng seed data làm nguồn demo mặc định.
- Nếu một số query/report chưa có backend thật, ghi chú rõ để phase 9 nối sau.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Không áp dụng UI coding ở phase này.
- Chỉ đảm bảo seed data hỗ trợ các UI patterns sau:
  - DataGridView có vài dòng thật,
  - KPI cards có số liệu,
  - empty/error states có dữ liệu kiểm thử,
  - sales/inventory screens có đủ mẫu để demo fast entry.

**Acceptance criteria**

- `schema.sql` chạy được từ đầu đến cuối.
- `seed.sql` chạy sau `schema.sql` không lỗi.
- Dữ liệu demo đủ để form master data, inventory, sales, dashboard và assistant hiển thị meaningful content.
- Có ghi nhận rõ các gap còn lại nếu chưa đủ.

**Build/test commands**

```powershell
sqlcmd -S (localdb)\MSSQLLocalDB -i database\schema.sql
sqlcmd -S (localdb)\MSSQLLocalDB -i database\seed.sql
```

Nếu máy không có `sqlcmd`, dùng Visual Studio/SSMS để chạy theo đúng thứ tự.

**Suggested commit message**

- `docs: validate database demo readiness for remaining ui phases`

**Risks**

- Seed thiếu data làm dashboard/report trông rỗng.
- Quan hệ FK hoặc `IsActive` chưa thật sự phù hợp cho demo.
- Gaps schema/seed phát hiện muộn sẽ ảnh hưởng phase 4-7.

**Dependencies on Dũ/Hùng**

- Không bắt buộc chờ.
- Có thể phối hợp nếu cần họ xác nhận contract data quan trọng.

**Can Châu start immediately?**

- Có. Đây là phase đầu tiên sau roadmap.

---

## Phase 2 — `phase-2-winforms-ui-shell`

**OpenSpec change name**

- `phase-2-winforms-ui-shell`

**Purpose**

Dựng khung ứng dụng WinForms chính: main layout, sidebar navigation, top bar, screen host, luồng điều hướng cơ bản.

**Goal**

Có một shell app ổn định để các màn hình sau chỉ cần gắn vào host mà không phải đụng kiến trúc chính.

**Exact scope**

- Thiết kế `FrmMain` thành layout chính.
- Tạo sidebar điều hướng theo nhóm nghiệp vụ.
- Tạo top bar cho title, user, quick actions.
- Tạo vùng host để load từng form con hoặc user control.
- Chuẩn hóa luồng mở form từ menu.
- Chuẩn hóa trạng thái app khi chưa đăng nhập / đã đăng nhập.
- Chuẩn bị khung cho status strip hoặc message area.

**Out of scope**

- Không làm chi tiết từng module nghiệp vụ.
- Không bind dữ liệu thật.
- Không tối ưu theme sâu.
- Không viết DAL/BLL.

**Files/modules likely affected**

- `src/QuanLyKhoBanHang.WinForms/Forms/Main/FrmMain.cs`
- `src/QuanLyKhoBanHang.WinForms/Forms/Auth/FrmLogin.cs`
- `src/QuanLyKhoBanHang.WinForms/Forms/Common/PlaceholderForm.cs`
- `src/QuanLyKhoBanHang.WinForms/Program.cs`
- có thể thêm các helper host/navigation trong WinForms project

**Service contracts used**

- `AuthService`
- `UserDto` / `ServiceResult<T>` nếu login flow cần
- Các form khác chỉ theo contract navigation, chưa cần data thật

**Mock/stub strategy**

- Dùng stub login/user context nếu auth thật chưa hoàn tất.
- Dùng placeholder screens cho module chưa làm.
- Navigation menu có thể mở màn hình stub thay vì màn hình dữ liệu thật.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Sidebar 220-240 px.
- Top bar rõ title, user, quick actions.
- Main content full-width, dễ thay screen.
- Layout phải dùng `Dock`, `Anchor`, `Padding`, `TableLayoutPanel`/`Panel`.
- Không dùng absolute coordinates cho shell cuối.
- Phù hợp với app vận hành, không phải landing page.

**Acceptance criteria**

- Ứng dụng mở vào shell ổn định.
- Sidebar và top bar hiển thị đúng.
- Menu điều hướng mở được các screen placeholder.
- Không có WinForms nào gọi DAL trực tiếp.
- Shell resize ổn và không vỡ layout lớn.

**Build/test commands**

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

**Suggested commit message**

- `feat(ui): add winforms application shell and navigation host`

**Risks**

- Nếu shell quá cứng sẽ làm các screen sau khó tích hợp.
- Nếu chưa chuẩn hóa host, mỗi form sẽ tự layout riêng và gây nợ kỹ thuật.

**Dependencies on Dũ/Hùng**

- Không bắt buộc chờ.
- Chỉ cần contract auth đủ ổn để login flow không mơ hồ.

**Can Châu start immediately?**

- Sau phase 1, có thể bắt đầu ngay.

---

## Phase 3 — `phase-3-shared-ui-theme-components`

**OpenSpec change name**

- `phase-3-shared-ui-theme-components`

**Purpose**

Tạo nền UI dùng chung để mọi màn hình sau đồng nhất, nhanh build, ít sửa lại.

**Goal**

Có `AppTheme`, `UiFactory`, DataGridView styling và các component dùng lại được cho toàn app.

**Exact scope**

- Tạo theme chuẩn.
- Tạo factory/helper để dựng control nhất quán.
- Chuẩn hóa style `DataGridView`.
- Tạo KPI card reusable.
- Tạo toolbar/search panel reusable.
- Tạo empty/error state reusable.
- Tạo input group pattern cho form nhập liệu.
- Chuẩn hóa màu sắc, spacing, font, button height.

**Out of scope**

- Không làm logic nghiệp vụ.
- Không làm screen nghiệp vụ hoàn chỉnh.
- Không đổi DTO/service.

**Files/modules likely affected**

- `src/QuanLyKhoBanHang.WinForms/` shared UI helpers
- possibly `Forms/Common/` or `Controls/`
- `FrmDashboard` chỉ nếu cần test component, nhưng chưa làm dashboard thật

**Service contracts used**

- Không cần service thật cho phần lớn work.
- Chỉ giữ compatibility với DTO/ServiceResult đã chốt để helper không khóa contract.

**Mock/stub strategy**

- Component test bằng dữ liệu giả lập trong WinForms.
- Dùng dummy list để xem grid, KPI, empty state.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Business palette sạch, chuyên nghiệp.
- Font Segoe UI, 10-11 pt base.
- Screen title 16-20 pt bold.
- Grid full-row select, alternating row color, explicit columns.
- Empty states rõ ràng.
- Validation messages rõ, không chỉ dựa vào màu.

**Acceptance criteria**

- Có bộ helper dùng lại được cho nhiều form.
- Một màn hình demo dùng component chung không cần style riêng quá nhiều.
- UI đồng nhất về spacing, font, màu.
- Build solution thành công.

**Build/test commands**

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

**Suggested commit message**

- `feat(ui): introduce shared theme and reusable winforms components`

**Risks**

- Nếu shared components thiết kế không đủ linh hoạt, các màn hình sau sẽ bị lock cứng.
- Nếu theme quá nặng, form sẽ khó bảo trì.

**Dependencies on Dũ/Hùng**

- Không bắt buộc chờ.

**Can Châu start immediately?**

- Có, sau khi shell đã xong hoặc song song nếu shell contract đủ rõ.

---

## Phase 4 — `phase-4-master-data-ui`

**OpenSpec change name**

- `phase-4-master-data-ui`

**Purpose**

Làm UI cho sản phẩm, loại hàng, nhà cung cấp, khách hàng theo contract BLL ổn định.

**Goal**

Người dùng có thể xem, tìm kiếm, thêm, sửa, ngừng kích hoạt master data với trải nghiệm rõ ràng.

**Exact scope**

- `FrmProduct`
- `FrmCategory`
- `FrmSupplier`
- `FrmCustomer`
- Search/filter top row.
- Grid trung tâm.
- Edit panel hoặc modal-style input area.
- Actions: Add, Edit, Save, Cancel, Deactivate, Refresh.
- Validation UI cơ bản cho trường bắt buộc.

**Out of scope**

- Không thêm tính năng kho/bán hàng ở đây.
- Không làm dashboard/report.
- Không sửa contract nếu BLL chưa ổn trừ phase 0 gap thật.

**Files/modules likely affected**

- `src/QuanLyKhoBanHang.WinForms/Forms/MasterData/FrmProduct.cs`
- `src/QuanLyKhoBanHang.WinForms/Forms/MasterData/FrmCategory.cs`
- `src/QuanLyKhoBanHang.WinForms/Forms/MasterData/FrmSupplier.cs`
- `src/QuanLyKhoBanHang.WinForms/Forms/Sales/FrmCustomer.cs`
- shared UI helpers từ phase 3

**Service contracts used**

- `ProductService`
- `CategoryService`
- `SupplierService`
- `CustomerService`

**Mock/stub strategy**

- Nếu service thật chưa hoàn tất, dùng stub list CRUD in-memory để UI và navigation hoàn thiện.
- Stub phải giữ đúng shape `ServiceResult<T>`.
- Không fake business logic phức tạp; chỉ demo safe CRUD flow.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Search/filter row phía trên.
- Grid dùng `BindingSource`.
- Dòng action không nhét button vào từng row trừ khi thật cần.
- Label tiếng Việt rõ ràng.
- Trạng thái active/inactive hiển thị dễ hiểu.
- Form nhập liệu không được chật, không clip text tiếng Việt.

**Acceptance criteria**

- Bốn màn hình master data mở được từ shell.
- Search, add, edit, deactivate, refresh hoạt động ở mức UI + stub/real service sẵn có.
- Empty/error state hiển thị đúng.
- Không có DAL reference trong WinForms.

**Build/test commands**

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

**Suggested commit message**

- `feat(ui): implement master data screens for products categories suppliers and customers`

**Risks**

- Nếu DTO master data đổi muộn thì form sẽ phải chỉnh nhiều.
- Nếu search criteria contract chưa đủ, UI lọc sẽ phải tái cấu trúc.

**Dependencies on Dũ/Hùng**

- Phụ thuộc mạnh vào contract ổn định của Dũ/Hùng.
- Có thể bắt đầu bằng stub, nhưng tích hợp thật tốt nhất khi Dũ/Hùng đã ổn contract.

**Can Châu start immediately?**

- Có thể bắt đầu với stub ngay; tích hợp thật nên chờ service contracts ổn định.

---

## Phase 5 — `phase-5-inventory-ui`

**OpenSpec change name**

- `phase-5-inventory-ui`

**Purpose**

Làm UI cho nhập kho, tồn kho hiện tại, hàng sắp hết, và kiểm kê.

**Goal**

Cung cấp luồng thao tác nhập kho nhanh, dễ nhập liệu, dễ xem tồn và kiểm kê.

**Exact scope**

- `FrmPurchaseReceipt`
- `FrmInventory`
- `FrmStocktake`
- Product search.
- Line item grid.
- Quantity input.
- Summary/total panel.
- Low-stock view.
- Stocktake line comparison UI.
- Validation display rõ.

**Out of scope**

- Không làm sales invoice.
- Không làm báo cáo doanh thu.
- Không thêm rule mới ngoài contract đã chốt.

**Files/modules likely affected**

- `src/QuanLyKhoBanHang.WinForms/Forms/Inventory/FrmPurchaseReceipt.cs`
- `src/QuanLyKhoBanHang.WinForms/Forms/Inventory/FrmInventory.cs`
- `src/QuanLyKhoBanHang.WinForms/Forms/Inventory/FrmStocktake.cs`
- shared component từ phase 3

**Service contracts used**

- `PurchaseService`
- `InventoryService`
- `StocktakeService`
- `ProductService` để tìm sản phẩm

**Mock/stub strategy**

- Nếu backend chưa xong, dùng mock lines và mock transaction results.
- Demo nhập kho phải có thể thực hiện mà không cần DAL thật.
- Low-stock/current-stock screen có thể đọc seed/stub data.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Fast entry workflow.
- Product search dễ dùng.
- Grid line item rõ ràng.
- Total/counter summary dễ nhìn.
- Cảnh báo validation ngắn gọn, không phá layout.

**Acceptance criteria**

- Ba màn hình inventory mở được từ shell.
- Purchase receipt form hỗ trợ thêm/xóa dòng và hiển thị validation.
- Current stock và low-stock hiển thị được dữ liệu.
- Stocktake form hiển thị chênh lệch rõ.

**Build/test commands**

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

**Suggested commit message**

- `feat(ui): add inventory workflow screens for receipt stock and stocktake`

**Risks**

- Nếu line-item UX thiết kế kém sẽ làm nhập kho chậm.
- Nếu contract `StocktakeDto` hoặc receipt dto thiếu field, phải tách follow-up contract change.

**Dependencies on Dũ/Hùng**

- Dũ phụ trách backend kho, nên UI này có thể bắt đầu bằng stub nhưng cần hợp tác khi integration.

**Can Châu start immediately?**

- Có thể start với stub, nhưng integration thật phụ thuộc Dũ.

---

## Phase 6 — `phase-6-sales-ui`

**OpenSpec change name**

- `phase-6-sales-ui`

**Purpose**

Làm UI bán hàng/hóa đơn với tìm sản phẩm, invoice lines, totals và validation hiển thị rõ.

**Goal**

Cho phép tạo hóa đơn nhanh, xem dòng hàng rõ, kiểm tra tồn và tổng tiền dễ hiểu.

**Exact scope**

- `FrmSalesInvoice`
- Product search/select.
- Add/remove invoice lines.
- Quantity and price editing.
- Customer selection.
- Discount / total / final total display.
- Validation messages cho thiếu tồn, thiếu dòng, số lượng âm, giảm giá sai.

**Out of scope**

- Không làm báo cáo dashboard.
- Không làm assistant.
- Không làm in ấn nâng cao nếu contract chưa chốt.

**Files/modules likely affected**

- `src/QuanLyKhoBanHang.WinForms/Forms/Sales/FrmSalesInvoice.cs`
- shared components từ phase 3
- nếu cần: customer selector UI reuse từ master data

**Service contracts used**

- `SalesService`
- `CustomerService`
- `ProductService`

**Mock/stub strategy**

- Dùng stub product/customer list nếu service thật chưa có.
- Hóa đơn demo cần tạo được trong UI flow mà không phụ thuộc DAL thật.
- Validation UI tự phản hồi trước khi gọi service.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Tối ưu tốc độ nhập liệu.
- Grid chi tiết hóa đơn rõ ràng.
- Tổng tiền, giảm giá, cuối cùng phải nổi bật.
- Cảnh báo tồn thấp/không đủ tồn phải rõ.
- Khách hàng và sản phẩm phải tìm nhanh được.

**Acceptance criteria**

- Màn hình bán hàng dùng được với stub hoặc service thật.
- Add/remove line hoạt động.
- Validation cho invoice lines hiển thị đúng.
- Tổng tiền cập nhật theo dữ liệu nhập.

**Build/test commands**

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

**Suggested commit message**

- `feat(ui): implement sales invoice workflow and validation`

**Risks**

- Sales UI rất dễ bị vỡ nếu contract line item hoặc customer lookup thay đổi.
- Nếu không có stub tốt, Châu sẽ bị block chờ Hùng.

**Dependencies on Dũ/Hùng**

- Phụ thuộc mạnh Hùng cho sales/customer/report contracts.
- Nên bắt đầu bằng stub để tránh chờ backend.

**Can Châu start immediately?**

- Có thể bắt đầu bằng stub, nhưng tích hợp thật phụ thuộc Hùng.

---

## Phase 7 — `phase-7-dashboard-reports-ui`

**OpenSpec change name**

- `phase-7-dashboard-reports-ui`

**Purpose**

Làm dashboard và báo cáo vận hành.

**Goal**

Dashboard phải đọc trong 10 giây: KPI cards, top products, low-stock, recent activity. Reports screen phải có bộ lọc ngày và bảng tổng hợp.

**Exact scope**

- `FrmDashboard`
- `FrmReport`
- KPI cards: revenue today, revenue this month, invoice count today, low-stock count.
- Tables: top products, low-stock products, recent activity.
- Reports screen: date range, refresh, summary grid, top products/customers.
- Placeholder export nếu chưa có thật.

**Out of scope**

- Không thêm chart phức tạp nếu chưa cần.
- Không xây logic AI.
- Không sửa contract lớn nếu chỉ là presentation layer.

**Files/modules likely affected**

- `src/QuanLyKhoBanHang.WinForms/Forms/Dashboard/FrmDashboard.cs`
- `src/QuanLyKhoBanHang.WinForms/Forms/Reports/FrmReport.cs`
- shared KPI/grid/empty states from phase 3

**Service contracts used**

- `ReportService`
- `InventoryService`
- possibly `DashboardService` nếu Phase 0 contract đã có và ổn

**Mock/stub strategy**

- Dùng stub summaries nếu report backend chưa merge.
- Seed data và cached demo data phải đủ để dashboard không rỗng.
- Nếu `DashboardService` tồn tại nhưng chưa dùng, chỉ tích hợp khi output contract rõ.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Dashboard phải quét nhanh trong 10 giây.
- KPI cards trên cùng.
- Tables gọn, dễ đọc.
- Không overbuild chart nếu không cần.
- Reports lọc ngày rõ ràng, refresh rõ ràng.

**Acceptance criteria**

- Dashboard hiển thị meaningful data từ stub/real service.
- Reports screen có date range filter và summary display.
- Empty state và loading/error state hoạt động.

**Build/test commands**

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

**Suggested commit message**

- `feat(ui): add dashboard and reporting screens`

**Risks**

- Nếu summary DTO thiếu field, dashboard sẽ phải điều chỉnh.
- KPI/summary dễ bị lệch với backend nếu contract không rõ.

**Dependencies on Dũ/Hùng**

- Phụ thuộc Hùng cho report contracts.
- Một phần dashboard có thể dùng seed/stub trước.

**Can Châu start immediately?**

- Có thể start phần shell/UI stub trước.

---

## Phase 8 — `phase-8-assistant-ui`

**OpenSpec change name**

- `phase-8-assistant-ui`

**Purpose**

Làm UI trợ lý quản lý/chat cho các command rule-based và fallback/stub behavior.

**Goal**

Người dùng nhập câu hỏi/ngữ cảnh và nhận phản hồi có ích, deterministic, demo tốt.

**Exact scope**

- `FrmAssistant`
- input box
- suggested command buttons
- conversation/result area
- fallback/stub response if real assistant logic not ready
- link to dashboard/report style answers if available

**Out of scope**

- Không gọi AI API thật nếu chưa có chủ trương.
- Không làm OCR/voice.
- Không đổi public contract nếu chỉ cần stub UI.

**Files/modules likely affected**

- `src/QuanLyKhoBanHang.WinForms/Forms/Assistant/FrmAssistant.cs`
- assistant DTO/service contract if follow-up gap appears

**Service contracts used**

- `AssistantService`
- maybe `ReportService` / `InventoryService` as backing sources for deterministic answers

**Mock/stub strategy**

- Rule-based or canned stub responses phải hoạt động ngay.
- Suggested commands:
  - doanh thu hôm nay
  - hàng sắp hết
  - top sản phẩm bán chạy
  - khách hàng mua nhiều nhất
- Fallback message phải lịch sự và rõ ràng.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Trông như command panel cho quản lý.
- Có gợi ý command sẵn.
- Conversation area dễ đọc.
- Response area không bị ngập thông tin.

**Acceptance criteria**

- Assistant screen có thể trả lời bằng stub mà không chờ AI thật.
- Suggested command buttons hoạt động.
- Fallback behavior rõ ràng.

**Build/test commands**

```powershell
dotnet build .\QuanLyKhoBanHang.sln
```

**Suggested commit message**

- `feat(ui): add manager assistant chat screen with stub responses`

**Risks**

- Assistant contract mơ hồ dễ làm UI demo thiếu ổn định.
- Nếu command set đổi muộn, UI phải chỉnh gợi ý nhiều.

**Dependencies on Dũ/Hùng**

- Hùng cho command liên quan doanh thu/top sản phẩm.
- Có thể start stub ngay.

**Can Châu start immediately?**

- Có, nếu dùng stub behavior.

---

## Phase 9 — `phase-9-integration-demo-polish`

**OpenSpec change name**

- `phase-9-integration-demo-polish`

**Purpose**

Tích hợp service thật từ Dũ/Hùng, polish UX, chuẩn bị demo cuối, build/test toàn bộ.

**Goal**

Chuyển từ stub sang real services, dọn trải nghiệm UI, chốt kịch bản demo, đảm bảo build/test ổn định.

**Exact scope**

- Thay stub/mock bằng service thật khi contract đã sẵn sàng.
- Rà lại toàn bộ navigation và form flows.
- Fix inconsistency UI/validation/empty state.
- Chuẩn hóa loading/error/empty states.
- Kiểm tra demo checklist cuối.
- Tối ưu những màn hình có vấn đề về spacing, resize, focus, keyboard flow.
- Đảm bảo không form nào gọi DAL trực tiếp.
- Chuẩn bị bản build/test cuối.

**Out of scope**

- Không mở rộng scope nghiệp vụ mới lớn.
- Không thêm feature chưa có trong roadmap.
- Không đổi contract trừ khi là bug hợp đồng cuối cùng.

**Files/modules likely affected**

- Toàn bộ `src/QuanLyKhoBanHang.WinForms/`
- có thể chạm `src/QuanLyKhoBanHang.BLL/Services/` chỉ khi cần follow-up contract nhỏ
- có thể chạm `docs/06_ChecklistDemo.md` nếu cần bổ sung kịch bản demo

**Service contracts used**

- Tất cả service đã chốt:
  - `ProductService`
  - `CategoryService`
  - `SupplierService`
  - `CustomerService`
  - `PurchaseService`
  - `InventoryService`
  - `StocktakeService`
  - `SalesService`
  - `ReportService`
  - `AssistantService`
  - `AuthService`

**Mock/stub strategy**

- Stub được giữ lại làm fallback nhưng ưu tiên real service.
- Khi real service chưa sẵn sàng, giữ tạm stub để không chặn demo.

**UI/UX requirements from winforms-inventory-sales-ui skill**

- Rà lại layout shell, grid, button sizes, titles, labels.
- Đảm bảo Vietnamese labels không bị cắt.
- Đảm bảo forms resize tốt.
- Đảm bảo data grids, filters, action rows đồng nhất.

**Acceptance criteria**

- UI chạy với real services ở những phần đã merge.
- Stub vẫn hoạt động cho phần backend chưa xong.
- Build solution thành công.
- Demo checklist đầy đủ.
- Final PR rõ ràng, review được.

**Build/test commands**

```powershell
dotnet build .\QuanLyKhoBanHang.sln
dotnet test .\tests\QuanLyKhoBanHang.Tests\QuanLyKhoBanHang.Tests.csproj
```

**Suggested commit message**

- `refactor(ui): integrate real services and polish demo readiness`

**Risks**

- Đây là phase dễ sinh bug integration nhất.
- Nếu contract drift từ Dũ/Hùng chưa khớp, cần follow-up nhỏ, không dồn vào một PR lớn.

**Dependencies on Dũ/Hùng**

- Có.
- Đây là phase tích hợp thực sự, nên chỉ làm mạnh khi các service cần dùng đã merge ổn.

**Can Châu start immediately?**

- Chỉ phần chuẩn bị polish/stub cleanup; tích hợp thật nên chờ service ổn định.

---

## 3. Recommended execution order

### Recommended order

1. Phase 1 — database demo readiness
2. Phase 2 — WinForms UI shell
3. Phase 3 — shared UI theme/components
4. Phase 4 — master data UI
5. Phase 5 — inventory UI
6. Phase 6 — sales UI
7. Phase 7 — dashboard/reports UI
8. Phase 8 — assistant UI
9. Phase 9 — integration demo polish

### Why this order works

- Phase 1 locks the demo data foundation.
- Phase 2 gives a stable app shell.
- Phase 3 prevents duplicated UI work.
- Phases 4-8 are feature slices that can be built and committed independently.
- Phase 9 is the only place where large-scale integration/polish happens.

### Which phase Châu should start right after this roadmap

- Start with **Phase 1 — `phase-1-database-demo-readiness`**.
- Reason: it is the least risky way to confirm the demo foundation before touching more UI structure.

---

## 4. When to create `/opsx:propose`

Use `/opsx:propose` when:

- scope is large enough to need a new OpenSpec change,
- the change impacts more than one form/service area,
- the task could break contract or architecture if done casually,
- you want a proposal/design/tasks pack before coding,
- you are about to start a new phase from this roadmap.

Do **not** skip to coding when:

- the work touches several screens,
- there is uncertainty about contract gaps,
- stub behavior needs to be defined,
- Dũ/Hùng may still change backend contract details.

Rule of thumb:

- One phase = one OpenSpec change candidate.
- One small bugfix or local cleanup may not need a full new proposal.
- Anything affecting multiple screens or shared components should go through `/opsx:propose`.

---

## 5. When to use GPT-5.5 versus Claude Opus

### Use GPT-5.5 when

- you need fast planning,
- the task is mostly organization, analysis, or implementation drafting,
- you are breaking work into phases or tasks,
- you are reconciling docs, contracts, and roadmap details,
- you want balanced speed and quality for routine OpenSpec work.

### Use Claude Opus when

- you need deep reasoning on architecture trade-offs,
- you are designing tricky UI/UX flow or interaction complexity,
- you need careful refactoring of a broad screen shell,
- you need strong long-context analysis across many files,
- you are reviewing complex integration risk near phase 9.

### Practical rule for this project

- Default to GPT-5.5 for proposing/organizing each phase.
- Escalate to Claude Opus for complicated integration, layout decisions, or anything where a wrong architecture choice would be expensive to undo.
- Do not use model choice as a substitute for SDD. The OpenSpec change still needs proposal/design/tasks.

---

## 6. Checklist before each commit

- [ ] Read the relevant OpenSpec change and tasks.
- [ ] Confirm the change is small and scoped to one phase.
- [ ] Verify no accidental contract drift was introduced.
- [ ] Ensure WinForms still does not reference DAL directly.
- [ ] Ensure no SQL appears in WinForms.
- [ ] Check the solution builds.
- [ ] Check tests that are relevant to the change.
- [ ] Confirm only intended files are modified.
- [ ] Update OpenSpec tasks if the phase requires it.
- [ ] Write a commit message that explains why the change exists.

### Suggested local build/test baseline

```powershell
dotnet build .\QuanLyKhoBanHang.sln
dotnet test .\tests\QuanLyKhoBanHang.Tests\QuanLyKhoBanHang.Tests.csproj
```

If the phase does not touch test-covered logic, still run at least `dotnet build`.

---

## 7. Checklist before opening the final PR

- [ ] All planned phases in scope for the current PR are complete.
- [ ] No contract changes are undocumented.
- [ ] No stub/mock code remains where real service integration is required for the demo.
- [ ] No WinForms form calls DAL directly.
- [ ] No SQL appears in WinForms code.
- [ ] UI screens resize and remain readable.
- [ ] Empty/error states are present.
- [ ] Build passes.
- [ ] Relevant tests pass.
- [ ] Demo checklist is updated.
- [ ] PR description lists changed services, DTOs, and any schema impacts.
- [ ] Screenshots or demo notes are ready if needed.
- [ ] The PR stays reviewable; no giant all-in-one change.

---

## 8. Phase gap handling rule

If, during any phase, Châu finds that DTO or service contract is missing something essential for the UI:

1. stop implementation of that slice,
2. write the gap clearly,
3. decide whether it is a tiny follow-up contract fix or a new OpenSpec change,
4. never silently patch the UI around a broken contract in a way that hides the issue,
5. keep the follow-up change small and separately reviewable.

This is the main guardrail against vibe coding.

---

## 9. Final recommendation

The safest path is:

- keep Phase 1 strictly about demo-readiness confirmation,
- build shell and shared components before touching detailed screens,
- use stub services aggressively until backend is stable,
- keep each OpenSpec change phase-sized and mergeable,
- reserve real integration work for Phase 9.

That will let Châu keep moving without blocking Dũ or Hùng, while preserving SDD discipline.
