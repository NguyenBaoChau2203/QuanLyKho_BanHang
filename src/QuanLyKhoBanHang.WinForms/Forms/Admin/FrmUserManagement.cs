using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Admin;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Admin;

public sealed class FrmUserManagement : Form
{
    private readonly UserAccountService _service = new();
    private readonly BindingSource _source = new();
    private readonly DataGridView _grid = new();
    private readonly TextBox _searchBox = new();
    private readonly TextBox _usernameBox = new();
    private readonly TextBox _fullNameBox = new();
    private readonly TextBox _passwordBox = new();
    private readonly ComboBox _roleBox = new();
    private readonly CheckBox _activeBox = new();
    private readonly Label _messageLabel = new();

    private List<UserAccountDto> _items = [];
    private int _selectedId;
    private bool _isEditing;

    public FrmUserManagement()
    {
        Text = "Tài khoản";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1180, 720);

        BuildUi();
        RefreshData();
        SetEditorEnabled(false);
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = AppTheme.PagePadding
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        root.Controls.Add(UiFactory.HeaderPanel(
            "Quản lý tài khoản",
            "Quản trị viên xem và cập nhật tài khoản demo theo vai trò. Dữ liệu đang chạy ở chế độ stub an toàn."), 0, 0);
        root.Controls.Add(BuildSearchBar(), 0, 1);
        root.Controls.Add(BuildBody(), 0, 2);

        _messageLabel.Dock = DockStyle.Fill;
        _messageLabel.TextAlign = ContentAlignment.MiddleLeft;
        _messageLabel.ForeColor = AppTheme.StatusText;
        root.Controls.Add(_messageLabel, 0, 3);

