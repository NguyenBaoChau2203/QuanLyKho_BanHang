using System.ComponentModel;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Admin;
using QuanLyKhoBanHang.DTO.Common;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Admin;

public sealed class FrmRolePermission : Form
{
    private readonly PermissionService _permissionService = new();
    private readonly BindingSource _source = new();
    private readonly DataGridView _grid = new();
    private readonly Label _messageLabel = new();
    private readonly Label _summaryLabel = new();
    private readonly Panel _emptyPanel = new();
    private readonly Label _emptyTitleLabel = new();
    private readonly Label _emptyMessageLabel = new();
    private readonly RoundedPanel _gridCard = new();
    private readonly RoundedPanel _headerCard = new();
    private readonly Panel _statusCard = new();
    private readonly IconButton _refreshButton;

    public FrmRolePermission()
    {
        Text = "Phân quyền";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1100, 620);

        _refreshButton = UiFactory.IconActionButton("Làm mới", IconChar.Rotate, (_, _) => LoadPermissions(), 124);

        ConfigureActionButtons();
        BuildUi();
        LoadPermissions();
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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        ConfigureHeaderCard();
        ConfigureGridCard();
        ConfigureStatusBar();

        root.Controls.Add(_headerCard, 0, 0);
        root.Controls.Add(BuildToolbar(), 0, 1);
        root.Controls.Add(_gridCard, 0, 2);
        root.Controls.Add(_statusCard, 0, 3);
        Controls.Add(root);
    }

    private void ConfigureHeaderCard()
    {
        _headerCard.Dock = DockStyle.Fill;
        _headerCard.FillColor = AppTheme.Surface;
        _headerCard.BorderColor = AppTheme.Border;
        _headerCard.Radius = 8;
        _headerCard.ShadowSize = 1;
        _headerCard.Padding = new Padding(18, 14, 18, 14);
        _headerCard.Margin = Padding.Empty;

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280));

        _summaryLabel.Dock = DockStyle.Fill;
        _summaryLabel.Font = AppTheme.SectionFont(10.5F);
        _summaryLabel.ForeColor = AppTheme.Primary;
        _summaryLabel.TextAlign = ContentAlignment.MiddleRight;
        _summaryLabel.AutoEllipsis = true;
        _summaryLabel.Margin = new Padding(12, 0, 0, 0);

        headerLayout.Controls.Add(UiFactory.SectionHeader(
            "Ma trận phân quyền",
            "Xem nhanh mỗi vai trò được phép mở màn hình nào trong bản demo.",
            IconChar.ShieldHalved), 0, 0);
        headerLayout.Controls.Add(_summaryLabel, 1, 0);
        _headerCard.Controls.Add(headerLayout);
    }

    private Control BuildToolbar()
    {
        var card = UiFactory.Card();
        card.Padding = new Padding(14, 8, 14, 8);
        card.Margin = Padding.Empty;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 138));

        var hintLabel = new Label
        {
            Dock = DockStyle.Fill,
            Text = "Bảng ma trận quyền truy cập các màn hình theo từng vai trò.",
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.BodyFont(9.5F),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };

        layout.Controls.Add(hintLabel, 0, 0);
        layout.Controls.Add(WrapRefreshButtonInBorderFrame(), 1, 0);
        card.Controls.Add(layout);
        return card;
    }

    private Control WrapRefreshButtonInBorderFrame()
    {
        _refreshButton.Margin = Padding.Empty;
        _refreshButton.Dock = DockStyle.Fill;

        var frame = new Panel
        {
            BackColor = AppTheme.BorderStrong,
            Padding = new Padding(1),
            Margin = new Padding(0, 0, 8, 0),
            Width = _refreshButton.Width + 2,
            Height = _refreshButton.Height + 2
        };
        frame.Controls.Add(_refreshButton);
        return frame;
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
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _emptyTitleLabel.Dock = DockStyle.Fill;
        _emptyTitleLabel.Font = AppTheme.SectionFont(12F);
        _emptyTitleLabel.ForeColor = AppTheme.Text;
        _emptyTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
        _emptyTitleLabel.Text = "Chưa có dữ liệu phân quyền";

        _emptyMessageLabel.Dock = DockStyle.Fill;
        _emptyMessageLabel.ForeColor = AppTheme.TextMuted;
        _emptyMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
        _emptyMessageLabel.AutoEllipsis = true;
        _emptyMessageLabel.Text = "Dữ liệu phân quyền sẽ hiển thị ở đây sau khi tải thành công.";

        var iconTile = UiFactory.IconTile(IconChar.ShieldHalved, AppTheme.Primary, AppTheme.PrimarySoft, 64, 28);
        iconTile.Anchor = AnchorStyles.None;
        layout.Controls.Add(iconTile, 0, 0);
        layout.Controls.Add(_emptyTitleLabel, 0, 1);
        layout.Controls.Add(_emptyMessageLabel, 0, 2);
        layout.Controls.Add(new Label
        {
            Text = "Bảng ma trận hiển thị quyền Có/Không của từng vai trò với mỗi màn hình chức năng.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleCenter,
            AutoEllipsis = true
        }, 0, 3);

        card.Controls.Add(layout);
        _emptyPanel.Controls.Add(card);
    }

    private void BuildColumns()
    {
        _grid.Columns.Clear();
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PermissionMatrixRow.GroupName),
            HeaderText = "Nhóm",
            FillWeight = 110,
            MinimumWidth = 100
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PermissionMatrixRow.FeatureName),
            HeaderText = "Màn hình",
            FillWeight = 145,
            MinimumWidth = 140
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PermissionMatrixRow.Admin),
            HeaderText = "Quản trị",
            FillWeight = 90,
            MinimumWidth = 85
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PermissionMatrixRow.Manager),
            HeaderText = "Quản lý",
            FillWeight = 90,
            MinimumWidth = 85
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PermissionMatrixRow.Warehouse),
            HeaderText = "Nhân viên kho",
            FillWeight = 110,
            MinimumWidth = 100
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PermissionMatrixRow.Sales),
            HeaderText = "Nhân viên bán hàng",
            FillWeight = 125,
            MinimumWidth = 115
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PermissionMatrixRow.Note),
            HeaderText = "Ghi chú",
            FillWeight = 160,
            MinimumWidth = 80
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
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _messageLabel.Dock = DockStyle.Fill;
        _messageLabel.TextAlign = ContentAlignment.MiddleLeft;
        _messageLabel.ForeColor = AppTheme.StatusText;

        var footerHint = new Label
        {
            Text = "Bảng ma trận phân quyền demo.",
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(8, 0, 0, 0)
        };

        layout.Controls.Add(_messageLabel, 0, 0);
        layout.Controls.Add(footerHint, 1, 0);
        _statusCard.Controls.Add(layout);
    }

    private void LoadPermissions()
    {
        _source.DataSource = null;
        ShowEmpty(false);
        UiFactory.SetMessage(_messageLabel, "Đang tải dữ liệu...");

        var result = _permissionService.GetPermissionMatrix();
        if (!result.Success)
        {
            _source.DataSource = new BindingList<PermissionMatrixRow>();
            ShowEmpty(true);
            UpdateSummary(0);
            UiFactory.SetMessage(_messageLabel, result.Message, true);
            return;
        }

        var rows = (result.Data ?? [])
            .GroupBy(x => x.FeatureKey)
            .Select(PermissionMatrixRow.FromGroup)
            .ToList();

        var list = new BindingList<PermissionMatrixRow>(rows);
        _source.DataSource = list;
        ShowEmpty(list.Count == 0);
        UpdateSummary(list.Count);
        UiFactory.SetMessage(_messageLabel, result.Message);
    }

    private void ShowEmpty(bool show)
    {
        _emptyPanel.Visible = show;
        _grid.Visible = !show;
    }

    private void UpdateSummary(int count)
    {
        _summaryLabel.Text = count > 0 ? $"{count} màn hình" : "Không có dữ liệu";
    }

    private void ConfigureActionButtons()
    {
        _refreshButton.BackColor = AppTheme.SurfaceSubtle;
        _refreshButton.ForeColor = AppTheme.Primary;
        _refreshButton.IconColor = AppTheme.Primary;
        _refreshButton.FlatAppearance.BorderSize = 0;
        _refreshButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(219, 234, 254);
        _refreshButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(191, 219, 254);
        _refreshButton.Padding = new Padding(10, 0, 10, 0);
        _refreshButton.TextImageRelation = TextImageRelation.ImageBeforeText;
        _refreshButton.ImageAlign = ContentAlignment.MiddleLeft;
        _refreshButton.TextAlign = ContentAlignment.MiddleCenter;
    }

    private sealed class PermissionMatrixRow
    {
        public string GroupName { get; set; } = string.Empty;
        public string FeatureName { get; set; } = string.Empty;
        public string Admin { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string Warehouse { get; set; } = string.Empty;
        public string Sales { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        public static PermissionMatrixRow FromGroup(IGrouping<string, RolePermissionDto> group)
        {
            var items = group.ToList();
            var first = items[0];
            return new PermissionMatrixRow
            {
                GroupName = first.GroupName,
                FeatureName = first.FeatureName,
                Admin = AccessText(items, UserRole.Admin),
                Manager = AccessText(items, UserRole.Manager),
                Warehouse = AccessText(items, UserRole.WarehouseStaff),
                Sales = AccessText(items, UserRole.SalesStaff),
                Note = first.Note
            };
        }

        private static string AccessText(IEnumerable<RolePermissionDto> items, UserRole role)
        {
            return items.FirstOrDefault(x => x.Role == role)?.CanAccess == true ? "Có" : "Không";
        }
    }
}
