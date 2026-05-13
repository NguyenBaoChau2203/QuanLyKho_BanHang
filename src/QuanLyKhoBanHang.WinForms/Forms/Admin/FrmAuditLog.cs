using System.ComponentModel;
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
    private readonly DataGridView _grid = new();

    public FrmAuditLog()
    {
        Text = "Nhật ký hệ thống";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1180, 720);

        BuildUi();
        ReloadLogs();
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
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        root.Controls.Add(UiFactory.HeaderPanel(
            "Nhật ký hệ thống",
            "Xem nhật ký thao tác demo, lọc theo ngày và từ khóa người dùng hoặc hành động."), 0, 0);
        root.Controls.Add(BuildFilterBar(), 0, 1);

        _grid.DataSource = _source;
        _grid.AutoGenerateColumns = false;
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        UiFactory.StyleGrid(_grid);
        BuildColumns();
        root.Controls.Add(_grid, 0, 2);

        _messageLabel.Dock = DockStyle.Fill;
        _messageLabel.TextAlign = ContentAlignment.MiddleLeft;
        _messageLabel.ForeColor = AppTheme.StatusText;
        root.Controls.Add(_messageLabel, 0, 3);

        Controls.Add(root);
    }

    private Control BuildFilterBar()
    {
        var panel = UiFactory.Card();
        panel.Padding = new Padding(14, 10, 14, 10);

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        _fromDate.Format = DateTimePickerFormat.Custom;
        _fromDate.CustomFormat = "dd/MM/yyyy";
        _fromDate.Width = 130;
        _fromDate.Value = DateTime.Today.AddDays(-7);

        _toDate.Format = DateTimePickerFormat.Custom;
        _toDate.CustomFormat = "dd/MM/yyyy";
        _toDate.Width = 130;
        _toDate.Value = DateTime.Today;

        _keywordBox.Width = 260;
        _keywordBox.PlaceholderText = "Từ khóa, người dùng, hành động";
        _keywordBox.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ReloadLogs();
            }
        };

        flow.Controls.Add(new Label { Text = "Từ ngày", AutoSize = true, Padding = new Padding(0, 8, 8, 0) });
        flow.Controls.Add(_fromDate);
        flow.Controls.Add(new Label { Text = "Đến ngày", AutoSize = true, Padding = new Padding(16, 8, 8, 0) });
        flow.Controls.Add(_toDate);
        flow.Controls.Add(new Label { Text = "Từ khóa", AutoSize = true, Padding = new Padding(16, 8, 8, 0) });
        flow.Controls.Add(_keywordBox);
        flow.Controls.Add(UiFactory.ActionButton("Lọc", (_, _) => ReloadLogs(), 90));
        flow.Controls.Add(UiFactory.ActionButton("Xóa lọc", (_, _) => ClearFilter(), 100));

        panel.Controls.Add(flow);
        return panel;
    }

    private void BuildColumns()
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(AuditLogRow.Time), HeaderText = "Thời gian", Width = 150 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(AuditLogRow.User), HeaderText = "Người dùng", Width = 160 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(AuditLogRow.Action), HeaderText = "Hành động", Width = 140 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(AuditLogRow.Entity), HeaderText = "Đối tượng", Width = 140 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(AuditLogRow.Description), HeaderText = "Mô tả" });
    }

    private void ReloadLogs()
    {
        var result = _service.GetAuditLogs(_fromDate.Value, _toDate.Value, _keywordBox.Text);
        if (!result.Success)
        {
            _source.DataSource = new BindingList<AuditLogRow>();
            UiFactory.SetMessage(_messageLabel, result.Message, true);
            return;
        }

        var rows = (result.Data ?? []).Select(AuditLogRow.FromDto).ToList();
        _source.DataSource = new BindingList<AuditLogRow>(rows);
        UiFactory.SetMessage(_messageLabel, $"{result.Message} Số dòng: {rows.Count:N0}.");
    }

    private void ClearFilter()
    {
        _fromDate.Value = DateTime.Today.AddDays(-7);
        _toDate.Value = DateTime.Today;
        _keywordBox.Clear();
        ReloadLogs();
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
