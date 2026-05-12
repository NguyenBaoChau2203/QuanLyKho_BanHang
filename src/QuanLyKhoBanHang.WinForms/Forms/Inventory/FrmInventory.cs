using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Inventory;

public sealed class FrmInventory : Form
{
    private readonly InventoryService _inventoryService = new();
    private readonly ProductService _productService = new();
    private readonly BindingSource _stockSource = new();
    private readonly BindingSource _lowStockSource = new();
    private readonly Label _message = new();
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
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = AppTheme.PagePadding };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildBody(), 0, 1);
        root.Controls.Add(_message, 0, 2);
        Controls.Add(root);
        _message.Dock = DockStyle.Fill;
        _message.ForeColor = AppTheme.StatusText;
    }

    private Control BuildHeader()
    {
        return UiFactory.HeaderPanel(
            "Tồn kho hiện tại",
            "Bố cục 2 bảng: tồn kho đầy đủ và hàng sắp hết để demo nhanh.");
    }

    private Control BuildBody()
    {
        var splitter = UiFactory.HorizontalSplitter(760, 280);
        splitter.Panel1.Controls.Add(BuildGridPanel("Danh sách tồn kho", _stockSource, BuildStockGrid));
        splitter.Panel2.Controls.Add(BuildGridPanel("Hàng sắp hết", _lowStockSource, BuildLowStockGrid));
        return splitter;
    }

    private Control BuildGridPanel(string title, BindingSource source, Action<DataGridView> configure)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(new Label { Text = title, Dock = DockStyle.Fill, Font = AppTheme.SectionFont() }, 0, 0);
        var grid = UiFactory.ReadOnlyGrid(source);
        configure(grid);
        panel.Controls.Add(grid, 0, 1);
        return panel;
    }

    private static void BuildStockGrid(DataGridView grid)
    {
        grid.AutoGenerateColumns = false;
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Code), HeaderText = "Mã" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Name), HeaderText = "Tên" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.CategoryName), HeaderText = "Loại" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.QuantityOnHand), HeaderText = "Tồn" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.MinStockLevel), HeaderText = "Tối thiểu" });
    }

    private static void BuildLowStockGrid(DataGridView grid)
    {
        grid.AutoGenerateColumns = false;
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Code), HeaderText = "Mã" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Name), HeaderText = "Tên" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.QuantityOnHand), HeaderText = "Tồn" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.MinStockLevel), HeaderText = "Cảnh báo" });
    }

    private void LoadData()
    {
        var productResult = _productService.GetAllProducts();
        _items = productResult.Success && productResult.Data is { Count: > 0 } ? productResult.Data! : CreateStubProducts();
        _stockSource.DataSource = new BindingList<ProductDto>(_items);
        _lowStockSource.DataSource = new BindingList<ProductDto>(_items.Where(x => x.QuantityOnHand <= x.MinStockLevel).ToList());
        _ = _inventoryService.GetCurrentStock();
        _message.Text = productResult.Message;
    }

    private static List<ProductDto> CreateStubProducts() =>
    [
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", CategoryName = "Đồ uống", QuantityOnHand = 110, MinStockLevel = 30 },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", CategoryName = "Đồ uống", QuantityOnHand = 68, MinStockLevel = 25 },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", CategoryName = "Thực phẩm", QuantityOnHand = 195, MinStockLevel = 50 },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", CategoryName = "Gia dụng", QuantityOnHand = 32, MinStockLevel = 35 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", CategoryName = "Vệ sinh", QuantityOnHand = 30, MinStockLevel = 35 },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", CategoryName = "Vệ sinh", QuantityOnHand = 118, MinStockLevel = 15 }
    ];
}
