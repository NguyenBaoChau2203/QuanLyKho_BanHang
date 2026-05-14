using System.ComponentModel;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Inventory;

public sealed class FrmInventory : Form
{
    private readonly InventoryService _inventoryService = new();
    private readonly ProductService _productService = new();
    private readonly BindingSource _stockSource = new();
    private readonly DataGridView _stockGrid = new();
    private readonly TextBox _searchBox = new();
    private readonly ComboBox _categoryFilter = new();
    private readonly ComboBox _statusFilter = new();
    private readonly Label _message = new();
    private readonly Label _totalProductsLabel = new();
    private readonly Label _lowStockLabel = new();
    private readonly Label _outOfStockLabel = new();
    private readonly Label _totalQuantityLabel = new();
    private List<ProductDto> _items = [];

    public FrmInventory()
    {
        Text = "Tồn kho";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1200, 720);
        BuildUi();
        LoadData();
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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildMetrics(), 0, 1);
        root.Controls.Add(BuildFilterBar(), 0, 2);
        root.Controls.Add(BuildGridCard(), 0, 3);
        root.Controls.Add(_message, 0, 4);
        Controls.Add(root);

        _message.Dock = DockStyle.Fill;
        _message.TextAlign = ContentAlignment.MiddleLeft;
    }

    private Control BuildHeader()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(18, 12, 18, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.Controls.Add(UiFactory.IconTile(IconChar.Warehouse, AppTheme.Primary, AppTheme.PrimarySoft, 46, 24), 0, 0);

        var text = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        text.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        text.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        text.Controls.Add(new Label
        {
            Text = "Tổng quan tồn kho",
            Dock = DockStyle.Fill,
            Font = AppTheme.TitleFont(17F),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        text.Controls.Add(new Label
        {
            Text = "Theo dõi tồn hiện tại, lọc trạng thái và nhận diện nhanh các mặt hàng cần nhập.",
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(9.5F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);
        layout.Controls.Add(text, 1, 0);
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

        layout.Controls.Add(UiFactory.MetricCard("Tổng sản phẩm", _totalProductsLabel, IconChar.BoxesStacked, AppTheme.Primary, AppTheme.PrimarySoft), 0, 0);
        layout.Controls.Add(UiFactory.MetricCard("Sắp hết", _lowStockLabel, IconChar.TriangleExclamation, AppTheme.Warning, AppTheme.WarningSoft), 1, 0);
        layout.Controls.Add(UiFactory.MetricCard("Hết hàng", _outOfStockLabel, IconChar.CircleExclamation, AppTheme.Danger, AppTheme.DangerSoft), 2, 0);
        layout.Controls.Add(UiFactory.MetricCard("Tổng số lượng", _totalQuantityLabel, IconChar.LayerGroup, AppTheme.Success, AppTheme.SuccessSoft), 3, 0);
        return layout;
    }

    private Control BuildFilterBar()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(14, 12, 14, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 4));

        _searchBox.Dock = DockStyle.Fill;
        _searchBox.PlaceholderText = "Tìm theo mã hoặc tên sản phẩm...";
        _searchBox.TextChanged += (_, _) => ApplyFilters();
        layout.Controls.Add(_searchBox, 0, 0);

        _categoryFilter.Dock = DockStyle.Fill;
        _categoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _categoryFilter.SelectedIndexChanged += (_, _) => ApplyFilters();
        layout.Controls.Add(_categoryFilter, 1, 0);

        _statusFilter.Dock = DockStyle.Fill;
        _statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _statusFilter.Items.AddRange(new object[] { "Tất cả trạng thái", "Ổn định", "Sắp hết", "Hết hàng" });
        _statusFilter.SelectedIndex = 0;
        _statusFilter.SelectedIndexChanged += (_, _) => ApplyFilters();
        layout.Controls.Add(_statusFilter, 2, 0);

        layout.Controls.Add(CreateButton("Làm mới", IconChar.RotateRight, (_, _) => LoadData(), 104, AppTheme.Primary), 3, 0);
        layout.Controls.Add(CreateButton("Xuất Excel", IconChar.FileExport, (_, _) => SetMessage("Xuất Excel tồn kho đang ở chế độ demo."), 104, AppTheme.Success), 4, 0);

        card.Controls.Add(layout);
        return card;
    }

    private Control BuildGridCard()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0);
        card.Padding = new Padding(14, 12, 14, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(UiFactory.SectionHeader("Danh sách tồn kho", "Bảng ưu tiên đọc nhanh số lượng, mức cảnh báo và trạng thái.", IconChar.ListCheck), 0, 0);
        ConfigureGrid();
        layout.Controls.Add(_stockGrid, 0, 1);
        card.Controls.Add(layout);
        return card;
    }

    private void ConfigureGrid()
    {
        _stockGrid.Dock = DockStyle.Fill;
        _stockGrid.ReadOnly = true;
        _stockGrid.AllowUserToAddRows = false;
        _stockGrid.AllowUserToDeleteRows = false;
        _stockGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _stockGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _stockGrid.MultiSelect = false;
        _stockGrid.DataSource = _stockSource;
        UiFactory.StyleGrid(_stockGrid);
        _stockGrid.AutoGenerateColumns = false;
        _stockGrid.Columns.Clear();
        _stockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(InventoryRow.ProductCode), HeaderText = "Mã SP", FillWeight = 78 });
        _stockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(InventoryRow.ProductName), HeaderText = "Tên sản phẩm", FillWeight = 190 });
        _stockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(InventoryRow.CategoryName), HeaderText = "Loại", FillWeight = 100 });
        _stockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(InventoryRow.Unit), HeaderText = "ĐVT", FillWeight = 58 });
        _stockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(InventoryRow.QuantityOnHand), HeaderText = "Tồn", FillWeight = 68 });
        _stockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(InventoryRow.MinStockLevel), HeaderText = "Mức tối thiểu", FillWeight = 92 });
        _stockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(InventoryRow.StockValue), HeaderText = "Giá trị tồn", DefaultCellStyle = { Format = "N0" }, FillWeight = 100 });
        _stockGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(InventoryRow.Status), HeaderText = "Trạng thái", FillWeight = 96 });
        _stockGrid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || _stockGrid.Rows[e.RowIndex].DataBoundItem is not InventoryRow row)
            {
                return;
            }

            _stockGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = row.Status switch
            {
                "Hết hàng" => AppTheme.Danger,
                "Sắp hết" => AppTheme.Warning,
                _ => AppTheme.Text
            };
        };
    }

    private void LoadData()
    {
        var productResult = _inventoryService.GetCurrentStock();
        _items = productResult.Success && productResult.Data is { Count: > 0 } ? productResult.Data! : CreateStubProducts();
        LoadFilterOptions();
        ApplyFilters();
        SetMessage(productResult.Success ? "Đã cập nhật tồn kho." : $"{productResult.Message} - Đang dùng dữ liệu demo.", !productResult.Success);
    }

    private void LoadFilterOptions()
    {
        var selected = _categoryFilter.SelectedItem?.ToString();
        _categoryFilter.Items.Clear();
        _categoryFilter.Items.Add("Tất cả loại hàng");
        foreach (var category in _items.Select(x => string.IsNullOrWhiteSpace(x.CategoryName) ? "Chưa phân loại" : x.CategoryName).Distinct().OrderBy(x => x))
        {
            _categoryFilter.Items.Add(category);
        }

        _categoryFilter.SelectedItem = selected is not null && _categoryFilter.Items.Contains(selected) ? selected : "Tất cả loại hàng";
    }

    private void ApplyFilters()
    {
        var keyword = _searchBox.Text.Trim();
        var category = _categoryFilter.SelectedItem?.ToString() ?? "Tất cả loại hàng";
        var status = _statusFilter.SelectedItem?.ToString() ?? "Tất cả trạng thái";

        var rows = _items.Select(ToRow)
            .Where(x => string.IsNullOrWhiteSpace(keyword)
                || x.ProductCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || x.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .Where(x => category == "Tất cả loại hàng" || x.CategoryName == category)
            .Where(x => status == "Tất cả trạng thái" || x.Status == status)
            .ToList();

        _stockSource.DataSource = new BindingList<InventoryRow>(rows);
        UpdateMetrics();
    }

    private void UpdateMetrics()
    {
        var rows = _items.Select(ToRow).ToList();
        _totalProductsLabel.Text = rows.Count.ToString("N0");
        _lowStockLabel.Text = rows.Count(x => x.Status == "Sắp hết").ToString("N0");
        _outOfStockLabel.Text = rows.Count(x => x.Status == "Hết hàng").ToString("N0");
        _totalQuantityLabel.Text = rows.Sum(x => x.QuantityOnHand).ToString("N0");
    }

    private static InventoryRow ToRow(ProductDto product)
    {
        var status = product.QuantityOnHand <= 0
            ? "Hết hàng"
            : product.QuantityOnHand <= product.MinStockLevel ? "Sắp hết" : "Ổn định";
        var unitCost = product.CostPrice > 0 ? product.CostPrice : Math.Round(product.SellingPrice * 0.72M, 0);
        return new InventoryRow
        {
            ProductCode = product.Code,
            ProductName = product.Name,
            CategoryName = string.IsNullOrWhiteSpace(product.CategoryName) ? "Chưa phân loại" : product.CategoryName,
            Unit = string.IsNullOrWhiteSpace(product.Unit) ? "Cái" : product.Unit,
            QuantityOnHand = product.QuantityOnHand,
            MinStockLevel = product.MinStockLevel,
            StockValue = unitCost * product.QuantityOnHand,
            Status = status
        };
    }

    private void SetMessage(string message, bool error = false)
    {
        UiFactory.SetMessage(_message, message, error);
    }

    private static IconButton CreateButton(string text, IconChar icon, EventHandler handler, int width, Color color)
    {
        var button = new IconButton
        {
            Text = text,
            Dock = DockStyle.Fill,
            Width = width,
            Height = 36,
            Margin = new Padding(8, 0, 0, 0),
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

    private static List<ProductDto> CreateStubProducts() =>
    [
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", CategoryName = "Đồ uống", Unit = "Chai", CostPrice = 4200, SellingPrice = 6000, QuantityOnHand = 110, MinStockLevel = 30 },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", CategoryName = "Đồ uống", Unit = "Lon", CostPrice = 7800, SellingPrice = 11000, QuantityOnHand = 68, MinStockLevel = 25 },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", CategoryName = "Thực phẩm", Unit = "Gói", CostPrice = 3500, SellingPrice = 5000, QuantityOnHand = 195, MinStockLevel = 50 },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", CategoryName = "Gia dụng", Unit = "Chai", CostPrice = 18000, SellingPrice = 25000, QuantityOnHand = 32, MinStockLevel = 35 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", CategoryName = "Vệ sinh", Unit = "Tuýp", CostPrice = 12800, SellingPrice = 18000, QuantityOnHand = 30, MinStockLevel = 35 },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", CategoryName = "Vệ sinh", Unit = "Gói", CostPrice = 9000, SellingPrice = 12500, QuantityOnHand = 118, MinStockLevel = 15 },
        new ProductDto { Id = 7, Code = "SP007", Name = "Bàn chải đánh răng", CategoryName = "Vệ sinh", Unit = "Cái", CostPrice = 7000, SellingPrice = 12000, QuantityOnHand = 0, MinStockLevel = 20 }
    ];

    private sealed class InventoryRow
    {
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public int MinStockLevel { get; set; }
        public decimal StockValue { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
