namespace QuanLyKhoBanHang.WinForms.Forms.Main;

public sealed class FrmMain : Form
{
    private readonly Panel _contentPanel = new();

    public FrmMain(string fullName)
    {
        Text = "Quản lý kho & bán hàng";
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1180, 720);

        var menuPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            Width = 220,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(12),
            BackColor = Color.FromArgb(245, 247, 250)
        };

        var header = new Label
        {
            Text = $"Xin chào, {fullName}",
            Width = 190,
            Height = 56,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        menuPanel.Controls.Add(header);

        AddMenuButton(menuPanel, "Dashboard", () => new Dashboard.FrmDashboard());
        AddMenuButton(menuPanel, "Sản phẩm", () => new MasterData.FrmProduct());
        AddMenuButton(menuPanel, "Loại hàng", () => new MasterData.FrmCategory());
        AddMenuButton(menuPanel, "Nhà cung cấp", () => new MasterData.FrmSupplier());
        AddMenuButton(menuPanel, "Khách hàng", () => new Sales.FrmCustomer());
        AddMenuButton(menuPanel, "Nhập kho", () => new Inventory.FrmPurchaseReceipt());
        AddMenuButton(menuPanel, "Tồn kho", () => new Inventory.FrmInventory());
        AddMenuButton(menuPanel, "Kiểm kê", () => new Inventory.FrmStocktake());
        AddMenuButton(menuPanel, "Bán hàng", () => new Sales.FrmSalesInvoice());
        AddMenuButton(menuPanel, "Báo cáo", () => new Reports.FrmReport());
        AddMenuButton(menuPanel, "Trợ lý", () => new Assistant.FrmAssistant());

        _contentPanel.Dock = DockStyle.Fill;
        Controls.Add(_contentPanel);
        Controls.Add(menuPanel);

        LoadChild(new Dashboard.FrmDashboard());
    }

    private void AddMenuButton(FlowLayoutPanel menuPanel, string text, Func<Form> createForm)
    {
        var button = new Button
        {
            Text = text,
            Width = 190,
            Height = 38,
            TextAlign = ContentAlignment.MiddleLeft
        };
        button.Click += (_, _) => LoadChild(createForm());
        menuPanel.Controls.Add(button);
    }

    private void LoadChild(Form child)
    {
        foreach (Control control in _contentPanel.Controls)
        {
            control.Dispose();
        }

        _contentPanel.Controls.Clear();
        child.TopLevel = false;
        child.FormBorderStyle = FormBorderStyle.None;
        child.Dock = DockStyle.Fill;
        _contentPanel.Controls.Add(child);
        child.Show();
    }
}
