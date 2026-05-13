---
name: winforms-inventory-sales-ui
description: Build polished, maintainable C# WinForms .NET 8 UI for the QuanLyKhoBanHang inventory and sales management project. Use when planning or implementing WinForms screens, shared UI components, dashboards, navigation, DataGridView views, data-entry forms, validation UX, assistant/chat UI, visual polish, or responsive layout for warehouse, stock, purchasing, sales, customer, invoice, and reporting workflows.
license: MIT
compatibility: C# WinForms .NET 8, QuanLyKhoBanHang 3-layer architecture.
metadata:
  author: QuanLyKhoBanHang team
  version: "1.0"
---

# WinForms Inventory Sales UI

## Core Intent

Build a practical operations app, not a flashy landing page. The UI must feel like software a small warehouse, retail shop, or internal sales team can actually use every day.

Optimize for:

- stable layout
- fast data entry
- readable Vietnamese labels
- clean tables
- clear validation messages
- professional dashboard
- minimal rework when backend services change

## Architecture Rules

- WinForms calls BLL services only.
- WinForms never calls DAL.
- SQL never appears in WinForms.
- Event handlers only read control input, call BLL services, and display results.
- Business validation belongs in BLL. UI may do simple required-field checks for user experience.
- Use DTOs and `ServiceResult<T>` exactly as the contract defines.
- Do not change DTO or service signatures during UI work unless the current OpenSpec change explicitly allows it.

## Dependency Rule

Default to standard WinForms controls in .NET 8. Do not add a third-party UI framework unless an OpenSpec change explicitly chooses it and records why.

Recommended default controls:

- `Panel`, `TableLayoutPanel`, `FlowLayoutPanel`
- `DataGridView` with `BindingSource`
- `TabControl` when it reduces screen switching
- `StatusStrip` for status and sync messages
- `ToolTip` for compact actions
- `SplitContainer` only when it improves a workflow

## Visual Direction

Use a quiet professional business palette:

- App background: `#F5F7FA`
- Surface: `#FFFFFF`
- Border: `#D9E1EA`
- Text primary: `#1F2937`
- Text secondary: `#6B7280`
- Primary blue: `#2563EB`
- Inventory green: `#059669`
- Warning amber: `#D97706`
- Danger red: `#DC2626`

Avoid:

- purple/blue gradient-heavy themes
- dark dashboards everywhere
- decorative blobs, orbs, or bokeh
- oversized hero sections
- nested cards
- hard-coded absolute layouts that break on resize

## Layout Pattern

Use this shell:

- Left sidebar: 220-240 px
- Top bar: current screen title, current user, quick actions
- Main content: full-width work area
- Optional status strip: current status, validation summary, demo hints

Layout rules:

- Use `Dock`, `Anchor`, `Padding`, and layout panels.
- Use stable `MinimumSize` for forms and important panels.
- Prefer percent/absolute row and column styles over manual resize math.
- Use consistent spacing: 8 px between compact controls, 16-24 px for page padding.
- Do not place all controls with raw x/y coordinates for final UI.

## Shared UI Components

Create shared helpers before building many screens:

- `AppTheme`
- `UiFactory`
- reusable KPI card control
- reusable toolbar/action row
- reusable search/filter panel
- reusable empty-state/error panel
- standardized `DataGridView` styling helper
- standardized form input group pattern

Use shared components to keep the app visually consistent and reduce later cleanup.

## DataGridView Rules

For list screens:

- Use `DataGridView` plus `BindingSource`.
- Use explicit columns for final UI.
- Disable row headers unless needed.
- Use full-row select.
- Use alternating row color.
- Keep action buttons in a toolbar, not repeated inside every row unless strongly justified.
- Format money as VND.
- Format dates as `dd/MM/yyyy`.
- Show empty states when no rows are returned.

## Screen Guidance

### Dashboard

Must be scannable in 10 seconds.

Show:

- revenue today
- revenue this month
- invoice count today
- low-stock count
- top selling products
- low-stock products
- recent stock or sales transactions

Use KPI cards and simple tables. Do not overbuild custom charts unless the current phase requires them.

### Master Data

Screens:

- products
- categories
- suppliers
- customers

Use:

- search/filter top row
- grid center
- edit panel or modal-style input area
- actions: Add, Edit, Save, Cancel, Deactivate, Refresh

### Inventory

Screens:

- purchase receipt
- current stock
- stocktake

Optimize for fast entry:

- product search
- quantity input
- line item grid
- total/counter summary
- clear validation messages

### Sales

Sales invoice UI must support:

- product search
- add/remove invoice lines
- quantity and price
- customer selection
- total, discount, final total
- save invoice
- clear warning when stock is insufficient

### Reports

Reports UI must support:

- date range
- refresh button
- revenue summary grid
- top products
- top customers
- export placeholder only if export is not implemented yet

### Assistant

Assistant UI should feel like a manager command panel:

- input box
- suggested command buttons
- conversation/result area
- deterministic responses from `AssistantService`

Useful suggestions:

- `doanh thu hôm nay`
- `hàng sắp hết`
- `top sản phẩm bán chạy`
- `khách hàng mua nhiều nhất`

Do not require real AI API for the demo. Rule-based or stub mode must work.

## Typography And Spacing

- Font: Segoe UI.
- Base font: 10-11 pt.
- Screen title: 16-20 pt bold.
- Section title: 12-14 pt semibold.
- Button height: 36-40 px.
- Form padding: 16-24 px.
- Avoid text clipping on Vietnamese labels.

## Accessibility And Usability

- Set sensible `TabIndex`.
- Place labels before text boxes.
- Set `AccessibleName` for important controls.
- Do not rely on color alone for errors.
- Destructive actions require confirmation.
- Buttons must have clear Vietnamese text.
- Keyboard flow should work for common data-entry screens.

## Implementation Workflow

Before coding a screen:

1. Read the current OpenSpec change.
2. Read the relevant DTO and BLL service contract.
3. Identify whether the backend is real or stubbed.
4. Plan the screen sections before editing.
5. Implement layout with panels/table layouts.
6. Bind service results through DTOs.
7. Run `dotnet build QuanLyKhoBanHang.sln`.
8. Do not move to the next screen until the current screen builds.

## Done Criteria

A UI change is done only when:

- the solution builds
- the screen opens from `FrmMain`
- no DAL reference is added to WinForms
- no SQL appears in WinForms
- layout resizes acceptably
- Vietnamese labels are clear
- empty/error states are handled
- service integration points are obvious
- no unrelated files are changed
