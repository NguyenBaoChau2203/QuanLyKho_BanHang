## 1. OpenSpec and scope

- [x] 1.1 Create OpenSpec artifacts for `phase-8-assistant-ui`.
- [x] 1.2 Confirm the phase stays within assistant UI scope only (no Phase 9 integration).

## 2. Assistant screen

- [x] 2.1 Implement `FrmAssistant` with command input, conversation history, suggested commands, response cards/blocks, deterministic stub fallback, and clear conversation.
- [x] 2.2 Wire suggestions: `doanh thu hôm nay`, `hàng sắp hết`, `top sản phẩm bán chạy`, `khách hàng mua nhiều nhất`, `kiểm kê hôm nay`.
- [x] 2.3 Call `AssistantService.Ask` first; enrich from `ReportService` / `InventoryService` / `StocktakeService` by intent; stub when results are empty.

## 3. Shell integration

- [x] 3.1 Ensure `FrmMain` opens Trợ lý (`FrmAssistant`) without crashing via existing navigation.

## 4. Validation

- [x] 4.1 Run `openspec validate phase-8-assistant-ui` (or `npx --yes --package @fission-ai/openspec openspec validate phase-8-assistant-ui`).
- [x] 4.2 Run `dotnet build QuanLyKhoBanHang.sln`.
- [x] 4.3 Run `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`.
- [x] 4.4 Verify WinForms has no DAL reference for this feature, no SQL strings, no AI/network usage.

## 5. Delivery

- [x] 5.1 Update this tasks file to mark completed work.
- [x] 5.2 Commit Phase 8 with `feat(ui): add manager assistant chat screen with stub responses` (paths: `src/QuanLyKhoBanHang.WinForms`, `openspec/changes/phase-8-assistant-ui`).
