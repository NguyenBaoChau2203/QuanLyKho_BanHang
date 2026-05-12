using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Inventory;
using QuanLyKhoBanHang.DTO.MasterData;

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
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new Font("Segoe UI", 10F);
        MinimumSize = new Size(1200, 720);
        BuildUi();
        LoadData();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(18) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildBody(), 0, 1);
        root.Controls.Add(_message, 0, 2);
        Controls.Add(root);
        _message.Dock = DockStyle.Fill;
        _message.ForeColor = Color.FromArgb(92, 102, 121);
    }

    private Control BuildHeader()
    {
        var panel = new Panel { Dock = DockStyle.Fill };
        panel.Controls.Add(new Label { Text = "Kiểm kê kho", Dock = DockStyle.Top, Height = 34, Font = new Font("Segoe UI", 18F, FontStyle.Bold) });
        panel.Controls.Add(new Label { Text = "So sánh số hệ thống và thực tế theo từng dòng để hỗ trợ demo kiểm kê.", Dock = DockStyle.Bottom, Height = 22, ForeColor = Color.FromArgb(96, 108, 129) });
        return panel;
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

        var grid = new DataGridView { Dock = DockStyle.Fill, DataSource = _source, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White };
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
        var button = new Button { Text = text, Height = 36, Width = 100, Margin = new Padding(0, 0, 8, 0) };
        button.Click += handler;
        return button;
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
        new ProductDto { Id = 1, Code = "SP-001", Name = "Bút bi Thiên Long", QuantityOnHand = 120 },
        new ProductDto { Id = 2, Code = "SP-002", Name = "Sổ tay A5", QuantityOnHand = 15 },
        new ProductDto { Id = 3, Code = "SP-003", Name = "Thùng carton 5 lớp", QuantityOnHand = 8 }
    ];
}
