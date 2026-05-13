using System.ComponentModel;
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

    public FrmRolePermission()
    {
        Text = "Phân quyền";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1180, 720);

        BuildUi();
        LoadPermissions();
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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        root.Controls.Add(UiFactory.HeaderPanel(
            "Ma trận phân quyền",
            "Xem nhanh mỗi vai trò được phép mở màn hình nào trong bản demo."), 0, 0);

        _grid.DataSource = _source;
        _grid.AutoGenerateColumns = false;
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        UiFactory.StyleGrid(_grid);
        BuildColumns();
        root.Controls.Add(_grid, 0, 1);

        _messageLabel.Dock = DockStyle.Fill;
        _messageLabel.TextAlign = ContentAlignment.MiddleLeft;
        _messageLabel.ForeColor = AppTheme.StatusText;
        root.Controls.Add(_messageLabel, 0, 2);

        Controls.Add(root);
    }

    private void BuildColumns()
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PermissionMatrixRow.GroupName), HeaderText = "Nhóm", Width = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PermissionMatrixRow.FeatureName), HeaderText = "Màn hình", Width = 160 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PermissionMatrixRow.Admin), HeaderText = "Quản trị", Width = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PermissionMatrixRow.Manager), HeaderText = "Quản lý", Width = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PermissionMatrixRow.Warehouse), HeaderText = "Nhân viên kho", Width = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PermissionMatrixRow.Sales), HeaderText = "Nhân viên bán hàng", Width = 140 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(PermissionMatrixRow.Note), HeaderText = "Ghi chú" });
    }

    private void LoadPermissions()
    {
        var result = _permissionService.GetPermissionMatrix();
        if (!result.Success)
        {
            _source.DataSource = new BindingList<PermissionMatrixRow>();
            UiFactory.SetMessage(_messageLabel, result.Message, true);
            return;
        }

        var rows = (result.Data ?? [])
            .GroupBy(x => x.FeatureKey)
            .Select(PermissionMatrixRow.FromGroup)
            .ToList();

        _source.DataSource = new BindingList<PermissionMatrixRow>(rows);
        UiFactory.SetMessage(_messageLabel, result.Message);
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
