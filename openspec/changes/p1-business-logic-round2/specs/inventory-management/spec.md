# Spec: Quản lý Nhập kho và Kiểm kê

## MODIFIED Requirements

### Requirement: Gộp các thao tác nhập kho vào cùng một SQL Transaction
- Mô tả: Khi lưu phiếu nhập, hệ thống MUST thực hiện các thao tác insert phiếu nhập, insert chi tiết phiếu nhập, update số lượng tồn kho sản phẩm và insert lịch sử giao dịch trong cùng một transaction.
- Cập nhật số lượng sản phẩm: Số lượng hiện tại của sản phẩm sẽ được cộng thêm số lượng nhập.
- Lịch sử giao dịch: Ghi nhận số lượng thay đổi, số lượng sau thay đổi, mã tham chiếu và loại giao dịch.

#### Scenario: Thành công khi lưu phiếu nhập hợp lệ
- Dữ liệu: Một phiếu nhập hợp lệ có ít nhất một chi tiết sản phẩm.
- Hành động: Lưu phiếu nhập.
- Kết quả: Phiếu nhập và chi tiết được lưu, số lượng sản phẩm được cập nhật, lịch sử được tạo. Transaction được Commit.

#### Scenario: Rollback khi có lỗi
- Dữ liệu: Một phiếu nhập mà quá trình xử lý gặp lỗi (ví dụ không tìm thấy sản phẩm).
- Hành động: Thử lưu phiếu nhập.
- Kết quả: Ngoại lệ xảy ra, transaction bị Rollback, không có dữ liệu nào được lưu trữ hoặc cập nhật vào database.

### Requirement: Gộp các thao tác kiểm kê vào cùng một SQL Transaction
- Mô tả: Tương tự phiếu nhập, khi lưu phiếu kiểm kê, hệ thống MUST dùng chung một transaction cho quá trình insert phiếu kiểm kê, insert chi tiết, update số lượng (nếu có chênh lệch) và insert lịch sử giao dịch.
- Cập nhật số lượng sản phẩm: Số lượng sản phẩm sẽ được thay bằng số lượng thực tế kiểm kê.
- Lịch sử giao dịch: Sinh lịch sử kiểm kê cho các mặt hàng có chênh lệch giữa số thực tế và số hệ thống.

#### Scenario: Thành công khi lưu phiếu kiểm kê
- Dữ liệu: Một phiếu kiểm kê hợp lệ, có chênh lệch số lượng.
- Hành động: Lưu phiếu kiểm kê.
- Kết quả: Phiếu kiểm kê và chi tiết được lưu, sản phẩm được cập nhật, lịch sử được tạo. Transaction Commit.

#### Scenario: Rollback khi kiểm kê lỗi
- Dữ liệu: Một phiếu kiểm kê gặp lỗi kỹ thuật trong quá trình lưu.
- Hành động: Thử lưu phiếu kiểm kê.
- Kết quả: Exception bị bắt, transaction Rollback, không có sự thay đổi nào trong database.
