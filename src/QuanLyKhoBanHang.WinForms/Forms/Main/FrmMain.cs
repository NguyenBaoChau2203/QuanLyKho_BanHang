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
    private readonly Panel _contentHost = new();
    private readonly Label _titleLabel = new();
    private readonly Label _subtitleLabel = new();
    private readonly ToolStripStatusLabel _statusLabel = new();

    public FrmMain(string fullName)
    {
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

        root.Controls.Add(BuildSidebar(fullName), 0, 0);
        root.Controls.Add(BuildShell(), 1, 0);
        Controls.Add(root);

        Shown += (_, _) => LoadView(new FrmDashboard());
    }

    private Control BuildSidebar(string fullName)
    {
        var sidebar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Sidebar,
            Padding = new Padding(18)
        };

        var stack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 12,
            ColumnCount = 1,
        };
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
        for (var i = 3; i < 12; i++)
        {
            stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        }

        stack.Controls.Add(new Label
        {
            Text = "QUẢN LÝ KHO\n& BÁN HÀNG",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = AppTheme.TitleFont(16F),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        stack.Controls.Add(new Label
        {
            Text = fullName,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.SidebarTextMuted,
            Font = AppTheme.BodyFont(),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);

        AddNavButton(stack, "Dashboard", () => new FrmDashboard(), 3);
        AddNavButton(stack, "Sản phẩm", () => new FrmProduct(), 4);
        AddNavButton(stack, "Loại hàng", () => new FrmCategory(), 5);
        AddNavButton(stack, "Nhà cung cấp", () => new FrmSupplier(), 6);
        AddNavButton(stack, "Khách hàng", () => new FrmCustomer(), 7);
        AddNavButton(stack, "Nhập kho", () => new FrmPurchaseReceipt(), 8);
        AddNavButton(stack, "Tồn kho", () => new FrmInventory(), 9);
        AddNavButton(stack, "Kiểm kê", () => new FrmStocktake(), 10);
        AddNavButton(stack, "Bán hàng", () => new FrmSalesInvoice(), 11);

        sidebar.Controls.Add(stack);
        return sidebar;
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

        _titleLabel.Text = "Dashboard";
        _titleLabel.Font = AppTheme.TitleFont();
        _titleLabel.Dock = DockStyle.Top;
        _titleLabel.Height = 36;

        _subtitleLabel.Text = "Sẵn sàng cho demo và tích hợp backend theo từng phase.";
        _subtitleLabel.Font = AppTheme.BodyFont();
        _subtitleLabel.ForeColor = AppTheme.TextMuted;
        _subtitleLabel.Dock = DockStyle.Top;
        _subtitleLabel.Height = 24;

        var quickActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 320,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 4, 0, 0)
        };
        quickActions.Controls.Add(CreateActionButton("Bán hàng", () => new FrmSalesInvoice()));
        quickActions.Controls.Add(CreateActionButton("Báo cáo", () => new FrmReport()));
        quickActions.Controls.Add(CreateActionButton("Trợ lý", () => new FrmAssistant()));

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

    private void AddNavButton(TableLayoutPanel stack, string text, Func<Form> factory, int row)
    {
        var button = CreateNavButton(text);
        button.Click += (_, _) => LoadView(factory());
        stack.Controls.Add(button, 0, row);
    }

    private Button CreateNavButton(string text)
    {
        return UiFactory.SidebarButton(text);
    }

    private Button CreateActionButton(string text, Func<Form> factory)
    {
        var button = UiFactory.ActionButton(text, (_, _) => LoadView(factory()), 88);
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
        _subtitleLabel.Text = $"Đang mở: {form.Text}";
        _statusLabel.Text = $"Đã chuyển sang {form.Text}";
    }
}
