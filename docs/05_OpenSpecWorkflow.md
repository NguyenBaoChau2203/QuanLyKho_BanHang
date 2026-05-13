# OpenSpec Workflow

## Cài Đặt

Nếu muốn cài OpenSpec global:

```powershell
npm install -g @fission-ai/openspec@latest
openspec --version
```

Nếu không cài global, dùng trực tiếp qua `npx`:

```powershell
npx --yes --package @fission-ai/openspec openspec validate <change-name>
```

## Cấu Trúc Hiện Có

Repo hiện đã có thư mục:

```text
openspec/
  changes/
  specs/
```

Mỗi change nên có:

```text
openspec/changes/<change-name>/
  proposal.md
  design.md
  tasks.md
  specs/<capability>/spec.md
```

## Workflow Chung

1. Tạo OpenSpec change trước khi code tính năng lớn.
2. Review kỹ `proposal.md`, `design.md`, `tasks.md`, và `spec.md`.
3. Validate change.
4. Implement đúng scope.
5. Build/test.
6. Tick task trong `tasks.md`.
7. Commit riêng cho từng phase/change.
8. Chỉ archive khi change đã được review và chấp nhận.

## Khi Dùng Codex App

Cursor `/opsx:*` không còn cần thiết. Codex đã có skill:

```text
C:\Users\chau1\.codex\skills\openspec-sdd
```

Khi muốn Codex làm theo OpenSpec, bắt đầu prompt bằng:

```text
Use the openspec-sdd skill.
```

Hướng dẫn chi tiết và prompt mẫu nằm ở:

```text
docs/09_CodexOpenSpecWorkflow.md
```

## Khi Dùng Antigravity

Antigravity đọc `AGENTS.md` và `GEMINI.md` ở root project. Xem hướng dẫn riêng tại:

```text
docs/10_AntigravityOpenSpecWorkflow.md
```

## Quy Định Nhóm

- Tính năng lớn phải có OpenSpec change hoặc cập nhật docs trước.
- Khi scope đổi, cập nhật `proposal.md`, `design.md`, `tasks.md` hoặc spec delta.
- Khi hoàn thành task, tick vào `tasks.md`.
- Không archive khi code chưa build, chưa test hoặc chưa demo được.
- Không đổi DTO/service contract ngầm; nếu đổi phải ghi rõ trong OpenSpec và PR description.
