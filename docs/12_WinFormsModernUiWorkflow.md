# WinForms Modern UI Workflow

## Mục Tiêu

Tài liệu này dùng để làm mới giao diện WinForms trong `QuanLyKhoBanHang` theo một hệ thống chung, thay vì thiết kế từng form riêng lẻ. Mục tiêu là các màn sau nhìn đồng bộ, dễ demo, dễ code tiếp và không làm vỡ kiến trúc 3 lớp.

## Nguyên Tắc Chung

- `FrmMain` là nơi duy nhất sở hữu sidebar, header shell, content host và status bar.
- Mỗi màn chức năng chỉ thiết kế phần nội dung bên trong content host, không tự tạo sidebar riêng.
- WinForms chỉ gọi service ở BLL, không gọi DAL, không chứa SQL.
- Ưu tiên control dùng lại trong `Forms/Common`: `AppTheme`, `UiFactory`, `RoundedPanel`.
- Icon dùng `FontAwesome.Sharp`, không dùng icon PNG sinh bằng AI trừ khi thật sự là ảnh minh họa nghiệp vụ.
- Giao diện ưu tiên sáng, sạch, chuyên nghiệp: trắng/xám nhạt, xanh dương cho primary, xanh lá cho doanh thu/tích cực, cam cho hóa đơn/nghiệp vụ, đỏ cho cảnh báo.
- Không dùng layout kiểu landing page, mobile app, glassmorphism, 3D hoặc hiệu ứng khó dựng bằng WinForms.

## Component Chuẩn

- `AppTheme`: màu, font, spacing chuẩn.
- `RoundedPanel`: card/panel bo góc, viền nhẹ, bóng rất nhẹ.
- `UiFactory.Card()`: card trắng dùng cho khu dữ liệu.
- `UiFactory.MetricCard(...)`: KPI card có icon tile và accent bar.
- `UiFactory.SectionHeader(...)`: tiêu đề section có icon, title, subtitle.
- `UiFactory.StyleGrid(...)`: chuẩn hóa `DataGridView`.
- `UiFactory.SidebarButton(...)`: nút sidebar có icon và selected state.

## Header Nội Dung Trong Màn Con

- `FrmMain` đã có header shell hiển thị tên màn đang mở, vì vậy header đầu tiên bên trong từng form không được lặp lại y chang tên màn.
- Với các màn CRUD/list, dùng `UiFactory.SectionHeader(...)` cho header nội dung theo kiểu Dashboard: icon xanh bên trái, title xanh ngắn gọn, subtitle mô tả nghiệp vụ ở dòng dưới.
- Title header nội dung nên là tên khu vực/chức năng cụ thể, ví dụ `Danh mục hàng hóa`, `Nhóm hàng và phân loại`, `Đối tác cung ứng`, `Hồ sơ khách hàng`, thay vì lặp `Sản phẩm`, `Loại hàng`, `Nhà cung cấp`, `Khách hàng`.
- Summary/count đặt bên phải header, canh giữa theo chiều dọc, dùng `AppTheme.SectionFont(10.5F)`, `AppTheme.Primary`, `AutoEllipsis = true`, và đủ rộng để không bị wrap hoặc chìm xuống mép dưới.
- Header nội dung là phần layout phẳng trong content area, không tạo card lớn riêng nếu shell/header ngoài đã có khoảng phân tách rõ.

## Quy Trình Làm Một Màn

1. Đọc `AGENTS.md`, docs liên quan và OpenSpec change nếu màn thuộc một phase lớn.
2. Kiểm tra form hiện tại và service contract đang dùng.
3. Không sửa DTO/public service nếu chỉ làm UI.
4. Nếu màn chỉ là CRUD/bảng/lọc/form nhập, dựng trực tiếp theo component chuẩn.
5. Nếu màn có workflow phức tạp, xin mockup GPT Image 2 trước rồi chuyển thành WinForms-friendly layout.
6. Không thêm sidebar vào mockup từng màn. Sidebar chỉ thiết kế một lần ở `FrmMain`.
7. Sau khi code: chạy `dotnet build QuanLyKhoBanHang.sln`.
8. Nếu đã build rồi: chạy `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`.
9. Dùng chế độ QA phù hợp bên dưới trước khi bàn giao.

## Chế Độ QA Giao Diện

### Chế độ tiết kiệm token

Dùng mặc định khi Châu muốn làm nhanh và sẽ tự gửi ảnh lỗi nếu có:

- Không tự mở app để chụp/xem màn hình.
- Vẫn phải đọc layout/code cẩn thận và chạy build/test.
- Nếu nghi ngờ lỗi giao diện khó chắc chắn bằng code, báo rõ để Châu gửi ảnh màn hình.
- Khi bàn giao, ghi rõ: đã build/test, chưa mở app theo chế độ tiết kiệm token.

