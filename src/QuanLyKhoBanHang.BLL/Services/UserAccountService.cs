using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.DTO.Admin;
using QuanLyKhoBanHang.DTO.Auth;
using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class UserAccountService
{
    private static readonly object SyncRoot = new();
    private static readonly List<UserAccountDto> Accounts =
    [
        new()
        {
            Id = 1,
            Username = "admin",
            DemoPassword = "admin123",
            FullName = "Châu",
            Role = UserRole.Admin,
            RoleName = PermissionService.GetRoleDisplayName(UserRole.Admin),
            IsActive = true,
            CreatedAt = DateTime.Today.AddDays(-30).AddHours(8)
        },
        new()
        {
            Id = 2,
            Username = "manager",
            DemoPassword = "123456",
            FullName = "Quản lý demo",
            Role = UserRole.Manager,
            RoleName = PermissionService.GetRoleDisplayName(UserRole.Manager),
            IsActive = true,
            CreatedAt = DateTime.Today.AddDays(-21).AddHours(9)
        },
        new()
        {
            Id = 3,
            Username = "du",
            DemoPassword = "123456",
            FullName = "Dũ",
            Role = UserRole.WarehouseStaff,
            RoleName = PermissionService.GetRoleDisplayName(UserRole.WarehouseStaff),
            IsActive = true,
            CreatedAt = DateTime.Today.AddDays(-14).AddHours(10)
        },
        new()
        {
            Id = 4,
            Username = "hung",
            DemoPassword = "123456",
            FullName = "Hùng",
            Role = UserRole.SalesStaff,
            RoleName = PermissionService.GetRoleDisplayName(UserRole.SalesStaff),
            IsActive = true,
            CreatedAt = DateTime.Today.AddDays(-14).AddHours(10).AddMinutes(30)
        }
    ];

    public ServiceResult<List<UserAccountDto>> GetAllAccounts()
    {
        lock (SyncRoot)
        {
            var accounts = Accounts
                .OrderBy(x => x.Id)
                .Select(CloneForUi)
                .ToList();

            return ServiceResult<List<UserAccountDto>>.Ok(accounts, "Đã tải danh sách tài khoản demo.");
        }
    }

    public ServiceResult<int> CreateAccount(UserAccountDto account)
    {
        var validation = ValidateAccount(account, isCreate: true);
        if (!validation.Success)
        {
            return ServiceResult<int>.Fail(validation.Message);
        }

        lock (SyncRoot)
        {
            if (Accounts.Any(x => x.Username.Equals(account.Username.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return ServiceResult<int>.Fail("Tên đăng nhập đã tồn tại.");
            }

            var id = Accounts.Count == 0 ? 1 : Accounts.Max(x => x.Id) + 1;
            Accounts.Add(new UserAccountDto
            {
                Id = id,
                Username = account.Username.Trim(),
                DemoPassword = string.IsNullOrWhiteSpace(account.DemoPassword) ? "123456" : account.DemoPassword,
                FullName = account.FullName.Trim(),
                Role = account.Role,
                RoleName = PermissionService.GetRoleDisplayName(account.Role),
                IsActive = account.IsActive,
                CreatedAt = DateTime.Now
            });

            return ServiceResult<int>.Ok(id, "Đã tạo tài khoản demo trong bộ nhớ.");
        }
    }

    public ServiceResult<bool> UpdateAccount(UserAccountDto account)
    {
        var validation = ValidateAccount(account, isCreate: false);
        if (!validation.Success)
        {
            return ServiceResult<bool>.Fail(validation.Message);
        }

        lock (SyncRoot)
        {
            var existing = Accounts.FirstOrDefault(x => x.Id == account.Id);
            if (existing is null)
            {
                return ServiceResult<bool>.Fail("Không tìm thấy tài khoản cần cập nhật.");
            }

            if (Accounts.Any(x => x.Id != account.Id
                && x.Username.Equals(account.Username.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return ServiceResult<bool>.Fail("Tên đăng nhập đã tồn tại.");
            }

            existing.Username = account.Username.Trim();
            existing.FullName = account.FullName.Trim();
            existing.Role = account.Role;
            existing.RoleName = PermissionService.GetRoleDisplayName(account.Role);
            existing.IsActive = account.IsActive;

            if (!string.IsNullOrWhiteSpace(account.DemoPassword))
            {
                existing.DemoPassword = account.DemoPassword;
            }

            return ServiceResult<bool>.Ok(true, "Đã cập nhật tài khoản demo trong bộ nhớ.");
        }
    }

    public ServiceResult<bool> DeactivateAccount(int id)
    {
        if (id <= 0)
        {
            return ServiceResult<bool>.Fail("Vui lòng chọn tài khoản cần ngừng kích hoạt.");
        }

        lock (SyncRoot)
        {
            var account = Accounts.FirstOrDefault(x => x.Id == id);
            if (account is null)
            {
                return ServiceResult<bool>.Fail("Không tìm thấy tài khoản cần ngừng kích hoạt.");
            }

            if (account.Role == UserRole.Admin
                && account.IsActive
                && Accounts.Count(x => x.Role == UserRole.Admin && x.IsActive) <= 1)
            {
                return ServiceResult<bool>.Fail("Không thể ngừng kích hoạt quản trị viên demo cuối cùng.");
            }

            account.IsActive = false;
            return ServiceResult<bool>.Ok(true, "Đã ngừng kích hoạt tài khoản demo trong bộ nhớ.");
        }
    }

    internal static UserDto? AuthenticateDemoUser(string username, string password)
    {
        lock (SyncRoot)
        {
            var account = Accounts.FirstOrDefault(x =>
                x.IsActive
                && x.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase)
                && x.DemoPassword == password);

            if (account is null)
            {
                return null;
            }

            return new UserDto
            {
                Id = account.Id,
                Username = account.Username,
                FullName = account.FullName,
                Role = account.Role,
                IsActive = account.IsActive
            };
        }
    }

    private static ServiceResult<bool> ValidateAccount(UserAccountDto account, bool isCreate)
    {
        if (!isCreate && account.Id <= 0)
        {
            return ServiceResult<bool>.Fail("Id tài khoản không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(account.Username))
        {
            return ServiceResult<bool>.Fail("Tên đăng nhập là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(account.FullName))
        {
            return ServiceResult<bool>.Fail("Họ tên là bắt buộc.");
        }

        if (!Enum.IsDefined(account.Role))
        {
            return ServiceResult<bool>.Fail("Vai trò không hợp lệ.");
        }

        return ServiceResult<bool>.Ok(true);
    }

    private static UserAccountDto CloneForUi(UserAccountDto account)
    {
        return new UserAccountDto
        {
            Id = account.Id,
            Username = account.Username,
            FullName = account.FullName,
            Role = account.Role,
            RoleName = PermissionService.GetRoleDisplayName(account.Role),
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            DemoPassword = string.Empty
        };
    }
}
