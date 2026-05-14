using System.ComponentModel;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Admin;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Admin;

public sealed class FrmUserManagement : Form
{
    private readonly UserAccountService _service = new();
    private readonly int _currentUserId;
    private readonly BindingSource _source = new();
    private readonly DataGridView _grid = new();
    private readonly TextBox _searchBox = new();
    private readonly TextBox _usernameBox = new();
    private readonly TextBox _fullNameBox = new();
    private readonly TextBox _passwordBox = new();
    private readonly ComboBox _roleBox = new();
    private readonly CheckBox _activeBox = new();
    private readonly Label _summaryLabel = new();
    private readonly Label _gridStateLabel = new();
    private readonly Label _selectedStateLabel = new();
    private readonly Label _emptyTitleLabel = new();
    private readonly Label _emptyMessageLabel = new();
    private readonly Label _searchHintLabel = new();
    private readonly Label _editHintLabel = new();
    private readonly Label _metricTotalValue = new();
    private readonly Label _metricActiveValue = new();
    private readonly Label _metricAdminValue = new();
    private readonly Label _filterSummaryLabel = new();
    private readonly Label _editModeLabel = new();
    private readonly RoundedPanel _searchCard = new();
    private readonly RoundedPanel _gridCard = new();
    private readonly RoundedPanel _editCard = new();
    private readonly Panel _statusCard = new();
    private readonly Panel _emptyPanel = new();
    private readonly TableLayoutPanel _editGrid = new();
    private readonly IconButton _clearSearchButton;
    private readonly IconButton _refreshButton;
    private readonly IconButton _addButton;
    private readonly IconButton _editButton;
    private readonly IconButton _saveButton;
    private readonly IconButton _cancelButton;
    private readonly IconButton _deactivateButton;
    private readonly IconButton _setReadonlyButton;
    private List<UserAccountDto> _items = [];
    private List<UserAccountDto> _filteredItems = [];
    private int _selectedId;
    private bool _isEditing;
    private string _dataStateMessage = "Sẵn sàng";
    private bool _dataStateIsError;

    public FrmUserManagement() : this(currentUserId: 1)
    {
    }

    public FrmUserManagement(int currentUserId)
    {
        _currentUserId = currentUserId;
        Text = "Tài khoản";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1280, 760);

        _clearSearchButton = UiFactory.IconActionButton("Xóa lọc", IconChar.Eraser, (_, _) => { _searchBox.Clear(); ApplyFilter(); }, 96);
        _refreshButton = UiFactory.IconActionButton("Làm mới", IconChar.Rotate, (_, _) => RefreshData(), 100);
        _addButton = UiFactory.IconActionButton("Thêm mới", IconChar.Plus, (_, _) => BeginAdd(), 104);
        _editButton = UiFactory.IconActionButton("Chỉnh sửa", IconChar.PenToSquare, (_, _) => BeginEdit(), 108);
        _saveButton = UiFactory.IconActionButton("Lưu", IconChar.FloppyDisk, (_, _) => SaveCurrent(), 82);
        _cancelButton = UiFactory.IconActionButton("Hủy", IconChar.CircleXmark, (_, _) => CancelEdit(), 82);
        _deactivateButton = UiFactory.IconActionButton("Ngừng kích hoạt", IconChar.ToggleOff, (_, _) => DeactivateCurrent(), 142);
        _setReadonlyButton = UiFactory.IconActionButton("Chỉ xem", IconChar.Eye, (_, _) => { SetEditorEnabled(false); UpdateSelectionState(); }, 88);

