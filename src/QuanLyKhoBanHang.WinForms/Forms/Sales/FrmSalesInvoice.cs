using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.DTO.Sales;

namespace QuanLyKhoBanHang.WinForms.Forms.Sales;

public sealed class FrmSalesInvoice : Form
{
    private readonly SalesService _salesService = new();
    private readonly ProductService _productService = new();
    private readonly CustomerService _customerService = new();
    private readonly BindingSource _lineSource = new();
    private readonly BindingSource _productSource = new();
    private readonly BindingSource _customerSource = new();
    private readonly DataGridView _lineGrid = new();
    private readonly DataGridView _productGrid = new();
    private readonly DataGridView _customerGrid = new();
    private readonly TextBox _productSearch = new();
    private readonly TextBox _customerSearch = new();
    private readonly TextBox _invoiceCode = new();
    private readonly NumericUpDown _quantity = new();
    private readonly NumericUpDown _unitPrice = new();
    private readonly NumericUpDown _discount = new();
    private readonly TextBox _note = new();
    private readonly Label _message = new();
    private readonly Label _totalLabel = new();
    private readonly Label _finalLabel = new();
    private readonly Label _customerHint = new();
    private readonly List<SalesInvoiceLineDto> _lines = [];
    private List<ProductDto> _products = [];
    private List<CustomerDto> _customers = [];
    private int? _selectedCustomerId;

    public FrmSalesInvoice()
    {
        Text = "Bán hàng";
        BackColor = Color.FromArgb(245, 247, 250);
        Font = new Font("Segoe UI", 10F);
        MinimumSize = new Size(1320, 780);
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
        panel.Controls.Add(new Label { Text = "Hóa đơn bán hàng", Dock = DockStyle.Top, Height = 34, Font = new Font("Segoe UI", 18F, FontStyle.Bold) });
        panel.Controls.Add(new Label { Text = "Chọn khách hàng, thêm dòng hàng và theo dõi tổng tiền, chiết khấu, thành tiền.", Dock = DockStyle.Bottom, Height = 22, ForeColor = Color.FromArgb(96, 108, 129) });
        return panel;
    }

