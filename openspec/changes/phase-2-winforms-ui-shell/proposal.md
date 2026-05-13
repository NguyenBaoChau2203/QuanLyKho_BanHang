## Why

Châu needs a stable, polished WinForms shell so future screens can be plugged in cleanly without waiting for backend completion.

## What Changes

- Improve the login and main shell experience.
- Build a responsive top bar, sidebar navigation, content host, and status area.
- Keep main menu items stable and safe by loading placeholder or existing screens only.
- Keep the shell independent of DAL and SQL, using placeholder behavior where needed.

## Impact

- Future inventory, sales, and reporting screens can be added without reworking the application shell.
- The UI becomes easier to demo and less likely to crash during navigation.
- No real backend dependency is introduced.
