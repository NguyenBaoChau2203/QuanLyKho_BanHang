using System.ComponentModel;
using FontAwesome.Sharp;
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
    private readonly DataGridView _grid = new();
    private readonly TextBox _searchBox = new();
    private readonly TextBox _note = new();
    private readonly ComboBox _differenceFilter = new();
    private readonly ComboBox _sessionStatus = new();
    private readonly DateTimePicker _stocktakeDate = new();
    private readonly Label _message = new();
    private readonly Label _checkedLabel = new();
    private readonly Label _shortageLabel = new();
    private readonly Label _excessLabel = new();
    private readonly Label _unchangedLabel = new();
    private List<StocktakeLineRow> _lines = [];
    private readonly int _currentUserId;

    public FrmStocktake(int currentUserId)
    {
        _currentUserId = currentUserId;
        Text = "Kiểm kê";
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
            RowCount = 6,
            Padding = AppTheme.PagePadding
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildOverview(), 0, 1);
        root.Controls.Add(BuildFilterBar(), 0, 2);
        root.Controls.Add(BuildGridCard(), 0, 3);
        root.Controls.Add(BuildActionBar(), 0, 4);
        root.Controls.Add(_message, 0, 5);
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
        layout.Controls.Add(UiFactory.IconTile(IconChar.ClipboardCheck, AppTheme.Primary, AppTheme.PrimarySoft, 46, 24), 0, 0);

        var text = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        text.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        text.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        text.Controls.Add(new Label
        {
            Text = "Phiên kiểm kê kho",
            Dock = DockStyle.Fill,
            Font = AppTheme.TitleFont(17F),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        text.Controls.Add(new Label
        {
            Text = "So sánh số hệ thống và thực tế, đánh dấu thiếu/thừa để xử lý điều chỉnh.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont(9.5F),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);
        layout.Controls.Add(text, 1, 0);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildOverview()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
        layout.Controls.Add(BuildSessionCard(), 0, 0);
        layout.Controls.Add(BuildMetricCard("Đã kiểm", _checkedLabel, IconChar.ListCheck, AppTheme.Primary, AppTheme.PrimarySoft), 1, 0);
        layout.Controls.Add(BuildMetricCard("Thiếu", _shortageLabel, IconChar.ArrowTrendDown, AppTheme.Danger, AppTheme.DangerSoft), 2, 0);
        layout.Controls.Add(BuildMetricCard("Thừa", _excessLabel, IconChar.ArrowTrendUp, AppTheme.Warning, AppTheme.WarningSoft), 3, 0);
        layout.Controls.Add(BuildMetricCard("Khớp", _unchangedLabel, IconChar.CircleCheck, AppTheme.Success, AppTheme.SuccessSoft, true), 4, 0);
        return layout;
    }

    private static Control BuildMetricCard(string title, Label valueLabel, IconChar icon, Color accent, Color iconFill, bool last = false)
    {
        var card = UiFactory.MetricCard(title, valueLabel, icon, accent, iconFill);
        card.Margin = new Padding(0, 0, last ? 0 : 14, 12);
        return card;
    }

    private Control BuildSessionCard()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 12, 12);
        card.Padding = new Padding(14, 12, 14, 12);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _stocktakeDate.Format = DateTimePickerFormat.Custom;
        _stocktakeDate.CustomFormat = "dd/MM/yyyy";
        _sessionStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        _sessionStatus.Items.AddRange(new object[] { "Nháp", "Đang kiểm", "Chờ xác nhận" });
        _sessionStatus.SelectedIndex = 0;
        _note.PlaceholderText = "Ghi chú phiên kiểm kê...";
        layout.Controls.Add(BuildField("Ngày kiểm kê", _stocktakeDate), 0, 0);
        layout.Controls.Add(BuildField("Trạng thái", _sessionStatus), 1, 0);
        layout.Controls.Add(BuildField("Ghi chú", _note, true), 0, 1);
        layout.SetColumnSpan(layout.GetControlFromPosition(0, 1)!, 2);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildFilterBar()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(14, 12, 14, 12);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 5, RowCount = 1 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 4));

        _searchBox.Dock = DockStyle.Fill;
        _searchBox.PlaceholderText = "Tìm theo mã hoặc tên sản phẩm...";
        _searchBox.TextChanged += (_, _) => ApplyFilters();
        layout.Controls.Add(_searchBox, 0, 0);

        _differenceFilter.Dock = DockStyle.Fill;
        _differenceFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _differenceFilter.Items.AddRange(new object[] { "Tất cả chênh lệch", "Thiếu", "Thừa", "Khớp" });
        _differenceFilter.SelectedIndex = 0;
        _differenceFilter.SelectedIndexChanged += (_, _) => ApplyFilters();
        layout.Controls.Add(_differenceFilter, 1, 0);

        layout.Controls.Add(CreateButton("Làm mới", IconChar.RotateRight, (_, _) => LoadData(), AppTheme.Primary), 2, 0);
        layout.Controls.Add(CreateButton("Xuất Excel", IconChar.FileExport, (_, _) => SetMessage("Xuất Excel kiểm kê đang ở chế độ demo."), AppTheme.Success), 3, 0);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildGridCard()
    {
        var card = UiFactory.Card();
        card.Margin = new Padding(0, 0, 0, 12);
        card.Padding = new Padding(14, 12, 14, 12);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(UiFactory.SectionHeader("Bảng đối chiếu kiểm kê", "Cột thực tế có thể sửa trực tiếp để tính lại chênh lệch.", IconChar.TableList), 0, 0);
        ConfigureGrid();
        layout.Controls.Add(_grid, 0, 1);
        card.Controls.Add(layout);
        return card;
    }

    private Control BuildActionBar()
    {
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.RightToLeft
        };
        panel.Controls.Add(CreateButton("Xác nhận kiểm kê", IconChar.CircleCheck, (_, _) => Save(), AppTheme.Warning, 164));
        panel.Controls.Add(CreateOutlineButton("Lưu nháp", IconChar.FloppyDisk, (_, _) => SetMessage("Đã lưu nháp phiên kiểm kê trong chế độ demo."), 112));
        panel.Controls.Add(CreateOutlineButton("Đặt lại số thực tế", IconChar.RotateLeft, (_, _) => ResetActualQuantity(), 150));
        return panel;
    }

    private void ConfigureGrid()
    {
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = false;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.DataSource = _source;
        UiFactory.StyleGrid(_grid);
        _grid.AutoGenerateColumns = false;
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineRow.ProductCode), HeaderText = "Mã SP", ReadOnly = true, FillWeight = 80 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineRow.ProductName), HeaderText = "Tên sản phẩm", ReadOnly = true, FillWeight = 190 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineRow.CategoryName), HeaderText = "Loại", ReadOnly = true, FillWeight = 100 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineRow.SystemQuantity), HeaderText = "Hệ thống", ReadOnly = true, FillWeight = 76 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineRow.ActualQuantity), HeaderText = "Thực tế", ReadOnly = false, FillWeight = 76 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineRow.Difference), HeaderText = "Chênh lệch", ReadOnly = true, FillWeight = 84 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineRow.Status), HeaderText = "Trạng thái", ReadOnly = true, FillWeight = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(StocktakeLineRow.Note), HeaderText = "Ghi chú", ReadOnly = false, FillWeight = 140 });
        _grid.CellEndEdit += (_, _) =>
        {
            _grid.Refresh();
            UpdateSummary();
        };
        _grid.DataError += (_, e) =>
        {
            e.ThrowException = false;
            SetMessage("Số lượng thực tế phải là số nguyên hợp lệ.", true);
        };
        _grid.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not StocktakeLineRow row)
            {
                return;
            }

            _grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = row.Difference < 0
                ? AppTheme.Danger
                : row.Difference > 0 ? AppTheme.Warning : AppTheme.Text;
        };
    }

    private void LoadData()
    {
        var result = _productService.GetAllProducts();
        var products = result.Success && result.Data is { Count: > 0 } ? result.Data! : CreateStubProducts();
        _lines = products.Select(p => new StocktakeLineRow
        {
            ProductId = p.Id,
            ProductCode = p.Code,
            ProductName = p.Name,
            CategoryName = string.IsNullOrWhiteSpace(p.CategoryName) ? "Chưa phân loại" : p.CategoryName,
            SystemQuantity = p.QuantityOnHand,
            ActualQuantity = Math.Max(0, p.QuantityOnHand - (p.Id % 3)),
            Note = p.Id % 3 == 0 ? "Đã khớp" : string.Empty
        }).ToList();
        _stocktakeDate.Value = DateTime.Today;
        _ = _stocktakeService.GetStocktakeById(1);
        ApplyFilters();
        SetMessage(result.Success ? "Đã tải dữ liệu kiểm kê." : $"{result.Message} - Đang dùng dữ liệu demo.", !result.Success);
    }

    private void ApplyFilters()
    {
        var keyword = _searchBox.Text.Trim();
        var status = _differenceFilter.SelectedItem?.ToString() ?? "Tất cả chênh lệch";
        var rows = _lines
            .Where(x => string.IsNullOrWhiteSpace(keyword)
                || x.ProductCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || x.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .Where(x => status == "Tất cả chênh lệch" || x.Status == status)
            .ToList();
        _source.DataSource = new BindingList<StocktakeLineRow>(rows);
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        _checkedLabel.Text = _lines.Count.ToString("N0");
        _shortageLabel.Text = _lines.Count(x => x.Difference < 0).ToString("N0");
        _excessLabel.Text = _lines.Count(x => x.Difference > 0).ToString("N0");
        _unchangedLabel.Text = _lines.Count(x => x.Difference == 0).ToString("N0");
    }

    private void ResetActualQuantity()
    {
        foreach (var line in _lines)
        {
            line.ActualQuantity = line.SystemQuantity;
        }

        ApplyFilters();
        SetMessage("Đã đặt lại số thực tế bằng số hệ thống.");
    }

    private void Save()
    {
        var stocktake = new StocktakeDto
        {
            StocktakeCode = $"KK-{DateTime.Now:yyyyMMdd-HHmm}",
            StocktakeDate = _stocktakeDate.Value.Date,
            Note = _note.Text.Trim(),
            CreatedByUserId = _currentUserId,
            Lines = _lines.Select(x => new StocktakeLineDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                SystemQuantity = x.SystemQuantity,
                ActualQuantity = x.ActualQuantity
            }).ToList()
        };

        var result = _stocktakeService.CreateStocktake(stocktake);
        if (result.Success)
        {
            SetMessage(result.Message);
            LoadData();
        }
        else
        {
            SetMessage(result.Message, true);
        }
    }

    private void SetMessage(string message, bool error = false)
    {
        UiFactory.SetMessage(_message, message, error);
    }

    private static Control BuildField(string label, Control control, bool multiline = false)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(0, 0, 10, multiline ? 2 : 6) };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, ForeColor = AppTheme.TextMuted }, 0, 0);
        control.Dock = DockStyle.Top;
        control.Margin = Padding.Empty;
        if (!multiline)
        {
            control.Height = 28;
        }
        else if (control is TextBox textBox)
        {
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.None;
            textBox.Height = 50;
            textBox.MinimumSize = new Size(0, 50);
        }

        panel.Controls.Add(control, 0, 1);
        return panel;
    }

    private static IconButton CreateButton(string text, IconChar icon, EventHandler handler, Color color, int width = 0)
    {
        var button = new IconButton
        {
            Text = text,
            Dock = width == 0 ? DockStyle.Fill : DockStyle.None,
            Width = width == 0 ? 110 : width,
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

    private static IconButton CreateOutlineButton(string text, IconChar icon, EventHandler handler, int width)
    {
        var button = new IconButton
        {
            Text = text,
            Width = width,
            Height = 36,
            Margin = new Padding(8, 0, 0, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.Surface,
            ForeColor = AppTheme.Primary,
            IconChar = icon,
            IconColor = AppTheme.Primary,
            IconFont = IconFont.Auto,
            IconSize = 15,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            TextAlign = ContentAlignment.MiddleCenter,
            ImageAlign = ContentAlignment.MiddleCenter,
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderColor = AppTheme.BorderStrong;
        button.FlatAppearance.MouseOverBackColor = AppTheme.PrimarySoft;
        button.Click += handler;
        return button;
    }

    private static List<ProductDto> CreateStubProducts() =>
    [
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", CategoryName = "Đồ uống", QuantityOnHand = 110 },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", CategoryName = "Đồ uống", QuantityOnHand = 68 },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", CategoryName = "Thực phẩm", QuantityOnHand = 195 },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", CategoryName = "Gia dụng", QuantityOnHand = 32 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", CategoryName = "Vệ sinh", QuantityOnHand = 30 },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", CategoryName = "Vệ sinh", QuantityOnHand = 118 }
    ];

    private sealed class StocktakeLineRow
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int SystemQuantity { get; set; }
        public int ActualQuantity { get; set; }
        public int Difference => ActualQuantity - SystemQuantity;
        public string Status => Difference < 0 ? "Thiếu" : Difference > 0 ? "Thừa" : "Khớp";
        public string Note { get; set; } = string.Empty;
    }
}
