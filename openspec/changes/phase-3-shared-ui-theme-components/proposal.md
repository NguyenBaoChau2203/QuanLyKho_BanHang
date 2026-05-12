## Why

Châu needs shared UI helpers so future WinForms screens look consistent without repeating theme and layout code in every form.

## What Changes

- Add shared WinForms theme and helper infrastructure.
- Introduce reusable styling for cards, grids, and common shell UI patterns.
- Apply the helpers only where safe to existing shell and placeholder screens.
- Keep the UI helper layer independent from DAL and SQL.

## Impact

- Later forms can be built faster with consistent spacing, colors, and control styling.
- Existing shell screens gain a more uniform professional feel.
- No full CRUD screens or third-party UI packages are introduced.
