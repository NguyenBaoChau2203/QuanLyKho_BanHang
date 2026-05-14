using System.ComponentModel;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.MasterData;

public sealed class FrmProduct : CrudListForm<ProductDto>
{
    private readonly ProductService _service = new();
    private readonly Label _summaryLabel = new();
    private readonly Label _gridStateLabel = new();
    private readonly Label _selectedStateLabel = new();
    private readonly Label _emptyTitleLabel = new();
    private readonly Label _emptyMessageLabel = new();
    private readonly Label _searchHintLabel = new();
    private readonly Label _editHintLabel = new();
    private readonly Label _metricTotalValue = new();
    private readonly Label _metricActiveValue = new();
    private readonly Label _metricLowStockValue = new();
    private readonly Label _filterSummaryLabel = new();
    private readonly Label _editModeLabel = new();
    private readonly TextBox _priceBox = new();
    private readonly TextBox _quantityBox = new();
    private readonly TextBox _categoryBox = new();
    private readonly TextBox _unitBox = new();
    private readonly TextBox _minStockBox = new();
    private readonly RoundedPanel _searchCard = new();
    private readonly RoundedPanel _gridCard = new();
    private readonly Panel _statusCard = new();
    private readonly RoundedPanel _editCard = new();
    private readonly TableLayoutPanel _editGrid = new();
    private readonly IconButton _clearSearchButton;
    private readonly IconButton _refreshButton;
    private readonly IconButton _addButton;
    private readonly IconButton _editButton;
    private readonly IconButton _saveButton;
    private readonly IconButton _cancelButton;
    private readonly IconButton _deactivateButton;
    private readonly IconButton _setReadonlyButton;
    private List<ProductDto> _items = [];
    private List<ProductDto> _filteredItems = [];
    private ProductDto? _selectedItem;
    private string _dataStateMessage = "Sẵn sàng";
    private bool _dataStateIsError;

    public FrmProduct() : base("Sản phẩm", "Quản lý danh mục sản phẩm, giá bán, đơn vị và trạng thái tồn kho.")
    {
        _clearSearchButton = UiFactory.IconActionButton("Xóa lọc", IconChar.Eraser, (_, _) => { SearchBox.Clear(); ApplyFilter(); }, 96);
        _refreshButton = UiFactory.IconActionButton("Làm mới", IconChar.Rotate, (_, _) => RefreshData(), 100);
        _addButton = UiFactory.IconActionButton("Thêm mới", IconChar.Plus, (_, _) => BeginAdd(), 104);
        _editButton = UiFactory.IconActionButton("Chỉnh sửa", IconChar.PenToSquare, (_, _) => BeginEdit(), 108);
        _saveButton = UiFactory.IconActionButton("Lưu", IconChar.FloppyDisk, (_, _) => SaveCurrent(), 82);
        _cancelButton = UiFactory.IconActionButton("Hủy", IconChar.CircleXmark, (_, _) => CancelEdit(), 82);
        _deactivateButton = UiFactory.IconActionButton("Ngừng kích hoạt", IconChar.ToggleOff, (_, _) => DeactivateCurrent(), 142);
        _setReadonlyButton = UiFactory.IconActionButton("Chỉ xem", IconChar.Eye, (_, _) => ToggleEditing(false), 88);

        ConfigureActionButtons();
        ConfigureScreen();
        RefreshData();
        ToggleEditing(false);
    }

    protected override Control BuildHeader(string title, string subtitle)
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        header.Controls.Add(UiFactory.SectionHeader(title, subtitle, IconChar.BoxOpen), 0, 0);