### Chế độ mở app kiểm tra đầy đủ

Chỉ dùng khi Châu yêu cầu rõ là cần Codex/agent mở app kiểm tra trực quan:

- Chạy app hoặc mở form liên quan trong app.
- Khi cần đăng nhập để review UI, dùng tài khoản demo mặc định: username `admin`, password `admin123`.
- Nếu màn đăng nhập đang bật nhớ đăng nhập và mật khẩu, lần review sau chỉ cần bấm nút đăng nhập.
- Kiểm tra sidebar, header, màn hình nội dung và status bar.
- Kiểm tra kích thước app maximized và kích thước tối thiểu nếu form có `MinimumSize`.
- Đảm bảo icon không bị phóng quá cỡ hoặc cắt mép.
- Đảm bảo chữ tiếng Việt không bị mất dấu, hụt dòng, clip trong panel/button/card.
- Đảm bảo bảng `DataGridView` có header đọc được, row không quá chật và selected row rõ.
- Nếu app đang chạy làm khóa file `.exe`, đóng đúng process WinForms rồi build lại.

## Khi Nào Cần GPT Image 2 Mockup

Không cần mockup cho các màn CRUD hoặc bảng đơn giản:

- `Sản phẩm`
- `Loại hàng`
- `Nhà cung cấp`
- `Khách hàng`
- `Tài khoản`
- `Phân quyền`
- `Nhật ký hệ thống`

Nên có mockup cho các màn có luồng thao tác phức tạp hoặc cần bố cục đặc biệt:

- `Bán hàng`: chọn khách, tìm sản phẩm, dòng hóa đơn, tổng tiền, cảnh báo tồn.
- `Nhập kho`: chọn nhà cung cấp, thêm dòng hàng, tổng số lượng/giá trị, xác nhận phiếu.
- `Kiểm kê`: so sánh tồn hệ thống/thực tế, chênh lệch, ghi chú, trạng thái.
- `Báo cáo`: bộ lọc ngày, summary, bảng top, có thể thêm chart nhẹ.
- `Trợ lý AI`: vùng chat, gợi ý câu hỏi, trạng thái online/offline, kết quả nghiệp vụ.

Có thể cần mockup nếu muốn polish mạnh:

- `Tồn kho`: nếu chỉ là bảng thì không cần; nếu muốn dashboard tồn kho có cảnh báo, filter và summary card thì nên có mockup.
- `Sidebar`: chỉ mockup một lần cho toàn app, không lặp theo từng màn.

## Prompt GPT Image 2 Chuẩn

Khi cần mockup, dùng yêu cầu tiếng Anh theo khung sau:

```text
Design only the content area for a C# WinForms desktop inventory and sales management application named QuanLyKhoBanHang.

Strict constraints:
- Do not add a sidebar.
- Do not add a left navigation menu.
- Do not redesign the whole application shell.
- The screen must be practical to implement with WinForms controls: Panel, Label, Button, ComboBox, TextBox, DateTimePicker, DataGridView, TableLayoutPanel, and custom UserControls.
- Use a modern light desktop business software style.
- Keep Vietnamese labels readable.
- Use subtle rounded corners, light borders, and minimal shadows.
- Use FontAwesome-style line icons only; do not generate separate decorative icon art.
- If a background is used, also provide a background-only image with no text, no cards, no tables, no icons.

Screen content:
<describe the specific screen fields, buttons, grids, KPI cards, and workflow here>

After generating the mockup, briefly explain whether it is suitable for WinForms implementation and which parts may need simplification.
```

## Icon Mapping Gợi Ý

- Dashboard: `House`
- Sản phẩm: `BoxOpen`
- Loại hàng: `Tags`
- Nhà cung cấp: `Truck`
- Khách hàng: `Users`
- Nhập kho: `TruckRampBox`
- Tồn kho: `Warehouse`
- Kiểm kê: `ClipboardCheck`
- Bán hàng: `CartShopping`
- Báo cáo: `ChartBar`
- Trợ lý AI: `Robot`
- Tài khoản: `UserGear`
- Phân quyền: `ShieldHalved`
- Nhật ký hệ thống: `ClockRotateLeft`

## Lưu Ý Demo

- Giao diện phải đọc được trong 10 giây khi thuyết trình.
- Bảng cần có header rõ, dòng xen kẽ nhẹ, selected row dễ nhìn.
- Nút thao tác chính đặt ở toolbar hoặc panel nhập liệu, không nhét quá nhiều nút vào từng dòng.
- Empty/error state cần rõ ràng nhưng không phá layout.
- Nếu app đang chạy làm khóa file `.exe`, đóng app rồi build lại.
