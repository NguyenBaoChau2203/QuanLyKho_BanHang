# Phân công công việc

File này là mục lục phân công. Mỗi thành viên mở đúng file của mình trong thư mục `docs/phan-cong/` để đọc nhiệm vụ chi tiết, branch cần dùng, checklist bàn giao và quy định phối hợp.

## Danh sách phân công

- [Châu - Nhóm trưởng, kiến trúc, OpenSpec, WinForms UI, tích hợp](phan-cong/Chau.md)
- [Dũ - Backend kho, sản phẩm, nhập kho, tồn kho, kiểm kê](phan-cong/Du.md)
- [Hùng - Backend bán hàng, khách hàng, hóa đơn, báo cáo](phan-cong/Hung.md)

## Nguyên tắc chung

- Cả nhóm bắt đầu bằng [Phase 0 - Contract Foundation](07_ContractFoundation.md) để chốt database, DTO và service contract.
- Châu chịu trách nhiệm UI và tích hợp cuối.
- Dũ và Hùng tập trung backend nghiệp vụ, test và service contract.
- Dũ và Hùng không cần hỗ trợ tích hợp UI.
- Sau Phase 0, ba người được làm song song: Châu làm UI bằng mock/stub, Dũ làm backend kho, Hùng làm backend bán hàng/báo cáo.
- Khi đổi DTO, public method hoặc kiểu trả về, phải ghi rõ trong pull request.
- Mỗi người làm đúng branch của mình, không push trực tiếp lên `main`.

## Branch làm việc

- Châu: `feature/project-ui-chau`
- Dũ: `feature/inventory-du`
- Hùng: `feature/sales-report-hung`

## Thứ tự đọc tài liệu

1. Đọc file phân công cá nhân.
2. Đọc [Workflow làm việc](03_WorkflowLamViec.md).
3. Đọc [Quy chuẩn chung](04_QuyChuanChung.md).
4. Đọc [Phase 0 - Contract Foundation](07_ContractFoundation.md).
5. Đọc OpenSpec change hiện tại trong `openspec/changes/bootstrap-inventory-sales-mvp/`.
