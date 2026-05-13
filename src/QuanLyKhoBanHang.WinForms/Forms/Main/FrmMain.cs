using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Auth;
using QuanLyKhoBanHang.WinForms.Forms.Admin;
using QuanLyKhoBanHang.WinForms.Forms.Assistant;
using QuanLyKhoBanHang.WinForms.Forms.Common;
using QuanLyKhoBanHang.WinForms.Forms.Dashboard;
using QuanLyKhoBanHang.WinForms.Forms.Inventory;
using QuanLyKhoBanHang.WinForms.Forms.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Reports;
using QuanLyKhoBanHang.WinForms.Forms.Sales;

namespace QuanLyKhoBanHang.WinForms.Forms.Main;

public sealed class FrmMain : Form
{
    private readonly UserDto _currentUser;
    private readonly PermissionService _permissionService = new();
    private readonly Dictionary<string, Func<Form>> _formFactories;
    private readonly Panel _contentHost = new();
    private readonly Label _titleLabel = new();
    private readonly Label _subtitleLabel = new();
    private readonly ToolStripStatusLabel _statusLabel = new();

    public FrmMain(UserDto currentUser)
    {
        _currentUser = currentUser;
        _formFactories = BuildFormFactories();

        Text = "Quản lý kho & bán hàng";
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1280, 780);
        BackColor = AppTheme.ShellBackground;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 236));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        root.Controls.Add(BuildSidebar(), 0, 0);
        root.Controls.Add(BuildShell(), 1, 0);
        Controls.Add(root);

        Shown += (_, _) => LoadDefaultView();
    }

    private Control BuildSidebar()
    {
        var sidebar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Sidebar,
            Padding = new Padding(18)
        };

        var permissions = _permissionService.GetAccessibleFeatures(_currentUser.Role).Data ?? [];
        var entries = BuildSidebarEntries(permissions);

        var stack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = entries.Count + 4,
            ColumnCount = 1
        };
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 8));

        stack.Controls.Add(new Label
        {
            Text = "QUẢN LÝ KHO\n& BÁN HÀNG",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = AppTheme.TitleFont(15F),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        stack.Controls.Add(BuildUserBadge(), 0, 1);

        var row = 3;
        foreach (var entry in entries)
        {
            if (entry.IsSection)
            {
                stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
                stack.Controls.Add(new Label
                {
                    Text = entry.Text.ToUpperInvariant(),
                    Dock = DockStyle.Fill,
                    ForeColor = AppTheme.SidebarTextMuted,
                    Font = AppTheme.SectionFont(8.5F),
                    TextAlign = ContentAlignment.BottomLeft
                }, 0, row++);
                continue;
            }

            stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            var button = CreateNavButton(entry.Text);
            button.Click += (_, _) => OpenFeature(entry.FeatureKey);
            stack.Controls.Add(button, 0, row++);
        }

        stack.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        sidebar.Controls.Add(stack);
        return sidebar;
    }

    private Control BuildUserBadge()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 54));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 46));

        panel.Controls.Add(new Label
        {
            Text = _currentUser.FullName,
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = AppTheme.SectionFont(10F),
            TextAlign = ContentAlignment.BottomLeft,
            AutoEllipsis = true
        }, 0, 0);

        panel.Controls.Add(new Label
        {
            Text = PermissionService.GetRoleDisplayName(_currentUser.Role),
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.SidebarTextMuted,
            Font = AppTheme.BodyFont(9F),
            TextAlign = ContentAlignment.TopLeft,
            AutoEllipsis = true
        }, 0, 1);

        return panel;
    }

    private Control BuildShell()
    {
        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(0, 0, 18, 18),
            BackColor = Color.Transparent
        };
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(20, 16, 20, 16)
        };

        _titleLabel.Text = "Sẵn sàng";
        _titleLabel.Font = AppTheme.TitleFont();
        _titleLabel.Dock = DockStyle.Top;
        _titleLabel.Height = 36;

        _subtitleLabel.Text = $"Đăng nhập: {_currentUser.FullName} - {PermissionService.GetRoleDisplayName(_currentUser.Role)}.";
        _subtitleLabel.Font = AppTheme.BodyFont();
        _subtitleLabel.ForeColor = AppTheme.TextMuted;
        _subtitleLabel.Dock = DockStyle.Top;
        _subtitleLabel.Height = 24;

        var quickActions = BuildQuickActions();
        header.Controls.Add(quickActions);
        header.Controls.Add(_subtitleLabel);
        header.Controls.Add(_titleLabel);

        _contentHost.Dock = DockStyle.Fill;
        _contentHost.BackColor = AppTheme.Surface;
        _contentHost.Padding = new Padding(18);

        var statusStrip = new StatusStrip
        {
            SizingGrip = false,
            BackColor = AppTheme.ShellBackground
        };
        _statusLabel.Text = "Sẵn sàng";
        statusStrip.Items.Add(_statusLabel);

        shell.Controls.Add(header, 0, 0);
        shell.Controls.Add(_contentHost, 0, 1);
        shell.Controls.Add(statusStrip, 0, 2);
        return shell;
    }

    private Control BuildQuickActions()
    {
        var quickActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 430,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 4, 0, 0)
        };

        AddQuickActionIfAllowed(quickActions, PermissionService.FeatureSalesInvoice, "Bán hàng", 92);
        AddQuickActionIfAllowed(quickActions, PermissionService.FeaturePurchaseReceipt, "Nhập kho", 92);
        AddQuickActionIfAllowed(quickActions, PermissionService.FeatureReport, "Báo cáo", 92);
        AddQuickActionIfAllowed(quickActions, PermissionService.FeatureAssistant, "Trợ lý AI", 104);
        return quickActions;
    }

    private void AddQuickActionIfAllowed(FlowLayoutPanel panel, string featureKey, string text, int width)
    {
        var result = _permissionService.CanAccess(_currentUser.Role, featureKey);
        if (!result.Success || result.Data != true)
        {
            return;
        }

        panel.Controls.Add(CreateActionButton(text, featureKey, width));
    }

    private void LoadDefaultView()
    {
        var result = _permissionService.GetDefaultFeature(_currentUser.Role);
        if (!result.Success || string.IsNullOrWhiteSpace(result.Data))
        {
            _statusLabel.Text = result.Message;
            return;
        }

        OpenFeature(result.Data);
    }

    private void OpenFeature(string featureKey)
    {
        var access = _permissionService.CanAccess(_currentUser.Role, featureKey);
        if (!access.Success || access.Data != true)
        {
            MessageBox.Show(
                "Vai trò hiện tại không có quyền mở màn hình này.",
                "Phân quyền",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            _statusLabel.Text = "Đã chặn truy cập trái quyền.";
            return;
        }

        if (!_formFactories.TryGetValue(featureKey, out var factory))
        {
            MessageBox.Show(
                "Màn hình chưa được cấu hình trong shell.",
                "Điều hướng",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        LoadView(factory());
    }

    private Button CreateNavButton(string text)
    {
        var button = UiFactory.SidebarButton(text);
        button.Height = 38;
        button.Margin = new Padding(0, 0, 0, 4);
        return button;
    }

    private Button CreateActionButton(string text, string featureKey, int width)
    {
        var button = UiFactory.ActionButton(text, (_, _) => OpenFeature(featureKey), width);
        button.Height = 34;
        return button;
    }

    private void LoadView(Form form)
    {
        foreach (Control control in _contentHost.Controls)
        {
            control.Dispose();
        }

        _contentHost.Controls.Clear();
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        _contentHost.Controls.Add(form);
        form.Show();

        _titleLabel.Text = form.Text;
        _subtitleLabel.Text = $"Đang mở: {form.Text} - người dùng {_currentUser.FullName} ({PermissionService.GetRoleDisplayName(_currentUser.Role)}).";
        _statusLabel.Text = $"Đã chuyển sang {form.Text}";
    }

    private static Dictionary<string, Func<Form>> BuildFormFactories()
    {
        return new Dictionary<string, Func<Form>>(StringComparer.OrdinalIgnoreCase)
        {
            [PermissionService.FeatureDashboard] = () => new FrmDashboard(),
            [PermissionService.FeatureProduct] = () => new FrmProduct(),
            [PermissionService.FeatureCategory] = () => new FrmCategory(),
            [PermissionService.FeatureSupplier] = () => new FrmSupplier(),
            [PermissionService.FeatureCustomer] = () => new FrmCustomer(),
            [PermissionService.FeaturePurchaseReceipt] = () => new FrmPurchaseReceipt(),
            [PermissionService.FeatureInventory] = () => new FrmInventory(),
            [PermissionService.FeatureStocktake] = () => new FrmStocktake(),
            [PermissionService.FeatureSalesInvoice] = () => new FrmSalesInvoice(),
            [PermissionService.FeatureReport] = () => new FrmReport(),
            [PermissionService.FeatureAssistant] = () => new FrmAssistant(),
            [PermissionService.FeatureUserManagement] = () => new FrmUserManagement(),
            [PermissionService.FeatureRolePermission] = () => new FrmRolePermission(),
            [PermissionService.FeatureAuditLog] = () => new FrmAuditLog()
        };
    }

    private static List<SidebarEntry> BuildSidebarEntries(IReadOnlyList<QuanLyKhoBanHang.DTO.Admin.RolePermissionDto> permissions)
    {
        var entries = new List<SidebarEntry>();
        var adminSectionAdded = false;

        foreach (var permission in permissions)
        {
            if (permission.GroupName == "Quản trị" && !adminSectionAdded)
            {
                entries.Add(SidebarEntry.Section("Quản trị"));
                adminSectionAdded = true;
            }

            entries.Add(SidebarEntry.Button(permission.FeatureKey, permission.FeatureName));
        }

        return entries;
    }

    private sealed record SidebarEntry(string FeatureKey, string Text, bool IsSection)
    {
        public static SidebarEntry Button(string featureKey, string text) => new(featureKey, text, false);
        public static SidebarEntry Section(string text) => new(string.Empty, text, true);
    }
}
