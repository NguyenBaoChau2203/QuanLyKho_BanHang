# AGENTS.md

## Mục tiêu dự án

Xây dựng ứng dụng WinForms quản lý kho và bán hàng có thể demo tốt cho đồ án cuối kỳ và có nền tảng đủ sạch để dùng cơ bản trong thực tế.

## Kiến trúc bắt buộc

- `QuanLyKhoBanHang.WinForms` chỉ gọi service ở tầng BLL.
- `QuanLyKhoBanHang.BLL` chứa validation, nghiệp vụ và orchestration.
- `QuanLyKhoBanHang.DAL` chứa ADO.NET, SQL query, mapping và transaction.
- `QuanLyKhoBanHang.DTO` chỉ chứa model truyền dữ liệu.

## Quy định khi AI hoặc thành viên nhóm làm việc

- Đọc `docs/` trước khi code.
- Đọc `openspec/changes/bootstrap-inventory-sales-mvp/` để nắm scope MVP.
- Không sửa UI chính nếu không phải Châu hoặc chưa thống nhất với Châu.
- Không push trực tiếp lên `main`.
- Không nối chuỗi SQL từ input người dùng; luôn dùng parameter khi triển khai DAL thật.
- Khi đổi public method, DTO hoặc kiểu trả về, phải cập nhật PR description và docs liên quan.

## Service contract mặc định

Các service public nên trả về `ServiceResult<T>` với:

- `Success`
- `Message`
- `Data`

Tên method cần rõ nghĩa, ví dụ:

- `ProductService.GetAllProducts()`
- `PurchaseService.CreateReceipt(receipt)`
- `SalesService.CreateInvoice(invoice)`
- `ReportService.GetRevenue(fromDate, toDate)`

## OpenSpec khi dùng Codex

- Khi tạo, review hoặc implement tính năng lớn, dùng Codex skill `openspec-sdd`.
- Prompt nên bắt đầu bằng: `Use the openspec-sdd skill.`
- Không code tính năng lớn trước khi có OpenSpec change hợp lệ.
- Validate bằng `openspec validate <change-name>` hoặc `npx --yes --package @fission-ai/openspec openspec validate <change-name>`.
- Xem chi tiết tại `docs/09_CodexOpenSpecWorkflow.md`.
