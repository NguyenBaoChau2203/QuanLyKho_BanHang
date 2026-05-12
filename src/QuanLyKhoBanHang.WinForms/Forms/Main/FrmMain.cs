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
        BackColor = Color.FromArgb(242, 244, 248);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 248));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var sidebar = BuildSidebar(fullName);
        var shell = BuildShell();

        root.Controls.Add(sidebar, 0, 0);
        root.Controls.Add(shell, 1, 0);
        Controls.Add(root);

        LoadView(new FrmDashboard());
    }

    private Control BuildSidebar(string fullName)
    {
        var sidebar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(28, 47, 73),
            Padding = new Padding(18)
        };

        var stack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 10,
            ColumnCount = 1,
        };
        for (var i = 0; i < 10; i++)
        {
            stack.RowStyles.Add(i == 0 ? new RowStyle(SizeType.Absolute, 92) : new RowStyle(SizeType.Absolute, 46));
        }

        stack.Controls.Add(new Label
        {
            Text = "QUẢN LÝ KHO\n& BÁN HÀNG",
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        stack.Controls.Add(new Label
        {
            Text = fullName,
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(190, 205, 225),
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);

        AddNavButton(stack, "Dashboard", () => new FrmDashboard(), 2);
        AddNavButton(stack, "Sản phẩm", () => new FrmProduct(), 3);
        AddNavButton(stack, "Loại hàng", () => new FrmCategory(), 4);
        AddNavButton(stack, "Nhà cung cấp", () => new FrmSupplier(), 5);
        AddNavButton(stack, "Khách hàng", () => new FrmCustomer(), 6);
        AddNavButton(stack, "Nhập kho", () => new FrmPurchaseReceipt(), 7);
        AddNavButton(stack, "Tồn kho", () => new FrmInventory(), 8);
        AddNavButton(stack, "Kiểm kê", () => new FrmStocktake(), 9);

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
            BackColor = Color.White,
            Padding = new Padding(24, 18, 24, 18)
        };

        _titleLabel.Text = "Dashboard";
        _titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        _titleLabel.Dock = DockStyle.Top;
        _titleLabel.Height = 36;

        _subtitleLabel.Text = "Sẵn sàng cho demo và tích hợp backend theo từng phase.";
        _subtitleLabel.Font = new Font("Segoe UI", 10F);
        _subtitleLabel.ForeColor = Color.FromArgb(96, 108, 129);
        _subtitleLabel.Dock = DockStyle.Top;
        _subtitleLabel.Height = 24;

        var quickActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 300,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 6, 0, 0)
        };
        quickActions.Controls.Add(CreateActionButton("Bán hàng", () => new FrmSalesInvoice()));
        quickActions.Controls.Add(CreateActionButton("Báo cáo", () => new FrmReport()));
        quickActions.Controls.Add(CreateActionButton("Trợ lý", () => new FrmAssistant()));

        header.Controls.Add(quickActions);
        header.Controls.Add(_subtitleLabel);
        header.Controls.Add(_titleLabel);

        _contentHost.Dock = DockStyle.Fill;
        _contentHost.BackColor = Color.White;
        _contentHost.Padding = new Padding(18);

        var statusStrip = new StatusStrip
        {
            SizingGrip = false,
            BackColor = Color.FromArgb(242, 244, 248)
        };
        _statusLabel.Text = "Sẵn sàng";
        statusStrip.Items.Add(_statusLabel);

        shell.Controls.Add(header, 0, 0);
        shell.Controls.Add(_contentHost, 0, 1);
        shell.Controls.Add(statusStrip, 0, 2);
        Controls.Add(shell);
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
        return new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            Height = 40,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(42, 66, 98),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0)
        };
    }

    private Button CreateActionButton(string text, Func<Form> factory)
    {
        var button = new Button
        {
            Text = text,
            Width = 88,
            Height = 34,
            Margin = new Padding(0, 0, 8, 0)
        };
        button.Click += (_, _) => LoadView(factory());
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