        _summaryLabel.Dock = DockStyle.Fill;
        _summaryLabel.Font = AppTheme.SectionFont(10.5F);
        _summaryLabel.ForeColor = AppTheme.Primary;
        _summaryLabel.TextAlign = ContentAlignment.MiddleRight;
        _summaryLabel.AutoEllipsis = true;
        _summaryLabel.Margin = new Padding(12, 0, 0, 0);
        header.Controls.Add(_summaryLabel, 1, 0);
        return header;
    }

    protected override void RefreshData()
    {
        var result = _service.GetAllProducts();
        _items = result.Success && result.Data is { Count: > 0 } ? result.Data! : CreateStubItems();
        _dataStateMessage = result.Success ? result.Message : $"{result.Message} - đang hiển thị dữ liệu mẫu.";
        _dataStateIsError = !result.Success;
        _filteredItems = _items.ToList();
        BindingSource.DataSource = new BindingList<ProductDto>(_filteredItems);
        ShowEmpty(_filteredItems.Count == 0);
        UpdateSummary();
        UpdateMetrics();
        UpdateGridState();
        UpdateSelectionState();
    }

    protected override void ApplyFilter()
    {
        var keyword = SearchBox.Text.Trim();
        _filteredItems = string.IsNullOrWhiteSpace(keyword)
            ? _items.ToList()
            : _items.Where(p =>
                    p.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    p.CategoryName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    p.Unit.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

        BindingSource.DataSource = new BindingList<ProductDto>(_filteredItems);
        ShowEmpty(_filteredItems.Count == 0);
        UpdateGridState();
        UpdateMetrics();
        UpdateSummary();
        UpdateFilterSummary();
    }

    protected override void OnSelectionChanged()
    {
        _selectedItem = BindingSource.Current as ProductDto;
        if (_selectedItem is null)
        {
            SelectedId = 0;
            CodeBox.Clear();
            NameBox.Clear();
            DescriptionBox.Clear();
            _categoryBox.Clear();
            _unitBox.Clear();
            _priceBox.Clear();
            _quantityBox.Clear();
            _minStockBox.Clear();
            ActiveBox.Checked = false;
            _selectedStateLabel.Text = "Chưa chọn sản phẩm";
            _editModeLabel.Text = "Sẵn sàng xem hoặc nhập mới.";
            UpdateSelectionState();
            return;
        }

        SelectedId = _selectedItem.Id;
        CodeBox.Text = _selectedItem.Code;
        NameBox.Text = _selectedItem.Name;
        _categoryBox.Text = _selectedItem.CategoryName;
        _unitBox.Text = _selectedItem.Unit;
        _priceBox.Text = _selectedItem.SellingPrice.ToString("N0");
        _quantityBox.Text = _selectedItem.QuantityOnHand.ToString("N0");
        _minStockBox.Text = _selectedItem.MinStockLevel.ToString("N0");
        DescriptionBox.Text = $"Loại hàng: {_selectedItem.CategoryName}\r\nĐơn vị tính: {_selectedItem.Unit}\r\nGiá vốn: {_selectedItem.CostPrice:N0}";
        ActiveBox.Checked = _selectedItem.IsActive;
        _selectedStateLabel.Text = $"Đã chọn: {_selectedItem.Code} - {_selectedItem.Name}";
        _editModeLabel.Text = _selectedItem.IsActive ? "Sản phẩm đang hoạt động." : "Sản phẩm đang tạm ngừng.";
        UpdateSelectionState();
    }

    protected override void BeginAdd()
    {
        SelectedId = 0;
        _selectedItem = null;
        CodeBox.Clear();
        NameBox.Clear();
        DescriptionBox.Clear();
        _categoryBox.Clear();
        _unitBox.Clear();
        _priceBox.Clear();
        _quantityBox.Clear();
        _minStockBox.Clear();
        ActiveBox.Checked = true;
        ToggleEditing(true);
        _selectedStateLabel.Text = "Đang tạo sản phẩm mới";
        _editModeLabel.Text = "Nhập thông tin rồi bấm Lưu.";
        UiFactory.SetMessage(_gridStateLabel, "Sẵn sàng nhập sản phẩm mới.");
        UpdateSelectionState();
    }

    protected override void BeginEdit()
    {
        if (SelectedId <= 0 || _selectedItem is null)
        {
            UiFactory.SetMessage(_gridStateLabel, "Vui lòng chọn một sản phẩm để sửa.", true);
            return;
        }

        ToggleEditing(true);
        _selectedStateLabel.Text = $"Đang sửa: {_selectedItem.Code}";
        _editModeLabel.Text = "Chỉnh sửa thông tin đã chọn.";
        UiFactory.SetMessage(_gridStateLabel, "Đang chỉnh sửa sản phẩm đã chọn.");
        UpdateSelectionState();
    }

    protected override void SaveCurrent()
    {
        if (!IsEditing)
        {
            UiFactory.SetMessage(_gridStateLabel, "Hãy chọn Thêm mới hoặc Chỉnh sửa trước khi lưu.", true);
            return;
        }

        if (string.IsNullOrWhiteSpace(CodeBox.Text) || string.IsNullOrWhiteSpace(NameBox.Text))
        {
            UiFactory.SetMessage(_gridStateLabel, "Mã và tên sản phẩm là bắt buộc.", true);
            return;
        }

        if (!TryParseDecimal(_priceBox.Text, out var price) || price < 0)
        {
            UiFactory.SetMessage(_gridStateLabel, "Giá bán không hợp lệ.", true);
            return;
        }

        if (!TryParseDecimal(_quantityBox.Text, out var quantity) || quantity < 0)
        {
            UiFactory.SetMessage(_gridStateLabel, "Tồn kho không hợp lệ.", true);
            return;
        }

        if (!TryParseDecimal(_minStockBox.Text, out var minStock) || minStock < 0)
        {
            UiFactory.SetMessage(_gridStateLabel, "Mức tồn tối thiểu không hợp lệ.", true);
            return;
        }

        var dto = new ProductDto
        {
            Id = SelectedId,
            Code = CodeBox.Text.Trim(),
            Name = NameBox.Text.Trim(),
            CategoryName = _categoryBox.Text.Trim(),
            Unit = _unitBox.Text.Trim(),
            SellingPrice = price,
            QuantityOnHand = (int)quantity,
            MinStockLevel = (int)minStock,
            IsActive = ActiveBox.Checked
        };

        if (SelectedId <= 0)
        {
            var result = _service.CreateProduct(dto);
            if (result.Success)
            {
                ToggleEditing(false);
                RefreshData();
                UiFactory.SetMessage(_gridStateLabel, result.Message);
            }
            else
            {
                UiFactory.SetMessage(_gridStateLabel, result.Message, true);
            }
        }
        else
        {
            var result = _service.UpdateProduct(dto);
            if (result.Success)
            {
                ToggleEditing(false);
                RefreshData();
                UiFactory.SetMessage(_gridStateLabel, result.Message);
            }
            else
            {
                UiFactory.SetMessage(_gridStateLabel, result.Message, true);
            }
        }
    }

    protected override void CancelEdit()
    {
        ToggleEditing(false);
        RefreshData();
        UiFactory.SetMessage(_gridStateLabel, "Đã hủy chỉnh sửa.");
    }

    protected override void DeactivateCurrent()
    {
        if (SelectedId <= 0)
        {
            UiFactory.SetMessage(_gridStateLabel, "Vui lòng chọn một sản phẩm để ngừng kích hoạt.", true);
            return;
        }

        var result = _service.DeactivateProduct(SelectedId);
        if (result.Success)
        {
            RefreshData();
            UiFactory.SetMessage(_gridStateLabel, result.Message);
        }
        else
        {
            UiFactory.SetMessage(_gridStateLabel, result.Message, true);
        }
    }

    private void ConfigureScreen()
    {
        BackColor = AppTheme.AppBackground;
        Padding = Padding.Empty;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1280, 760);

        var root = Controls.OfType<TableLayoutPanel>().FirstOrDefault();
        if (root is null)
        {
            return;
        }

        root.BackColor = AppTheme.AppBackground;
        root.Padding = AppTheme.PagePadding;
        root.RowStyles[0] = new RowStyle(SizeType.Absolute, 78);
        root.RowStyles[1] = new RowStyle(SizeType.Absolute, 72);
        root.RowStyles[2] = new RowStyle(SizeType.Percent, 100);
        root.RowStyles[3] = new RowStyle(SizeType.Absolute, 36);

        ConfigureSearchBar();
        ConfigureGridCard();
        ConfigureEditCard();
        ConfigureStatusBar();

        root.Controls.Clear();
        root.Controls.Add(BuildHeader("Danh mục hàng hóa", "Theo dõi sản phẩm, giá bán, đơn vị và trạng thái tồn kho."), 0, 0);
        root.Controls.Add(_searchCard, 0, 1);
        root.Controls.Add(BuildBody(), 0, 2);
        root.Controls.Add(_statusCard, 0, 3);
    }

    private Control BuildBody()
    {
        var splitter = UiFactory.HorizontalSplitter(900, 360);
        splitter.Panel1.Padding = new Padding(0, 0, 12, 0);
        splitter.Panel2.Padding = Padding.Empty;
        splitter.Panel1.Controls.Add(BuildGridPanel());
        splitter.Panel2.Controls.Add(_editCard);
        return splitter;
    }

    private Control BuildGridPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 1
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(BuildToolbarCard(), 0, 0);
        layout.Controls.Add(BuildQuickMetrics(), 0, 1);
        layout.Controls.Add(BuildGridHeader(), 0, 2);
        layout.Controls.Add(_gridCard, 0, 3);
        return layout;
    }

    private Control BuildToolbarCard()
    {
        var card = UiFactory.Card();
        card.Padding = new Padding(14, 10, 14, 10);
        card.Margin = Padding.Empty;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        buttons.Controls.AddRange([
            _addButton,
            _editButton,
            _saveButton,
            _cancelButton,
            _deactivateButton,
            _refreshButton,
            _clearSearchButton,
            _setReadonlyButton
        ]);

        _filterSummaryLabel.Dock = DockStyle.Fill;
        _filterSummaryLabel.Font = AppTheme.BodyFont(9.5F);
        _filterSummaryLabel.ForeColor = AppTheme.TextMuted;
        _filterSummaryLabel.TextAlign = ContentAlignment.MiddleRight;
        _filterSummaryLabel.AutoEllipsis = true;

        layout.Controls.Add(buttons, 0, 0);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildGridHeader()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 10, 0, 8);
        card.Padding = new Padding(14, 10, 14, 10);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(UiFactory.SectionHeader("Danh sách sản phẩm", "Bảng dữ liệu ưu tiên đọc nhanh, lọc nhanh và chọn dòng rõ ràng.", IconChar.BoxOpen), 0, 0);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildBadge()
    {
        var badge = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            Radius = 10,
            BorderColor = AppTheme.Border,
            FillColor = AppTheme.SurfaceSubtle,
            Margin = Padding.Empty,
            Padding = new Padding(12, 8, 12, 8)
        };

        badge.Controls.Add(new Label
        {
            Text = "Bộ lọc đang áp dụng",
            Dock = DockStyle.Top,
            Height = 18,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont(9F),
            TextAlign = ContentAlignment.MiddleRight
        });
        badge.Controls.Add(new Label
        {
            Text = "Tất cả sản phẩm",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Primary,
            Font = AppTheme.SectionFont(10.5F),
            TextAlign = ContentAlignment.MiddleRight
        });
        return badge;
    }

    private void ConfigureSearchBar()
    {
        StyleCard(_searchCard, new Padding(16, 14, 16, 14));

        SearchBox.Width = 420;
        SearchBox.Anchor = AnchorStyles.Left;
        SearchBox.Margin = new Padding(0);
        SearchBox.Font = AppTheme.BodyFont(10F);
        SearchBox.PlaceholderText = "Tìm theo mã, tên, loại hàng, đơn vị...";

        _searchHintLabel.Dock = DockStyle.Fill;
        _searchHintLabel.ForeColor = AppTheme.TextMuted;
        _searchHintLabel.TextAlign = ContentAlignment.MiddleLeft;
        _searchHintLabel.AutoEllipsis = true;
        _searchHintLabel.Text = "Lọc theo mã, tên, loại hàng hoặc đơn vị tính.";

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 4,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 88));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new IconPictureBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            IconChar = IconChar.MagnifyingGlass,
            IconColor = AppTheme.Primary,
            IconFont = IconFont.Auto,
            IconSize = 18,
            Padding = new Padding(0, 9, 10, 9)
        }, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = "Tìm kiếm",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = AppTheme.SectionFont(11F),
            ForeColor = AppTheme.Text
        }, 1, 0);

        layout.Controls.Add(SearchBox, 2, 0);
        layout.Controls.Add(_searchHintLabel, 3, 0);
        _searchCard.Controls.Clear();
        _searchCard.Controls.Add(layout);
    }

    private void ConfigureGridCard()
    {
        StyleCard(_gridCard, new Padding(1));
        _gridCard.Controls.Clear();

        Grid.AutoGenerateColumns = false;
        Grid.Dock = DockStyle.Fill;
        Grid.ReadOnly = true;
        Grid.AllowUserToAddRows = false;
        Grid.AllowUserToDeleteRows = false;
        Grid.RowHeadersVisible = false;
        Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        Grid.MultiSelect = false;
        Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Grid.AllowUserToResizeRows = false;
        Grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        Grid.DataSource = BindingSource;
        Grid.DataBindingComplete += (_, _) => UpdateGridState();
        UiFactory.StyleGrid(Grid);

        Grid.Columns.Clear();
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Code), HeaderText = "Mã SP", FillWeight = 82, MinimumWidth = 86 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Name), HeaderText = "Tên sản phẩm", FillWeight = 170, MinimumWidth = 160 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.CategoryName), HeaderText = "Loại hàng", FillWeight = 118, MinimumWidth = 112 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Unit), HeaderText = "Đơn vị", FillWeight = 72, MinimumWidth = 76 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.SellingPrice), HeaderText = "Giá bán", DefaultCellStyle = { Format = "N0" }, FillWeight = 92, MinimumWidth = 94 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.QuantityOnHand), HeaderText = "Tồn kho", DefaultCellStyle = { Format = "N0" }, FillWeight = 76, MinimumWidth = 86 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.MinStockLevel), HeaderText = "Tồn tối thiểu", DefaultCellStyle = { Format = "N0" }, FillWeight = 96, MinimumWidth = 112 });
        Grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(ProductDto.IsActive), HeaderText = "Hoạt động", FillWeight = 78, MinimumWidth = 96 });

        EmptyPanel.Dock = DockStyle.Fill;
        EmptyPanel.BackColor = AppTheme.Surface;
        EmptyPanel.Padding = new Padding(24);
        EmptyPanel.Visible = false;
        EmptyPanel.Controls.Clear();
        _gridCard.Controls.Add(EmptyPanel);
        _gridCard.Controls.Add(Grid);
    }

    private void ConfigureEditCard()
    {
        StyleCard(_editCard, new Padding(16));
        _editCard.Controls.Clear();

        _editHintLabel.Dock = DockStyle.Fill;
        _editHintLabel.ForeColor = AppTheme.TextMuted;
        _editHintLabel.TextAlign = ContentAlignment.MiddleLeft;
        _editHintLabel.AutoEllipsis = true;
        _editHintLabel.Text = "Nhập liệu gọn, nút rõ ràng, trạng thái lưu hiển thị ngay bên dưới.";

        _editModeLabel.Dock = DockStyle.Fill;
        _editModeLabel.ForeColor = AppTheme.Primary;
        _editModeLabel.TextAlign = ContentAlignment.MiddleRight;
        _editModeLabel.AutoEllipsis = true;
        _editModeLabel.Font = AppTheme.BodyFont(9.5F);

        _selectedStateLabel.Dock = DockStyle.Fill;
        _selectedStateLabel.ForeColor = AppTheme.TextMuted;
        _selectedStateLabel.TextAlign = ContentAlignment.MiddleLeft;
        _selectedStateLabel.AutoEllipsis = true;
        _selectedStateLabel.Font = AppTheme.BodyFont(9.5F);

        _editGrid.Dock = DockStyle.Fill;
        _editGrid.ColumnCount = 2;
        _editGrid.RowCount = 5;
        _editGrid.Padding = Padding.Empty;
        _editGrid.Margin = Padding.Empty;
        _editGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _editGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        _editGrid.RowStyles.Clear();
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

        ConfigureField(CodeBox, 0, 0, "Mã sản phẩm", true);
        ConfigureField(NameBox, 1, 0, "Tên sản phẩm", true);
        ConfigureField(_categoryBox, 0, 1, "Loại hàng", true);
        ConfigureField(_unitBox, 1, 1, "Đơn vị tính", true);
        ConfigureField(_priceBox, 0, 2, "Giá bán", true);
        ConfigureField(_quantityBox, 1, 2, "Tồn kho", true);
        ConfigureField(_minStockBox, 0, 3, "Mức tồn tối thiểu", true);
        ConfigureField(DescriptionBox, 1, 3, "Mô tả / ghi chú", false, multiline: true);

        ActiveBox.Text = "Đang hoạt động";
        ActiveBox.AutoSize = true;
        ActiveBox.Margin = new Padding(0, 10, 0, 0);
        _editGrid.Controls.Add(ActiveBox, 0, 4);
        _editGrid.SetColumnSpan(ActiveBox, 2);

        var wrapper = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5
        };
        wrapper.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        wrapper.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        wrapper.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        wrapper.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        wrapper.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        wrapper.Controls.Add(UiFactory.SectionHeader("Chi tiết sản phẩm", "Thông tin nhập liệu và trạng thái", IconChar.BoxOpen), 0, 0);
        wrapper.Controls.Add(_editHintLabel, 0, 1);
        wrapper.Controls.Add(_editGrid, 0, 2);
        wrapper.Controls.Add(_editModeLabel, 0, 3);
        wrapper.Controls.Add(_selectedStateLabel, 0, 4);
        _editCard.Controls.Add(wrapper);

        DescriptionBox.Multiline = true;
        DescriptionBox.ScrollBars = ScrollBars.None;
    }

    private void ConfigureField(Control control, int column, int row, string labelText, bool fullWidth = false, bool multiline = false)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(column == 0 ? 0 : 8, 0, column == 1 ? 0 : 8, 10)
        };

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = AppTheme.BodyFont(9.5F),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 0, 0);

        var inputControl = control;
        if (control is TextBox box)
        {
            box.Dock = DockStyle.Fill;
            box.BorderStyle = multiline ? BorderStyle.None : BorderStyle.FixedSingle;
            box.BackColor = multiline ? AppTheme.SurfaceSubtle : AppTheme.Surface;
            box.Font = AppTheme.BodyFont(10F);
            box.Margin = Padding.Empty;
            box.PlaceholderText = labelText switch
            {
                "Mã sản phẩm" => "VD: SP001",
                "Tên sản phẩm" => "VD: Nước suối 500ml",
                "Loại hàng" => "VD: Đồ uống",
                "Đơn vị tính" => "VD: Chai, lon, hộp...",
                "Giá bán" => "VD: 15000",
                "Tồn kho" => "VD: 120",
                "Mức tồn tối thiểu" => "VD: 20",
                "Mô tả / ghi chú" => "Mô tả ngắn, ghi chú hoặc thông tin bổ sung",
                _ => "VD: Nhập dữ liệu..."
            };
            box.Multiline = multiline;
            box.ScrollBars = ScrollBars.None;
            box.MinimumSize = new Size(0, multiline ? 72 : 30);
            box.ReadOnly = true;
            if (box == DescriptionBox)
            {
                box.WordWrap = true;
            }

            if (multiline)
            {
                var host = new RoundedPanel
                {
                    Dock = DockStyle.Fill,
                    FillColor = AppTheme.SurfaceSubtle,
                    BorderColor = AppTheme.Border,
                    Radius = 6,
                    ShadowSize = 0,
                    Padding = new Padding(10, 7, 10, 7),
                    Margin = Padding.Empty
                };
                host.Controls.Add(box);
                inputControl = host;
            }
        }

        layout.Controls.Add(inputControl, 0, 1);
        panel.Controls.Add(layout);
        _editGrid.Controls.Add(panel, column, row);
    }

    private Control BuildQuickMetrics()
    {
        var metrics = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = new Padding(0)
        };
        metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        metrics.Controls.Add(UiFactory.MetricCard("Tổng sản phẩm", _metricTotalValue, IconChar.BoxOpen, AppTheme.Primary, AppTheme.PrimarySoft), 0, 0);
        metrics.Controls.Add(UiFactory.MetricCard("Đang hoạt động", _metricActiveValue, IconChar.CircleCheck, AppTheme.Success, AppTheme.SuccessSoft), 1, 0);
        var lowStockCard = UiFactory.MetricCard("Sắp hết hàng", _metricLowStockValue, IconChar.TriangleExclamation, AppTheme.Warning, AppTheme.WarningSoft);
        lowStockCard.Margin = Padding.Empty;
        metrics.Controls.Add(lowStockCard, 2, 0);
        return metrics;
    }

    private void ConfigureStatusBar()
    {
        _statusCard.Dock = DockStyle.Fill;
        _statusCard.BackColor = Color.Transparent;
        _statusCard.Controls.Clear();

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320));

        _gridStateLabel.Dock = DockStyle.Fill;
        _gridStateLabel.ForeColor = AppTheme.StatusText;
        _gridStateLabel.TextAlign = ContentAlignment.MiddleLeft;

        layout.Controls.Add(_gridStateLabel, 0, 0);
        layout.Controls.Add(new Label
        {
            Text = "Sẵn sàng hiển thị trạng thái trống.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleRight,
            AutoEllipsis = true
        }, 1, 0);
        _statusCard.Controls.Add(layout);
    }

    private void UpdateSummary()
    {
        var active = _items.Count(p => p.IsActive);
        var lowStock = _items.Count(p => p.QuantityOnHand <= p.MinStockLevel);
        _summaryLabel.Text = $"{_items.Count} sản phẩm | {active} đang hoạt động | {lowStock} sắp hết";
    }

    private void UpdateMetrics()
    {
        var active = _filteredItems.Count(p => p.IsActive);
        var lowStock = _filteredItems.Count(p => p.QuantityOnHand <= p.MinStockLevel);
        _metricTotalValue.Text = _filteredItems.Count.ToString("N0");
        _metricActiveValue.Text = active.ToString("N0");
        _metricLowStockValue.Text = lowStock.ToString("N0");
    }

    private void UpdateFilterSummary()
    {
        _filterSummaryLabel.Text = string.IsNullOrWhiteSpace(SearchBox.Text)
            ? "Đang xem toàn bộ danh sách"
            : $"Đang lọc theo: {SearchBox.Text.Trim()}";
        _clearSearchButton.Enabled = !IsEditing || SearchBox.TextLength > 0;
    }

    private void UpdateGridState()
    {
        var total = _filteredItems.Count;
        var countMessage = total == 0 ? "Không có dữ liệu phù hợp" : $"Hiển thị {total} dòng";
        UiFactory.SetMessage(
            _gridStateLabel,
            _dataStateIsError ? $"{_dataStateMessage} | {countMessage}" : countMessage,
            _dataStateIsError);
        UpdateFilterSummary();

        if (total != 0)
        {
            return;
        }

        EmptyPanel.Controls.Clear();
        _emptyTitleLabel.Text = "Không tìm thấy sản phẩm phù hợp";
        _emptyMessageLabel.Text = "Hãy thử đổi từ khóa tìm kiếm, bấm Xóa lọc hoặc Làm mới để xem lại danh sách.";

        var card = UiFactory.Card();
        card.Padding = new Padding(28);
        card.Margin = Padding.Empty;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            RowStyles =
            {
                new RowStyle(SizeType.Absolute, 72),
                new RowStyle(SizeType.Absolute, 30),
                new RowStyle(SizeType.Absolute, 40),
                new RowStyle(SizeType.Absolute, 42),
                new RowStyle(SizeType.Absolute, 28)
            }
        };

        _emptyTitleLabel.Dock = DockStyle.Fill;
        _emptyTitleLabel.Font = AppTheme.SectionFont(12F);
        _emptyTitleLabel.ForeColor = AppTheme.Text;
        _emptyTitleLabel.TextAlign = ContentAlignment.MiddleCenter;

        _emptyMessageLabel.Dock = DockStyle.Fill;
        _emptyMessageLabel.ForeColor = AppTheme.TextMuted;
        _emptyMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
        _emptyMessageLabel.AutoEllipsis = true;

        var iconTile = UiFactory.IconTile(IconChar.BoxOpen, AppTheme.Primary, AppTheme.PrimarySoft, 64, 28);
        iconTile.Anchor = AnchorStyles.None;
        layout.Controls.Add(iconTile, 0, 0);
        layout.Controls.Add(_emptyTitleLabel, 0, 1);
        layout.Controls.Add(_emptyMessageLabel, 0, 2);
        layout.Controls.Add(new Label
        {
            Text = "Mẹo: dùng mã sản phẩm, tên hoặc loại hàng để lọc nhanh.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleCenter,
            AutoEllipsis = true
        }, 0, 3);
        layout.Controls.Add(new Label
        {
            Text = "Kết quả lọc sẽ hiển thị lại ngay khi bạn đổi điều kiện.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont(9F),
            TextAlign = ContentAlignment.MiddleCenter,
            AutoEllipsis = true
        }, 0, 4);
        card.Controls.Add(layout);
        EmptyPanel.Controls.Add(card);
    }

    private void UpdateSelectionState()
    {
        var canEdit = SelectedId > 0 || IsEditing;
        _editButton.Enabled = SelectedId > 0 && !IsEditing;
        _deactivateButton.Enabled = SelectedId > 0 && !IsEditing;
        _saveButton.Enabled = IsEditing;
        _cancelButton.Enabled = IsEditing;
        _addButton.Enabled = !IsEditing;
        _clearSearchButton.Enabled = !IsEditing || SearchBox.TextLength > 0;
        _refreshButton.Enabled = !IsEditing;
        _setReadonlyButton.Enabled = IsEditing;

        CodeBox.ReadOnly = !IsEditing;
        NameBox.ReadOnly = !IsEditing;
        _categoryBox.ReadOnly = !IsEditing;
        _unitBox.ReadOnly = !IsEditing;
        _priceBox.ReadOnly = !IsEditing;
        _quantityBox.ReadOnly = !IsEditing;
        _minStockBox.ReadOnly = !IsEditing;
        DescriptionBox.ReadOnly = !IsEditing;
        ActiveBox.Enabled = IsEditing;
        _selectedStateLabel.ForeColor = canEdit ? AppTheme.TextMuted : AppTheme.Warning;
    }

    private static bool TryParseDecimal(string? value, out decimal number) => decimal.TryParse(value, out number);

    private static List<ProductDto> CreateStubItems() =>
    [
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", CategoryName = "Đồ uống", Unit = "Chai", SellingPrice = 6000, QuantityOnHand = 110, MinStockLevel = 30, CostPrice = 4200, IsActive = true },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", CategoryName = "Đồ uống", Unit = "Lon", SellingPrice = 11000, QuantityOnHand = 68, MinStockLevel = 25, CostPrice = 7800, IsActive = true },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", CategoryName = "Thực phẩm", Unit = "Gói", SellingPrice = 5000, QuantityOnHand = 195, MinStockLevel = 50, CostPrice = 3200, IsActive = true },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", CategoryName = "Gia dụng", Unit = "Chai", SellingPrice = 25000, QuantityOnHand = 32, MinStockLevel = 35, CostPrice = 17000, IsActive = true },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", CategoryName = "Vệ sinh", Unit = "Tuýp", SellingPrice = 18000, QuantityOnHand = 30, MinStockLevel = 35, CostPrice = 12000, IsActive = true },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", CategoryName = "Vệ sinh", Unit = "Gói", SellingPrice = 12500, QuantityOnHand = 118, MinStockLevel = 15, CostPrice = 8200, IsActive = true }
    ];

    private static void StyleCard(RoundedPanel card, Padding padding)
    {
        card.Dock = DockStyle.Fill;
        card.FillColor = AppTheme.Surface;
        card.BorderColor = AppTheme.Border;
        card.Radius = 8;
        card.ShadowSize = 1;
        card.Padding = padding;
        card.Margin = Padding.Empty;
    }

    private void ConfigureActionButtons()
    {
        StyleButton(_addButton, AppTheme.Primary, Color.White, Color.FromArgb(29, 78, 216), Color.FromArgb(30, 64, 175));
        StyleButton(_saveButton, AppTheme.Success, Color.White, Color.FromArgb(4, 120, 87), Color.FromArgb(6, 95, 70));
        StyleButton(_deactivateButton, AppTheme.Warning, Color.White, Color.FromArgb(194, 65, 12), Color.FromArgb(154, 52, 18));

        StyleButton(_editButton, AppTheme.SurfaceSubtle, AppTheme.Primary, AppTheme.PrimarySoft, Color.FromArgb(219, 234, 254), AppTheme.BorderStrong);
        StyleButton(_refreshButton, AppTheme.SurfaceSubtle, AppTheme.Primary, AppTheme.PrimarySoft, Color.FromArgb(219, 234, 254), AppTheme.BorderStrong);
        StyleButton(_clearSearchButton, AppTheme.SurfaceSubtle, AppTheme.TextMuted, Color.FromArgb(241, 245, 249), Color.FromArgb(226, 232, 240), AppTheme.Border);
        StyleButton(_setReadonlyButton, AppTheme.SurfaceSubtle, AppTheme.TextMuted, Color.FromArgb(241, 245, 249), Color.FromArgb(226, 232, 240), AppTheme.Border);
        StyleButton(_cancelButton, AppTheme.DangerSoft, AppTheme.Danger, Color.FromArgb(254, 202, 202), Color.FromArgb(252, 165, 165), AppTheme.DangerSoft);
    }

    private static void StyleButton(IconButton button, Color backColor, Color foreColor, Color hoverColor, Color downColor, Color? borderColor = null)
    {
        button.BackColor = backColor;
        button.ForeColor = foreColor;
        button.IconColor = foreColor;
        button.FlatAppearance.BorderColor = borderColor ?? backColor;
        button.FlatAppearance.BorderSize = borderColor is null ? 0 : 1;
        button.FlatAppearance.MouseOverBackColor = hoverColor;
        button.FlatAppearance.MouseDownBackColor = downColor;
    }
}
