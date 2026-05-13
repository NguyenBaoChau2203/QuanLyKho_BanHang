using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Inventory;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Inventory;

public sealed class FrmStocktake : Form
{
    private readonly StocktakeService _stocktakeService = new();
    private readonly ProductService _productService = new();
    private readonly BindingSource _source = new();
    private readonly Label _message = new();
    private List<StocktakeLineDto> _lines = [];

    public FrmStocktake()
    {
        Text = "Kiểm kê";
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
            "Kiểm kê kho",
            "So sánh số hệ thống và thực tế theo từng dòng để hỗ trợ demo kiểm kê.");
    }

    private Control BuildBody()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        actions.Controls.Add(CreateButton("Làm mới", (_, _) => LoadData()));
        actions.Controls.Add(CreateButton("Ghi nhận", (_, _) => Save()));
        panel.Controls.Add(actions, 0, 0);

        var grid = UiFactory.ReadOnlyGrid(_source);
        grid.AutoGenerateColumns = false;
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineDto.ProductId), HeaderText = "SP" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineDto.SystemQuantity), HeaderText = "Hệ thống" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineDto.ActualQuantity), HeaderText = "Thực tế" });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineDto.Difference), HeaderText = "Chênh lệch" });
        panel.Controls.Add(grid, 0, 1);
        return panel;
    }

    private Button CreateButton(string text, EventHandler handler)
    {
        return UiFactory.ActionButton(text, handler, 100);
    }

    private void LoadData()
    {
        var result = _productService.GetAllProducts();
        var products = result.Success && result.Data is { Count: > 0 } ? result.Data! : CreateStubProducts();
        _lines = products.Select(p => new StocktakeLineDto { ProductId = p.Id, SystemQuantity = p.QuantityOnHand, ActualQuantity = p.QuantityOnHand - (p.Id % 3) }).ToList();
        _source.DataSource = new BindingList<StocktakeLineDto>(_lines);
        _ = _stocktakeService.GetStocktakeById(1);
        _message.Text = result.Message;
    }

    private void Save()
    {
        _message.Text = "Đã ghi nhận kiểm kê trong chế độ stub, chờ backend thật xử lý.";
    }

    private static List<ProductDto> CreateStubProducts() =>
    [
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", QuantityOnHand = 110 },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", QuantityOnHand = 68 },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", QuantityOnHand = 195 },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", QuantityOnHand = 32 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", QuantityOnHand = 30 },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", QuantityOnHand = 118 }
    ];
}
