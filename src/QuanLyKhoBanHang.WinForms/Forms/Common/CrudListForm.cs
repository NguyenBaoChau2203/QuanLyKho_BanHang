using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Common;

public abstract class CrudListForm<TItem> : Form where TItem : class, new()
{
    protected readonly BindingSource BindingSource = new();
    protected readonly DataGridView Grid = new();
    protected readonly TextBox SearchBox = new();
    protected readonly TextBox CodeBox = new();
    protected readonly TextBox NameBox = new();
    protected readonly TextBox DescriptionBox = new();
    protected readonly CheckBox ActiveBox = new();
    protected readonly Label MessageLabel = new();
    protected readonly Panel EmptyPanel = new();
    protected readonly Panel EditPanel = new();
    protected readonly Panel ContentPanel = new();
    protected readonly FlowLayoutPanel ActionBar = new();

    protected bool IsEditing;
    protected int SelectedId;

    protected CrudListForm(string title, string subtitle)
    {
        Text = title;
        BackColor = AppTheme.Surface;
        MinimumSize = new Size(1180, 720);
        Font = AppTheme.BodyFont();

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(18)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        root.Controls.Add(BuildHeader(title, subtitle), 0, 0);
        root.Controls.Add(BuildSearchBar(), 0, 1);
        root.Controls.Add(BuildBody(), 0, 2);
        root.Controls.Add(MessageLabel, 0, 3);
        Controls.Add(root);

        MessageLabel.Dock = DockStyle.Fill;
        MessageLabel.ForeColor = AppTheme.StatusText;
        MessageLabel.TextAlign = ContentAlignment.MiddleLeft;

        Grid.Dock = DockStyle.Fill;
        Grid.ReadOnly = true;
        Grid.AllowUserToAddRows = false;
        Grid.AllowUserToDeleteRows = false;
        Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        Grid.MultiSelect = false;
        UiFactory.StyleGrid(Grid);
        Grid.DataSource = BindingSource;
        Grid.SelectionChanged += (_, _) => OnSelectionChanged();

        SearchBox.PlaceholderText = "Tìm kiếm...";
        SearchBox.Width = 320;
        SearchBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        SearchBox.TabIndex = 0;
        SearchBox.TextChanged += (_, _) => ApplyFilter();

        CodeBox.Width = 220;
        NameBox.Width = 260;
        DescriptionBox.Width = 260;
        ActiveBox.Text = "Đang hoạt động";
        ActiveBox.AutoSize = true;
        ActiveBox.MinimumSize = new Size(160, 30);
        ActiveBox.Margin = new Padding(0, 6, 0, 8);

        MessageLabel.Text = "Sẵn sàng";
    }

    protected virtual Control BuildHeader(string title, string subtitle)
    {
        var panel = new Panel { Dock = DockStyle.Fill };
        panel.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Font = AppTheme.TitleFont(),
            Height = 34
        });
        panel.Controls.Add(new Label
        {
            Text = subtitle,
            Dock = DockStyle.Bottom,
            ForeColor = AppTheme.TextMuted,
            Height = 22
        });
        return panel;
    }

    private Control BuildSearchBar()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
        panel.Controls.Add(new Label { Text = "Tìm kiếm", AutoSize = true, Padding = new Padding(0, 9, 8, 0) });
        panel.Controls.Add(SearchBox);
        panel.Controls.Add(new Button { Text = "Làm mới", Width = 90, Height = 34, Margin = new Padding(12, 0, 0, 0) });
        ((Button)panel.Controls[^1]).Click += (_, _) => RefreshData();
        return panel;
    }

    private Control BuildBody()
    {
        var splitter = UiFactory.HorizontalSplitter(760, 260);
        splitter.Panel1.Controls.Add(Grid);
        splitter.Panel2.Padding = new Padding(12, 0, 0, 0);
        splitter.Panel2.Controls.Add(BuildEditPanel());
        return splitter;
    }

    private Control BuildEditPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill };
        var layout = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 1, RowCount = 7, AutoSize = true };
        layout.Controls.Add(BuildField("Mã", CodeBox), 0, 0);
        layout.Controls.Add(BuildField("Tên", NameBox), 0, 1);
        layout.Controls.Add(BuildField("Mô tả", DescriptionBox, true), 0, 2);
        layout.Controls.Add(ActiveBox, 0, 3);

        ActionBar.Dock = DockStyle.Top;
        ActionBar.Height = 44;
        ActionBar.WrapContents = false;
        ActionBar.Controls.Add(CreateActionButton("Thêm", (_, _) => BeginAdd()));
        ActionBar.Controls.Add(CreateActionButton("Sửa", (_, _) => BeginEdit()));
        ActionBar.Controls.Add(CreateActionButton("Lưu", (_, _) => SaveCurrent()));
        ActionBar.Controls.Add(CreateActionButton("Hủy", (_, _) => CancelEdit()));
        ActionBar.Controls.Add(CreateActionButton("Ngừng kích hoạt", (_, _) => DeactivateCurrent()));
        layout.Controls.Add(ActionBar, 0, 4);

        EmptyPanel.Dock = DockStyle.Fill;
        EmptyPanel.Visible = false;
        EmptyPanel.Controls.Add(new Label
        {
            Text = "Chưa có dữ liệu hoặc đang chờ service trả về.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = AppTheme.TextMuted
        });

        panel.Controls.Add(EmptyPanel);
        panel.Controls.Add(layout);
        return panel;
    }

    private static Control BuildField(string label, TextBox box, bool multiline = false)
    {
        if (multiline) box.Multiline = true;
        var panel = new Panel { Dock = DockStyle.Top, Height = multiline ? 96 : 68, Padding = new Padding(0, 0, 0, 8) };
        panel.Controls.Add(box);
        box.Dock = DockStyle.Bottom;
        box.Height = multiline ? 56 : 30;
        panel.Controls.Add(new Label { Text = label, Dock = DockStyle.Top, Height = 20 });
        return panel;
    }

    protected Button CreateActionButton(string text, EventHandler onClick)
    {
        var button = new Button { Text = text, Width = 104, Height = 34, Margin = new Padding(0, 0, 8, 0) };
        button.Click += onClick;
        return button;
    }

    protected void SetMessage(string message, bool isError = false)
    {
        UiFactory.SetMessage(MessageLabel, message, isError);
    }

    protected void ToggleEditing(bool editing)
    {
        IsEditing = editing;
        CodeBox.ReadOnly = !editing;
        NameBox.ReadOnly = !editing;
        DescriptionBox.ReadOnly = !editing;
        ActiveBox.Enabled = editing;
    }

    protected void ShowEmpty(bool show)
    {
        EmptyPanel.Visible = show;
        Grid.Visible = !show;
    }

    protected abstract void RefreshData();
    protected abstract void ApplyFilter();
    protected abstract void OnSelectionChanged();
    protected abstract void BeginAdd();
    protected abstract void BeginEdit();
    protected abstract void SaveCurrent();
    protected abstract void CancelEdit();
    protected abstract void DeactivateCurrent();

    protected static List<T> ToList<T>(ServiceResult<List<T>> result, Func<T, bool>? filter = null)
    {
        var data = result.Success ? result.Data ?? [] : [];
        return filter is null ? data : data.Where(filter).ToList();
    }
}
