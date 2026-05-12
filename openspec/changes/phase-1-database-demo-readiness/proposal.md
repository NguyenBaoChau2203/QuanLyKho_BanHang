## Why

Châu needs reliable demo-ready database scripts so the UI can show login, dashboard, master data, and stock state without waiting for DAL/BLL integration.

## What Changes

- Review and stabilize `database/schema.sql` and `database/seed.sql` for demo readiness.
- Ensure seed data supports login, dashboard visibility, products, categories, suppliers, customers, starting stock, and a low-stock example.
- Keep schema and seed changes contract-preserving and avoid DAL or UI implementation work.
- Refresh `database/README.md` if needed to describe demo accounts and seed expectations.

## Impact

- WinForms can rely on the seeded data for demo flows.
- Later backend work can integrate against a more realistic and stable dataset.
- No DAL repositories, no real DB access layer, and no UI changes are introduced in this phase.
