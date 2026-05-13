## Why

Châu needs a dashboard and reports UI that looks credible in a real warehouse/sales operation while the backend is still being completed. The screens should be useful for demos, remain backend-safe, and keep all logic inside WinForms presentation code.

## What Changes

- Implement a dashboard screen with KPI cards, top products, low-stock products, and recent activity.
- Implement a reports screen with date-range filters, refresh action, revenue summary, top products, top customers, and an export placeholder.
- Use deterministic stub data whenever BLL services return empty results so the screens remain demoable.
- Keep the WinForms layer dependent only on BLL service contracts and shared UI helpers.
- Ensure the shell opens Dashboard and Báo cáo without crashing.

## Impact

- Managers can demo the system with a useful overview and reporting workspace.
- The UI remains presentation-only and safe to merge before the real DAL/report implementation is complete.
- Future backend integration can replace stub data without redesigning the screens.
