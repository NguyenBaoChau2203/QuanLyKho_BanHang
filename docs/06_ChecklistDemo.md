# Checklist demo

## Trước khi demo

- Build solution thành công.
- Database đã chạy `schema.sql` và `seed.sql`.
- App đăng nhập được bằng `admin/admin123`.
- App đăng nhập được bằng `manager/123456`, `du/123456`, `hung/123456`.
- Có dữ liệu sản phẩm, khách hàng, nhà cung cấp.
- Không còn exception khi mở từng menu chính.

## Luồng demo

- Đăng nhập bằng `admin/admin123`.
- Xem dashboard để kiểm tra KPI, sản phẩm bán chạy và tồn thấp.
- Mở danh mục sản phẩm, loại hàng, nhà cung cấp và khách hàng để kiểm tra layout và tìm kiếm.
- Tạo phiếu nhập kho với dữ liệu demo.
- Mở tồn kho và kiểm kê để xem bảng và trạng thái cảnh báo.
- Tạo hóa đơn bán hàng, kiểm tra chọn khách hàng và dòng hàng.
- Mở báo cáo doanh thu theo khoảng ngày.
- Hỏi trợ lý: `doanh thu hôm nay`.
- Hỏi trợ lý: `hàng sắp hết`.
- Hỏi trợ lý: `top sản phẩm bán chạy`.
- Hỏi trợ lý: `khách hàng mua nhiều nhất`.
- Hỏi trợ lý: `kiểm kê hôm nay`.
- Kiểm tra Admin section với `admin/admin123`: `Tài khoản`, `Phân quyền`, `Nhật ký hệ thống`.
- Kiểm tra role menu nhanh: Manager không thấy Admin-only; WarehouseStaff không thấy bán hàng/báo cáo/Admin-only; SalesStaff không thấy nhập kho/kiểm kê/báo cáo/Admin-only.
- Nêu rõ các màn hình vẫn dùng dữ liệu stub an toàn nếu backend chưa trả đủ dữ liệu, nhưng vẫn giữ trải nghiệm demo nhất quán.

## Nội dung cần nói khi thuyết trình

- Ứng dụng dùng mô hình 3 lớp.
- UI không gọi database trực tiếp.
- DAL/repository thật sẽ do Dũ và Hùng hoàn thiện theo contract chung; hiện phần UI Châu đang dùng BLL và stub/fallback demo an toàn ở các chỗ backend chưa sẵn sàng.
- OpenSpec giúp nhóm thống nhất yêu cầu trước khi code.
- Dashboard và trợ lý quản lý là điểm nâng cấp so với CRUD thông thường.
