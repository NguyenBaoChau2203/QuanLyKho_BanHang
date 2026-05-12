# OpenSpec workflow

## Cài đặt

```powershell
npm install -g @fission-ai/openspec@latest
openspec --version
```

## Khởi tạo

```powershell
openspec init
```

Trong repo hiện tại đã có sẵn cấu trúc `openspec/` và change đầu tiên:

```text
openspec/changes/bootstrap-inventory-sales-mvp/
```

## Luồng làm việc

1. Tạo change bằng `/opsx:new <ten-change>`.
2. Sinh proposal/design/tasks/specs bằng `/opsx:ff`.
3. Review kỹ tài liệu trước khi code.
4. Implement theo `tasks.md`.
5. Sau khi test xong, chạy `/opsx:archive` để đưa specs vào source of truth.

## Quy định nhóm

- Tính năng lớn phải có OpenSpec change hoặc cập nhật docs trước.
- Khi scope đổi, cập nhật `proposal.md` hoặc `tasks.md`.
- Khi hoàn thành task, tick vào `tasks.md`.
- Không archive khi code chưa build hoặc chưa demo được.