        ConfigureActionButtons();
        BuildUi();
        RefreshData();
        SetEditorEnabled(false);
        UpdateSelectionState();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = AppTheme.PagePadding,
            BackColor = AppTheme.AppBackground
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        ConfigureSearchBar();
        ConfigureGridCard();
        ConfigureEditCard();
        ConfigureStatusBar();

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(_searchCard, 0, 1);
        root.Controls.Add(BuildBody(), 0, 2);
        root.Controls.Add(_statusCard, 0, 3);
        Controls.Add(root);
    }

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 390));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        header.Controls.Add(UiFactory.SectionHeader(
            "Quản trị người dùng",
            "Quản lý tài khoản demo theo vai trò và trạng thái hoạt động.",
            IconChar.UserGear), 0, 0);

        _summaryLabel.Dock = DockStyle.Fill;
        _summaryLabel.Font = AppTheme.SectionFont(10.5F);
        _summaryLabel.ForeColor = AppTheme.Primary;
        _summaryLabel.TextAlign = ContentAlignment.MiddleRight;
        _summaryLabel.AutoEllipsis = true;
        _summaryLabel.Margin = new Padding(12, 0, 0, 0);
        header.Controls.Add(_summaryLabel, 1, 0);
        return header;
    }

    private Control BuildBody()
    {
        var splitter = UiFactory.HorizontalSplitter(920, 330);
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
        card.Margin = new Padding(0, 8, 0, 8);
        card.Padding = new Padding(14, 10, 14, 10);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.Controls.Add(UiFactory.SectionHeader("Danh sách tài khoản", "Tài khoản được quản lý qua BLL, phân vai trò rõ ràng.", IconChar.UserGear), 0, 0);
        card.Controls.Add(layout);
        return card;
    }

    private void ConfigureSearchBar()
    {
        StyleCard(_searchCard, new Padding(16, 14, 16, 14));

        _searchBox.Width = 420;
        _searchBox.Anchor = AnchorStyles.Left;
        _searchBox.Margin = Padding.Empty;
        _searchBox.Font = AppTheme.BodyFont(10F);
        _searchBox.PlaceholderText = "Tìm theo tên đăng nhập, họ tên, vai trò...";
        _searchBox.TextChanged += (_, _) => ApplyFilter();

        _searchHintLabel.Dock = DockStyle.Fill;
        _searchHintLabel.ForeColor = AppTheme.TextMuted;
        _searchHintLabel.TextAlign = ContentAlignment.MiddleLeft;
        _searchHintLabel.AutoEllipsis = true;
        _searchHintLabel.Text = "Lọc theo tên đăng nhập, họ tên, vai trò hoặc trạng thái.";

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

        layout.Controls.Add(_searchBox, 2, 0);
        layout.Controls.Add(_searchHintLabel, 3, 0);
        _searchCard.Controls.Clear();
        _searchCard.Controls.Add(layout);
    }

    private void ConfigureGridCard()
    {
        StyleCard(_gridCard, new Padding(1));
        _gridCard.Controls.Clear();

        _grid.DataSource = _source;
        _grid.AutoGenerateColumns = false;
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.AllowUserToResizeRows = false;
        _grid.RowHeadersVisible = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        _grid.DataBindingComplete += (_, _) => UpdateGridState();
        _grid.SelectionChanged += (_, _) => FillEditorFromSelection();
        UiFactory.StyleGrid(_grid);

        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.Username), HeaderText = "Tên đăng nhập", FillWeight = 135, MinimumWidth = 140 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.FullName), HeaderText = "Họ tên", FillWeight = 155, MinimumWidth = 150 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.RoleName), HeaderText = "Vai trò", FillWeight = 120, MinimumWidth = 130 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.ActiveStatus), HeaderText = "Trạng thái", FillWeight = 112, MinimumWidth = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(UserAccountRow.CreatedDate), HeaderText = "Ngày tạo", FillWeight = 90, MinimumWidth = 100 });

        _emptyPanel.Dock = DockStyle.Fill;
        _emptyPanel.BackColor = AppTheme.Surface;
        _emptyPanel.Padding = new Padding(24);
        _emptyPanel.Visible = false;
        _emptyPanel.Controls.Clear();
        _gridCard.Controls.Add(_emptyPanel);
        _gridCard.Controls.Add(_grid);
    }

    private void ConfigureEditCard()
    {
        StyleCard(_editCard, new Padding(16));
        _editCard.Controls.Clear();

        _roleBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _roleBox.DataSource = RoleOption.CreateAll();
        _roleBox.DisplayMember = nameof(RoleOption.Text);
        _roleBox.ValueMember = nameof(RoleOption.Value);

        _passwordBox.UseSystemPasswordChar = true;

        _editHintLabel.Dock = DockStyle.Fill;
        _editHintLabel.ForeColor = AppTheme.TextMuted;
        _editHintLabel.TextAlign = ContentAlignment.MiddleLeft;
        _editHintLabel.AutoEllipsis = true;
        _editHintLabel.Text = "Tạo tài khoản, phân vai trò và trạng thái hoạt động.";

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
        _editGrid.ColumnCount = 1;
        _editGrid.RowCount = 6;
        _editGrid.Padding = Padding.Empty;
        _editGrid.Margin = Padding.Empty;
        _editGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _editGrid.RowStyles.Clear();
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        _editGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));

        ConfigureTextField(_usernameBox, 0, "Tên đăng nhập", "VD: manager");
        ConfigureTextField(_fullNameBox, 1, "Họ tên", "VD: Quản lý demo");
        ConfigureComboField(_roleBox, 2, "Vai trò");
        ConfigureTextField(_passwordBox, 3, "Mật khẩu", "Để trống khi sửa nếu muốn giữ nguyên");

        _activeBox.Text = "Đang hoạt động";
        _activeBox.AutoSize = true;
        _activeBox.Margin = new Padding(0, 10, 0, 0);
        _editGrid.Controls.Add(_activeBox, 0, 4);

        // Xóa dòng hướng dẫn mật khẩu demo

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
        wrapper.Controls.Add(UiFactory.SectionHeader("Thông tin tài khoản", "Thông tin đăng nhập và quyền truy cập", IconChar.UserGear), 0, 0);
        wrapper.Controls.Add(_editHintLabel, 0, 1);
        wrapper.Controls.Add(_editGrid, 0, 2);
        wrapper.Controls.Add(_editModeLabel, 0, 3);
        wrapper.Controls.Add(_selectedStateLabel, 0, 4);
        _editCard.Controls.Add(wrapper);
    }

    private void ConfigureTextField(TextBox box, int row, string labelText, string placeholder)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 0, 0, 10)
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

        box.Dock = DockStyle.Fill;
        box.BorderStyle = BorderStyle.FixedSingle;
        box.BackColor = AppTheme.Surface;
        box.Font = AppTheme.BodyFont(10F);
        box.Margin = Padding.Empty;
        box.PlaceholderText = placeholder;
        box.MinimumSize = new Size(0, 30);
        box.ReadOnly = true;

        layout.Controls.Add(box, 0, 1);
        panel.Controls.Add(layout);
        _editGrid.Controls.Add(panel, 0, row);
    }

    private void ConfigureComboField(ComboBox box, int row, string labelText)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 0, 0, 10)
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

        box.Dock = DockStyle.Fill;
        box.Font = AppTheme.BodyFont(10F);
        box.Margin = Padding.Empty;
        layout.Controls.Add(box, 0, 1);
        panel.Controls.Add(layout);
        _editGrid.Controls.Add(panel, 0, row);
    }

    private Control BuildQuickMetrics()
    {
        var metrics = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty
        };
        metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        metrics.Controls.Add(UiFactory.MetricCard("Tổng tài khoản", _metricTotalValue, IconChar.UserGear, AppTheme.Primary, AppTheme.PrimarySoft), 0, 0);
        metrics.Controls.Add(UiFactory.MetricCard("Đang hoạt động", _metricActiveValue, IconChar.CircleCheck, AppTheme.Success, AppTheme.SuccessSoft), 1, 0);
        var adminCard = UiFactory.MetricCard("Quản trị viên", _metricAdminValue, IconChar.ShieldHalved, AppTheme.Warning, AppTheme.WarningSoft);
        adminCard.Margin = Padding.Empty;
        metrics.Controls.Add(adminCard, 2, 0);
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280));

        _gridStateLabel.Dock = DockStyle.Fill;
        _gridStateLabel.ForeColor = AppTheme.StatusText;
        _gridStateLabel.TextAlign = ContentAlignment.MiddleLeft;

        layout.Controls.Add(_gridStateLabel, 0, 0);
        layout.Controls.Add(new Label
        {
            Text = "Sẵn sàng quản lý tài khoản demo.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleRight,
            AutoEllipsis = true
        }, 1, 0);
        _statusCard.Controls.Add(layout);
    }

    private void RefreshData()
    {
        var result = _service.GetAllAccounts();
        _items = result.Success ? result.Data ?? [] : [];
        _dataStateMessage = result.Message;
        _dataStateIsError = !result.Success;
        ApplyFilter();
        UpdateSummary();
        UpdateMetrics();
        UpdateGridState();
        UpdateSelectionState();
    }

    private void ApplyFilter()
    {
        var keyword = _searchBox.Text.Trim();
        _filteredItems = string.IsNullOrWhiteSpace(keyword)
            ? _items.ToList()
            : _items.Where(account =>
                    account.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    account.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    account.RoleName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    (account.IsActive ? "đang hoạt động" : "ngừng kích hoạt").Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

        _source.DataSource = new BindingList<UserAccountRow>(_filteredItems.Select(UserAccountRow.FromDto).ToList());
        ShowEmpty(_filteredItems.Count == 0);
        UpdateSummary();
        UpdateMetrics();
        UpdateGridState();
        UpdateFilterSummary();
    }

    private void FillEditorFromSelection()
    {
        if (_source.Current is not UserAccountRow row || _isEditing)
        {
            return;
        }

        _selectedId = row.Id;
        _usernameBox.Text = row.Username;
        _fullNameBox.Text = row.FullName;
        _roleBox.SelectedValue = row.Role;
        _activeBox.Checked = row.IsActive;
        _passwordBox.Clear();
        _selectedStateLabel.Text = $"Đã chọn: {row.Username} - {row.FullName}";
        _editModeLabel.Text = row.IsActive ? "Tài khoản đang hoạt động." : "Tài khoản đang tạm ngừng.";
        UpdateSelectionState();
    }

    private void BeginAdd()
    {
        _selectedId = 0;
        _usernameBox.Clear();
        _fullNameBox.Clear();
        _passwordBox.Clear();
        _roleBox.SelectedValue = UserRole.Manager;
        _activeBox.Checked = true;
        SetEditorEnabled(true);
        _selectedStateLabel.Text = "Đang tạo tài khoản mới";
        _editModeLabel.Text = "Nhập thông tin rồi bấm Lưu.";
        SetMessage("Đang tạo tài khoản mới.");
        UpdateSelectionState();
    }

    private void BeginEdit()
    {
        if (_selectedId <= 0)
        {
            SetMessage("Vui lòng chọn tài khoản cần sửa.", true);
            return;
        }

        SetEditorEnabled(true);
        _selectedStateLabel.Text = $"Đang sửa tài khoản #{_selectedId}";
        _editModeLabel.Text = "Chỉnh sửa thông tin đã chọn.";
        SetMessage("Đang chỉnh sửa tài khoản đã chọn.");
        UpdateSelectionState();
    }

    private void SaveCurrent()
    {
        if (!_isEditing)
        {
            SetMessage("Hãy chọn Thêm mới hoặc Chỉnh sửa trước khi lưu.", true);
            return;
        }

        var account = new UserAccountDto
        {
            Id = _selectedId,
            Username = _usernameBox.Text,
            FullName = _fullNameBox.Text,
            Role = _roleBox.SelectedValue is UserRole role ? role : UserRole.Manager,
            IsActive = _activeBox.Checked,
            DemoPassword = _passwordBox.Text
        };

        var result = _selectedId == 0
            ? ToSaveResult(_service.CreateAccount(account, _currentUserId))
            : ToSaveResult(_service.UpdateAccount(account, _currentUserId));

        if (!result.Success)
        {
            SetMessage(result.Message, true);
            return;
        }

        SetEditorEnabled(false);
        RefreshData();
        SetMessage(result.Message);
    }

    private void CancelEdit()
    {
        SetEditorEnabled(false);
        FillEditorFromSelection();
        SetMessage("Đã hủy chỉnh sửa.");
        UpdateSelectionState();
    }

    private void DeactivateCurrent()
    {
        if (_selectedId <= 0)
        {
            SetMessage("Vui lòng chọn tài khoản cần ngừng kích hoạt.", true);
            return;
        }

        var result = _service.DeactivateAccount(_selectedId, _currentUserId);
        RefreshData();
        SetMessage(result.Message, !result.Success);
    }

    private void SetEditorEnabled(bool enabled)
    {
        _isEditing = enabled;
        _usernameBox.ReadOnly = !enabled;
        _fullNameBox.ReadOnly = !enabled;
        _passwordBox.ReadOnly = !enabled;
        _roleBox.Enabled = enabled;
        _activeBox.Enabled = enabled;
    }

    private void UpdateSummary()
    {
        var active = _items.Count(account => account.IsActive);
        var adminCount = _items.Count(account => account.Role == UserRole.Admin);
        _summaryLabel.Text = $"{_items.Count} tài khoản | {active} đang hoạt động | {adminCount} quản trị";
    }

    private void UpdateMetrics()
    {
        var active = _filteredItems.Count(account => account.IsActive);
        var adminCount = _filteredItems.Count(account => account.Role == UserRole.Admin);
        _metricTotalValue.Text = _filteredItems.Count.ToString("N0");
        _metricActiveValue.Text = active.ToString("N0");
        _metricAdminValue.Text = adminCount.ToString("N0");
    }

    private void UpdateFilterSummary()
    {
        var hasKeyword = !string.IsNullOrWhiteSpace(_searchBox.Text);
        var keyword = _searchBox.Text.Trim();
        _filterSummaryLabel.Text = !hasKeyword
            ? "Đang xem toàn bộ danh sách"
            : $"Đang lọc theo: {keyword}";
        _clearSearchButton.Enabled = !_isEditing || _searchBox.TextLength > 0;
    }

    private void UpdateGridState()
    {
        var total = _filteredItems.Count;
        var countMessage = total == 0 ? "Không có dữ liệu phù hợp" : $"Hiển thị {total} dòng";
        SetMessage(_dataStateIsError ? $"{_dataStateMessage} | {countMessage}" : countMessage, _dataStateIsError);
        UpdateFilterSummary();

        if (total != 0)
        {
            return;
        }

        _emptyPanel.Controls.Clear();
        _emptyTitleLabel.Text = "Không tìm thấy tài khoản phù hợp";
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

        var iconTile = UiFactory.IconTile(IconChar.UserGear, AppTheme.Primary, AppTheme.PrimarySoft, 64, 28);
        iconTile.Anchor = AnchorStyles.None;
        layout.Controls.Add(iconTile, 0, 0);
        layout.Controls.Add(_emptyTitleLabel, 0, 1);
        layout.Controls.Add(_emptyMessageLabel, 0, 2);
        layout.Controls.Add(new Label
        {
            Text = "Mẹo: dùng tên đăng nhập, họ tên hoặc vai trò để lọc nhanh.",
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
        _emptyPanel.Controls.Add(card);
    }

    private void ShowEmpty(bool show)
    {
        _emptyPanel.Visible = show;
        _grid.Visible = !show;
    }

    private void UpdateSelectionState()
    {
        var canEdit = _selectedId > 0 || _isEditing;
        _editButton.Enabled = _selectedId > 0 && !_isEditing;
        _deactivateButton.Enabled = _selectedId > 0 && !_isEditing;
        _saveButton.Enabled = _isEditing;
        _cancelButton.Enabled = _isEditing;
        _addButton.Enabled = !_isEditing;
        _clearSearchButton.Enabled = !_isEditing || _searchBox.TextLength > 0;
        _refreshButton.Enabled = !_isEditing;
        _setReadonlyButton.Enabled = _isEditing;
        _selectedStateLabel.ForeColor = canEdit ? AppTheme.TextMuted : AppTheme.Warning;
    }

    private void SetMessage(string message, bool isError = false)
    {
        UiFactory.SetMessage(_gridStateLabel, message, isError);
    }

    private static SaveResult ToSaveResult(ServiceResult<int> result) => new(result.Success, result.Message);

    private static SaveResult ToSaveResult(ServiceResult<bool> result) => new(result.Success, result.Message);

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

    private readonly record struct SaveResult(bool Success, string Message);

    private sealed class RoleOption
    {
        public required UserRole Value { get; init; }
        public required string Text { get; init; }

        public static List<RoleOption> CreateAll() =>
        [
            new() { Value = UserRole.Admin, Text = PermissionService.GetRoleDisplayName(UserRole.Admin) },
            new() { Value = UserRole.Manager, Text = PermissionService.GetRoleDisplayName(UserRole.Manager) },
            new() { Value = UserRole.WarehouseStaff, Text = PermissionService.GetRoleDisplayName(UserRole.WarehouseStaff) },
            new() { Value = UserRole.SalesStaff, Text = PermissionService.GetRoleDisplayName(UserRole.SalesStaff) }
        ];
    }

    private sealed class UserAccountRow
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string ActiveStatus { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;

        public static UserAccountRow FromDto(UserAccountDto dto)
        {
            return new UserAccountRow
            {
                Id = dto.Id,
                Username = dto.Username,
                FullName = dto.FullName,
                Role = dto.Role,
                RoleName = dto.RoleName,
                IsActive = dto.IsActive,
                ActiveStatus = dto.IsActive ? "Đang hoạt động" : "Ngừng kích hoạt",
                CreatedDate = dto.CreatedAt.ToString("dd/MM/yyyy")
            };
        }
    }
}
