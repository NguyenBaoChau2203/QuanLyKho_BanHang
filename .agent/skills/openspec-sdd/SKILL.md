---
name: openspec-sdd
description: Use when working in this repository with OpenSpec/SDD: creating proposals, writing proposal/design/tasks/spec artifacts, validating OpenSpec changes, implementing only approved scopes, reviewing scope drift, or replacing Cursor /opsx workflows in Codex.
---

# OpenSpec SDD

Use this skill for OpenSpec-driven work in this WinForms inventory/sales repository.

## First Checks

1. Run `git status -sb`.
2. Confirm the branch when the user names an expected branch.
3. If the working tree is dirty, report the dirty files before editing unless the request clearly continues those edits.
4. Read project guidance before planning or editing:
   - `AGENTS.md`
   - `README.md`
   - relevant `docs/*.md`
   - relevant `openspec/changes/<change-name>/`

## Architecture Guardrails

- Preserve `WinForms -> BLL -> DAL -> DTO`.
- WinForms must call BLL only.
- WinForms must not reference DAL.
- WinForms must not contain SQL.
- DAL owns SQL/data access.
- DTO contains data only.
- Do not change public DTO/service contracts unless the OpenSpec change explicitly requires it.

## Validate

Prefer:

```powershell
openspec validate <change-name>
```

Fallback:

```powershell
npx --yes --package @fission-ai/openspec openspec validate <change-name>
```

Validate before implementation and again after implementation.

## Create Proposal

When replacing Cursor `/opsx:propose <change-name>`, create:

```text
openspec/changes/<change-name>/
  proposal.md
  design.md
  tasks.md
  specs/<capability>/spec.md
```

Include:

- Goal and scope.
- Explicit out-of-scope items.
- Architecture constraints.
- Affected modules.
- Acceptance criteria.
- Validation/build/test commands.
- Concrete checklist tasks.

Use valid OpenSpec delta headings:

```markdown
## ADDED Requirements

### Requirement: Example
The system SHALL ...

#### Scenario: Example
- WHEN ...
- THEN ...
```

## Implement Change

When replacing Cursor `/opsx:apply <change-name>`:

1. Read all artifacts for the change.
2. Implement only the approved scope.
3. Keep fallback/mock behavior deterministic when real services are not ready.
4. Mark tasks complete only after validation/build/test pass.
5. Run:

```powershell
openspec validate <change-name>
dotnet build QuanLyKhoBanHang.sln
dotnet test QuanLyKhoBanHang.sln --no-build --no-restore
```

Use the `npx` OpenSpec fallback if `openspec` is unavailable.

## Review Change

When asked to review, do not modify files. Report findings first, ordered by severity, with file references when useful.

## Prompt Pattern

```text
Use the openspec-sdd skill.

Implement OpenSpec change <change-name>.
Run git status -sb first.
Read the change artifacts.
Keep the scope strict.
Run OpenSpec validation, build, and tests.
Mark tasks complete only after validation passes.
Commit with message: <message>
```
