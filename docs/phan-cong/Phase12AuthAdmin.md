# Phase 12 - Phan cong Auth/Admin backend that

OpenSpec change: `phase-12-auth-admin-real-dal-security`

## Muc tieu

Bien phan Admin/Auth da co o Phase 11 tu demo stub sang backend that hon:

- Users/Roles/Permissions di qua DAL/BLL that.
- AuditLogs doc/ghi qua DAL/BLL that.
- Password moi duoc hash, khong luu plaintext.
- WinForms van chi goi BLL, khong goi DAL va khong chua SQL.

## Nguyen tac phan cong

Phase 12 khong phai Chau tu lam het tren branch UI. Chau chi giu vai tro owner/reviewer va tich hop cuoi.

- Chau: OpenSpec, schema/seed review, contract review, UI integration cuoi.
- Du: Users/Roles/Permissions DAL va account backend.
- Hung: AuditLogs DAL, audit writer, auth/admin tests.

Password hashing va auth flow can co mot owner chinh khi implement de tranh lech logic. Neu Du lam account backend, Du nen lam hashing helper; Hung viet test va audit event lien quan.

## Phan viec cua Chau

- Duyet OpenSpec Phase 12.
- Review moi thay doi `database/schema.sql`, `database/seed.sql`, DTO va public service method.
- Dam bao WinForms -> BLL -> DAL -> DTO.
- Dam bao WinForms khong reference DAL va khong co SQL.
- Tich hop UI cuoi cho:
  - `FrmLogin`
  - `FrmMain`
  - `FrmUserManagement`
  - `FrmRolePermission`
  - `FrmAuditLog`
- Chay validation cuoi:
  - `npx --yes --package @fission-ai/openspec openspec validate phase-12-auth-admin-real-dal-security`
  - `dotnet build QuanLyKhoBanHang.sln`
  - `dotnet test QuanLyKhoBanHang.sln --no-build --no-restore`

Sau khi file nay va OpenSpec Phase 12 da co, Chau co the tiep tuc tinh chinh UI tren branch `feature/project-lead-chau`.

## Phan viec cua Du

Branch lam viec: `feature/inventory-du`

Du nhan Identity DAL va account backend:

- Doc Phase 11 admin/auth code:
  - `src/QuanLyKhoBanHang.BLL/Services/AuthService.cs`
  - `src/QuanLyKhoBanHang.BLL/Services/UserAccountService.cs`
  - `src/QuanLyKhoBanHang.BLL/Services/PermissionService.cs`
  - `src/QuanLyKhoBanHang.DTO/Admin/`
- Trien khai hoac cap nhat repository ADO.NET cho:
  - `Users`
  - `Roles`
  - permission mapping neu duoc Chau duyet schema
- Dung parameter cho moi SQL co input nguoi dung.
- Cap nhat `UserAccountService` de account CRUD di qua DAL that nhung giu `ServiceResult<T>`.
- Lam password hashing helper neu Du la owner account backend.
- Khong tra password hash ve WinForms DTO.
- Viet test cho:
  - duplicate username
  - invalid role
  - inactive account khong dang nhap duoc
  - account create/update/deactivate
  - password verify thanh cong/that bai

Ban giao cho Chau:

- Co sua schema/seed khong.
- Service method nao UI co the goi.
- Cach test nhanh account management va permission loading.

## Phan viec cua Hung

Branch lam viec: `feature/sales-report-hung`

Hung nhan AuditLogs DAL, audit writer va auth/admin tests:

- Doc Phase 11 admin/auth code:
  - `src/QuanLyKhoBanHang.BLL/Services/AuditLogService.cs`
  - `src/QuanLyKhoBanHang.BLL/Services/AuthService.cs`
  - `src/QuanLyKhoBanHang.BLL/Services/PermissionService.cs`
  - `src/QuanLyKhoBanHang.WinForms/Forms/Admin/FrmAuditLog.cs`
- Trien khai repository ADO.NET cho `AuditLogs`.
- Ho tro filter audit log theo:
  - date range
  - keyword
  - user
  - action
  - entity
- Dung parameterized SQL cho filter.
- Cap nhat `AuditLogService` de doc log qua DAL that.
- Them BLL method ghi audit log cho cac su kien neu feasible:
  - login thanh cong
  - login that bai
  - tao/sua/ngung kich hoat tai khoan
  - truy cap trai quyen
- Viet test cho:
  - audit filtering
  - login audit event
  - admin account action audit event

Ban giao cho Chau:

- Repository/service nao da xong.
- Audit event nao da duoc ghi tu dong.
- Cach test nhanh man `Nhat ky he thong`.

## Ngoai scope Phase 12

- Khong lam password reset/email.
- Khong lam claims framework phuc tap.
- Khong thay doi nghiep vu kho/ban hang/bao cao khong lien quan.
- Khong dua API key/secrets vao source code.
