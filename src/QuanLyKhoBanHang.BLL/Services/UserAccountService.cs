using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Security;
using QuanLyKhoBanHang.DAL.Auth;
using QuanLyKhoBanHang.DAL.Data;
using QuanLyKhoBanHang.DTO.Admin;
using QuanLyKhoBanHang.DTO.Auth;
using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class UserAccountService
{
    private readonly UserRepository _userRepo;
    private readonly RoleRepository _roleRepo;
    private readonly AuditLogRepository _auditRepo;

    public UserAccountService()
    {
        var options = new DatabaseOptions();
        _userRepo = new UserRepository(options);
        _roleRepo = new RoleRepository(options);
        _auditRepo = new AuditLogRepository(options);
    }

    public UserAccountService(DatabaseOptions options)
    {
        _userRepo = new UserRepository(options);
        _roleRepo = new RoleRepository(options);
        _auditRepo = new AuditLogRepository(options);
    }

    internal UserAccountService(UserRepository userRepo, RoleRepository roleRepo, AuditLogRepository auditRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _auditRepo = auditRepo;
    }

    public ServiceResult<List<UserAccountDto>> GetAllAccounts()
    {
        try
        {
            var users = _userRepo.GetAll();
            var accounts = users.Select(MapToAccountDto).OrderBy(x => x.Id).ToList();
            return ServiceResult<List<UserAccountDto>>.Ok(accounts, "Đã tải danh sách tài khoản.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UserAccountDto>>.Fail("Lỗi khi tải danh sách tài khoản: " + ex.Message);
        }
    }

    public ServiceResult<int> CreateAccount(UserAccountDto account, int createdByUserId)
    {
        var validation = ValidateAccount(account, isCreate: true);
        if (!validation.Success)
            return ServiceResult<int>.Fail(validation.Message);

        try
        {
            if (_userRepo.IsUsernameTaken(account.Username.Trim()))
                return ServiceResult<int>.Fail("Tên đăng nhập đã tồn tại.");

            var plainPassword = string.IsNullOrWhiteSpace(account.DemoPassword) ? "123456" : account.DemoPassword;
            if (!PasswordHasher.ValidatePasswordPolicy(plainPassword))
                return ServiceResult<int>.Fail("Mật khẩu phải có ít nhất 4 ký tự và không được chỉ chứa khoảng trắng.");

            var passwordHash = PasswordHasher.Hash(plainPassword);
            var roleId = (int)account.Role;
            var id = _userRepo.Create(account.Username.Trim(), passwordHash, account.FullName.Trim(), roleId, account.IsActive);

            _auditRepo.Write(createdByUserId, "Tạo tài khoản", "Users", id,
                $"Tạo tài khoản {account.Username.Trim()} với vai trò {PermissionService.GetRoleDisplayName(account.Role)}");

            return ServiceResult<int>.Ok(id, "Đã tạo tài khoản thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail("Lỗi khi tạo tài khoản: " + ex.Message);
        }
    }

    public ServiceResult<bool> UpdateAccount(UserAccountDto account, int updatedByUserId)
    {
        var validation = ValidateAccount(account, isCreate: false);
        if (!validation.Success)
            return ServiceResult<bool>.Fail(validation.Message);

        try
        {
            if (_userRepo.IsUsernameTaken(account.Username.Trim(), account.Id))
                return ServiceResult<bool>.Fail("Tên đăng nhập đã tồn tại.");

            _userRepo.Update(account.Id, account.Username.Trim(), account.FullName.Trim(), (int)account.Role, account.IsActive);

            if (!string.IsNullOrWhiteSpace(account.DemoPassword))
            {
                var plainPassword = account.DemoPassword;
                if (!PasswordHasher.ValidatePasswordPolicy(plainPassword))
                    return ServiceResult<bool>.Fail("Mật khẩu mới phải có ít nhất 4 ký tự và không được chỉ chứa khoảng trắng.");

                var passwordHash = PasswordHasher.Hash(plainPassword);
                _userRepo.UpdatePasswordHash(account.Id, passwordHash, mustChangePassword: true);
            }

            _auditRepo.Write(updatedByUserId, "Cập nhật tài khoản", "Users", account.Id,
                $"Cập nhật tài khoản {account.Username.Trim()}");

            return ServiceResult<bool>.Ok(true, "Đã cập nhật tài khoản thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail("Lỗi khi cập nhật tài khoản: " + ex.Message);
        }
    }

    public ServiceResult<bool> DeactivateAccount(int id, int deactivatedByUserId)
    {
        if (id <= 0)
            return ServiceResult<bool>.Fail("Vui lòng chọn tài khoản cần ngừng kích hoạt.");

        try
        {
            var user = _userRepo.GetById(id);
            if (user is null)
                return ServiceResult<bool>.Fail("Không tìm thấy tài khoản cần ngừng kích hoạt.");

            if (user.Role == UserRole.Admin && user.IsActive && _userRepo.CountActiveAdmins() <= 1)
                return ServiceResult<bool>.Fail("Không thể ngừng kích hoạt quản trị viên cuối cùng.");

            _userRepo.Deactivate(id);

            _auditRepo.Write(deactivatedByUserId, "Ngừng kích hoạt tài khoản", "Users", id,
                $"Ngừng kích hoạt tài khoản {user.Username}");

            return ServiceResult<bool>.Ok(true, "Đã ngừng kích hoạt tài khoản.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail("Lỗi khi ngừng kích hoạt tài khoản: " + ex.Message);
        }
    }

    public ServiceResult<bool> ResetPassword(int userId, string? newPassword, int resetByUserId)
    {
        if (userId <= 0)
            return ServiceResult<bool>.Fail("Vui lòng chọn tài khoản cần đặt lại mật khẩu.");

        try
        {
            var user = _userRepo.GetById(userId);
            if (user is null)
                return ServiceResult<bool>.Fail("Không tìm thấy tài khoản.");

            if (!user.IsActive)
                return ServiceResult<bool>.Fail("Không thể đặt lại mật khẩu cho tài khoản đã ngừng kích hoạt.");

            var plainPassword = string.IsNullOrWhiteSpace(newPassword) ? "Default@123" : newPassword;
            if (!PasswordHasher.ValidatePasswordPolicy(plainPassword))
                return ServiceResult<bool>.Fail("Mật khẩu phải có ít nhất 4 ký tự và không được chỉ chứa khoảng trắng.");

            var passwordHash = PasswordHasher.Hash(plainPassword);
            _userRepo.UpdatePasswordHash(userId, passwordHash, mustChangePassword: true);

            _auditRepo.Write(resetByUserId, "Đặt lại mật khẩu", "Users", userId,
                $"Đặt lại mật khẩu cho tài khoản {user.Username}");

            return ServiceResult<bool>.Ok(true, "Đã đặt lại mật khẩu. Người dùng sẽ được yêu cầu đổi mật khẩu khi đăng nhập lần sau.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail("Lỗi khi đặt lại mật khẩu: " + ex.Message);
        }
    }

    public ServiceResult<bool> ChangePassword(int userId, string currentPassword, string newPassword)
    {
        if (userId <= 0)
            return ServiceResult<bool>.Fail("Tài khoản không hợp lệ.");
        if (string.IsNullOrWhiteSpace(currentPassword))
            return ServiceResult<bool>.Fail("Vui lòng nhập mật khẩu hiện tại.");
        if (string.IsNullOrWhiteSpace(newPassword))
            return ServiceResult<bool>.Fail("Vui lòng nhập mật khẩu mới.");
        if (!PasswordHasher.ValidatePasswordPolicy(newPassword))
            return ServiceResult<bool>.Fail("Mật khẩu mới phải có ít nhất 4 ký tự và không được chỉ chứa khoảng trắng.");

        try
        {
            var storedHash = _userRepo.GetPasswordHash(userId);
            if (storedHash is null)
                return ServiceResult<bool>.Fail("Không tìm thấy tài khoản.");

            if (!PasswordHasher.Verify(currentPassword, storedHash))
                return ServiceResult<bool>.Fail("Mật khẩu hiện tại không đúng.");

            var newHash = PasswordHasher.Hash(newPassword);
            _userRepo.UpdatePasswordHash(userId, newHash, mustChangePassword: false);

            _auditRepo.Write(userId, "Đổi mật khẩu", "Users", userId, "Người dùng tự đổi mật khẩu.");

            return ServiceResult<bool>.Ok(true, "Đã đổi mật khẩu thành công.");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Fail("Lỗi khi đổi mật khẩu: " + ex.Message);
        }
    }

    internal static UserDto? AuthenticateDemoUser(string username, string password)
    {
        return null;
    }

    private static ServiceResult<bool> ValidateAccount(UserAccountDto account, bool isCreate)
    {
        if (!isCreate && account.Id <= 0)
            return ServiceResult<bool>.Fail("Id tài khoản không hợp lệ.");

        if (string.IsNullOrWhiteSpace(account.Username))
            return ServiceResult<bool>.Fail("Tên đăng nhập là bắt buộc.");

        if (string.IsNullOrWhiteSpace(account.FullName))
            return ServiceResult<bool>.Fail("Họ tên là bắt buộc.");

        if (!Enum.IsDefined(account.Role))
            return ServiceResult<bool>.Fail("Vai trò không hợp lệ.");

        return ServiceResult<bool>.Ok(true);
    }

    private static UserAccountDto MapToAccountDto(UserDto user)
    {
        return new UserAccountDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            RoleName = PermissionService.GetRoleDisplayName(user.Role),
            IsActive = user.IsActive,
            CreatedAt = DateTime.MinValue,
            DemoPassword = string.Empty
        };
    }
}
