# Workflow làm việc

## Branch

- Branch chính: `main`
- Branch của Châu: `feature/project-ui-chau`
- Branch của Dũ: `feature/inventory-du`
- Branch của Hùng: `feature/sales-report-hung`

## Quy trình

1. Pull code mới nhất từ `main`.
2. Tạo hoặc chuyển sang branch cá nhân.
3. Code đúng phần được giao.
4. Build project trước khi commit.
5. Commit theo format chung.
6. Push branch lên GitHub.
7. Tạo pull request vào `main`.
8. Châu review, xử lý conflict và merge.

## Commit format

- `feat(ui): add dashboard layout`
- `feat(bll): add sales invoice service`
- `feat(dal): add product repository`
- `fix(inventory): correct stock validation`
- `docs: update task assignment`
- `db: add stocktake tables`
- `test: add sales invoice tests`

## Quy định pull request

- Không push trực tiếp lên `main`.
- PR phải ghi rõ đã làm gì.
- PR phải ghi rõ có đổi DTO/service contract không.
- PR backend cần có ví dụ method Châu sẽ gọi từ UI.
- Không đưa `bin/`, `obj/`, `.vs/`, file database local vào Git.
