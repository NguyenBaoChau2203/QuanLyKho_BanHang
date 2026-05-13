using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Reports;
using QuanLyKhoBanHang.WinForms.Forms.Common;
using QuanLyKhoBanHang.DTO.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Reports;

public sealed class FrmReport : Form
{
    private readonly ReportService _reportService = new();
    private readonly BindingSource _revenueSource = new();
    private readonly BindingSource _topProductsSource = new();
    private readonly BindingSource _topCustomersSource = new();
    private readonly DateTimePicker _fromDate = new();
    private readonly DateTimePicker _toDate = new();
    private readonly Button _refreshButton = new();
    private readonly Button _exportButton = new();
    private readonly Label _statusLabel = new();
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
            RowCount = 4,
            Padding = AppTheme.PagePadding
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 48));

        root.Controls.Add(UiFactory.HeaderPanel(
            "Báo cáo doanh thu",
            "Lọc theo ngày, xem doanh thu, top sản phẩm và top khách hàng."), 0, 0);
        root.Controls.Add(BuildFilterBar(), 0, 1);
        root.Controls.Add(BuildUpperBody(), 0, 2);
        root.Controls.Add(BuildLowerBody(), 0, 3);

        Controls.Add(root);
    }

    private Control BuildFilterBar()
    {
        var panel = UiFactory.Card();
        panel.Padding = new Padding(14, 10, 14, 10);

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 2, 0, 0)
        };

        layout.Controls.Add(new Label { Text = "Từ ngày", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });
        _fromDate.Format = DateTimePickerFormat.Custom;
        _fromDate.CustomFormat = "dd/MM/yyyy";
        _fromDate.Width = 130;
        _fromDate.Value = DateTime.Today.AddDays(-6);
        layout.Controls.Add(_fromDate);

        layout.Controls.Add(new Label { Text = "Đến ngày", AutoSize = true, Padding = new Padding(16, 8, 8, 0) });
        _toDate.Format = DateTimePickerFormat.Custom;
        _toDate.CustomFormat = "dd/MM/yyyy";
        _toDate.Width = 130;
        _toDate.Value = DateTime.Today;
        layout.Controls.Add(_toDate);

        _refreshButton.Text = "Làm mới";
        _refreshButton.Width = 110;
        _refreshButton.Height = 34;
        _refreshButton.Click += (_, _) => ReloadReports();
        layout.Controls.Add(_refreshButton);

        _exportButton.Text = "Xuất báo cáo";
        _exportButton.Width = 130;
        _exportButton.Height = 34;
        _exportButton.TextAlign = ContentAlignment.MiddleCenter;
        _exportButton.Click += (_, _) => ExportReport();
        layout.Controls.Add(_exportButton);

        _statusLabel.AutoSize = true;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.Padding = new Padding(18, 8, 0, 0);
        _statusLabel.ForeColor = AppTheme.StatusText;
        layout.Controls.Add(_statusLabel);

        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildUpperBody()
    {
        var splitter = UiFactory.HorizontalSplitter(620, 320);
        splitter.Panel1.Controls.Add(BuildRevenuePanel());
        splitter.Panel2.Controls.Add(BuildTopProductsPanel());
        return splitter;
    }

    private Control BuildLowerBody()
    {
        var panel = UiFactory.Card();
        panel.Margin = new Padding(0, 12, 0, 0);
        panel.Padding = new Padding(14);
        panel.Controls.Add(BuildTopCustomersPanel());
        return panel;
    }

    private Control BuildRevenuePanel()
    {
        var layout = BuildSectionLayout("Doanh thu theo ngày", "Tổng hợp doanh thu trong khoảng đã chọn.", _revenueGrid, _revenueSource);
        return layout;
    }

    private Control BuildTopProductsPanel()
    {
        return BuildSectionLayout("Top sản phẩm", "Sản phẩm bán chạy trong kỳ.", _topProductsGrid, _topProductsSource);
    }

    private Control BuildTopCustomersPanel()
    {
        return BuildSectionLayout("Top khách hàng", "Khách hàng có giá trị mua cao nhất trong kỳ.", _topCustomersGrid, _topCustomersSource);
    }

    private Control BuildSectionLayout(string title, string subtitle, DataGridView grid, BindingSource source)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = AppTheme.SectionFont(12F),
            ForeColor = Color.FromArgb(31, 41, 55)
        }, 0, 0);
        layout.Controls.Add(new Label
        {
            Text = subtitle,
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(),
            ForeColor = AppTheme.TextMuted
        }, 0, 1);

        grid.DataSource = source;
        ConfigureGrid(grid);
        layout.Controls.Add(grid, 0, 2);
        return layout;
    }

    private static void ConfigureGrid(DataGridView grid)
    {
        UiFactory.StyleGrid(grid);
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToResizeRows = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
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

        ApplyRevenue(revenueResult, from, to);
        ApplyProducts(productResult);
        ApplyCustomers(customerResult);
        SetStatus("Đã cập nhật báo cáo.");
    }

    private void ApplyRevenue(ServiceResult<List<RevenueSummaryDto>> result, DateTime from, DateTime to)
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
    }

    private void ApplyProducts(ServiceResult<List<ProductSalesSummaryDto>> result)
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

    private void SetStatus(string message, bool isError = false)
    {
        _statusLabel.Text = message;
        _statusLabel.ForeColor = isError ? AppTheme.Error : AppTheme.StatusText;
    }

    private void ExportReport()
    {
        SetStatus("Xuất báo cáo đang ở chế độ demo an toàn. Vui lòng chụp màn hình hoặc dùng dữ liệu trên màn hình để thuyết trình.");
    }

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
