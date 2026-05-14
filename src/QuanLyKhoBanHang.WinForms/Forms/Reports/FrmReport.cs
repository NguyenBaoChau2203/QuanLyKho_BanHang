using System.ComponentModel;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Reports;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Reports;

public sealed class FrmReport : Form
{
    private readonly ReportService _reportService = new();
    private readonly BindingSource _revenueSource = new();
    private readonly BindingSource _topProductsSource = new();
    private readonly BindingSource _topCustomersSource = new();
    private readonly DateTimePicker _fromDate = new();
    private readonly DateTimePicker _toDate = new();
    private readonly ComboBox _reportType = new();
    private readonly Label _statusLabel = new();
    private readonly Label _revenueTotalLabel = new();
    private readonly Label _invoiceCountLabel = new();
    private readonly Label _profitLabel = new();
    private readonly Label _quantitySoldLabel = new();
    private readonly Label _periodRevenueLabel = new();
    private readonly Label _periodInvoiceLabel = new();
    private readonly Label _periodProfitLabel = new();
    private readonly DataGridView _revenueGrid = new();
    private readonly DataGridView _topProductsGrid = new();
    private readonly DataGridView _topCustomersGrid = new();

    public FrmReport()
    {
        Text = "Báo cáo";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1260, 760);
        BuildUi();
        Load += (_, _) => ReloadReports();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = AppTheme.PagePadding
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildFilterBar(), 0, 1);
        root.Controls.Add(BuildMetrics(), 0, 2);
        root.Controls.Add(BuildBody(), 0, 3);
        root.Controls.Add(_statusLabel, 0, 4);
        Controls.Add(root);

        _statusLabel.Dock = DockStyle.Fill;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.ForeColor = AppTheme.StatusText;
    }

    private Control BuildHeader()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(18, 12, 18, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.Controls.Add(UiFactory.IconTile(IconChar.ChartBar, AppTheme.Primary, AppTheme.PrimarySoft, 46, 24), 0, 0);

        var text = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        text.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        text.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        text.Controls.Add(new Label
        {
            Text = "Không gian báo cáo",
            Dock = DockStyle.Fill,
            Font = AppTheme.TitleFont(17F),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        text.Controls.Add(new Label
        {
            Text = "Lọc thời gian, xem KPI, xu hướng doanh thu và các bảng top phục vụ thuyết trình.",
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(9.5F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);
        layout.Controls.Add(text, 1, 0);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildFilterBar()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(14, 12, 14, 12);

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        _fromDate.Format = DateTimePickerFormat.Custom;
        _fromDate.CustomFormat = "dd/MM/yyyy";
        _fromDate.Value = DateTime.Today.AddDays(-6);
        _toDate.Format = DateTimePickerFormat.Custom;
        _toDate.CustomFormat = "dd/MM/yyyy";
        _toDate.Value = DateTime.Today;
        _reportType.DropDownStyle = ComboBoxStyle.DropDownList;
        _reportType.Items.AddRange(new object[] { "Doanh thu", "Sản phẩm bán chạy", "Khách hàng", "Tổng hợp" });
        _reportType.SelectedIndex = 3;
        _reportType.SelectedIndexChanged += (_, _) => SetStatus($"Đang xem nhóm báo cáo: {_reportType.Text}.");

        layout.Controls.Add(BuildFilterField("Từ ngày", _fromDate, 124));
        layout.Controls.Add(BuildFilterField("Đến ngày", _toDate, 124));
        layout.Controls.Add(BuildFilterField("Loại báo cáo", _reportType, 172));
        layout.Controls.Add(new Label
        {
            Text = "Dữ liệu demo được lấy qua ReportService, không gọi trực tiếp DAL.",
            Width = 420,
            Height = 36,
            Margin = new Padding(14, 22, 10, 0),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft
        });
        layout.Controls.Add(CreateButton("Xem báo cáo", IconChar.RotateRight, (_, _) => ReloadReports(), AppTheme.Primary));
        layout.Controls.Add(CreateButton("Xuất Excel", IconChar.FileExport, (_, _) => ExportReport(), AppTheme.Success));

        card.Controls.Add(layout);
        return card;
    }

    private Control BuildMetrics()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
        for (var i = 0; i < 4; i++)
        {
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }

        layout.Controls.Add(UiFactory.MetricCard("Doanh thu", _revenueTotalLabel, IconChar.MoneyBillTrendUp, AppTheme.Success, AppTheme.SuccessSoft), 0, 0);
        layout.Controls.Add(UiFactory.MetricCard("Hóa đơn", _invoiceCountLabel, IconChar.FileInvoice, AppTheme.Primary, AppTheme.PrimarySoft), 1, 0);
        layout.Controls.Add(UiFactory.MetricCard("Lợi nhuận ước tính", _profitLabel, IconChar.ChartLine, AppTheme.Warning, AppTheme.WarningSoft), 2, 0);
        layout.Controls.Add(UiFactory.MetricCard("Số lượng bán", _quantitySoldLabel, IconChar.BoxesStacked, AppTheme.Danger, AppTheme.DangerSoft), 3, 0);
        return layout;
    }

    private Control BuildBody()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 42));

        var top = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        top.Controls.Add(BuildRevenueCard(), 0, 0);
        top.Controls.Add(BuildTopProductsCard(), 1, 0);
        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(BuildTopCustomersCard(), 0, 1);
        return layout;
    }

    private Control BuildRevenueCard()
    {
        var card = BuildSectionCard("Doanh thu theo ngày", "Xem xu hướng doanh thu trong khoảng thời gian đã chọn.", IconChar.ChartSimple);
        var layout = (TableLayoutPanel)card.Controls[0];
        var body = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        body.Controls.Add(BuildRevenueSummaryPanel(), 0, 0);
        ConfigureRevenueGrid();
        body.Controls.Add(_revenueGrid, 0, 1);
        layout.Controls.Add(body, 0, 1);
        return card;
    }

    private Control BuildTopProductsCard()
    {
        var card = BuildSectionCard("Top sản phẩm", "Sản phẩm bán chạy trong kỳ.", IconChar.Trophy);
        var layout = (TableLayoutPanel)card.Controls[0];
        ConfigureTopProductsGrid();
        layout.Controls.Add(_topProductsGrid, 0, 1);
        return card;
    }

    private Control BuildTopCustomersCard()
    {
        var card = BuildSectionCard("Top khách hàng", "Khách hàng có giá trị mua cao nhất trong kỳ.", IconChar.Users);
        card.Margin = new Padding(0);
        var layout = (TableLayoutPanel)card.Controls[0];
        ConfigureTopCustomersGrid();
        layout.Controls.Add(_topCustomersGrid, 0, 1);
        return card;
    }

    private Control BuildRevenueSummaryPanel()
    {
        var panel = UiFactory.SoftTile(AppTheme.SurfaceSubtle, AppTheme.Border, 8);
        panel.Dock = DockStyle.Fill;
        panel.Padding = new Padding(14, 10, 14, 10);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        layout.Controls.Add(BuildRevenueSummaryLabel(_periodRevenueLabel, AppTheme.Success), 0, 0);
        layout.Controls.Add(BuildRevenueSummaryLabel(_periodInvoiceLabel, AppTheme.Primary), 1, 0);
        layout.Controls.Add(BuildRevenueSummaryLabel(_periodProfitLabel, AppTheme.Warning), 2, 0);
        panel.Controls.Add(layout);
        return panel;
    }

    private static Control BuildRevenueSummaryLabel(Label label, Color color)
    {
        label.Dock = DockStyle.Fill;
        label.Margin = new Padding(0, 0, 12, 0);
        label.Font = AppTheme.SectionFont(10.5F);
        label.ForeColor = color;
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.AutoEllipsis = true;
        return label;
    }

    private static Control BuildSectionCard(string title, string subtitle, IconChar icon)
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 12, 12);
        card.Padding = new Padding(14, 12, 14, 12);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(BuildReportSectionHeader(title, subtitle, icon), 0, 0);
        card.Controls.Add(layout);
        return card;
    }

    private static Control BuildReportSectionHeader(string title, string subtitle, IconChar icon)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(0, 2, 0, 6)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));

        var iconBox = new IconPictureBox
        {
            IconChar = icon,
            IconColor = AppTheme.Primary,
            IconFont = IconFont.Auto,
            IconSize = 17,
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 1, 8, 0),
            SizeMode = PictureBoxSizeMode.CenterImage
        };
        layout.Controls.Add(iconBox, 0, 0);
        layout.SetRowSpan(iconBox, 2);

        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = AppTheme.SectionFont(11F),
            ForeColor = AppTheme.Primary,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 1, 0);
        layout.Controls.Add(new Label
        {
            Text = subtitle,
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(9F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.TopLeft,
            AutoEllipsis = true
        }, 1, 1);
        return layout;
    }

    private void ConfigureRevenueGrid()
    {
        ConfigureGrid(_revenueGrid, _revenueSource);
        _revenueGrid.AutoGenerateColumns = false;
        _revenueGrid.Columns.Clear();
        _revenueGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RevenueRow.Date), HeaderText = "Ngày", FillWeight = 90 });
        _revenueGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RevenueRow.InvoiceCount), HeaderText = "Hóa đơn", FillWeight = 76 });
        _revenueGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RevenueRow.Revenue), HeaderText = "Doanh thu", DefaultCellStyle = { Format = "N0" }, FillWeight = 110 });
        _revenueGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(RevenueRow.EstimatedProfit), HeaderText = "Lợi nhuận", DefaultCellStyle = { Format = "N0" }, FillWeight = 110 });
    }

    private void ConfigureTopProductsGrid()
    {
        ConfigureGrid(_topProductsGrid, _topProductsSource);
        _topProductsGrid.AutoGenerateColumns = false;
        _topProductsGrid.Columns.Clear();
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.Rank), HeaderText = "#", FillWeight = 42 });
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.ProductCode), HeaderText = "Mã SP", FillWeight = 80 });
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.ProductName), HeaderText = "Tên sản phẩm", FillWeight = 170 });
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.QuantitySold), HeaderText = "SL bán", FillWeight = 72 });
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.Revenue), HeaderText = "Doanh thu", DefaultCellStyle = { Format = "N0" }, FillWeight = 110 });
    }

    private void ConfigureTopCustomersGrid()
    {
        ConfigureGrid(_topCustomersGrid, _topCustomersSource);
        _topCustomersGrid.AutoGenerateColumns = false;
        _topCustomersGrid.Columns.Clear();
        _topCustomersGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerRow.Rank), HeaderText = "#", FillWeight = 42 });
        _topCustomersGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerRow.CustomerName), HeaderText = "Khách hàng", FillWeight = 220 });
        _topCustomersGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerRow.InvoiceCount), HeaderText = "Hóa đơn", FillWeight = 80 });
        _topCustomersGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerRow.TotalAmount), HeaderText = "Tổng mua", DefaultCellStyle = { Format = "N0" }, FillWeight = 120 });
    }

    private static void ConfigureGrid(DataGridView grid, BindingSource source)
    {
        UiFactory.StyleGrid(grid);
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToResizeRows = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.DataSource = source;
    }

    private void ReloadReports()
    {
        var from = _fromDate.Value.Date;
        var to = _toDate.Value.Date;
        if (from > to)
        {
            SetStatus("Ngày bắt đầu không được lớn hơn ngày kết thúc.", true);
            return;
        }

        var revenueResult = _reportService.GetRevenue(from, to);
        var productResult = _reportService.GetTopSellingProducts(from, to);
        var customerResult = _reportService.GetTopCustomers(from, to);

        var revenueRows = ApplyRevenue(revenueResult, from, to);
        var productRows = ApplyProducts(productResult);
        ApplyCustomers(customerResult);
        UpdateMetrics(revenueRows, productRows);
        SetStatus("Đã cập nhật báo cáo.");
    }

    private List<RevenueSummaryDto> ApplyRevenue(ServiceResult<List<RevenueSummaryDto>> result, DateTime from, DateTime to)
    {
        var rows = result.Success ? result.Data ?? [] : CreateStubRevenue(from, to);
        _revenueSource.DataSource = new BindingList<RevenueRow>(rows.Select(x => new RevenueRow
        {
            Date = x.Date.ToString("dd/MM/yyyy"),
            InvoiceCount = x.InvoiceCount,
            Revenue = x.Revenue,
            EstimatedProfit = x.EstimatedProfit
        }).ToList());
        if (!result.Success || result.Data is { Count: 0 })
        {
            SetStatus(result.Message, !result.Success);
        }

        return rows;
    }

    private List<ProductSalesSummaryDto> ApplyProducts(ServiceResult<List<ProductSalesSummaryDto>> result)
    {
        var rows = result.Success ? result.Data ?? [] : CreateStubTopProducts();
        _topProductsSource.DataSource = new BindingList<ProductSalesRow>(rows.Select((x, index) => new ProductSalesRow
        {
            Rank = index + 1,
            ProductCode = x.ProductCode,
            ProductName = x.ProductName,
            QuantitySold = x.QuantitySold,
            Revenue = x.Revenue
        }).ToList());
        return rows;
    }

    private void ApplyCustomers(ServiceResult<List<CustomerPurchaseSummaryDto>> result)
    {
        var rows = result.Success ? result.Data ?? [] : CreateStubTopCustomers();
        _topCustomersSource.DataSource = new BindingList<CustomerRow>(rows.Select((x, index) => new CustomerRow
        {
            Rank = index + 1,
            CustomerName = x.CustomerName,
            InvoiceCount = x.InvoiceCount,
            TotalAmount = x.TotalAmount
        }).ToList());
    }

    private void UpdateMetrics(IReadOnlyCollection<RevenueSummaryDto> revenueRows, IReadOnlyCollection<ProductSalesSummaryDto> productRows)
    {
        var revenue = revenueRows.Sum(x => x.Revenue);
        var invoices = revenueRows.Sum(x => x.InvoiceCount);
        var profit = revenueRows.Sum(x => x.EstimatedProfit);

        _revenueTotalLabel.Text = FormatMoney(revenue);
        _invoiceCountLabel.Text = invoices.ToString("N0");
        _profitLabel.Text = FormatMoney(profit);
        _quantitySoldLabel.Text = productRows.Sum(x => x.QuantitySold).ToString("N0");
        _periodRevenueLabel.Text = $"Doanh thu: {FormatMoney(revenue)}";
        _periodInvoiceLabel.Text = $"Hóa đơn: {invoices:N0}";
        _periodProfitLabel.Text = $"Lợi nhuận: {FormatMoney(profit)}";
    }

    private void SetStatus(string message, bool isError = false)
    {
        _statusLabel.Text = message;
        _statusLabel.ForeColor = isError ? AppTheme.Error : AppTheme.StatusText;
    }

    private void ExportReport()
    {
        SetStatus("Xuất báo cáo đang ở chế độ demo. Có thể dùng bảng trên màn hình để chụp báo cáo đồ án.");
    }

    private static Control BuildFilterField(string label, Control control, int width)
    {
        var panel = new Panel { Width = width, Height = 58, Margin = new Padding(0, 0, 12, 0) };
        panel.Controls.Add(new Label
        {
            Text = label,
            Bounds = new Rectangle(0, 0, width, 20),
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont(8.5F),
            TextAlign = ContentAlignment.BottomLeft
        });
        control.SetBounds(0, 24, width, 28);
        control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        control.Margin = Padding.Empty;
        if (control is ComboBox comboBox)
        {
            comboBox.Height = 28;
        }
        else if (control is DateTimePicker picker)
        {
            picker.Width = width;
        }

        panel.Controls.Add(control);
        return panel;
    }

    private static IconButton CreateButton(string text, IconChar icon, EventHandler handler, Color color)
    {
        var button = new IconButton
        {
            Text = text,
            Dock = DockStyle.None,
            Width = 118,
            Height = 36,
            MinimumSize = new Size(0, 36),
            Margin = new Padding(4, 22, 8, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = color,
            ForeColor = Color.White,
            IconChar = icon,
            IconColor = Color.White,
            IconFont = IconFont.Auto,
            IconSize = 15,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            TextAlign = ContentAlignment.MiddleCenter,
            ImageAlign = ContentAlignment.MiddleCenter,
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(color, 0.08F);
        button.Click += handler;
        return button;
    }

    private static string FormatMoney(decimal value) => $"{value:N0} đ";

    private static List<RevenueSummaryDto> CreateStubRevenue(DateTime from, DateTime to)
    {
        return DateTime.Today >= from.Date && DateTime.Today <= to.Date
            ? [new RevenueSummaryDto { Date = DateTime.Today, InvoiceCount = 2, Revenue = 304000, EstimatedProfit = 46000 }]
            : [];
    }

    private static List<ProductSalesSummaryDto> CreateStubTopProducts() =>
    [
        new ProductSalesSummaryDto { ProductId = 2, ProductCode = "SP002", ProductName = "Nước ngọt cola lon", QuantitySold = 12, Revenue = 132000 },
        new ProductSalesSummaryDto { ProductId = 4, ProductCode = "SP004", ProductName = "Nước rửa chén 750ml", QuantitySold = 4, Revenue = 100000 },
        new ProductSalesSummaryDto { ProductId = 1, ProductCode = "SP001", ProductName = "Nước suối 500ml", QuantitySold = 10, Revenue = 60000 },
        new ProductSalesSummaryDto { ProductId = 6, ProductCode = "SP006", ProductName = "Khăn giấy 100 tờ", QuantitySold = 2, Revenue = 25000 }
    ];

    private static List<CustomerPurchaseSummaryDto> CreateStubTopCustomers() =>
    [
        new CustomerPurchaseSummaryDto { CustomerId = 2, CustomerName = "Cửa hàng Tạp hóa An Phú", InvoiceCount = 1, TotalAmount = 198000 },
        new CustomerPurchaseSummaryDto { CustomerId = 1, CustomerName = "Khách lẻ", InvoiceCount = 1, TotalAmount = 106000 },
        new CustomerPurchaseSummaryDto { CustomerId = 3, CustomerName = "Siêu thị Hòa Bình", InvoiceCount = 0, TotalAmount = 0 }
    ];

    private sealed class RevenueRow
    {
        public string Date { get; set; } = string.Empty;
        public int InvoiceCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal EstimatedProfit { get; set; }
    }

    private sealed class ProductSalesRow
    {
        public int Rank { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    private sealed class CustomerRow
    {
        public int Rank { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int InvoiceCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
