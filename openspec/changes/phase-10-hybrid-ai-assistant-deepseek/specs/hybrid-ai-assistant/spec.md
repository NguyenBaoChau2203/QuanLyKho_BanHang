# Hybrid AI Assistant

## ADDED Requirements

### Requirement: Hybrid assistant keeps deterministic offline fallback
The assistant SHALL keep the existing rule-based behavior as a guaranteed fallback when DeepSeek is not configured or cannot produce a valid response.

#### Scenario: API key is missing
- **WHEN** `DEEPSEEK_API_KEY` is not set
- **THEN** `AssistantService.Ask` SHALL answer through the rule-based assistant
- **AND** the response SHALL indicate offline rule-based mode
- **AND** the application SHALL not call DeepSeek

#### Scenario: Existing commands work offline
- **WHEN** the user asks `doanh thu hôm nay`, `hàng sắp hết`, `top sản phẩm bán chạy`, `khách hàng mua nhiều nhất`, or `kiểm kê hôm nay`
- **AND** `DEEPSEEK_API_KEY` is not set
- **THEN** `AssistantService.Ask` SHALL return a successful `ServiceResult<AssistantResponseDto>`
- **AND** the response SHALL be handled and contain a useful Vietnamese answer

### Requirement: Optional DeepSeek provider stays inside BLL
The assistant SHALL use DeepSeek only through BLL assistant orchestration and only when `DEEPSEEK_API_KEY` is available.

#### Scenario: DeepSeek is configured
- **WHEN** `DEEPSEEK_API_KEY` is set
- **THEN** `AssistantService` MAY call DeepSeek through an internal BLL provider
- **AND** the provider SHALL read optional `DEEPSEEK_MODEL` and `DEEPSEEK_BASE_URL` environment variables
- **AND** the default model SHALL be `deepseek-chat`
- **AND** the default base URL SHALL be `https://api.deepseek.com`

#### Scenario: DeepSeek answers successfully
- **WHEN** DeepSeek returns a valid approved intent and final answer
- **THEN** `AssistantService.Ask` SHALL return AI online mode metadata
- **AND** the final answer SHALL be based only on safe data produced by existing BLL services

### Requirement: DeepSeek failures never reach WinForms as exceptions
The assistant SHALL catch DeepSeek failures and return deterministic fallback instead of throwing to the UI.

#### Scenario: DeepSeek cannot be used
- **WHEN** the API key is invalid, the network fails, a timeout occurs, quota is unavailable, HTTP status is unsuccessful, or the response shape is invalid
- **THEN** `AssistantService.Ask` SHALL return the rule-based fallback answer
- **AND** the response SHALL indicate AI failed fallback mode
- **AND** WinForms SHALL not crash

### Requirement: AI is constrained to safe assistant tasks
DeepSeek SHALL only help classify Vietnamese natural-language questions and produce friendly final wording from safe BLL-provided answer context.

#### Scenario: AI prompt is prepared
- **WHEN** `AssistantService` sends a request to DeepSeek
- **THEN** the prompt SHALL list only approved assistant intents and safe BLL summaries
- **AND** the prompt SHALL prohibit SQL generation for execution
- **AND** the provider SHALL not expose API keys, connection strings, DAL objects, or raw database access

### Requirement: UI displays assistant mode without secrets
The WinForms assistant UI SHALL clearly display the assistant mode/status returned by BLL without exposing the API key.

#### Scenario: User opens assistant screen
- **WHEN** the assistant screen is shown
- **THEN** the UI SHALL get initial assistant mode/status from `AssistantService`
- **AND** include a clear status such as AI online, offline rule-based, or AI failed fallback
- **AND** no API key value SHALL be shown

#### Scenario: User navigates from main shell
- **WHEN** the user views the main sidebar
- **THEN** the sidebar SHALL include a clear assistant entry such as `Trợ lý AI`
- **AND** the top quick action label SHALL be clearer than the generic `Trợ lý`

### Requirement: Architecture boundary remains strict
The hybrid assistant SHALL preserve the WinForms -> BLL -> DAL -> DTO architecture.

#### Scenario: Assistant implementation is reviewed
- **WHEN** source code is searched
- **THEN** WinForms SHALL not reference DAL
- **AND** WinForms SHALL not contain SQL strings
- **AND** WinForms SHALL not call DeepSeek directly
- **AND** DeepSeek provider SHALL not call DAL directly
- **AND** real business data SHALL still come from existing BLL services
