## 1. OpenSpec and scope

- [x] 1.1 Create OpenSpec artifacts for `phase-7-dashboard-reports-ui`.
- [x] 1.2 Confirm the phase stays within dashboard and reports UI scope only.

## 2. Dashboard screen

- [x] 2.1 Implement `FrmDashboard` with KPI cards, top products, low-stock products, and recent activity.
- [x] 2.2 Add clear empty/error/fallback states and deterministic stub data.

## 3. Reports screen

- [x] 3.1 Implement `FrmReport` with date filters, refresh action, and summary grids.
- [x] 3.2 Add export placeholder behavior and deterministic stub data fallback.

## 4. Shell integration

- [x] 4.1 Ensure `FrmMain` opens Dashboard and Báo cáo safely.
- [x] 4.2 Keep shell navigation backend independent and stable.

## 5. Validation

- [x] 5.1 Run `openspec validate phase-7-dashboard-reports-ui`.
- [x] 5.2 Run `dotnet build QuanLyKhoBanHang.sln`.
- [x] 5.3 Run `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`.
- [x] 5.4 Verify WinForms has no DAL reference and no SQL strings.

## 6. Delivery

- [x] 6.1 Update this tasks file to mark completed work.
- [ ] 6.2 Commit phase 7 separately with the requested message.