    private Control BuildBody()
    {
        var splitter = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 700 };
        splitter.Panel1.Controls.Add(BuildLookupPanel());
        splitter.Panel2.Controls.Add(BuildEditorPanel());
        return splitter;
    }

    private Control BuildLookupPanel()
    {
        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildProductTab());
        tabs.TabPages.Add(BuildCustomerTab());
        return tabs;
    }

    private TabPage BuildProductTab()
    {
        var page = new TabPage("Sản phẩm");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        var search = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        search.Controls.Add(new Label { Text = "Tìm sản phẩm", AutoSize = true, Padding = new Padding(0, 10, 8, 0) });
        _productSearch.Width = 260;
        _productSearch.PlaceholderText = "Mã, tên...";
        _productSearch.TextChanged += (_, _) => ApplyProductFilter();
        search.Controls.Add(_productSearch);
        search.Controls.Add(CreateButton("Thêm dòng", (_, _) => AddSelectedProduct()));

        _productGrid.Dock = DockStyle.Fill;
        _productGrid.ReadOnly = true;
        _productGrid.AllowUserToAddRows = false;
        _productGrid.AllowUserToDeleteRows = false;
        _productGrid.RowHeadersVisible = false;
        _productGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _productGrid.DataSource = _productSource;
        _productGrid.AutoGenerateColumns = false;
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Code), HeaderText = "Mã" });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Name), HeaderText = "Tên" });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.SellingPrice), HeaderText = "Giá", DefaultCellStyle = { Format = "N0" } });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.QuantityOnHand), HeaderText = "Tồn" });
        _productGrid.SelectionChanged += (_, _) => SyncSelectedProductPrice();
        _productGrid.CellDoubleClick += (_, _) => AddSelectedProduct();

        var hint = new Label { Text = "Chọn sản phẩm, kiểm tra tồn và thêm vào hóa đơn.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(96, 108, 129) };
        layout.Controls.Add(search, 0, 0);
        layout.Controls.Add(_productGrid, 0, 1);
        layout.Controls.Add(hint, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private TabPage BuildCustomerTab()
    {
        var page = new TabPage("Khách hàng");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

        var search = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        search.Controls.Add(new Label { Text = "Tìm khách hàng", AutoSize = true, Padding = new Padding(0, 10, 8, 0) });
        _customerSearch.Width = 260;
        _customerSearch.PlaceholderText = "Mã, tên, điện thoại...";
        _customerSearch.TextChanged += (_, _) => ApplyCustomerFilter();
        search.Controls.Add(_customerSearch);
        search.Controls.Add(CreateButton("Chọn khách", (_, _) => SelectCustomer()));

        _customerGrid.Dock = DockStyle.Fill;
        _customerGrid.ReadOnly = true;
        _customerGrid.AllowUserToAddRows = false;
        _customerGrid.AllowUserToDeleteRows = false;
        _customerGrid.RowHeadersVisible = false;
        _customerGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _customerGrid.DataSource = _customerSource;
        _customerGrid.AutoGenerateColumns = false;
        _customerGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Code), HeaderText = "Mã" });
        _customerGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Name), HeaderText = "Tên" });
        _customerGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Phone), HeaderText = "Điện thoại" });
        _customerGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Email), HeaderText = "Email" });

        _customerHint.Text = "Khách hàng đang chọn: chưa có";
        _customerHint.Dock = DockStyle.Fill;
        _customerHint.TextAlign = ContentAlignment.MiddleLeft;
        _customerHint.ForeColor = Color.FromArgb(96, 108, 129);
        layout.Controls.Add(search, 0, 0);
        layout.Controls.Add(_customerGrid, 0, 1);
        layout.Controls.Add(_customerHint, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private Control BuildEditorPanel()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 5, ColumnCount = 1, Padding = new Padding(8, 0, 0, 0) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 148));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

        root.Controls.Add(new Label { Text = "Thông tin hóa đơn", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 14F, FontStyle.Bold) }, 0, 0);
        root.Controls.Add(BuildForm(), 0, 1);
        root.Controls.Add(BuildLineGrid(), 0, 2);
        root.Controls.Add(BuildSummary(), 0, 3);
        root.Controls.Add(BuildButtons(), 0, 4);
        return root;
    }

    private Control BuildForm()
    {
        var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3 };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        form.Controls.Add(BuildField("Mã hóa đơn", _invoiceCode), 0, 0);
        form.Controls.Add(BuildField("Số lượng", _quantity), 1, 0);
        form.Controls.Add(BuildField("Đơn giá", _unitPrice), 0, 1);
        form.Controls.Add(BuildField("Chiết khấu", _discount), 1, 1);
        var noteField = BuildField("Ghi chú", _note, true);
        form.Controls.Add(noteField, 0, 2);
        form.SetColumnSpan(noteField, 2);

        _quantity.Minimum = 1;
        _quantity.Maximum = 100000;
        _quantity.Value = 1;
        _unitPrice.Maximum = 100000000;
        _unitPrice.Increment = 1000;
        _discount.Maximum = 100000000;
        _discount.Increment = 1000;
        _discount.ValueChanged += (_, _) => UpdateSummary();
        _note.Multiline = true;
        return form;
    }

    private Control BuildLineGrid()
    {
        _lineGrid.Dock = DockStyle.Fill;
        _lineGrid.ReadOnly = true;
        _lineGrid.AllowUserToAddRows = false;
        _lineGrid.AllowUserToDeleteRows = false;
        _lineGrid.RowHeadersVisible = false;
        _lineGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _lineGrid.DataSource = _lineSource;
        _lineGrid.AutoGenerateColumns = false;
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesInvoiceLineDto.ProductName), HeaderText = "Sản phẩm" });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesInvoiceLineDto.Quantity), HeaderText = "SL" });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesInvoiceLineDto.UnitPrice), HeaderText = "Đơn giá", DefaultCellStyle = { Format = "N0" } });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesInvoiceLineDto.LineTotal), HeaderText = "Thành tiền", DefaultCellStyle = { Format = "N0" } });
        return _lineGrid;
    }

    private Control BuildSummary()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        _totalLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
        _finalLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        panel.Controls.Add(_totalLabel);
        panel.Controls.Add(_finalLabel);
        return panel;
    }

    private Control BuildButtons()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        panel.Controls.Add(CreateButton("Thêm dòng", (_, _) => AddSelectedProduct()));
        panel.Controls.Add(CreateButton("Xóa dòng", (_, _) => RemoveCurrentLine()));
        panel.Controls.Add(CreateButton("Lưu hóa đơn", (_, _) => SaveInvoice()));
        panel.Controls.Add(CreateButton("Làm mới", (_, _) => ResetForm()));
        return panel;
    }

    private static Control BuildField(string label, Control control, bool multiline = false)
    {
        var panel = new Panel { Dock = DockStyle.Top, Height = multiline ? 92 : 66, Padding = new Padding(0, 0, 12, 8) };
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Top, Height = 20 });
        control.Dock = DockStyle.Bottom;
        control.Height = multiline ? 52 : 30;
        panel.Controls.Add(control);
        return panel;
    }

    private Button CreateButton(string text, EventHandler handler)
    {
        var button = new Button { Text = text, Height = 36, Width = 110, Margin = new Padding(0, 0, 8, 0) };
        button.Click += handler;
        return button;
    }

    private void LoadData()
    {
        var productResult = _productService.GetAllProducts();
        var customerResult = _customerService.GetAllCustomers();
        _products = productResult.Success && productResult.Data is { Count: > 0 } ? productResult.Data! : CreateStubProducts();
        _customers = customerResult.Success && customerResult.Data is { Count: > 0 } ? customerResult.Data! : CreateStubCustomers();
        ApplyProductFilter();
        ApplyCustomerFilter();
        _invoiceCode.Text = $"HD-{DateTime.Now:yyyyMMdd-HHmm}";
        UpdateSummary();
        SetMessage("Sẵn sàng lập hóa đơn.");
    }

    private void ApplyProductFilter()
    {
        var keyword = _productSearch.Text.Trim();
        var filtered = string.IsNullOrWhiteSpace(keyword)
            ? _products
            : _products.Where(x => x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        _productSource.DataSource = new BindingList<ProductDto>(filtered);
        SyncSelectedProductPrice();
    }

    private void ApplyCustomerFilter()
    {
        var keyword = _customerSearch.Text.Trim();
        var filtered = string.IsNullOrWhiteSpace(keyword)
            ? _customers
            : _customers.Where(x => x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || (x.Phone?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        _customerSource.DataSource = new BindingList<CustomerDto>(filtered);
    }

    private void AddSelectedProduct()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is not ProductDto product)
        {
            SetMessage("Chọn sản phẩm trước khi thêm dòng.", true);
            return;
        }
        if (_quantity.Value <= 0 || _unitPrice.Value <= 0)
        {
            SetMessage("Số lượng và đơn giá phải lớn hơn 0.", true);
            return;
        }

        _lines.Add(new SalesInvoiceLineDto { ProductId = product.Id, ProductName = product.Name, Quantity = (int)_quantity.Value, UnitPrice = _unitPrice.Value });
        _lineSource.DataSource = new BindingList<SalesInvoiceLineDto>(_lines.ToList());
        UpdateSummary();
        SetMessage($"Đã thêm {product.Name} vào hóa đơn.");
    }

    private void RemoveCurrentLine()
    {
        if (_lineGrid.CurrentRow?.DataBoundItem is not SalesInvoiceLineDto line)
        {
            SetMessage("Chọn một dòng để xóa.", true);
            return;
        }
        _lines.Remove(line);
        _lineSource.DataSource = new BindingList<SalesInvoiceLineDto>(_lines.ToList());
        UpdateSummary();
        SetMessage("Đã xóa dòng hàng.");
    }

    private void SelectCustomer()
    {
        if (_customerGrid.CurrentRow?.DataBoundItem is not CustomerDto customer)
        {
            SetMessage("Chọn một khách hàng trong bảng.", true);
            return;
        }
        _selectedCustomerId = customer.Id;
        _customerHint.Text = $"Khách hàng đang chọn: {customer.Name} ({customer.Code})";
        SetMessage($"Đã chọn khách hàng: {customer.Name}");
    }

    private void SaveInvoice()
    {
        if (_lines.Count == 0)
        {
            SetMessage("Hóa đơn phải có ít nhất một dòng hàng.", true);
            return;
        }
        if (_selectedCustomerId is null)
        {
            SetMessage("Vui lòng chọn khách hàng.", true);
            return;
        }
        if (_discount.Value < 0)
        {
            SetMessage("Chiết khấu không hợp lệ.", true);
            return;
        }

        var total = _lines.Sum(x => x.LineTotal);
        if (_discount.Value > total)
        {
            SetMessage("Chiết khấu không được lớn hơn tổng tiền hóa đơn.", true);
            return;
        }

        var invoice = new SalesInvoiceDto
        {
            InvoiceCode = _invoiceCode.Text.Trim(),
            CustomerId = _selectedCustomerId,
            InvoiceDate = DateTime.Today,
            Note = _note.Text,
            Lines = _lines.ToList(),
            TotalAmount = total,
            DiscountAmount = _discount.Value
        };

        var result = _salesService.CreateInvoice(invoice);
        SetMessage(result.Message, !result.Success);
    }

    private void ResetForm()
    {
        _lines.Clear();
        _lineSource.DataSource = new BindingList<SalesInvoiceLineDto>(_lines);
        _discount.Value = 0;
        _quantity.Value = 1;
        _unitPrice.Value = 0;
        _note.Clear();
        _selectedCustomerId = null;
        _customerHint.Text = "Khách hàng đang chọn: chưa có";
        UpdateSummary();
        SetMessage("Đã làm mới hóa đơn.");
    }

    private void UpdateSummary()
    {
        var total = _lines.Sum(x => x.LineTotal);
        var final = total - _discount.Value;
        _totalLabel.Text = $"Tổng tiền: {total:N0} đ";
        _finalLabel.Text = $"Thành tiền sau chiết khấu: {final:N0} đ";
    }

    private void SetMessage(string message, bool error = false)
    {
        _message.Text = message;
        _message.ForeColor = error ? Color.Firebrick : Color.FromArgb(92, 102, 121);
    }

    private void SyncSelectedProductPrice()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is ProductDto product && product.SellingPrice >= 0 && product.SellingPrice <= _unitPrice.Maximum)
        {
            _unitPrice.Value = product.SellingPrice;
        }
    }

    private static List<ProductDto> CreateStubProducts() =>
    [
        new ProductDto { Id = 1, Code = "SP-001", Name = "Bút bi Thiên Long", SellingPrice = 5000, QuantityOnHand = 120 },
        new ProductDto { Id = 2, Code = "SP-002", Name = "Sổ tay A5", SellingPrice = 18000, QuantityOnHand = 15 },
        new ProductDto { Id = 3, Code = "SP-003", Name = "Thùng carton 5 lớp", SellingPrice = 45000, QuantityOnHand = 8 }
    ];

    private static List<CustomerDto> CreateStubCustomers() =>
    [
        new CustomerDto { Id = 1, Code = "KH-01", Name = "Nguyễn Văn An", Phone = "0912345678", Email = "an@example.com", Address = "Quận 1, TP.HCM" },
        new CustomerDto { Id = 2, Code = "KH-02", Name = "Trần Thị Mai", Phone = "0987654321", Email = "mai@example.com", Address = "Thủ Dầu Một, Bình Dương" }
    ];
}