        Controls.Add(root);
    }

    private Control BuildSearchBar()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(0, 8, 0, 0)
        };

        panel.Controls.Add(new Label { Text = "Tìm kiếm", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });

        _searchBox.Width = 320;
        _searchBox.PlaceholderText = "Tên đăng nhập, họ tên hoặc vai trò";
        _searchBox.TextChanged += (_, _) => ApplyFilter();
        panel.Controls.Add(_searchBox);

        var refreshButton = UiFactory.ActionButton("Làm mới", (_, _) => RefreshData(), 100);
        refreshButton.Margin = new Padding(12, 0, 0, 0);
        panel.Controls.Add(refreshButton);

        return panel;
    }

    private Control BuildBody()
    {
        var splitter = UiFactory.HorizontalSplitter(760, 320);

        _grid.DataSource = _source;
        _grid.AutoGenerateColumns = false;
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.AllowUserToResizeRows = false;
        UiFactory.StyleGrid(_grid);
        BuildGridColumns();
        _grid.SelectionChanged += (_, _) => FillEditorFromSelection();

        splitter.Panel1.Controls.Add(_grid);
        splitter.Panel2.Padding = new Padding(12, 0, 0, 0);
        splitter.Panel2.Controls.Add(BuildEditor());
        return splitter;
    }

    private Control BuildEditor()
    {
        var panel = UiFactory.Card();
        panel.Margin = Padding.Empty;
        panel.Padding = new Padding(16);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 1,
            RowCount = 8,
            AutoSize = true
        };

        _roleBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _roleBox.DataSource = RoleOption.CreateAll();
        _roleBox.DisplayMember = nameof(RoleOption.Text);
        _roleBox.ValueMember = nameof(RoleOption.Value);

        _passwordBox.UseSystemPasswordChar = true;
        _passwordBox.PlaceholderText = "Để trống sẽ giữ nguyên hoặc dùng 123456 khi tạo mới";

        _activeBox.Text = "Đang hoạt động";
        _activeBox.AutoSize = true;
        _activeBox.Margin = new Padding(0, 6, 0, 8);

        layout.Controls.Add(new Label
        {
            Text = "Thông tin tài khoản",
            Dock = DockStyle.Top,
            Height = 34,
            Font = AppTheme.SectionFont(12F)
        }, 0, 0);
        layout.Controls.Add(BuildTextField("Tên đăng nhập", _usernameBox), 0, 1);
        layout.Controls.Add(BuildTextField("Họ tên", _fullNameBox), 0, 2);
        layout.Controls.Add(BuildComboField("Vai trò", _roleBox), 0, 3);
        layout.Controls.Add(BuildTextField("Mật khẩu demo", _passwordBox), 0, 4);
        layout.Controls.Add(_activeBox, 0, 5);
        layout.Controls.Add(BuildActionBar(), 0, 6);
        layout.Controls.Add(new Label
        {
            Text = "Mật khẩu chỉ dùng cho demo trong bộ nhớ, không hiển thị trên danh sách và không thay thế cơ chế bảo mật thật.",
            Dock = DockStyle.Top,
            Height = 54,
            ForeColor = AppTheme.TextMuted
        }, 0, 7);

        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildActionBar()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 86,
            WrapContents = true
        };

        panel.Controls.Add(UiFactory.ActionButton("Thêm", (_, _) => BeginAdd(), 92));
        panel.Controls.Add(UiFactory.ActionButton("Sửa", (_, _) => BeginEdit(), 92));
        panel.Controls.Add(UiFactory.ActionButton("Lưu", (_, _) => SaveCurrent(), 92));
        panel.Controls.Add(UiFactory.ActionButton("Hủy", (_, _) => CancelEdit(), 92));
        panel.Controls.Add(UiFactory.ActionButton("Ngừng kích hoạt", (_, _) => DeactivateCurrent(), 150));
        return panel;
    }

    private static Control BuildTextField(string label, TextBox box)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = 68, Padding = new Padding(0, 0, 0, 8) };
        box.Dock = DockStyle.Bottom;
        box.Height = 30;
        panel.Controls.Add(box);
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Top, Height = 22 });
        return panel;
    }

    private static Control BuildComboField(string label, ComboBox box)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = 68, Padding = new Padding(0, 0, 0, 8) };
        box.Dock = DockStyle.Bottom;
        box.Height = 30;
        panel.Controls.Add(box);
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Top, Height = 22 });
        return panel;
    }

    private void BuildGridColumns()
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.Username), HeaderText = "Tên đăng nhập", Width = 150 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.FullName), HeaderText = "Họ tên", Width = 190 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.RoleName), HeaderText = "Vai trò", Width = 150 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.ActiveStatus), HeaderText = "Trạng thái", Width = 130 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.CreatedDate), HeaderText = "Ngày tạo", Width = 120 });
    }

    private void RefreshData()
    {
        var result = _service.GetAllAccounts();
        _items = result.Success ? result.Data ?? [] : [];
        ApplyFilter();
        SetMessage(result.Message, !result.Success);
    }

    private void ApplyFilter()
    {
        var keyword = _searchBox.Text.Trim();
        var filtered = string.IsNullOrWhiteSpace(keyword)
            ? _items
            : _items.Where(x =>
                x.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || x.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || x.RoleName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

        _source.DataSource = new BindingList<UserAccountRow>(filtered.Select(UserAccountRow.FromDto).ToList());
    }

    private void FillEditorFromSelection()
    {
        if (_source.Current is not UserAccountRow row || _isEditing)
        {
            return;
        }

        _selectedId = row.Id;
        _usernameBox.Text = row.Username;
        _fullNameBox.Text = row.FullName;
        _roleBox.SelectedValue = row.Role;
        _activeBox.Checked = row.IsActive;
        _passwordBox.Clear();
    }

    private void BeginAdd()
    {
        _selectedId = 0;
        _usernameBox.Clear();
        _fullNameBox.Clear();
        _passwordBox.Clear();
        _roleBox.SelectedValue = UserRole.Manager;
        _activeBox.Checked = true;
        SetEditorEnabled(true);
        SetMessage("Đang tạo tài khoản demo mới.");
    }

    private void BeginEdit()
    {
        if (_selectedId <= 0)
        {
            SetMessage("Vui lòng chọn tài khoản cần sửa.", true);
            return;
        }

        SetEditorEnabled(true);
        SetMessage("Đang chỉnh sửa tài khoản đã chọn.");
    }

    private void SaveCurrent()
    {
        if (!_isEditing)
        {
            SetMessage("Hãy chọn Thêm hoặc Sửa trước khi lưu.", true);
            return;
        }

        var account = new UserAccountDto
        {
            Id = _selectedId,
            Username = _usernameBox.Text,
            FullName = _fullNameBox.Text,
            Role = _roleBox.SelectedValue is UserRole role ? role : UserRole.Manager,
            IsActive = _activeBox.Checked,
            DemoPassword = _passwordBox.Text
        };

        var result = _selectedId == 0
            ? ToSaveResult(_service.CreateAccount(account))
            : ToSaveResult(_service.UpdateAccount(account));

        if (!result.Success)
        {
            SetMessage(result.Message, true);
            return;
        }

        SetEditorEnabled(false);
        RefreshData();
        SetMessage(result.Message);
    }

    private void CancelEdit()
    {
        SetEditorEnabled(false);
        FillEditorFromSelection();
        SetMessage("Đã hủy chỉnh sửa.");
    }

    private void DeactivateCurrent()
    {
        if (_selectedId <= 0)
        {
            SetMessage("Vui lòng chọn tài khoản cần ngừng kích hoạt.", true);
            return;
        }

        var result = _service.DeactivateAccount(_selectedId);
        RefreshData();
        SetMessage(result.Message, !result.Success);
    }

    private void SetEditorEnabled(bool enabled)
    {
        _isEditing = enabled;
        _usernameBox.ReadOnly = !enabled;
        _fullNameBox.ReadOnly = !enabled;
        _passwordBox.ReadOnly = !enabled;
        _roleBox.Enabled = enabled;
        _activeBox.Enabled = enabled;
    }

    private void SetMessage(string message, bool isError = false)
    {
        UiFactory.SetMessage(_messageLabel, message, isError);
    }

    private static SaveResult ToSaveResult(QuanLyKhoBanHang.BLL.Common.ServiceResult<int> result)
    {
        return new SaveResult(result.Success, result.Message);
    }

    private static SaveResult ToSaveResult(QuanLyKhoBanHang.BLL.Common.ServiceResult<bool> result)
    {
        return new SaveResult(result.Success, result.Message);
    }

    private readonly record struct SaveResult(bool Success, string Message);

    private sealed class RoleOption
    {
        public required UserRole Value { get; init; }
        public required string Text { get; init; }

        public static List<RoleOption> CreateAll() =>
        [
            new() { Value = UserRole.Admin, Text = PermissionService.GetRoleDisplayName(UserRole.Admin) },
            new() { Value = UserRole.Manager, Text = PermissionService.GetRoleDisplayName(UserRole.Manager) },
            new() { Value = UserRole.WarehouseStaff, Text = PermissionService.GetRoleDisplayName(UserRole.WarehouseStaff) },
            new() { Value = UserRole.SalesStaff, Text = PermissionService.GetRoleDisplayName(UserRole.SalesStaff) }
        ];
    }

    private sealed class UserAccountRow
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string ActiveStatus { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;

        public static UserAccountRow FromDto(UserAccountDto dto)
        {
            return new UserAccountRow
            {
                Id = dto.Id,
                Username = dto.Username,
                FullName = dto.FullName,
                Role = dto.Role,
                RoleName = dto.RoleName,
                IsActive = dto.IsActive,
                ActiveStatus = dto.IsActive ? "Đang hoạt động" : "Ngừng kích hoạt",
                CreatedDate = dto.CreatedAt.ToString("dd/MM/yyyy")
            };
        }
    }
}
