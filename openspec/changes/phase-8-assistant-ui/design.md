## Design

- Layout follows the shared shell: page padding, Segoe UI typography, quiet surface colors from `AppTheme`, and `Dock`/`TableLayoutPanel` rather than absolute positioning.
- Top section: title and subtitle; suggested commands render as compact buttons in a `FlowLayoutPanel`.
- Middle: command row (`TextBox` + primary actions: Gửi, Xóa hội thoại).
- Bottom: scrollable conversation stack built from lightweight card `Panel`s—user prompts left-aligned with a muted bubble, assistant replies as bordered cards with a title line and monospace-friendly detail text.
- Orchestration: always call `AssistantService.Ask` first for normalization and polite fallbacks; derive actionable intents from `AssistantResponseDto.Intent`, then supplement with lightweight keyword routing for phrases not covered by the service (for example kiểm kê, khách hàng mua nhiều).
- Data: WinForms calls only BLL services. When list results are empty or messages indicate no data, substitute deterministic stub rows consistent with reports/dashboard demos and label the card with a short “(Demo)” note.
