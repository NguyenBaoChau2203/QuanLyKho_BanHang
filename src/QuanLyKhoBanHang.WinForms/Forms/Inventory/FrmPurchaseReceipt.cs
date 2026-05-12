using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Inventory;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Inventory;

public sealed class FrmPurchaseReceipt : Form
{
    private readonly ProductService _productService = new();
    private readonly PurchaseService _purchaseService = new();
    private readonly BindingSource _lineSource = new();
    private readonly BindingSource _productSource = new();
    private readonly DataGridView _lineGrid = new();
    private readonly DataGridView _productGrid = new();
    private readonly TextBox _productSearch = new();
    private readonly TextBox _receiptCode = new();
    private readonly TextBox _supplier = new();
    private readonly NumericUpDown _quantity = new();
    private readonly NumericUpDown _unitCost = new();
    private readonly TextBox _note = new();
    private readonly Label _message = new();
    private readonly Label _totalLabel = new();
    private readonly List<PurchaseReceiptLineDto> _lines = [];
    private List<ProductDto> _products = [];

    public FrmPurchaseReceipt()
    {
        Text = "Nhập kho";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1280, 760);
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
        _message.Text = "Sẵn sàng nhập kho.";
    }

    private Control BuildHeader()
    {
        return UiFactory.HeaderPanel(
            "Phiếu nhập kho",
            "Tra cứu sản phẩm, thêm dòng và theo dõi tổng tiền bằng dữ liệu stub an toàn.");
    }

    private Control BuildBody()
    {
        var splitter = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 690 };
        splitter.Panel1.Controls.Add(BuildProductPanel());
        splitter.Panel2.Controls.Add(BuildEditorPanel());
        return splitter;
    }

    private Control BuildProductPanel()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 56));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 44));

        var searchRow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        searchRow.Controls.Add(new Label { Text = "Tìm sản phẩm", AutoSize = true, Padding = new Padding(0, 10, 8, 0) });
        _productSearch.Width = 280;
        _productSearch.PlaceholderText = "Mã, tên, loại hàng...";
        _productSearch.TextChanged += (_, _) => ApplyProductFilter();
        searchRow.Controls.Add(_productSearch);
        searchRow.Controls.Add(CreateButton("Thêm dòng", (_, _) => AddSelectedProduct()));

        _productGrid.Dock = DockStyle.Fill;
        _productGrid.ReadOnly = true;
        _productGrid.AllowUserToAddRows = false;
        _productGrid.AllowUserToDeleteRows = false;
        _productGrid.RowHeadersVisible = false;
        _productGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _productGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _productGrid.DataSource = _productSource;
        UiFactory.StyleGrid(_productGrid);
        _productGrid.Columns.Clear();
        _productGrid.AutoGenerateColumns = false;
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Code), HeaderText = "Mã" });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Name), HeaderText = "Tên" });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.SellingPrice), HeaderText = "Giá", DefaultCellStyle = { Format = "N0" } });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.QuantityOnHand), HeaderText = "Tồn" });

        var empty = new Label { Text = "Chọn sản phẩm từ danh sách bên dưới để thêm vào phiếu nhập.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = AppTheme.TextMuted };

        panel.Controls.Add(searchRow, 0, 0);
        panel.Controls.Add(_productGrid, 0, 1);
        panel.Controls.Add(empty, 0, 2);
        return panel;
    }

    private Control BuildEditorPanel()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, Padding = new Padding(8, 0, 0, 0) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

        var header = new Panel { Dock = DockStyle.Fill };
        header.Controls.Add(new Label { Text = "Thông tin phiếu", Dock = DockStyle.Fill, Font = AppTheme.TitleFont(14F) });

        var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4 };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66));
        form.Controls.Add(BuildField("Mã phiếu", _receiptCode), 0, 0);
        form.Controls.Add(BuildField("Nhà cung cấp", _supplier), 1, 0);
        form.Controls.Add(BuildField("Số lượng", _quantity), 0, 1);
        form.Controls.Add(BuildField("Đơn giá nhập", _unitCost), 1, 1);
        var noteField = BuildField("Ghi chú", _note, true);
        form.Controls.Add(noteField, 0, 2);
        form.SetColumnSpan(noteField, 2);

        _quantity.Minimum = 1;
        _quantity.Maximum = 100000;
        _quantity.Value = 1;
        _unitCost.Maximum = 100000000;
        _unitCost.Increment = 1000;
        _note.Multiline = true;

        _lineGrid.Dock = DockStyle.Fill;
        _lineGrid.ReadOnly = true;
        _lineGrid.AllowUserToAddRows = false;
        _lineGrid.AllowUserToDeleteRows = false;
        _lineGrid.RowHeadersVisible = false;
        _lineGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _lineGrid.DataSource = _lineSource;
        UiFactory.StyleGrid(_lineGrid);
        _lineGrid.AutoGenerateColumns = false;
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseReceiptLineDto.ProductId), HeaderText = "SP" });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseReceiptLineDto.Quantity), HeaderText = "SL" });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseReceiptLineDto.UnitCost), HeaderText = "Đơn giá", DefaultCellStyle = { Format = "N0" } });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseReceiptLineDto.LineTotal), HeaderText = "Thành tiền", DefaultCellStyle = { Format = "N0" } });

        var actionRow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        actionRow.Controls.Add(CreateButton("Lưu phiếu", (_, _) => SaveReceipt()));
        actionRow.Controls.Add(CreateButton("Xóa dòng", (_, _) => RemoveLine()));
        actionRow.Controls.Add(CreateButton("Làm mới", (_, _) => ResetForm()));
        _totalLabel.AutoSize = true;
        _totalLabel.Font = AppTheme.SectionFont();
        _totalLabel.Text = "Tổng tiền: 0 đ";
        actionRow.Controls.Add(_totalLabel);

        root.Controls.Add(header, 0, 0);
        root.Controls.Add(form, 0, 1);
        root.Controls.Add(_lineGrid, 0, 2);
        root.Controls.Add(actionRow, 0, 3);
        return root;
    }

    private static Control BuildField(string label, Control control, bool multiline = false)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = multiline ? 96 : 70, Padding = new Padding(0, 0, 12, 8) };
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Top, Height = 20 });
        control.Dock = DockStyle.Bottom;
        control.Height = multiline ? 56 : 30;
        panel.Controls.Add(control);
        return panel;
    }

    private Button CreateButton(string text, EventHandler handler)
    {
        return UiFactory.ActionButton(text, handler);
    }

    private void LoadData()
    {
        var result = _productService.GetAllProducts();
        _products = result.Success && result.Data is { Count: > 0 } ? result.Data! : CreateStubProducts();
        ApplyProductFilter();
        _receiptCode.Text = $"PN-{DateTime.Now:yyyyMMdd-HHmm}";
        _supplier.Text = "NCC demo";
        SetMessage(result.Message);
    }

    private void ApplyProductFilter()
    {
        var keyword = _productSearch.Text.Trim();
        var filtered = string.IsNullOrWhiteSpace(keyword)
            ? _products
            : _products.Where(x => x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        _productSource.DataSource = new BindingList<ProductDto>(filtered);
    }

    private void AddSelectedProduct()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is not ProductDto product)
        {
            SetMessage("Hãy chọn một sản phẩm để thêm vào phiếu nhập.", true);
            return;
        }

        var line = new PurchaseReceiptLineDto { ProductId = product.Id, Quantity = (int)_quantity.Value, UnitCost = _unitCost.Value };
        _lines.Add(line);
        _lineSource.DataSource = new BindingList<PurchaseReceiptLineDto>(_lines.ToList());
        UpdateTotal();
        SetMessage($"Đã thêm {product.Name} vào phiếu nhập.");
    }

    private void RemoveLine()
    {
        if (_lineGrid.CurrentRow?.DataBoundItem is not PurchaseReceiptLineDto line)
        {
            SetMessage("Chọn một dòng để xóa.", true);
            return;
        }
        _lines.Remove(line);
        _lineSource.DataSource = new BindingList<PurchaseReceiptLineDto>(_lines.ToList());
        UpdateTotal();
        SetMessage("Đã xóa dòng hàng.");
    }

    private void SaveReceipt()
    {
        var receipt = new PurchaseReceiptDto
        {
            ReceiptCode = _receiptCode.Text.Trim(),
            SupplierId = 1,
            ReceiptDate = DateTime.Today,
            Note = _note.Text,
            Lines = _lines.ToList(),
            TotalAmount = _lines.Sum(x => x.LineTotal)
        };
        var result = _purchaseService.CreateReceipt(receipt);
        SetMessage(result.Message, !result.Success);
    }

    private void ResetForm()
    {
        _lines.Clear();
        _lineSource.DataSource = new BindingList<PurchaseReceiptLineDto>(_lines);
        _note.Clear();
        _quantity.Value = 1;
        _unitCost.Value = 0;
        UpdateTotal();
        SetMessage("Đã làm mới phiếu nhập.");
    }

    private void UpdateTotal() => _totalLabel.Text = $"Tổng tiền: {_lines.Sum(x => x.LineTotal):N0} đ";

    private void SetMessage(string message, bool error = false)
    {
        UiFactory.SetMessage(_message, message, error);
    }

    private static List<ProductDto> CreateStubProducts() =>
    [
        new ProductDto { Id = 1, Code = "SP-001", Name = "Bút bi Thiên Long", SellingPrice = 5000, QuantityOnHand = 120 },
        new ProductDto { Id = 2, Code = "SP-002", Name = "Sổ tay A5", SellingPrice = 18000, QuantityOnHand = 15 },
        new ProductDto { Id = 3, Code = "SP-003", Name = "Thùng carton 5 lớp", SellingPrice = 45000, QuantityOnHand = 40 }
    ];
}
