## Design

- The dashboard uses a responsive WinForms layout with KPI cards across the top and three scannable data sections below.
- The reports screen uses a filter strip with from/to dates, refresh, and export placeholder actions above three grids.
- Both screens call BLL contracts only and fall back to deterministic stub data when service results are empty.
- Shared theme and UI factory helpers keep styling and spacing consistent across the phase.
