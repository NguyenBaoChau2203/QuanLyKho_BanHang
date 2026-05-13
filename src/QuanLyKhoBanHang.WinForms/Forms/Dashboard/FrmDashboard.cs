using System.ComponentModel;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.DTO.Inventory;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.DTO.Reports;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Dashboard;

public sealed class FrmDashboard : Form
{
    private readonly DashboardService _dashboardService = new();
    private readonly InventoryService _inventoryService = new();
    private readonly BindingSource _topProductsSource = new();
    private readonly BindingSource _lowStockSource = new();
    private readonly BindingSource _activitySource = new();
    private readonly Label _statusLabel = new();
    private readonly Label _updatedLabel = new();
    private readonly Label _revenueTodayValue = new();
    private readonly Label _revenueMonthValue = new();
    private readonly Label _invoiceTodayValue = new();
    private readonly Label _lowStockValue = new();
    private readonly DataGridView _topProductsGrid = new();
    private readonly DataGridView _lowStockGrid = new();
    private readonly DataGridView _activityGrid = new();

    public FrmDashboard()
    {
        Text = "Dashboard";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1240, 760);
        BuildUi();
        Load += (_, _) => RefreshDashboard();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(18, 16, 18, 18),
            BackColor = AppTheme.AppBackground
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 14));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 58));

        root.Controls.Add(UiFactory.SectionHeader(
            "Tổng quan vận hành",
            "Theo dõi nhanh doanh thu, đơn hàng, tồn kho và cảnh báo quan trọng.",
            IconChar.ChartColumn), 0, 0);
        root.Controls.Add(BuildKpiRow(), 0, 1);
        root.Controls.Add(BuildUpperBody(), 0, 3);
        root.Controls.Add(BuildLowerBody(), 0, 4);

        Controls.Add(root);
    }

    private Control BuildKpiRow()
    {
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1
        };

        for (var i = 0; i < 4; i++)
        {
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }

        var revenueToday = UiFactory.MetricCard(
            "Doanh thu hôm nay",
            _revenueTodayValue,
            IconChar.ChartLine,
            AppTheme.Success,
            AppTheme.SuccessSoft);

        var revenueMonth = UiFactory.MetricCard(
            "Doanh thu tháng này",
            _revenueMonthValue,
            IconChar.CalendarDays,
            AppTheme.Primary,
            AppTheme.PrimarySoft);

        var invoiceToday = UiFactory.MetricCard(
            "Hóa đơn hôm nay",
            _invoiceTodayValue,
            IconChar.FileInvoice,
            AppTheme.Warning,
            AppTheme.WarningSoft);

        var lowStock = UiFactory.MetricCard(
            "Sản phẩm sắp hết",
            _lowStockValue,
            IconChar.TriangleExclamation,
            AppTheme.Danger,
            AppTheme.DangerSoft);
        lowStock.Margin = Padding.Empty;

        cards.Controls.Add(revenueToday, 0, 0);
        cards.Controls.Add(revenueMonth, 1, 0);
        cards.Controls.Add(invoiceToday, 2, 0);
        cards.Controls.Add(lowStock, 3, 0);
        return cards;
    }

    private Control BuildUpperBody()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        var topProducts = BuildTopProductsPanel();
        topProducts.Margin = new Padding(0, 0, 7, 0);

        var lowStock = BuildLowStockPanel();
        lowStock.Margin = new Padding(7, 0, 0, 0);

        layout.Controls.Add(topProducts, 0, 0);
        layout.Controls.Add(lowStock, 1, 0);
        return layout;
    }

    private Control BuildLowerBody()
    {
        var panel = UiFactory.Card();
        panel.Padding = new Padding(16);
        panel.Margin = new Padding(0, 14, 0, 0);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        header.Controls.Add(UiFactory.SectionHeader(
            "Hoạt động gần đây",
            string.Empty,
            IconChar.ClockRotateLeft), 0, 0);
        header.Controls.Add(_updatedLabel, 1, 0);

        _updatedLabel.Dock = DockStyle.Fill;
        _updatedLabel.TextAlign = ContentAlignment.MiddleRight;
        _updatedLabel.ForeColor = AppTheme.TextMuted;
        _updatedLabel.Font = AppTheme.BodyFont(9.5F);

        _activityGrid.DataSource = _activitySource;
        ConfigureGrid(_activityGrid);
        ConfigureActivityColumns();

        layout.Controls.Add(header, 0, 0);
        layout.Controls.Add(_activityGrid, 0, 1);
        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildTopProductsPanel()
    {
        var panel = UiFactory.Card();
        panel.Padding = new Padding(16);
        panel.Controls.Add(BuildSectionLayout(
            "Top sản phẩm bán chạy",
            "Sản phẩm bán chạy trong kỳ gần nhất.",
            IconChar.Trophy,
            _topProductsGrid,
            _topProductsSource,
            ConfigureTopProductColumns));
        return panel;
    }

    private Control BuildLowStockPanel()
    {
        var panel = UiFactory.Card();
        panel.Padding = new Padding(16);
        panel.Controls.Add(BuildSectionLayout(
            "Tồn thấp cần nhập",
            "Các mặt hàng dưới ngưỡng cảnh báo.",
            IconChar.BoxesStacked,
            _lowStockGrid,
            _lowStockSource,
            ConfigureLowStockColumns));
        return panel;
    }

    private static Control BuildSectionLayout(
        string title,
        string subtitle,
        IconChar icon,
        DataGridView grid,
        BindingSource source,
        Action configureColumns)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(UiFactory.SectionHeader(title, subtitle, icon), 0, 0);

        grid.DataSource = source;
        ConfigureGrid(grid);
        configureColumns();
        layout.Controls.Add(grid, 0, 1);
        return layout;
    }

    private void ConfigureTopProductColumns()
    {
        _topProductsGrid.Columns.Clear();
        _topProductsGrid.AutoGenerateColumns = false;
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.Rank), HeaderText = "Rank", FillWeight = 65 });
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.ProductCode), HeaderText = "ProductCode", FillWeight = 120 });
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.ProductName), HeaderText = "ProductName", FillWeight = 170 });
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductSalesRow.QuantitySold), HeaderText = "QuantitySold", FillWeight = 120 });
        _topProductsGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(ProductSalesRow.Revenue),
            HeaderText = "Revenue",
            FillWeight = 120,
            DefaultCellStyle = { Format = "N0" }
        });
    }

    private void ConfigureLowStockColumns()
    {
        _lowStockGrid.Columns.Clear();
        _lowStockGrid.AutoGenerateColumns = false;
        _lowStockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(LowStockRow.ProductCode), HeaderText = "ProductCode", FillWeight = 110 });
        _lowStockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(LowStockRow.ProductName), HeaderText = "ProductName", FillWeight = 170 });
        _lowStockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(LowStockRow.QuantityOnHand), HeaderText = "QuantityOnHand", FillWeight = 130 });
        _lowStockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(LowStockRow.MinStockLevel), HeaderText = "MinStockLevel", FillWeight = 120 });
        _lowStockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(LowStockRow.Status), HeaderText = "Status", FillWeight = 110 });
    }

    private void ConfigureActivityColumns()
    {
        _activityGrid.Columns.Clear();
        _activityGrid.AutoGenerateColumns = false;
        _activityGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ActivityRow.When), HeaderText = "Thời gian", FillWeight = 160 });
        _activityGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ActivityRow.Category), HeaderText = "Loại", FillWeight = 120 });
        _activityGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ActivityRow.Description), HeaderText = "Mô tả", FillWeight = 300 });
        _activityGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ActivityRow.Reference), HeaderText = "Tham chiếu", FillWeight = 130 });
    }

    private static void ConfigureGrid(DataGridView grid)
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
    }

    private void RefreshDashboard()
    {
        var today = DateTime.Today;
        var summaryResult = _dashboardService.GetDashboardSummary(today);
        var lowStockResult = _inventoryService.GetLowStockProducts();
        var activityResult = _inventoryService.GetStockTransactions(today.AddDays(-6), today);

        if (summaryResult.Success && summaryResult.Data is { } summary)
        {
            ApplySummary(summary);
            SetStatus(summaryResult.Message);
        }
        else
        {
            ApplySummary(CreateStubSummary());
            SetStatus(summaryResult.Message, true, true);
        }

        ApplyLowStock(lowStockResult);
        ApplyActivity(activityResult);
        _updatedLabel.Text = $"Cập nhật: {DateTime.Now:dd/MM/yyyy HH:mm}";
    }

    private void ApplySummary(DashboardSummaryDto summary)
    {
        _revenueTodayValue.Text = summary.TodayRevenue.ToString("N0") + " đ";
        _revenueMonthValue.Text = summary.MonthRevenue.ToString("N0") + " đ";
        _invoiceTodayValue.Text = summary.TodayInvoiceCount.ToString("N0");
        _lowStockValue.Text = summary.LowStockProductCount.ToString("N0");

        var products = summary.TopSellingProducts.Count > 0 ? summary.TopSellingProducts : CreateStubTopProducts();
        _topProductsSource.DataSource = new BindingList<ProductSalesRow>(products.Select((x, index) => new ProductSalesRow
        {
            Rank = index + 1,
            ProductCode = x.ProductCode,
            ProductName = x.ProductName,
            QuantitySold = x.QuantitySold,
            Revenue = x.Revenue
        }).ToList());
    }

    private void ApplyLowStock(ServiceResult<List<ProductDto>> result)
    {
        var items = result.Success && result.Data is { Count: > 0 } data ? data : CreateStubLowStock();
        _lowStockSource.DataSource = new BindingList<LowStockRow>(items.Select(x => new LowStockRow
        {
            ProductCode = x.Code,
            ProductName = x.Name,
            QuantityOnHand = x.QuantityOnHand,
            MinStockLevel = x.MinStockLevel,
            Status = x.QuantityOnHand <= 0 ? "Hết hàng" : "Sắp hết"
        }).ToList());

        if (!result.Success)
        {
            SetStatus(result.Message, true, true);
        }
    }

    private void ApplyActivity(ServiceResult<List<StockTransactionDto>> result)
    {
        var items = result.Success && result.Data is { Count: > 0 } data ? data : CreateStubActivities();
        _activitySource.DataSource = new BindingList<ActivityRow>(items.Select(x => new ActivityRow
        {
            When = x.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
            Category = x.TransactionType.ToString(),
            Description = $"{x.ProductName}: {x.QuantityChange:+#;-#;0} (sau giao dịch {x.QuantityAfter:N0})",
            Reference = string.IsNullOrWhiteSpace(x.ReferenceCode) ? "-" : x.ReferenceCode
        }).ToList());

        if (!result.Success)
        {
            SetStatus(result.Message, true, true);
        }
    }

    private void SetStatus(string message, bool showFallback = false, bool isError = false)
    {
        _statusLabel.Text = showFallback ? $"Dữ liệu mẫu: {message}" : message;
        _statusLabel.ForeColor = isError ? AppTheme.Error : AppTheme.StatusText;
    }

    private static DashboardSummaryDto CreateStubSummary() => new()
    {
        TodayRevenue = 304000,
        MonthRevenue = 304000,
        TodayInvoiceCount = 2,
        LowStockProductCount = 2,
        TopSellingProducts = CreateStubTopProducts()
    };

    private static List<ProductSalesSummaryDto> CreateStubTopProducts() =>
    [
        new ProductSalesSummaryDto { ProductId = 2, ProductCode = "SP002", ProductName = "Nước ngọt cola lon", QuantitySold = 12, Revenue = 132000 },
        new ProductSalesSummaryDto { ProductId = 4, ProductCode = "SP004", ProductName = "Nước rửa chén 750ml", QuantitySold = 4, Revenue = 100000 },
        new ProductSalesSummaryDto { ProductId = 1, ProductCode = "SP001", ProductName = "Nước suối 500ml", QuantitySold = 10, Revenue = 60000 }
    ];

    private static List<ProductDto> CreateStubLowStock() =>
    [
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", QuantityOnHand = 32, MinStockLevel = 35 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", QuantityOnHand = 30, MinStockLevel = 35 }
    ];

    private static List<StockTransactionDto> CreateStubActivities() =>
    [
        new StockTransactionDto { ProductName = "Nước rửa chén 750ml", TransactionType = StockTransactionType.Purchase, QuantityChange = 18, QuantityAfter = 36, ReferenceCode = "PN0002", CreatedAt = new DateTime(2026, 5, 8, 9, 0, 0) },
        new StockTransactionDto { ProductName = "Kem đánh răng 110g", TransactionType = StockTransactionType.Purchase, QuantityChange = 15, QuantityAfter = 30, ReferenceCode = "PN0002", CreatedAt = new DateTime(2026, 5, 8, 9, 10, 0) },
        new StockTransactionDto { ProductName = "Khăn giấy 100 tờ", TransactionType = StockTransactionType.Purchase, QuantityChange = 60, QuantityAfter = 120, ReferenceCode = "PN0002", CreatedAt = new DateTime(2026, 5, 8, 9, 20, 0) },
        new StockTransactionDto { ProductName = "Nước suối 500ml", TransactionType = StockTransactionType.Sale, QuantityChange = -10, QuantityAfter = 110, ReferenceCode = "HD0001", CreatedAt = new DateTime(2026, 5, 11, 15, 0, 0) },
        new StockTransactionDto { ProductName = "Mì gói bò", TransactionType = StockTransactionType.Sale, QuantityChange = -5, QuantityAfter = 195, ReferenceCode = "HD0001", CreatedAt = new DateTime(2026, 5, 11, 15, 5, 0) },
        new StockTransactionDto { ProductName = "Khăn giấy 100 tờ", TransactionType = StockTransactionType.Sale, QuantityChange = -2, QuantityAfter = 118, ReferenceCode = "HD0001", CreatedAt = new DateTime(2026, 5, 11, 15, 10, 0) },
        new StockTransactionDto { ProductName = "Nước ngọt cola lon", TransactionType = StockTransactionType.Sale, QuantityChange = -12, QuantityAfter = 68, ReferenceCode = "HD0002", CreatedAt = new DateTime(2026, 5, 12, 10, 0, 0) },
        new StockTransactionDto { ProductName = "Nước rửa chén 750ml", TransactionType = StockTransactionType.Sale, QuantityChange = -4, QuantityAfter = 32, ReferenceCode = "HD0002", CreatedAt = new DateTime(2026, 5, 12, 10, 10, 0) },
        new StockTransactionDto { ProductName = "Nước rửa chén 750ml", TransactionType = StockTransactionType.StocktakeAdjustment, QuantityChange = -2, QuantityAfter = 30, ReferenceCode = "KK0001", CreatedAt = new DateTime(2026, 5, 12, 18, 0, 0) }
    ];

    private sealed class ProductSalesRow
    {
        public int Rank { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    private sealed class LowStockRow
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public int MinStockLevel { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    private sealed class ActivityRow
    {
        public string When { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }
}
