# AGENTS.md

## Mục Tiêu Dự Án

Xây dựng ứng dụng WinForms quản lý kho và bán hàng có thể demo tốt cho đồ án cuối kỳ, áp dụng mô hình 3 lớp và có nền tảng đủ sạch để mở rộng dùng cơ bản trong thực tế.

## Kiến Trúc Bắt Buộc

- `QuanLyKhoBanHang.WinForms` chỉ gọi service ở tầng BLL.
- `QuanLyKhoBanHang.BLL` chứa validation, nghiệp vụ và orchestration.
- `QuanLyKhoBanHang.DAL` chứa ADO.NET, SQL query, mapping và transaction.
- `QuanLyKhoBanHang.DTO` chỉ chứa model truyền dữ liệu, không chứa logic nghiệp vụ.

## Quy Định Khi AI Hoặc Thành Viên Nhóm Làm Việc

- Đọc `docs/` trước khi code.
- Đọc OpenSpec change liên quan trong `openspec/changes/` trước khi sửa tính năng lớn.
- Không sửa UI chính nếu không phải Châu hoặc chưa thống nhất với Châu.
- Không push trực tiếp lên `main`.
- Không nối chuỗi SQL từ input người dùng; luôn dùng parameter khi triển khai DAL thật.
- Khi đổi public method, DTO hoặc kiểu trả về, phải cập nhật OpenSpec/docs và mô tả rõ trong PR/commit.
- Không hard-code API key hoặc secret vào source code.

## Service Contract Mặc Định

Các service public nên trả về `ServiceResult<T>` với:

- `Success`
- `Message`
- `Data`

Tên method cần rõ nghĩa, ví dụ:

- `ProductService.GetAllProducts()`
- `PurchaseService.CreateReceipt(receipt)`
- `SalesService.CreateInvoice(invoice)`
- `ReportService.GetRevenue(fromDate, toDate)`

## OpenSpec / SDD Bắt Buộc

- Tính năng lớn phải có OpenSpec change trước khi code.
- Mỗi change nên có:
  - `proposal.md`
  - `design.md`
  - `tasks.md`
  - `specs/<capability>/spec.md`
- Validate bằng:

```powershell
npx --yes --package @fission-ai/openspec openspec validate <change-name>
```

- Không implement ngoài scope đã mô tả trong OpenSpec.
- Mark task complete chỉ sau khi validate/build/test pass.

## Tool-Specific Workflow

- Codex: xem `docs/09_CodexOpenSpecWorkflow.md`.
- Antigravity: xem `GEMINI.md` và `docs/10_AntigravityOpenSpecWorkflow.md`.
- Cursor: có thể tham khảo `.cursor/commands/` và `.cursor/skills/`.

## Build/Test Chuẩn

```powershell
dotnet build QuanLyKhoBanHang.sln
dotnet test QuanLyKhoBanHang.sln --no-build --no-restore
```

Nếu build WinForms bị lỗi file `.exe` đang bị khóa, đóng app đang chạy rồi build lại.
