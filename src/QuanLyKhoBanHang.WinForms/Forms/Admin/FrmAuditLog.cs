using System.ComponentModel;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Admin;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Admin;

public sealed class FrmAuditLog : Form
{
    private readonly AuditLogService _service = new();
    private readonly BindingSource _source = new();
    private readonly DateTimePicker _fromDate = new();
    private readonly DateTimePicker _toDate = new();
    private readonly TextBox _keywordBox = new();
    private readonly Label _messageLabel = new();
    private readonly Label _summaryLabel = new();
    private readonly Label _emptyTitleLabel = new();
    private readonly Label _emptyMessageLabel = new();
    private readonly DataGridView _grid = new();
    private readonly RoundedPanel _headerCard = new();
    private readonly RoundedPanel _filterCard = new();
    private readonly RoundedPanel _gridCard = new();
    private readonly Panel _statusCard = new();
    private readonly Panel _emptyPanel = new();
    private readonly IconButton _filterButton;
    private readonly IconButton _clearButton;

    public FrmAuditLog()
    {
        Text = "Nhật ký hệ thống";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1100, 640);

        _filterButton = UiFactory.IconActionButton("Lọc", IconChar.MagnifyingGlass, (_, _) => ReloadLogs(), 118);
        _clearButton = UiFactory.IconActionButton("Xóa lọc", IconChar.Eraser, (_, _) => ClearFilter(), 128);

        ConfigureActionButtons();
        BuildUi();
        ReloadLogs();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = AppTheme.PagePadding
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        ConfigureHeaderCard();
        ConfigureFilterCard();
        ConfigureGridCard();
        ConfigureStatusBar();

