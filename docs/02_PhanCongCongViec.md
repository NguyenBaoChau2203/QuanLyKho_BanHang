# Phân công công việc

## Châu

Vai trò: nhóm trưởng, kiến trúc, OpenSpec, database tổng thể, WinForms UI, tích hợp và demo.

Nhiệm vụ:

- Thiết lập kiến trúc project 3 lớp.
- Thiết lập OpenSpec, quản lý specs, tasks và tiến độ.
- Thiết kế database tổng thể và review schema cuối.
- Làm toàn bộ WinForms UI: login, main layout, dashboard, sản phẩm, loại hàng, nhà cung cấp, khách hàng, nhập kho, tồn kho, kiểm kê, bán hàng, hóa đơn, báo cáo, trợ lý quản lý.
- Tự tích hợp service của Dũ và Hùng vào UI.
- Phụ trách dashboard, assistant hybrid, kiểm tra build, demo cuối.
- Viết báo cáo kỹ thuật về mô hình 3 lớp, OpenSpec, database, giao diện và tính năng nổi bật.

## Dũ

Vai trò: backend nghiệp vụ kho.

Nhiệm vụ:

- DTO/DAL/BLL cho sản phẩm.
- DTO/DAL/BLL cho loại hàng.
- DTO/DAL/BLL cho nhà cung cấp.
- DTO/DAL/BLL cho nhập kho.
- DTO/DAL/BLL cho tồn kho.
- DTO/DAL/BLL cho kiểm kê.
- Logic cảnh báo tồn thấp.
- Test nghiệp vụ nhập kho, tồn kho, kiểm kê.
- Cung cấp method BLL rõ ràng để Châu gọi từ UI.

## Hùng

Vai trò: backend nghiệp vụ bán hàng và báo cáo.

Nhiệm vụ:

- DTO/DAL/BLL cho khách hàng.
- DTO/DAL/BLL cho bán hàng.
- DTO/DAL/BLL cho hóa đơn và chi tiết hóa đơn.
- Báo cáo doanh thu.
- Top sản phẩm bán chạy.
- Khách hàng mua nhiều nhất.
- Logic xuất/in hóa đơn hoặc export PDF/Excel nếu kịp.
- Rule-based assistant commands liên quan bán hàng và báo cáo.
- Test luồng bán hàng, hóa đơn, báo cáo.
- Cung cấp method BLL rõ ràng để Châu gọi từ UI.

## Quy định phối hợp

- Dũ và Hùng không cần hỗ trợ tích hợp UI.
- Châu chịu trách nhiệm tích hợp cuối.
- Khi đổi DTO, tên method hoặc kiểu trả về, phải ghi rõ trong PR description.
- Nếu service chưa xong, Châu có thể dựng UI bằng mock data tạm.
