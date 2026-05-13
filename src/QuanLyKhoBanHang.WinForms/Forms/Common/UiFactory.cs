namespace QuanLyKhoBanHang.WinForms.Forms.Common;

internal static class UiFactory
{
    public static Label TitleLabel(string text, float size = 18F) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        Height = 36,
        Font = AppTheme.TitleFont(size),
        TextAlign = ContentAlignment.MiddleLeft
    };

    public static Label SubtitleLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Bottom,
        Height = 22,
        Font = AppTheme.BodyFont(),
        ForeColor = AppTheme.TextMuted,
        TextAlign = ContentAlignment.MiddleLeft
    };

    public static Panel HeaderPanel(string title, string subtitle)
    {
        var panel = new Panel { Dock = DockStyle.Fill };
        panel.Controls.Add(SubtitleLabel(subtitle));
        panel.Controls.Add(TitleLabel(title));
        return panel;
    }

    public static Panel Card() => new()
    {
        Dock = DockStyle.Fill,
        BackColor = AppTheme.SurfaceMuted,
        Margin = new Padding(0, 0, 12, 0),
        Padding = AppTheme.CardPadding
    };

    public static SplitContainer HorizontalSplitter(int preferredDistance, int minimumPanelWidth = 220)
    {
        var splitter = new SplitContainer
        {
            Dock = DockStyle.Fill
        };

        void ApplyPreferredDistance()
        {
            var width = splitter.ClientSize.Width;
            if (width <= splitter.SplitterWidth + 50)
            {
                return;
            }

            var safeMinimum = Math.Min(minimumPanelWidth, Math.Max(25, (width - splitter.SplitterWidth) / 3));
            var maxDistance = width - splitter.SplitterWidth - safeMinimum;
            if (maxDistance < safeMinimum)
            {
                return;
            }

            var distance = Math.Clamp(preferredDistance, safeMinimum, maxDistance);
            if (splitter.SplitterDistance != distance)
            {
                splitter.SplitterDistance = distance;
            }
        }

        splitter.SizeChanged += (_, _) => ApplyPreferredDistance();
        splitter.HandleCreated += (_, _) => ApplyPreferredDistance();
        return splitter;
    }

    public static Button ActionButton(string text, EventHandler handler, int width = 110)
    {
        var button = new Button
        {
            Text = text,
            Height = 36,
            Width = width,
            Margin = new Padding(0, 0, 8, 0),
            FlatStyle = FlatStyle.Standard
        };
        button.Click += handler;
        return button;
    }

    public static Button SidebarButton(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Fill,
        Height = 40,
        FlatStyle = FlatStyle.Flat,
        BackColor = AppTheme.SidebarButton,
        ForeColor = Color.White,
        Font = AppTheme.BodyFont(),
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(12, 0, 0, 0)
    };

    public static DataGridView ReadOnlyGrid(BindingSource? source = null)
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            BackgroundColor = AppTheme.Surface,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            DataSource = source
        };

        StyleGrid(grid);
        return grid;
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = AppTheme.Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.RowHeadersVisible = false;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        grid.DefaultCellStyle.SelectionBackColor = AppTheme.Selection;
        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.SectionFont(10F);
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 240, 247);
        grid.EnableHeadersVisualStyles = false;
    }

    public static void SetMessage(Label label, string message, bool isError = false)
    {
        label.Text = message;
        label.ForeColor = isError ? AppTheme.Error : AppTheme.StatusText;
    }
}
