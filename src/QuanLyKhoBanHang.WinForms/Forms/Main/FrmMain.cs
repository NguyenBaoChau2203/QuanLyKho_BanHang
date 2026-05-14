using FontAwesome.Sharp;
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
    private readonly Dictionary<string, Button> _navButtons = new(StringComparer.OrdinalIgnoreCase);
    private readonly Panel _contentHost = new();
    private readonly Label _titleLabel = new();
    private readonly Label _subtitleLabel = new();
    private readonly ToolStripStatusLabel _statusLabel = new();

    public bool LogoutRequested { get; private set; }

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
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 248));
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
            Padding = new Padding(16, 18, 16, 18)
        };

        var permissions = _permissionService.GetAccessibleFeatures(_currentUser.Role).Data ?? [];
        var entries = BuildSidebarEntries(permissions);

        var stack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5
        };
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
        stack.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        stack.Controls.Add(BuildBrand(), 0, 0);
        stack.Controls.Add(BuildUserBadge(), 0, 1);
        stack.Controls.Add(BuildMenu(entries), 0, 3);
        stack.Controls.Add(BuildLogoutButton(), 0, 4);
        sidebar.Controls.Add(stack);
        return sidebar;
    }

    private static Control BuildBrand()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new IconPictureBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            IconChar = IconChar.BoxesStacked,
            IconColor = Color.White,
            IconFont = IconFont.Auto,
            IconSize = 28,
            Padding = new Padding(0, 14, 10, 14)
        }, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = "QuanLyKhoBanHang",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = AppTheme.SectionFont(11F),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 1, 0);

        return layout;
    }

    private Control BuildUserBadge()
    {
        var badge = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            FillColor = Color.FromArgb(34, 57, 87),
            BorderColor = Color.FromArgb(52, 78, 115),
            Radius = 8,
            ShadowSize = 0,
            Padding = new Padding(10, 8, 10, 8)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new IconPictureBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            IconChar = IconChar.CircleUser,
            IconColor = AppTheme.SidebarTextMuted,
            IconFont = IconFont.Auto,
            IconSize = 34,
            Padding = new Padding(0, 5, 10, 5)
        }, 0, 0);

        var textStack = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 9, 0, 0)
        };
        var nameLabel = new Label
        {
            Text = _currentUser.FullName,
            Width = 140,
            Height = 22,
            ForeColor = Color.White,
            Font = AppTheme.SectionFont(9.5F),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            Margin = Padding.Empty
        };

        var roleLabel = new Label
        {
            Text = PermissionService.GetRoleDisplayName(_currentUser.Role),
            Width = 140,
            Height = 20,
            ForeColor = AppTheme.SidebarTextMuted,
            Font = AppTheme.BodyFont(8.8F),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            Margin = new Padding(0, 1, 0, 0)
        };

        textStack.Controls.Add(nameLabel);
        textStack.Controls.Add(roleLabel);
        textStack.SizeChanged += (_, _) =>
        {
            var width = Math.Max(80, textStack.ClientSize.Width - 2);
            nameLabel.Width = width;
            roleLabel.Width = width;
        };

        layout.Controls.Add(textStack, 1, 0);
        badge.Controls.Add(layout);
        return badge;
    }

    private Control BuildMenu(IReadOnlyList<SidebarEntry> entries)
    {
        var menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(0, 2, 0, 0)
        };

        foreach (var entry in entries)
        {
            if (entry.IsSection)
            {
                menu.Controls.Add(new Label
                {
                    Text = entry.Text.ToUpperInvariant(),
                    Height = 30,
                    Margin = new Padding(2, 8, 0, 2),
                    ForeColor = AppTheme.SidebarTextMuted,
                    Font = AppTheme.SectionFont(8.5F),
                    TextAlign = ContentAlignment.BottomLeft
                });
                continue;
            }

            var button = CreateNavButton(entry);
            button.Click += (_, _) => OpenFeature(entry.FeatureKey);
            _navButtons[entry.FeatureKey] = button;
            menu.Controls.Add(button);
        }

        menu.SizeChanged += (_, _) => ResizeSidebarMenuItems(menu);
        menu.HandleCreated += (_, _) => ResizeSidebarMenuItems(menu);
        return menu;
    }

    private Control BuildLogoutButton()
    {
        var button = new IconButton
        {
            Text = "Đăng xuất",
            Dock = DockStyle.Fill,
            Height = 42,
            Margin = new Padding(0, 6, 0, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(54, 73, 104),
            ForeColor = Color.White,
            Font = AppTheme.BodyFont(),
            IconChar = IconChar.RightFromBracket,
            IconColor = AppTheme.SidebarTextMuted,
            IconFont = IconFont.Auto,
            IconSize = 18,
            TextAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0),
            UseVisualStyleBackColor = false,
            Cursor = Cursors.Hand
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(69, 92, 126);
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(84, 105, 137);
        button.Click += HandleLogout;
        return button;
    }

    private static void ResizeSidebarMenuItems(FlowLayoutPanel menu)
    {
        var width = Math.Max(120, menu.ClientSize.Width - 8);
        foreach (Control control in menu.Controls)
        {
            control.Width = width;
        }
    }

    private Control BuildShell()
    {
        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(0, 0, 0, 0),
            BackColor = AppTheme.ShellBackground
        };
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(22, 16, 22, 14)
        };

        _titleLabel.Text = "Sẵn sàng";
        _titleLabel.Font = AppTheme.TitleFont();
        _titleLabel.ForeColor = AppTheme.Text;
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
        _contentHost.BackColor = AppTheme.AppBackground;
        _contentHost.Padding = Padding.Empty;

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
            Width = 450,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 4, 0, 0)
        };

        AddQuickActionIfAllowed(quickActions, PermissionService.FeatureAssistant, "Trợ lý AI", IconChar.Robot, 112);
        AddQuickActionIfAllowed(quickActions, PermissionService.FeatureReport, "Báo cáo", IconChar.ChartBar, 104);
        AddQuickActionIfAllowed(quickActions, PermissionService.FeaturePurchaseReceipt, "Nhập kho", IconChar.TruckRampBox, 112);
        AddQuickActionIfAllowed(quickActions, PermissionService.FeatureSalesInvoice, "Bán hàng", IconChar.CartShopping, 112);
        return quickActions;
    }

    private void AddQuickActionIfAllowed(FlowLayoutPanel panel, string featureKey, string text, IconChar icon, int width)
    {
        var result = _permissionService.CanAccess(_currentUser.Role, featureKey);
        if (!result.Success || result.Data != true)
        {
            return;
        }

        panel.Controls.Add(CreateActionButton(text, icon, featureKey, width));
    }

    private void LoadDefaultView()
    {
        var result = _permissionService.GetDefaultFeature(_currentUser.Role);
        if (!result.Success || string.IsNullOrWhiteSpace(result.Data))
        {
            _statusLabel.Text = result.Message;
            return;
        }

        OpenFeature(PermissionService.FeatureSupplier);
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
        SetActiveNav(featureKey);
    }

    private void HandleLogout(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "Bạn muốn đăng xuất và quay lại màn hình đăng nhập?",
            "Đăng xuất",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        LogoutRequested = true;
        _statusLabel.Text = "Đang đăng xuất...";
        Close();
    }

    private IconButton CreateNavButton(SidebarEntry entry)
    {
        var button = UiFactory.SidebarButton(entry.Text, GetFeatureIcon(entry.FeatureKey));
        button.Height = 42;
        button.Margin = new Padding(0, 0, 0, 5);
        return button;
    }

    private Button CreateActionButton(string text, IconChar icon, string featureKey, int width)
    {
        var button = UiFactory.IconActionButton(text, icon, (_, _) => OpenFeature(featureKey), width);
        button.Height = 34;
        return button;
    }

    private void SetActiveNav(string featureKey)
    {
        foreach (var pair in _navButtons)
        {
            UiFactory.SetSidebarButtonState(pair.Value, pair.Key.Equals(featureKey, StringComparison.OrdinalIgnoreCase));
        }
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

    private static IconChar GetFeatureIcon(string featureKey)
    {
        return featureKey switch
        {
            PermissionService.FeatureDashboard => IconChar.House,
            PermissionService.FeatureProduct => IconChar.BoxOpen,
            PermissionService.FeatureCategory => IconChar.Tags,
            PermissionService.FeatureSupplier => IconChar.Truck,
            PermissionService.FeatureCustomer => IconChar.Users,
            PermissionService.FeaturePurchaseReceipt => IconChar.TruckRampBox,
            PermissionService.FeatureInventory => IconChar.Warehouse,
            PermissionService.FeatureStocktake => IconChar.ClipboardCheck,
            PermissionService.FeatureSalesInvoice => IconChar.CartShopping,
            PermissionService.FeatureReport => IconChar.ChartBar,
            PermissionService.FeatureAssistant => IconChar.Robot,
            PermissionService.FeatureUserManagement => IconChar.UserGear,
            PermissionService.FeatureRolePermission => IconChar.ShieldHalved,
            PermissionService.FeatureAuditLog => IconChar.ClockRotateLeft,
            _ => IconChar.Circle
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
