using FontAwesome.Sharp;

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

    public static Panel Card() => new RoundedPanel
    {
        Dock = DockStyle.Fill,
        FillColor = AppTheme.Surface,
        BorderColor = AppTheme.Border,
        Radius = 8,
        ShadowSize = 1,
        Margin = new Padding(0, 0, 12, 0),
        Padding = AppTheme.CardPadding
    };

    public static RoundedPanel SoftTile(Color fillColor, Color borderColor, int radius = 8) => new()
    {
        FillColor = fillColor,
        BorderColor = borderColor,
        Radius = radius,
        ShadowSize = 0,
        Padding = Padding.Empty
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

    public static IconButton IconActionButton(string text, IconChar icon, EventHandler handler, int width = 120)
    {
        var button = new IconButton
        {
            Text = text,
            Height = 36,
            Width = width,
            Margin = new Padding(0, 0, 8, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.Primary,
            ForeColor = Color.White,
            IconChar = icon,
            IconColor = Color.White,
            IconFont = IconFont.Auto,
            IconSize = 16,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft,
            TextAlign = ContentAlignment.MiddleCenter,
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);
        button.Click += handler;
        return button;
    }

    public static Button SidebarButton(string text)
        => SidebarButton(text, IconChar.Circle, false);

    public static IconButton SidebarButton(string text, IconChar icon, bool selected = false)
    {
        var button = new IconButton
        {
            Text = text,
            Dock = DockStyle.Fill,
            Height = 42,
            FlatStyle = FlatStyle.Flat,
            BackColor = selected ? AppTheme.SidebarSelected : AppTheme.SidebarButton,
            ForeColor = Color.White,
            Font = AppTheme.BodyFont(),
            IconChar = icon,
            IconColor = selected ? Color.White : AppTheme.SidebarTextMuted,
            IconFont = IconFont.Auto,
            IconSize = 18,
            TextAlign = ContentAlignment.MiddleLeft,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ImageAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0),
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.BorderColor = AppTheme.SidebarButton;
        button.FlatAppearance.MouseOverBackColor = selected ? AppTheme.SidebarSelected : AppTheme.SidebarHover;
        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(58, 88, 128);
        return button;
    }

    public static void SetSidebarButtonState(Button button, bool selected)
    {
        button.BackColor = selected ? AppTheme.SidebarSelected : AppTheme.SidebarButton;
        button.ForeColor = Color.White;
        button.FlatAppearance.MouseOverBackColor = selected ? AppTheme.SidebarSelected : AppTheme.SidebarHover;

        if (button is IconButton iconButton)
        {
            iconButton.IconColor = selected ? Color.White : AppTheme.SidebarTextMuted;
        }
    }

    public static Control IconTile(IconChar icon, Color iconColor, Color fillColor, int size = 70, int iconSize = 32)
    {
        var tile = SoftTile(fillColor, fillColor, 10);
        tile.Width = size;
        tile.Height = size;
        tile.Margin = Padding.Empty;

        var iconBox = new IconPictureBox
        {
            Size = new Size(iconSize, iconSize),
            BackColor = Color.Transparent,
            IconChar = icon,
            IconColor = iconColor,
            IconFont = IconFont.Auto,
            IconSize = iconSize,
            SizeMode = PictureBoxSizeMode.CenterImage
        };

        void CenterIcon()
        {
            iconBox.Location = new Point(
                Math.Max(0, (tile.ClientSize.Width - iconBox.Width) / 2),
                Math.Max(0, (tile.ClientSize.Height - iconBox.Height) / 2));
        }

        tile.Controls.Add(iconBox);
        tile.SizeChanged += (_, _) => CenterIcon();
        tile.HandleCreated += (_, _) => CenterIcon();
        CenterIcon();

        return tile;
    }

    public static Control SectionHeader(string title, string subtitle, IconChar icon)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));

        layout.Controls.Add(new IconPictureBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            IconChar = icon,
            IconColor = AppTheme.Primary,
            IconFont = IconFont.Auto,
            IconSize = 20,
            Padding = new Padding(0, 3, 8, 0)
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
            Font = AppTheme.BodyFont(9.5F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft
        }, 1, 1);

        return layout;
    }

    public static Control MetricCard(string title, Label valueLabel, IconChar icon, Color accent, Color iconFill)
    {
        var card = Card();
        if (card is RoundedPanel rounded)
        {
            rounded.FillColor = AppTheme.Surface;
            rounded.BorderColor = AppTheme.Border;
            rounded.Radius = 8;
            rounded.ShadowSize = 1;
        }

        card.Margin = new Padding(0, 0, 14, 0);
        card.Padding = new Padding(0);

        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 5));
        shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        shell.Controls.Add(new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = accent
        }, 0, 0);

        var body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(20, 18, 22, 18)
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 86));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        body.Controls.Add(IconTile(icon, accent, iconFill, 70, 32), 0, 0);

        var textStack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        textStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        textStack.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        textStack.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(10F),
            ForeColor = AppTheme.Text,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 0, 0);

        valueLabel.Dock = DockStyle.Fill;
        valueLabel.Font = AppTheme.TitleFont(21F);
        valueLabel.TextAlign = ContentAlignment.TopLeft;
        valueLabel.ForeColor = accent;
        valueLabel.Text = "--";
        textStack.Controls.Add(valueLabel, 0, 1);

        body.Controls.Add(textStack, 1, 0);
        shell.Controls.Add(body, 1, 0);
        card.Controls.Add(shell);
        return card;
    }

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
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.GridColor = AppTheme.GridLine;
        grid.RowHeadersVisible = false;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        grid.ColumnHeadersHeight = 38;
        grid.RowTemplate.Height = 34;
        grid.DefaultCellStyle.Font = AppTheme.BodyFont(9.5F);
        grid.DefaultCellStyle.BackColor = AppTheme.Surface;
        grid.DefaultCellStyle.ForeColor = AppTheme.Text;
        grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        grid.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.SurfaceSubtle;
        grid.DefaultCellStyle.SelectionBackColor = AppTheme.Selection;
        grid.DefaultCellStyle.SelectionForeColor = AppTheme.Text;
        grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.SectionFont(9.5F);
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(239, 245, 255);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.Text;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 245, 255);
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = AppTheme.Text;
        grid.EnableHeadersVisualStyles = false;
    }

    public static void SetMessage(Label label, string message, bool isError = false)
    {
        label.Text = message;
        label.ForeColor = isError ? AppTheme.Error : AppTheme.StatusText;
    }
}