        root.Controls.Add(_headerCard, 0, 0);
        root.Controls.Add(_filterCard, 0, 1);
        root.Controls.Add(BuildToolbar(), 0, 2);
        root.Controls.Add(_gridCard, 0, 3);
        root.Controls.Add(_statusCard, 0, 4);
        Controls.Add(root);
    }

    private void ConfigureHeaderCard()
    {
        _headerCard.Dock = DockStyle.Fill;
        _headerCard.FillColor = AppTheme.Surface;
        _headerCard.BorderColor = AppTheme.Border;
        _headerCard.Radius = 8;
        _headerCard.ShadowSize = 1;
        _headerCard.Padding = new Padding(18, 12, 18, 12);
        _headerCard.Margin = Padding.Empty;

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _summaryLabel.Dock = DockStyle.None;
        _summaryLabel.Size = new Size(180, 30);
        _summaryLabel.Font = AppTheme.SectionFont(10.5F);
        _summaryLabel.ForeColor = AppTheme.Primary;
        _summaryLabel.TextAlign = ContentAlignment.MiddleRight;
        _summaryLabel.AutoEllipsis = true;
        _summaryLabel.Margin = Padding.Empty;

        headerLayout.Controls.Add(UiFactory.SectionHeader(
            "Dòng sự kiện hệ thống",
            "Lọc nhật ký thao tác theo thời gian, người dùng hoặc hành động.",
            IconChar.ClockRotateLeft), 0, 0);
        headerLayout.Controls.Add(BuildSummaryHost(), 1, 0);
        _headerCard.Controls.Add(headerLayout);
    }

    private Control BuildSummaryHost()
    {
        var host = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        host.Controls.Add(_summaryLabel);
        host.Layout += (_, _) =>
        {
            var x = Math.Max(0, host.ClientSize.Width - _summaryLabel.Width);
            var y = Math.Max(0, (host.ClientSize.Height - _summaryLabel.Height) / 2);
            _summaryLabel.Location = new Point(x, y);
        };

        return host;
    }

    private void ConfigureFilterCard()
    {
        _filterCard.Dock = DockStyle.Fill;
        _filterCard.FillColor = AppTheme.Surface;
        _filterCard.BorderColor = AppTheme.Border;
        _filterCard.Radius = 8;
        _filterCard.ShadowSize = 1;
        _filterCard.Padding = new Padding(18, 14, 18, 14);
        _filterCard.Margin = Padding.Empty;

        var filterLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 11,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        filterLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 36));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 144));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 144));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 124));
        filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136));

        _fromDate.Format = DateTimePickerFormat.Custom;
        _fromDate.CustomFormat = "dd/MM/yyyy";
        _fromDate.Width = 132;
        _fromDate.Value = DateTime.Today.AddDays(-7);

        _toDate.Format = DateTimePickerFormat.Custom;
        _toDate.CustomFormat = "dd/MM/yyyy";
        _toDate.Width = 132;
        _toDate.Value = DateTime.Today;

        _keywordBox.AutoSize = false;
        _keywordBox.Width = 246;
        _keywordBox.Height = 28;
        _keywordBox.BorderStyle = BorderStyle.FixedSingle;
        _keywordBox.Font = AppTheme.BodyFont(10F);
        _keywordBox.PlaceholderText = "Từ khóa, người dùng, hành động...";
        _keywordBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ReloadLogs();
            }
        };

        filterLayout.Controls.Add(MakeToolbarIcon(IconChar.CalendarDay), 0, 0);
        filterLayout.Controls.Add(MakeFieldLabel("Từ"), 1, 0);
        filterLayout.Controls.Add(MakeDateBox(_fromDate), 2, 0);
        filterLayout.Controls.Add(MakeFieldLabel("Đến"), 3, 0);
        filterLayout.Controls.Add(MakeDateBox(_toDate), 4, 0);
        filterLayout.Controls.Add(MakeToolbarIcon(IconChar.MagnifyingGlass), 5, 0);
        filterLayout.Controls.Add(MakeFieldLabel("Từ khóa"), 6, 0);
        filterLayout.Controls.Add(MakeTextBox(_keywordBox), 7, 0);
        filterLayout.Controls.Add(MakeButtonHost(_filterButton), 9, 0);
        filterLayout.Controls.Add(MakeButtonHost(_clearButton), 10, 0);

        _filterCard.Controls.Add(filterLayout);
    }

    private Control BuildToolbar()
    {
        var card = UiFactory.Card();
        card.Padding = new Padding(14, 8, 14, 8);
        card.Margin = new Padding(0, 8, 0, 0);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1
        };

        layout.Controls.Add(new Label
        {
            Text = "Lọc nhật ký theo khoảng thời gian và từ khóa. Bấm Lọc hoặc nhấn Enter để cập nhật danh sách.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont(9.5F),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 0, 0);

        card.Controls.Add(layout);
        return card;
    }

    private void ConfigureGridCard()
    {
        _gridCard.Dock = DockStyle.Fill;
        _gridCard.FillColor = AppTheme.Surface;
        _gridCard.BorderColor = AppTheme.Border;
        _gridCard.Radius = 8;
        _gridCard.ShadowSize = 1;
        _gridCard.Padding = new Padding(1);
        _gridCard.Margin = new Padding(0, 8, 0, 0);

        _grid.DataSource = _source;
        _grid.AutoGenerateColumns = false;
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.RowHeadersVisible = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        UiFactory.StyleGrid(_grid);
        BuildColumns();

        _emptyPanel.Dock = DockStyle.Fill;
        _emptyPanel.BackColor = AppTheme.Surface;
        _emptyPanel.Visible = false;

        BuildEmptyState();

        _gridCard.Controls.Add(_emptyPanel);
        _gridCard.Controls.Add(_grid);
    }

    private void BuildEmptyState()
    {
        _emptyPanel.Controls.Clear();

        var card = UiFactory.Card();
        card.Padding = new Padding(28);
        card.Margin = Padding.Empty;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _emptyTitleLabel.Dock = DockStyle.Fill;
        _emptyTitleLabel.Font = AppTheme.SectionFont(12F);
        _emptyTitleLabel.ForeColor = AppTheme.Text;
        _emptyTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
        _emptyTitleLabel.Text = "Không tìm thấy nhật ký phù hợp";

        _emptyMessageLabel.Dock = DockStyle.Fill;
        _emptyMessageLabel.ForeColor = AppTheme.TextMuted;
        _emptyMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
        _emptyMessageLabel.AutoEllipsis = true;
        _emptyMessageLabel.Text = "Hãy thử đổi khoảng thời gian hoặc từ khóa tìm kiếm rồi bấm Lọc.";

        var iconTile = UiFactory.IconTile(IconChar.ClockRotateLeft, AppTheme.Primary, AppTheme.PrimarySoft, 64, 28);
        iconTile.Anchor = AnchorStyles.None;
        layout.Controls.Add(iconTile, 0, 0);
        layout.Controls.Add(_emptyTitleLabel, 0, 1);
        layout.Controls.Add(_emptyMessageLabel, 0, 2);
        layout.Controls.Add(new Label
        {
            Text = "Mẹo: nhập tên người dùng, hành động hoặc đối tượng để lọc nhanh.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleCenter,
            AutoEllipsis = true
        }, 0, 3);
        layout.Controls.Add(new Label
        {
            Text = "Kết quả sẽ hiển thị lại khi bạn đổi điều kiện lọc và bấm Lọc.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont(9F),
            TextAlign = ContentAlignment.MiddleCenter,
            AutoEllipsis = true
        }, 0, 4);

        card.Controls.Add(layout);
        _emptyPanel.Controls.Add(card);
    }

    private void BuildColumns()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AuditLogRow.Time),
            HeaderText = "Thời gian",
            FillWeight = 138,
            MinimumWidth = 130
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AuditLogRow.User),
            HeaderText = "Người dùng",
            FillWeight = 140,
            MinimumWidth = 130
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AuditLogRow.Action),
            HeaderText = "Hành động",
            FillWeight = 112,
            MinimumWidth = 100
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AuditLogRow.Entity),
            HeaderText = "Đối tượng",
            FillWeight = 112,
            MinimumWidth = 100
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(AuditLogRow.Description),
            HeaderText = "Mô tả",
            FillWeight = 198,
            MinimumWidth = 120
        });
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200));

        _messageLabel.Dock = DockStyle.Fill;
        _messageLabel.TextAlign = ContentAlignment.MiddleLeft;
        _messageLabel.ForeColor = AppTheme.StatusText;

        layout.Controls.Add(_messageLabel, 0, 0);
        layout.Controls.Add(new Label
        {
            Text = "Nhật ký hệ thống — chỉ đọc.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleRight,
            AutoEllipsis = true
        }, 1, 0);
        _statusCard.Controls.Add(layout);
    }

    private void ReloadLogs()
    {
        _source.DataSource = null;
        ShowEmpty(false);
        UiFactory.SetMessage(_messageLabel, "Đang tải dữ liệu...");

        var result = _service.GetAuditLogs(_fromDate.Value, _toDate.Value, _keywordBox.Text);
        if (!result.Success)
        {
            _source.DataSource = new BindingList<AuditLogRow>();
            ShowEmpty(true);
            UpdateSummary(0);
            UiFactory.SetMessage(_messageLabel, result.Message, true);
            return;
        }

        var rows = (result.Data ?? []).Select(AuditLogRow.FromDto).ToList();
        var list = new BindingList<AuditLogRow>(rows);
        _source.DataSource = list;
        ShowEmpty(list.Count == 0);
        UpdateSummary(list.Count);
        UiFactory.SetMessage(_messageLabel, result.Message);
    }

    private void ClearFilter()
    {
        _fromDate.Value = DateTime.Today.AddDays(-7);
        _toDate.Value = DateTime.Today;
        _keywordBox.Clear();
        ReloadLogs();
    }

    private void ShowEmpty(bool show)
    {
        _emptyPanel.Visible = show;
        _grid.Visible = !show;
    }

    private void UpdateSummary(int count)
    {
        _summaryLabel.Text = count > 0 ? $"{count:N0} bản ghi" : "Không có bản ghi";
    }

    private void ConfigureActionButtons()
    {
        _filterButton.BackColor = AppTheme.Primary;
        _filterButton.ForeColor = Color.White;
        _filterButton.IconColor = Color.White;
        _filterButton.FlatAppearance.BorderSize = 0;
        _filterButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
        _filterButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);

        _clearButton.BackColor = AppTheme.SurfaceSubtle;
        _clearButton.ForeColor = AppTheme.TextMuted;
        _clearButton.IconColor = AppTheme.TextMuted;
        _clearButton.FlatAppearance.BorderColor = AppTheme.Border;
        _clearButton.FlatAppearance.BorderSize = 1;
        _clearButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
        _clearButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(226, 232, 240);
    }

    private static Control MakeToolbarIcon(IconChar icon)
    {
        var host = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = AppTheme.Surface,
            Padding = Padding.Empty
        };
        var box = new IconPictureBox
        {
            IconChar = icon,
            IconColor = AppTheme.Primary,
            IconFont = IconFont.Auto,
            IconSize = 18,
            BackColor = AppTheme.Surface,
            Size = new Size(24, 24)
        };
        host.Controls.Add(box);
        host.Layout += (_, _) =>
        {
            box.Location = new Point(0, Math.Max(0, (host.ClientSize.Height - box.Height) / 2));
        };

        return host;
    }

    private static Label MakeFieldLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        TextAlign = ContentAlignment.MiddleLeft,
        ForeColor = AppTheme.Text,
        Font = AppTheme.BodyFont(9.5F),
        BackColor = AppTheme.Surface,
        UseCompatibleTextRendering = false
    };

    private static Panel MakeDateBox(DateTimePicker picker)
    {
        return MakeInlineHost(picker, new Padding(0, 0, 8, 0));
    }

    private static Panel MakeTextBox(TextBox box)
    {
        return MakeInlineHost(box, new Padding(0, 0, 8, 0));
    }

    private static Panel MakeButtonHost(Control button)
    {
        button.Margin = Padding.Empty;
        return MakeInlineHost(button, Padding.Empty, alignRight: true);
    }

    private static Panel MakeInlineHost(Control control, Padding padding, bool alignRight = false)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = padding,
            BackColor = AppTheme.Surface
        };

        control.Dock = DockStyle.None;
        control.Margin = Padding.Empty;
        panel.Controls.Add(control);
        panel.Layout += (_, _) =>
        {
            var innerHeight = Math.Max(0, panel.ClientSize.Height - panel.Padding.Vertical);
            var x = alignRight
                ? Math.Max(panel.Padding.Left, panel.ClientSize.Width - panel.Padding.Right - control.Width)
                : panel.Padding.Left;
            var y = panel.Padding.Top + Math.Max(0, (innerHeight - control.Height) / 2);
            control.Location = new Point(x, y);
        };

        return panel;
    }

    private sealed class AuditLogRow
    {
        public string Time { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public static AuditLogRow FromDto(AuditLogDto dto)
        {
            return new AuditLogRow
            {
                Time = dto.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                User = $"{dto.FullName} ({dto.Username})",
                Action = dto.Action,
                Entity = dto.EntityName,
                Description = dto.Description
            };
        }
    }
}
