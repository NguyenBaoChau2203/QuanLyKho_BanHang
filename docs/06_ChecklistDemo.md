# Checklist demo

## Trước khi demo

- Build solution thành công.
- Database đã chạy `schema.sql` và `seed.sql`.
- App đăng nhập được bằng `admin/admin123`.
- Có dữ liệu sản phẩm, khách hàng, nhà cung cấp.
- Không còn exception khi mở từng menu chính.

## Luồng demo

- Đăng nhập.
- Xem dashboard.
- Mở danh sách sản phẩm và kiểm tra tồn kho.
- Tạo phiếu nhập kho, tồn kho tăng.
- Tạo hóa đơn bán hàng, tồn kho giảm.
- Thử bán quá tồn và app báo lỗi.
- Xem báo cáo doanh thu.
- Hỏi trợ lý: `doanh thu hôm nay`.
- Hỏi trợ lý: `hàng sắp hết`.
- Hỏi trợ lý: `top sản phẩm bán chạy`.

## Nội dung cần nói khi thuyết trình

- Ứng dụng dùng mô hình 3 lớp.
- UI không gọi database trực tiếp.
- DAL dùng ADO.NET và transaction cho nghiệp vụ quan trọng.
- OpenSpec giúp nhóm thống nhất yêu cầu trước khi code.
- Dashboard và trợ lý quản lý là điểm nâng cấp so với CRUD thông thường.
