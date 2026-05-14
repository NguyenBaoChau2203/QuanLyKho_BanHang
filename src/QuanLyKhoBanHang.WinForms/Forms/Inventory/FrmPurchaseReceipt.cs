using System.ComponentModel;
using FontAwesome.Sharp;
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
    private readonly TextBox _supplierNote = new();
    private readonly TextBox _note = new();
    private readonly DateTimePicker _receiptDate = new();
    private readonly TextBox _quantity = new();
    private readonly NumericUpDown _unitCost = new();
    private readonly Label _message = new();
    private readonly Label _productCodeLabel = new();
    private readonly Label _productNameLabel = new();
    private readonly Label _stockLabel = new();
    private readonly Label _costLabel = new();
    private readonly Label _lineCountLabel = new();
    private readonly Label _totalLinesLabel = new();
    private readonly Label _totalQuantityLabel = new();
    private readonly Label _totalCostLabel = new();
    private readonly List<PurchaseLineRow> _lines = [];
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
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = AppTheme.PagePadding
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 174));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildWorkspace(), 0, 1);
        root.Controls.Add(BuildBottomCards(), 0, 2);
        root.Controls.Add(_message, 0, 3);
        Controls.Add(root);

        _message.Dock = DockStyle.Fill;
        _message.TextAlign = ContentAlignment.MiddleLeft;
    }

    private Control BuildHeader()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(18, 12, 18, 12);

        var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44));

        var title = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        title.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56));
        title.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        title.Controls.Add(UiFactory.IconTile(IconChar.TruckRampBox, AppTheme.Warning, AppTheme.WarningSoft, 46, 24), 0, 0);

        var stack = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        stack.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        stack.Controls.Add(new Label
        {
            Text = "Lập phiếu nhập kho",
            Dock = DockStyle.Fill,
            Font = AppTheme.TitleFont(17F),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        stack.Controls.Add(new Label
        {
            Text = "Chọn nhà cung cấp, thêm dòng hàng và kiểm tra tổng giá trị trước khi lưu.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont(9.5F),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);
        title.Controls.Add(stack, 1, 0);

        var info = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 6, 0, 0)
        };
        _receiptCode.Width = 160;
        _receiptCode.ReadOnly = true;
        _receiptDate.Width = 128;
        _receiptDate.Format = DateTimePickerFormat.Custom;
        _receiptDate.CustomFormat = "dd/MM/yyyy";
        info.Controls.Add(_receiptCode);
        info.Controls.Add(BuildInlineLabel("Mã phiếu"));
        info.Controls.Add(_receiptDate);
        info.Controls.Add(BuildInlineLabel("Ngày nhập"));

        shell.Controls.Add(title, 0, 0);
        shell.Controls.Add(info, 1, 0);
        card.Controls.Add(shell);
        return card;
    }

    private Control BuildWorkspace()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        layout.Controls.Add(BuildProductCard(), 0, 0);
        layout.Controls.Add(BuildLineCard(), 1, 0);
        return layout;
    }

    private Control BuildSupplierCard()
    {
        var (card, content) = BuildSmallCard("Nhà cung cấp", IconChar.Truck);
        var layout = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 1, RowCount = 4, Padding = new Padding(0, 0, 8, 0), Height = 104 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        _supplier.PlaceholderText = "Nhập hoặc chọn nhà cung cấp...";
        _supplierNote.PlaceholderText = "Mã đơn đặt hàng / số chứng từ liên quan...";

        layout.Controls.Add(BuildPlainFieldLabel("Nhà cung cấp"), 0, 0);
        layout.Controls.Add(PreparePlainInput(_supplier), 0, 1);
        layout.Controls.Add(BuildPlainFieldLabel("Tham chiếu"), 0, 2);
        layout.Controls.Add(PreparePlainInput(_supplierNote), 0, 3);
        content.Controls.Add(layout, 0, 1);
        return card;
    }

    private Control BuildProductCard()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 12, 12);
        card.Padding = new Padding(16, 12, 16, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));
        layout.Controls.Add(UiFactory.SectionHeader("Tìm kiếm sản phẩm", "Chọn mặt hàng, nhập số lượng và giá nhập.", IconChar.MagnifyingGlass), 0, 0);

        _productSearch.Dock = DockStyle.Top;
        _productSearch.Height = 28;
        _productSearch.Margin = new Padding(0, 2, 0, 8);
        _productSearch.PlaceholderText = "Nhập mã, tên hoặc loại hàng...";
        _productSearch.TextChanged += (_, _) => ApplyProductFilter();
        layout.Controls.Add(_productSearch, 0, 1);

        ConfigureProductGrid();
        layout.Controls.Add(_productGrid, 0, 2);
        layout.Controls.Add(BuildAddPanel(), 0, 3);

        card.Controls.Add(layout);
        return card;
    }

    private Control BuildAddPanel()
    {
        var shell = UiFactory.SoftTile(AppTheme.SurfaceSubtle, AppTheme.Border, 8);
        shell.Dock = DockStyle.Fill;
        shell.Margin = new Padding(0, 10, 0, 0);
        shell.Padding = new Padding(12, 8, 12, 8);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var readouts = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
        readouts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));
        readouts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));
        readouts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        readouts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44));
        readouts.Controls.Add(BuildReadout("Mã SP", _productCodeLabel), 0, 0);
        readouts.Controls.Add(BuildReadout("Tồn", _stockLabel), 1, 0);
        readouts.Controls.Add(BuildReadout("Giá gợi ý", _costLabel), 2, 0);
        readouts.Controls.Add(BuildReadout("Tên sản phẩm", _productNameLabel), 3, 0);
        layout.Controls.Add(readouts, 0, 0);

        var entryRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 4, 0, 0)
        };
        entryRow.Controls.Add(BuildFixedTextInput("Số lượng", _quantity, 84));
        entryRow.Controls.Add(BuildFixedInput("Đơn giá nhập", _unitCost, 194));
        var addButton = CreatePrimaryButton("Thêm vào phiếu nhập", IconChar.Plus, (_, _) => AddSelectedProduct(), AppTheme.Warning);
        addButton.Dock = DockStyle.None;
        addButton.Width = 190;
        addButton.Height = 34;
        addButton.Margin = new Padding(18, 18, 0, 0);
        entryRow.Controls.Add(addButton);
        layout.Controls.Add(entryRow, 0, 1);

        shell.Controls.Add(layout);
        return shell;
    }

    private Control BuildLineCard()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(14, 12, 14, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        header.Controls.Add(UiFactory.SectionHeader("Danh sách hàng nhập", "Kiểm tra dòng hàng trước khi lưu phiếu.", IconChar.BoxesPacking), 0, 0);
        _lineCountLabel.Dock = DockStyle.Fill;
        _lineCountLabel.TextAlign = ContentAlignment.MiddleRight;
        _lineCountLabel.ForeColor = AppTheme.TextMuted;
        header.Controls.Add(_lineCountLabel, 1, 0);
        layout.Controls.Add(header, 0, 0);

        ConfigureLineGrid();
        layout.Controls.Add(_lineGrid, 0, 1);

        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        actions.Controls.Add(CreateDangerButton("Xóa dòng", IconChar.Trash, (_, _) => RemoveLine(), 120));
        actions.Controls.Add(CreateSecondaryButton("Xóa tất cả", IconChar.TrashCan, (_, _) => ClearLines(), 126));
        layout.Controls.Add(actions, 0, 2);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildBottomCards()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
        layout.Controls.Add(BuildSupplierCard(), 0, 0);
        layout.Controls.Add(BuildNoteCard(), 1, 0);
        layout.Controls.Add(BuildSummaryCard(), 2, 0);
        layout.Controls.Add(BuildActionPanel(), 3, 0);
        return layout;
    }

    private Control BuildNoteCard()
    {
        var (card, content) = BuildSmallCard("Ghi chú", IconChar.ClipboardList);
        _note.Dock = DockStyle.Fill;
        _note.Multiline = true;
        _note.PlaceholderText = "Nhập ghi chú cho phiếu nhập...";
        content.Controls.Add(_note, 0, 1);
        return card;
    }

    private Control BuildSummaryCard()
    {
        var (card, content) = BuildSmallCard("Tổng phiếu nhập", IconChar.Calculator);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
        AddSummaryRow(layout, 0, "Số dòng hàng", _totalLinesLabel);
        AddSummaryRow(layout, 1, "Tổng số lượng", _totalQuantityLabel);
        AddSummaryRow(layout, 2, "Tổng giá trị", _totalCostLabel, true, AppTheme.Warning);
        content.Controls.Add(layout, 0, 1);
        return card;
    }

    private Control BuildActionPanel()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0);
        card.Padding = new Padding(14, 12, 14, 12);

        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1 };
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        panel.Controls.Add(CreatePrimaryButton("Lưu phiếu nhập", IconChar.FloppyDisk, (_, _) => SaveReceipt(), AppTheme.Warning), 0, 0);
        panel.Controls.Add(CreateSecondaryButton("In phiếu", IconChar.Print, (_, _) => SetMessage("In phiếu nhập đang ở chế độ demo."), 0), 0, 1);
        panel.Controls.Add(CreateSecondaryButton("Lưu tạm", IconChar.Clock, (_, _) => SetMessage("Lưu tạm phiếu nhập đang ở chế độ demo."), 0), 0, 2);
        panel.Controls.Add(CreateSecondaryButton("Làm mới", IconChar.RotateRight, (_, _) => ResetForm(), 0), 0, 3);
        card.Controls.Add(panel);
        return card;
    }

    private void ConfigureProductGrid()
    {
        _productGrid.Dock = DockStyle.Fill;
        _productGrid.ReadOnly = true;
        _productGrid.AllowUserToAddRows = false;
        _productGrid.AllowUserToDeleteRows = false;
        _productGrid.RowHeadersVisible = false;
        _productGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _productGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _productGrid.MultiSelect = false;
        _productGrid.DataSource = _productSource;
        UiFactory.StyleGrid(_productGrid);
        _productGrid.AutoGenerateColumns = false;
        _productGrid.Columns.Clear();
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Code), HeaderText = "Mã SP", FillWeight = 70 });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Name), HeaderText = "Tên sản phẩm", FillWeight = 160 });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.QuantityOnHand), HeaderText = "Tồn", FillWeight = 58 });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.CostPrice), HeaderText = "Giá vốn", DefaultCellStyle = { Format = "N0" }, FillWeight = 82 });
        _productGrid.SelectionChanged += (_, _) => SyncSelectedProduct();
        _productGrid.CellDoubleClick += (_, _) => AddSelectedProduct();
    }

    private void ConfigureLineGrid()
    {
        _lineGrid.Dock = DockStyle.Fill;
        _lineGrid.ReadOnly = true;
        _lineGrid.AllowUserToAddRows = false;
        _lineGrid.AllowUserToDeleteRows = false;
        _lineGrid.RowHeadersVisible = false;
        _lineGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _lineGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _lineGrid.MultiSelect = false;
        _lineGrid.DataSource = _lineSource;
        UiFactory.StyleGrid(_lineGrid);
        _lineGrid.AutoGenerateColumns = false;
        _lineGrid.Columns.Clear();
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseLineRow.RowNumber), HeaderText = "STT", FillWeight = 48 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseLineRow.ProductCode), HeaderText = "Mã sản phẩm", FillWeight = 90 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseLineRow.ProductName), HeaderText = "Tên sản phẩm", FillWeight = 180 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseLineRow.Unit), HeaderText = "ĐVT", FillWeight = 60 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseLineRow.Quantity), HeaderText = "Số lượng", FillWeight = 76 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseLineRow.UnitCost), HeaderText = "Đơn giá nhập", DefaultCellStyle = { Format = "N0" }, FillWeight = 100 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseLineRow.LineTotal), HeaderText = "Thành tiền", DefaultCellStyle = { Format = "N0" }, FillWeight = 110 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PurchaseLineRow.CurrentStock), HeaderText = "Tồn hiện tại", FillWeight = 80 });
    }

    private void LoadData()
    {
        var result = _productService.GetAllProducts();
        _products = result.Success && result.Data is { Count: > 0 } ? result.Data! : CreateStubProducts();
        _receiptCode.Text = $"PN-{DateTime.Now:yyyyMMdd-HHmm}";
        _receiptDate.Value = DateTime.Today;
        _supplier.Text = "NCC demo";
        _supplierNote.Text = "Phiếu nhập demo";
        _quantity.Text = "1";
        _unitCost.Maximum = 100000000;
        _unitCost.ThousandsSeparator = true;
        _unitCost.Increment = 1000;
        ApplyProductFilter();
        RefreshLineSource();
        UpdateTotal();
        SetMessage("Sẵn sàng nhập kho.");
    }

    private void ApplyProductFilter()
    {
        var keyword = _productSearch.Text.Trim();
        var filtered = string.IsNullOrWhiteSpace(keyword)
            ? _products
            : _products.Where(x => x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.CategoryName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        _productSource.DataSource = new BindingList<ProductDto>(filtered);
        SyncSelectedProduct();
    }

    private void SyncSelectedProduct()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is not ProductDto product)
        {
            _productCodeLabel.Text = "---";
            _productNameLabel.Text = "---";
            _stockLabel.Text = "---";
            _costLabel.Text = "0 đ";
            return;
        }

        var cost = product.CostPrice > 0 ? product.CostPrice : Math.Round(product.SellingPrice * 0.72M, 0);
        _productCodeLabel.Text = product.Code;
        _productNameLabel.Text = product.Name;
        _stockLabel.Text = product.QuantityOnHand.ToString("N0");
        _costLabel.Text = FormatMoney(cost);
        if (cost >= 0 && cost <= _unitCost.Maximum)
        {
            _unitCost.Value = cost;
        }
    }

    private void AddSelectedProduct()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is not ProductDto product)
        {
            SetMessage("Hãy chọn một sản phẩm để thêm vào phiếu nhập.", true);
            return;
        }

        if (!int.TryParse(_quantity.Text.Trim(), out var quantity) || quantity <= 0)
        {
            SetMessage("Số lượng nhập phải là số nguyên lớn hơn 0.", true);
            return;
        }

        if (_unitCost.Value <= 0)
        {
            SetMessage("Đơn giá nhập phải lớn hơn 0.", true);
            return;
        }

        _lines.Add(new PurchaseLineRow
        {
            ProductId = product.Id,
            ProductCode = product.Code,
            ProductName = product.Name,
            Unit = string.IsNullOrWhiteSpace(product.Unit) ? "Cái" : product.Unit,
            Quantity = quantity,
            UnitCost = _unitCost.Value,
            CurrentStock = product.QuantityOnHand
        });
        RefreshLineSource();
        UpdateTotal();
        SetMessage($"Đã thêm {product.Name} vào phiếu nhập.");
    }

    private void RemoveLine()
    {
        if (_lineGrid.CurrentRow?.DataBoundItem is not PurchaseLineRow line)
        {
            SetMessage("Chọn một dòng để xóa.", true);
            return;
        }

        _lines.Remove(line);
        RefreshLineSource();
        UpdateTotal();
        SetMessage("Đã xóa dòng hàng.");
    }

    private void ClearLines()
    {
        _lines.Clear();
        RefreshLineSource();
        UpdateTotal();
        SetMessage("Đã xóa toàn bộ dòng hàng.");
    }

    private void SaveReceipt()
    {
        if (_lines.Count == 0)
        {
            SetMessage("Phiếu nhập phải có ít nhất một dòng hàng.", true);
            return;
        }

        var receipt = new PurchaseReceiptDto
        {
            ReceiptCode = _receiptCode.Text.Trim(),
            SupplierId = 1,
            ReceiptDate = _receiptDate.Value.Date,
            Note = string.Join(" - ", new[] { _supplier.Text.Trim(), _supplierNote.Text.Trim(), _note.Text.Trim() }.Where(x => !string.IsNullOrWhiteSpace(x))),
            Lines = _lines.Select(x => new PurchaseReceiptLineDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost
            }).ToList(),
            TotalAmount = _lines.Sum(x => x.LineTotal)
        };

        var result = _purchaseService.CreateReceipt(receipt);
        SetMessage(result.Message, !result.Success);
    }

    private void ResetForm()
    {
        _lines.Clear();
        _note.Clear();
        _quantity.Text = "1";
        _unitCost.Value = 0;
        _receiptCode.Text = $"PN-{DateTime.Now:yyyyMMdd-HHmm}";
        _receiptDate.Value = DateTime.Today;
        RefreshLineSource();
        UpdateTotal();
        SetMessage("Đã làm mới phiếu nhập.");
    }

    private void RefreshLineSource()
    {
        for (var i = 0; i < _lines.Count; i++)
        {
            _lines[i].RowNumber = i + 1;
        }

        _lineSource.DataSource = new BindingList<PurchaseLineRow>(_lines.ToList());
        _lineCountLabel.Text = $"{_lines.Count} dòng";
    }

    private void UpdateTotal()
    {
        _totalLinesLabel.Text = _lines.Count.ToString("N0");
        _totalQuantityLabel.Text = _lines.Sum(x => x.Quantity).ToString("N0");
        _totalCostLabel.Text = FormatMoney(_lines.Sum(x => x.LineTotal));
    }

    private void SetMessage(string message, bool error = false)
    {
        UiFactory.SetMessage(_message, message, error);
    }

    private static Control BuildHintTile(string text)
    {
        var panel = UiFactory.SoftTile(AppTheme.WarningSoft, AppTheme.Border, 8);
        panel.Dock = DockStyle.Fill;
        panel.Padding = new Padding(12);
        panel.Controls.Add(new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        });
        return panel;
    }

    private static Control BuildFixedTextInput(string label, TextBox textBox, int width)
    {
        var panel = BuildFixedInputShell(label, width);
        textBox.SetBounds(0, 24, width, 24);
        textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Margin = Padding.Empty;
        textBox.BackColor = AppTheme.Surface;
        panel.Controls.Add(textBox);
        return panel;
    }

    private static Control BuildFixedInput(string label, Control control, int width)
    {
        var panel = BuildFixedInputShell(label, width);
        control.SetBounds(0, 24, width, 26);
        control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        control.Margin = Padding.Empty;
        if (control is UpDownBase upDown)
        {
            upDown.BorderStyle = BorderStyle.FixedSingle;
        }
        panel.Controls.Add(control);
        return panel;
    }

    private static Panel BuildFixedInputShell(string label, int width)
    {
        var panel = new Panel { Width = width, Height = 54, Margin = new Padding(0, 0, 16, 0) };
        panel.Controls.Add(new Label
        {
            Text = label,
            Bounds = new Rectangle(0, 0, width, 20),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.BottomLeft
        });
        return panel;
    }

    private static Label BuildPlainFieldLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        ForeColor = AppTheme.TextMuted,
        TextAlign = ContentAlignment.BottomLeft,
        Margin = Padding.Empty
    };

    private static TextBox PreparePlainInput(TextBox textBox)
    {
        textBox.Dock = DockStyle.Top;
        textBox.Height = 24;
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Margin = new Padding(0, 0, 0, 6);
        textBox.BackColor = AppTheme.Surface;
        return textBox;
    }

    private static Control BuildPlainInputField(string label, TextBox textBox)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(0, 0, 10, 0) };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.Controls.Add(BuildPlainFieldLabel(label), 0, 0);
        textBox.Dock = DockStyle.Top;
        textBox.Height = 24;
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Margin = Padding.Empty;
        textBox.BackColor = AppTheme.Surface;
        layout.Controls.Add(textBox, 0, 1);
        return layout;
    }

    private static Control BuildReadout(string label, Label value)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(0, 0, 10, 4) };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, Font = AppTheme.BodyFont(8.5F), ForeColor = AppTheme.TextMuted }, 0, 0);
        value.Dock = DockStyle.Fill;
        value.Font = AppTheme.SectionFont(9.5F);
        value.AutoEllipsis = true;
        panel.Controls.Add(value, 0, 1);
        return panel;
    }

    private static (Control Card, TableLayoutPanel Content) BuildSmallCard(string title, IconChar icon)
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 12, 0);
        card.Padding = new Padding(14, 12, 14, 12);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(BuildCompactSectionHeader(title, icon), 0, 0);
        card.Controls.Add(layout);
        return (card, layout);
    }

    private static Control BuildCompactSectionHeader(string title, IconChar icon)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.Controls.Add(new IconPictureBox
        {
            IconChar = icon,
            IconColor = AppTheme.Primary,
            IconFont = IconFont.Auto,
            IconSize = 16,
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 1, 8, 0),
            SizeMode = PictureBoxSizeMode.CenterImage
        }, 0, 0);
        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = AppTheme.SectionFont(10.5F),
            ForeColor = AppTheme.Primary,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 1, 0);
        return layout;
    }

    private static void AddSummaryRow(TableLayoutPanel layout, int row, string label, Label value, bool strong = false, Color? color = null)
    {
        layout.Controls.Add(new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = strong ? AppTheme.SectionFont(10.5F) : AppTheme.BodyFont(9.5F)
        }, 0, row);
        value.Dock = DockStyle.Fill;
        value.TextAlign = ContentAlignment.MiddleRight;
        value.Font = strong ? AppTheme.TitleFont(15F) : AppTheme.SectionFont(10F);
        value.ForeColor = color ?? AppTheme.Text;
        layout.Controls.Add(value, 1, row);
    }

    private static Label BuildInlineLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Padding = new Padding(12, 7, 8, 0),
        ForeColor = AppTheme.Text
    };

    private static IconButton CreatePrimaryButton(string text, IconChar icon, EventHandler handler, Color color)
    {
        var button = CreateBaseButton(text, icon, handler, 0);
        button.BackColor = color;
        button.ForeColor = Color.White;
        button.IconColor = Color.White;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(color, 0.08F);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.16F);
        return button;
    }

    private static IconButton CreateSecondaryButton(string text, IconChar icon, EventHandler handler, int width)
    {
        var button = CreateBaseButton(text, icon, handler, width);
        button.BackColor = AppTheme.Surface;
        button.ForeColor = AppTheme.Primary;
        button.IconColor = AppTheme.Primary;
        button.FlatAppearance.BorderColor = AppTheme.BorderStrong;
        button.FlatAppearance.MouseOverBackColor = AppTheme.PrimarySoft;
        return button;
    }

    private static IconButton CreateDangerButton(string text, IconChar icon, EventHandler handler, int width)
    {
        var button = CreateBaseButton(text, icon, handler, width);
        button.BackColor = AppTheme.Surface;
        button.ForeColor = AppTheme.Danger;
        button.IconColor = AppTheme.Danger;
        button.FlatAppearance.BorderColor = AppTheme.Danger;
        button.FlatAppearance.MouseOverBackColor = AppTheme.DangerSoft;
        return button;
    }

    private static IconButton CreateBaseButton(string text, IconChar icon, EventHandler handler, int width)
    {
        var button = new IconButton
        {
            Text = text,
            Dock = width == 0 ? DockStyle.Fill : DockStyle.None,
            Width = width == 0 ? 120 : width,
            Height = 36,
            Margin = new Padding(0, 0, 8, 4),
            FlatStyle = FlatStyle.Flat,
            IconChar = icon,
            IconColor = AppTheme.Primary,
            IconFont = IconFont.Auto,
            IconSize = 16,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            TextAlign = ContentAlignment.MiddleCenter,
            ImageAlign = ContentAlignment.MiddleCenter,
            UseVisualStyleBackColor = false
        };
        button.Click += handler;
        return button;
    }

    private static string FormatMoney(decimal value) => $"{value:N0} đ";

    private static List<ProductDto> CreateStubProducts() =>
    [
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", CategoryName = "Đồ uống", Unit = "Chai", CostPrice = 4200, SellingPrice = 6000, QuantityOnHand = 110, MinStockLevel = 30 },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", CategoryName = "Đồ uống", Unit = "Lon", CostPrice = 7800, SellingPrice = 11000, QuantityOnHand = 68, MinStockLevel = 25 },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", CategoryName = "Thực phẩm", Unit = "Gói", CostPrice = 3500, SellingPrice = 5000, QuantityOnHand = 195, MinStockLevel = 50 },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", CategoryName = "Gia dụng", Unit = "Chai", CostPrice = 18000, SellingPrice = 25000, QuantityOnHand = 32, MinStockLevel = 35 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", CategoryName = "Vệ sinh", Unit = "Tuýp", CostPrice = 12800, SellingPrice = 18000, QuantityOnHand = 30, MinStockLevel = 35 },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", CategoryName = "Vệ sinh", Unit = "Gói", CostPrice = 9000, SellingPrice = 12500, QuantityOnHand = 118, MinStockLevel = 15 }
    ];

    private sealed class PurchaseLineRow
    {
        public int RowNumber { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal => Quantity * UnitCost;
        public int CurrentStock { get; set; }
    }
}
