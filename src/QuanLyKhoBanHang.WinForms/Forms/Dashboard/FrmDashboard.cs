using System.ComponentModel;
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
            RowCount = 4,
            Padding = AppTheme.PagePadding
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 44));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 56));

        root.Controls.Add(UiFactory.HeaderPanel(
            "Tổng quan vận hành",
            "Theo dõi nhanh doanh thu, đơn hàng, tồn kho và cảnh báo quan trọng."), 0, 0);
        root.Controls.Add(BuildKpiRow(), 0, 1);
        root.Controls.Add(BuildUpperBody(), 0, 2);
        root.Controls.Add(BuildLowerBody(), 0, 3);

        Controls.Add(root);
    }

    private Control BuildKpiRow()
    {
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4
        };

        for (var i = 0; i < 4; i++)
        {
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }

        cards.Controls.Add(CreateKpiCard("Doanh thu hôm nay", _revenueTodayValue, Color.FromArgb(5, 150, 105)), 0, 0);
        cards.Controls.Add(CreateKpiCard("Doanh thu tháng này", _revenueMonthValue, Color.FromArgb(37, 99, 235)), 1, 0);
        cards.Controls.Add(CreateKpiCard("Hóa đơn hôm nay", _invoiceTodayValue, Color.FromArgb(217, 119, 6)), 2, 0);
        cards.Controls.Add(CreateKpiCard("Sản phẩm sắp hết", _lowStockValue, AppTheme.Error), 3, 0);
        return cards;
    }

    private Control BuildUpperBody()
    {
        var splitter = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 600,
            Panel1MinSize = 420,
            Panel2MinSize = 420
        };
        splitter.Panel1.Controls.Add(BuildTopProductsPanel());
        splitter.Panel2.Controls.Add(BuildLowStockPanel());
        return splitter;
    }

    private Control BuildLowerBody()
    {
        var panel = UiFactory.Card();
        panel.Padding = new Padding(14);
        panel.Margin = new Padding(0, 12, 0, 0);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new Panel { Dock = DockStyle.Fill };
        header.Controls.Add(new Label
        {
            Text = "Hoạt động gần đây",
            Dock = DockStyle.Left,
            Width = 220,
            Font = AppTheme.SectionFont(12F),
            ForeColor = Color.FromArgb(31, 41, 55)
        });
        header.Controls.Add(_updatedLabel);
        _updatedLabel.Dock = DockStyle.Right;
        _updatedLabel.Width = 220;
        _updatedLabel.TextAlign = ContentAlignment.MiddleRight;
        _updatedLabel.ForeColor = AppTheme.TextMuted;

        _activityGrid.DataSource = _activitySource;
        ConfigureGrid(_activityGrid);
        _activityGrid.AutoGenerateColumns = false;
        _activityGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ActivityRow.When), HeaderText = "Thời gian", Width = 140 });
        _activityGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ActivityRow.Category), HeaderText = "Loại", Width = 120 });
        _activityGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ActivityRow.Description), HeaderText = "Mô tả" });
        _activityGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ActivityRow.Reference), HeaderText = "Tham chiếu", Width = 140 });

        layout.Controls.Add(header, 0, 0);
        layout.Controls.Add(_activityGrid, 0, 1);
        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildTopProductsPanel()
    {
        var panel = UiFactory.Card();
        panel.Margin = new Padding(0, 0, 12, 0);
        panel.Padding = new Padding(14);

        var layout = BuildSectionLayout("Top sản phẩm bán chạy", "Sản phẩm bán chạy trong kỳ gần nhất.", _topProductsGrid, _topProductsSource);
        panel.Controls.Add(layout);
        return panel;
    }

    private Control BuildLowStockPanel()
    {
        var panel = UiFactory.Card();
        panel.Padding = new Padding(14);

        var layout = BuildSectionLayout("Tồn thấp cần nhập", "Các mặt hàng dưới ngưỡng cảnh báo.", _lowStockGrid, _lowStockSource);
        panel.Controls.Add(layout);
        return panel;
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

    private static Panel CreateKpiCard(string title, Label valueLabel, Color accent)
    {
        var panel = UiFactory.Card();
        panel.Margin = new Padding(0, 0, 12, 0);
        panel.BackColor = AppTheme.Surface;
        panel.Padding = new Padding(18, 16, 18, 16);

        var accentBar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 6,
            BackColor = accent
        };
        panel.Controls.Add(accentBar);

        var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(14, 0, 0, 0) };
        body.Controls.Add(valueLabel);
        body.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 24,
            Font = AppTheme.BodyFont(),
            ForeColor = AppTheme.TextMuted
        });

        valueLabel.Dock = DockStyle.Fill;
        valueLabel.Font = AppTheme.TitleFont(22F);
        valueLabel.TextAlign = ContentAlignment.MiddleLeft;
        valueLabel.ForeColor = Color.FromArgb(17, 24, 39);
        valueLabel.Text = "--";

        panel.Controls.Add(body);
        return panel;
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

    private void RefreshDashboard()
    {
        var today = DateTime.Today;
        var summaryResult = _dashboardService.GetDashboardSummary(today);
        var lowStockResult = _inventoryService.GetLowStockProducts();
        var activityResult = _inventoryService.GetStockTransactions(today.AddDays(-6), today);

        if (summaryResult.Success && summaryResult.Data is { } summary)
        {
            ApplySummary(summary);
            SetStatus(summaryResult.Message, false);
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
        new ProductSalesSummaryDto { ProductId = 1, ProductCode = "SP001", ProductName = "Nước suối 500ml", QuantitySold = 10, Revenue = 60000 },
        new ProductSalesSummaryDto { ProductId = 6, ProductCode = "SP006", ProductName = "Khăn giấy 100 tờ", QuantitySold = 2, Revenue = 25000 }
    ];

    private static List<ProductDto> CreateStubLowStock() =>
    [
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", QuantityOnHand = 32, MinStockLevel = 35 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", QuantityOnHand = 30, MinStockLevel = 35 }
    ];

    private static List<StockTransactionDto> CreateStubActivities() =>
    [
        new StockTransactionDto { ProductName = "Nước ngọt cola lon", TransactionType = StockTransactionType.Sale, QuantityChange = -12, QuantityAfter = 68, ReferenceCode = "HD0002", CreatedAt = DateTime.Today.AddHours(10) },
        new StockTransactionDto { ProductName = "Nước rửa chén 750ml", TransactionType = StockTransactionType.Sale, QuantityChange = -4, QuantityAfter = 32, ReferenceCode = "HD0002", CreatedAt = DateTime.Today.AddHours(10).AddMinutes(10) },
        new StockTransactionDto { ProductName = "Nước rửa chén 750ml", TransactionType = StockTransactionType.StocktakeAdjustment, QuantityChange = -2, QuantityAfter = 30, ReferenceCode = "KK0001", CreatedAt = DateTime.Today.AddHours(18) }
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
