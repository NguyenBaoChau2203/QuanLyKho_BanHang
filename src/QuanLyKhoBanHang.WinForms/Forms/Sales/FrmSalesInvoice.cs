using System.ComponentModel;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.DTO.Sales;
using QuanLyKhoBanHang.WinForms.Forms.Common;

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
    private readonly TextBox _note = new();
    private readonly DateTimePicker _saleDate = new();
    private readonly NumericUpDown _quantity = new();
    private readonly NumericUpDown _unitPrice = new();
    private readonly NumericUpDown _discount = new();
    private readonly NumericUpDown _paidAmount = new();
    private readonly Label _message = new();
    private readonly Label _selectedCustomerLabel = new();
    private readonly Label _selectedProductLabel = new();
    private readonly Label _totalLabel = new();
    private readonly Label _discountLabel = new();
    private readonly Label _finalLabel = new();
    private readonly Label _remainingLabel = new();
    private readonly Label _lineCountLabel = new();
    private readonly List<SalesLineRow> _lines = [];
    private List<ProductDto> _products = [];
    private List<CustomerDto> _customers = [];
    private int? _selectedCustomerId;
    private readonly int _currentUserId;

    public FrmSalesInvoice(int currentUserId)
    {
        _currentUserId = currentUserId;
        Text = "Bán hàng";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1320, 780);
        BuildUi();
        LoadData();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = AppTheme.PagePadding
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildBody(), 0, 1);
        root.Controls.Add(_message, 0, 2);
        Controls.Add(root);

        _message.Dock = DockStyle.Fill;
        _message.TextAlign = ContentAlignment.MiddleLeft;
    }

    private Control BuildHeader()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 500));
        panel.Controls.Add(UiFactory.SectionHeader(
            "Hóa đơn bán hàng",
            "Chọn khách hàng, thêm dòng hàng và theo dõi tổng tiền, chiết khấu, thành tiền.",
            IconChar.CartShopping), 0, 0);

        var meta = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Padding = new Padding(0, 6, 0, 0)
        };
        meta.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
        meta.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136));
        meta.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96));
        meta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _invoiceCode.Width = 150;
        _invoiceCode.ReadOnly = true;
        _invoiceCode.Dock = DockStyle.Fill;
        _saleDate.Width = 120;
        _saleDate.Dock = DockStyle.Fill;
        _saleDate.Format = DateTimePickerFormat.Custom;
        _saleDate.CustomFormat = "dd/MM/yyyy";
        meta.Controls.Add(BuildInlineLabel("Ngày\u00A0bán"), 0, 0);
        meta.Controls.Add(_saleDate, 1, 0);
        meta.Controls.Add(BuildInlineLabel("Mã hóa đơn"), 2, 0);
        meta.Controls.Add(_invoiceCode, 3, 0);
        panel.Controls.Add(meta, 1, 0);
        return panel;
    }

    private Control BuildBody()
    {
        var splitter = UiFactory.HorizontalSplitter(570, 520);
        splitter.Panel1.Padding = new Padding(0, 0, 12, 0);
        splitter.Panel2.Padding = new Padding(0);
        splitter.Panel1.Controls.Add(BuildLookupCard());
        splitter.Panel2.Controls.Add(BuildInvoiceCard());
        return splitter;
    }

    private Control BuildLookupCard()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0);
        card.Padding = new Padding(14, 12, 14, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.Controls.Add(BuildSalesSectionHeader("Tra cứu bán hàng", "Giữ khu vực tìm kiếm rộng rãi như layout cũ.", IconChar.MagnifyingGlass), 0, 0);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(BuildProductTab());
        tabs.TabPages.Add(BuildCustomerTab());
        layout.Controls.Add(tabs, 0, 1);

        _selectedProductLabel.Dock = DockStyle.Fill;
        _selectedProductLabel.ForeColor = AppTheme.TextMuted;
        _selectedProductLabel.TextAlign = ContentAlignment.MiddleLeft;
        _selectedProductLabel.AutoEllipsis = true;
        layout.Controls.Add(_selectedProductLabel, 0, 2);

        card.Controls.Add(layout);
        return card;
    }

    private TabPage BuildProductTab()
    {
        var page = new TabPage("Sản phẩm");
        page.BackColor = AppTheme.Surface;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(8) };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var search = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
        search.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 84));
        search.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        search.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116));
        search.Controls.Add(new Label { Text = "Tìm sản phẩm", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        _productSearch.Dock = DockStyle.Fill;
        _productSearch.PlaceholderText = "Mã, tên...";
        _productSearch.TextChanged += (_, _) => ApplyProductFilter();
        search.Controls.Add(_productSearch, 1, 0);
        search.Controls.Add(CreatePrimaryButton("Thêm dòng", IconChar.Plus, (_, _) => AddSelectedProduct(), 108), 2, 0);
        layout.Controls.Add(search, 0, 0);

        ConfigureProductGrid();
        layout.Controls.Add(_productGrid, 0, 1);
        page.Controls.Add(layout);
        return page;
    }

    private TabPage BuildCustomerTab()
    {
        var page = new TabPage("Khách hàng");
        page.BackColor = AppTheme.Surface;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(8) };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        var search = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
        search.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
        search.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        search.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 116));
        search.Controls.Add(new Label { Text = "Tìm khách", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        _customerSearch.Dock = DockStyle.Fill;
        _customerSearch.PlaceholderText = "Mã, tên, SĐT...";
        _customerSearch.TextChanged += (_, _) => ApplyCustomerFilter();
        search.Controls.Add(_customerSearch, 1, 0);
        search.Controls.Add(CreatePrimaryButton("Chọn khách", IconChar.Check, (_, _) => SelectCustomer(), 108), 2, 0);
        layout.Controls.Add(search, 0, 0);

        ConfigureCustomerGrid();
        layout.Controls.Add(_customerGrid, 0, 1);
        _selectedCustomerLabel.Dock = DockStyle.Fill;
        _selectedCustomerLabel.TextAlign = ContentAlignment.MiddleLeft;
        _selectedCustomerLabel.ForeColor = AppTheme.TextMuted;
        _selectedCustomerLabel.AutoEllipsis = true;
        layout.Controls.Add(_selectedCustomerLabel, 0, 2);
        page.Controls.Add(layout);
        return page;
    }

    private Control BuildInvoiceCard()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0);
        card.Padding = new Padding(14, 12, 14, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 208));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.Controls.Add(BuildSalesSectionHeader("Thông tin hóa đơn", "Nhập số lượng, giá bán và quản lý dòng hàng.", IconChar.FileInvoiceDollar), 0, 0);
        layout.Controls.Add(BuildInvoiceForm(), 0, 1);
        layout.Controls.Add(BuildLineGridPanel(), 0, 2);
        layout.Controls.Add(BuildSummaryPanel(), 0, 3);
        layout.Controls.Add(BuildActionRow(), 0, 4);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildInvoiceForm()
    {
        var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3 };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        form.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        _quantity.Minimum = 1;
        _quantity.Maximum = 100000;
        _quantity.Value = 1;
        _unitPrice.Maximum = 100000000;
        _unitPrice.ThousandsSeparator = true;
        _unitPrice.Increment = 1000;
        _discount.Maximum = 100000000;
        _discount.ThousandsSeparator = true;
        _discount.Increment = 1000;
        _discount.ValueChanged += (_, _) => UpdateSummary();
        _paidAmount.Maximum = 100000000;
        _paidAmount.ThousandsSeparator = true;
        _paidAmount.Increment = 10000;
        _paidAmount.ValueChanged += (_, _) => UpdateSummary();
        _note.Multiline = true;
        _note.PlaceholderText = "Ghi chú hóa đơn...";

        form.Controls.Add(BuildField("Số lượng", _quantity), 0, 0);
        form.Controls.Add(BuildField("Đơn giá", _unitPrice), 1, 0);
        form.Controls.Add(BuildField("Chiết khấu", _discount), 0, 1);
        form.Controls.Add(BuildField("Đã thanh toán", _paidAmount), 1, 1);
        form.Controls.Add(BuildField("Ghi chú", _note, true), 0, 2);
        form.SetColumnSpan(form.GetControlFromPosition(0, 2)!, 2);
        return form;
    }

    private Control BuildLineGridPanel()
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(0, 8, 0, 0) };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _lineCountLabel.Dock = DockStyle.Fill;
        _lineCountLabel.TextAlign = ContentAlignment.MiddleLeft;
        _lineCountLabel.ForeColor = AppTheme.TextMuted;
        panel.Controls.Add(_lineCountLabel, 0, 0);
        ConfigureLineGrid();
        panel.Controls.Add(_lineGrid, 0, 1);
        return panel;
    }

    private Control BuildSummaryPanel()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4, Padding = new Padding(0, 8, 0, 0) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (var i = 0; i < 4; i++)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        }

        AddSummaryRow(layout, 0, "Tổng tiền", _totalLabel);
        AddSummaryRow(layout, 1, "Chiết khấu", _discountLabel);
        AddSummaryRow(layout, 2, "Thành tiền", _finalLabel, true, AppTheme.Primary);
        AddSummaryRow(layout, 3, "Tiền còn lại", _remainingLabel, true, AppTheme.Success);
        return layout;
    }

    private Control BuildActionRow()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        panel.Controls.Add(CreatePrimaryButton("Thêm dòng", IconChar.Plus, (_, _) => AddSelectedProduct(), 116));
        panel.Controls.Add(CreateSecondaryButton("Xóa dòng", IconChar.Trash, (_, _) => RemoveCurrentLine(), 112, AppTheme.Danger));
        panel.Controls.Add(CreatePrimaryButton("Lưu hóa đơn", IconChar.FloppyDisk, (_, _) => SaveInvoice(), 120));
        panel.Controls.Add(CreateSecondaryButton("Làm mới", IconChar.RotateRight, (_, _) => ResetForm(), 108, AppTheme.Primary));
        return panel;
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
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Code), HeaderText = "Mã", FillWeight = 72 });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Name), HeaderText = "Tên", FillWeight = 170 });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.SellingPrice), HeaderText = "Giá", DefaultCellStyle = { Format = "N0" }, FillWeight = 86 });
        _productGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.QuantityOnHand), HeaderText = "Tồn", FillWeight = 62 });
        _productGrid.SelectionChanged += (_, _) => SyncSelectedProduct();
        _productGrid.CellDoubleClick += (_, _) => AddSelectedProduct();
    }

    private void ConfigureCustomerGrid()
    {
        _customerGrid.Dock = DockStyle.Fill;
        _customerGrid.ReadOnly = true;
        _customerGrid.AllowUserToAddRows = false;
        _customerGrid.AllowUserToDeleteRows = false;
        _customerGrid.RowHeadersVisible = false;
        _customerGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _customerGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _customerGrid.MultiSelect = false;
        _customerGrid.DataSource = _customerSource;
        UiFactory.StyleGrid(_customerGrid);
        _customerGrid.AutoGenerateColumns = false;
        _customerGrid.Columns.Clear();
        _customerGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Code), HeaderText = "Mã KH", FillWeight = 80 });
        _customerGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Name), HeaderText = "Tên khách", FillWeight = 160 });
        _customerGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Phone), HeaderText = "SĐT", FillWeight = 96 });
        _customerGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Address), HeaderText = "Địa chỉ", FillWeight = 130 });
        _customerGrid.CellDoubleClick += (_, _) => SelectCustomer();
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
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesLineRow.ProductName), HeaderText = "Sản phẩm", FillWeight = 190 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesLineRow.Quantity), HeaderText = "SL", FillWeight = 55 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesLineRow.UnitPrice), HeaderText = "Đơn giá", DefaultCellStyle = { Format = "N0" }, FillWeight = 90 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesLineRow.LineTotal), HeaderText = "Thành tiền", DefaultCellStyle = { Format = "N0" }, FillWeight = 110 });
        _lineGrid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SalesLineRow.Stock), HeaderText = "Tồn", FillWeight = 60 });
    }

    private void LoadData()
    {
        var productResult = _productService.GetAllProducts();
        var customerResult = _customerService.GetAllCustomers();
        _products = productResult.Success && productResult.Data is { Count: > 0 } ? productResult.Data! : CreateStubProducts();
        _customers = customerResult.Success && customerResult.Data is { Count: > 0 } ? customerResult.Data! : CreateStubCustomers();
        _invoiceCode.Text = $"HD-{DateTime.Now:yyyyMMdd-HHmm}";
        _saleDate.Value = DateTime.Today;
        ApplyProductFilter();
        ApplyCustomerFilter();
        SelectDefaultCustomer();
        RefreshLineSource();
        UpdateSummary();
        var success = productResult.Success && customerResult.Success;
        var msg = success ? "Sẵn sàng lập hóa đơn." : $"{(productResult.Success ? customerResult.Message : productResult.Message)} - Đang dùng dữ liệu demo.";
        SetMessage(msg, !success);
    }

    private void ApplyProductFilter()
    {
        var keyword = _productSearch.Text.Trim();
        var filtered = string.IsNullOrWhiteSpace(keyword)
            ? _products
            : _products.Where(x => x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        _productSource.DataSource = new BindingList<ProductDto>(filtered);
        SyncSelectedProduct();
    }

    private void ApplyCustomerFilter()
    {
        var keyword = _customerSearch.Text.Trim();
        var filtered = string.IsNullOrWhiteSpace(keyword)
            ? _customers
            : _customers.Where(x => x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || (x.Phone?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        _customerSource.DataSource = new BindingList<CustomerDto>(filtered);
    }

    private void SyncSelectedProduct()
    {
        if (_productGrid.CurrentRow?.DataBoundItem is not ProductDto product)
        {
            _selectedProductLabel.Text = "Chọn sản phẩm, kiểm tra tồn và thêm vào hóa đơn.";
            return;
        }

        if (product.SellingPrice >= 0 && product.SellingPrice <= _unitPrice.Maximum)
        {
            _unitPrice.Value = product.SellingPrice;
        }

        _selectedProductLabel.Text = $"Đang chọn: {product.Code} - {product.Name} | Giá {product.SellingPrice:N0} đ | Tồn {product.QuantityOnHand:N0}";
    }

    private void SelectDefaultCustomer()
    {
        var customer = _customers.FirstOrDefault(x => x.Code == "KH001") ?? _customers.FirstOrDefault();
        if (customer is null)
        {
            _selectedCustomerLabel.Text = "Khách hàng đang chọn: chưa có";
            return;
        }

        ApplySelectedCustomer(customer);
    }

    private void SelectCustomer()
    {
        if (_customerGrid.CurrentRow?.DataBoundItem is not CustomerDto customer)
        {
            SetMessage("Chọn một khách hàng trong bảng.", true);
            return;
        }

        ApplySelectedCustomer(customer);
        SetMessage($"Đã chọn khách hàng: {customer.Name}");
    }

    private void ApplySelectedCustomer(CustomerDto customer)
    {
        _selectedCustomerId = customer.Id;
        var contact = string.Join(" - ", new[] { customer.Phone, customer.Address }.Where(x => !string.IsNullOrWhiteSpace(x)));
        _selectedCustomerLabel.Text = string.IsNullOrWhiteSpace(contact)
            ? $"Khách hàng đang chọn: {customer.Name} ({customer.Code})"
            : $"Khách hàng đang chọn: {customer.Name} ({customer.Code}) - {contact}";
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

        if (_quantity.Value > product.QuantityOnHand)
        {
            SetMessage("Số lượng bán vượt quá tồn kho hiện tại.", true);
            return;
        }

        _lines.Add(new SalesLineRow
        {
            ProductId = product.Id,
            ProductCode = product.Code,
            ProductName = product.Name,
            Quantity = (int)_quantity.Value,
            UnitPrice = _unitPrice.Value,
            Stock = product.QuantityOnHand
        });
        RefreshLineSource();
        UpdateSummary();
        SetMessage($"Đã thêm {product.Name} vào hóa đơn.");
    }

    private void RemoveCurrentLine()
    {
        if (_lineGrid.CurrentRow?.DataBoundItem is not SalesLineRow line)
        {
            SetMessage("Chọn một dòng để xóa.", true);
            return;
        }

        _lines.Remove(line);
        RefreshLineSource();
        UpdateSummary();
        SetMessage("Đã xóa dòng hàng.");
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
            CreatedByUserId = _currentUserId,
            InvoiceDate = _saleDate.Value.Date,
            Note = _note.Text,
            Lines = _lines.Select(x => new SalesInvoiceLineDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList(),
            TotalAmount = total,
            DiscountAmount = _discount.Value
        };

        var result = _salesService.CreateInvoice(invoice);
        if (result.Success)
        {
            SetMessage(result.Message);
            ResetForm();
        }
        else
        {
            SetMessage(result.Message, true);
        }
    }

    private void ResetForm()
    {
        _lines.Clear();
        _discount.Value = 0;
        _paidAmount.Value = 0;
        _quantity.Value = 1;
        _unitPrice.Value = 0;
        _note.Clear();
        _invoiceCode.Text = $"HD-{DateTime.Now:yyyyMMdd-HHmm}";
        _saleDate.Value = DateTime.Today;
        SelectDefaultCustomer();
        RefreshLineSource();
        UpdateSummary();
        SetMessage("Đã làm mới hóa đơn.");
    }

    private void RefreshLineSource()
    {
        _lineSource.DataSource = new BindingList<SalesLineRow>(_lines.ToList());
        _lineCountLabel.Text = $"Danh sách sản phẩm ({_lines.Count} dòng)";
    }

    private void UpdateSummary()
    {
        var total = _lines.Sum(x => x.LineTotal);
        var final = Math.Max(0, total - _discount.Value);
        var remaining = Math.Max(0, final - _paidAmount.Value);
        _totalLabel.Text = FormatMoney(total);
        _discountLabel.Text = FormatMoney(_discount.Value);
        _finalLabel.Text = FormatMoney(final);
        _remainingLabel.Text = FormatMoney(remaining);
    }

    private void SetMessage(string message, bool error = false)
    {
        UiFactory.SetMessage(_message, message, error);
    }

    private static Control BuildField(string label, Control control, bool multiline = false)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(0, 0, 12, 4) };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, ForeColor = AppTheme.Text }, 0, 0);
        control.Dock = multiline ? DockStyle.Fill : DockStyle.Top;
        control.Margin = Padding.Empty;
        if (!multiline)
        {
            control.Height = 28;
            if (control is UpDownBase upDown)
            {
                upDown.BorderStyle = BorderStyle.FixedSingle;
            }
        }
        else if (control is TextBox textBox)
        {
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.MinimumSize = new Size(0, 54);
        }

        panel.Controls.Add(control, 0, 1);
        return panel;
    }

    private static Control BuildSalesSectionHeader(string title, string subtitle, IconChar icon)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(0, 2, 0, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

        layout.Controls.Add(new IconPictureBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            IconChar = icon,
            IconColor = AppTheme.Primary,
            IconFont = IconFont.Auto,
            IconSize = 18,
            Padding = new Padding(0, 4, 8, 0)
        }, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = AppTheme.SectionFont(12F),
            ForeColor = AppTheme.Primary,
            TextAlign = ContentAlignment.MiddleLeft
        }, 1, 0);

        layout.Controls.Add(new Label
        {
            Text = subtitle,
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(9.25F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.TopLeft,
            AutoEllipsis = true
        }, 1, 1);

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
        value.Font = strong ? AppTheme.TitleFont(14F) : AppTheme.BodyFont(10F);
        value.ForeColor = color ?? AppTheme.Text;
        layout.Controls.Add(value, 1, row);
    }

    private static Label BuildInlineLabel(string text) => new()
    {
        Text = text,
        AutoSize = false,
        Dock = DockStyle.Fill,
        Margin = new Padding(0, 0, 8, 0),
        Padding = new Padding(0, 8, 0, 0),
        ForeColor = AppTheme.Text,
        TextAlign = ContentAlignment.TopLeft,
        AutoEllipsis = true
    };

    private static IconButton CreatePrimaryButton(string text, IconChar icon, EventHandler handler, int width)
    {
        var button = CreateBaseButton(text, icon, handler, width);
        button.BackColor = AppTheme.Primary;
        button.ForeColor = Color.White;
        button.IconColor = Color.White;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);
        return button;
    }

    private static IconButton CreateSecondaryButton(string text, IconChar icon, EventHandler handler, int width, Color color)
    {
        var button = CreateBaseButton(text, icon, handler, width);
        button.BackColor = AppTheme.Surface;
        button.ForeColor = color;
        button.IconColor = color;
        button.FlatAppearance.BorderColor = color == AppTheme.Danger ? AppTheme.Danger : AppTheme.BorderStrong;
        button.FlatAppearance.MouseOverBackColor = color == AppTheme.Danger ? AppTheme.DangerSoft : AppTheme.PrimarySoft;
        return button;
    }

    private static IconButton CreateBaseButton(string text, IconChar icon, EventHandler handler, int width)
    {
        var button = new IconButton
        {
            Text = text,
            Width = width,
            Height = 34,
            Margin = new Padding(0, 0, 8, 0),
            FlatStyle = FlatStyle.Flat,
            IconChar = icon,
            IconFont = IconFont.Auto,
            IconSize = 15,
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
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", SellingPrice = 6000, QuantityOnHand = 110 },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", SellingPrice = 11000, QuantityOnHand = 68 },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", SellingPrice = 5000, QuantityOnHand = 195 },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", SellingPrice = 25000, QuantityOnHand = 32 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", SellingPrice = 18000, QuantityOnHand = 30 },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", SellingPrice = 12500, QuantityOnHand = 118 }
    ];

    private static List<CustomerDto> CreateStubCustomers() =>
    [
        new CustomerDto { Id = 1, Code = "KH001", Name = "Khách lẻ" },
        new CustomerDto { Id = 2, Code = "KH002", Name = "Cửa hàng Tạp hóa An Phú", Phone = "0911111111", Email = "anphu@example.com", Address = "Bình Dương" },
        new CustomerDto { Id = 3, Code = "KH003", Name = "Siêu thị Hòa Bình", Phone = "0988777666", Email = "hoabinh@example.com", Address = "TP. Hồ Chí Minh" }
    ];

    private sealed class SalesLineRow
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
        public int Stock { get; set; }
    }
}
