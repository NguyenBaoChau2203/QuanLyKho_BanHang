using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Dashboard;

public sealed class FrmDashboard : Form
{
    public FrmDashboard()
    {
        Text = "Dashboard";
        BackColor = AppTheme.Surface;
        Padding = new Padding(8);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(UiFactory.HeaderPanel(
            "Tổng quan hệ thống",
            "Theo dõi nhanh doanh thu, tồn kho và cảnh báo phục vụ demo quản lý."), 0, 0);

        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 160,
            ColumnCount = 4
        };
        for (var i = 0; i < 4; i++)
        {
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            cards.Controls.Add(CreateCard($"Chỉ số {i + 1}", i == 0 ? "128" : i == 1 ? "36" : i == 2 ? "12" : "4"), i, 0);
        }

        var note = new Label
        {
            Text = "Dashboard tạm dùng dữ liệu mẫu để Châu hoàn thiện shell trước khi backend thật sẵn sàng.",
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(11F),
            ForeColor = AppTheme.TextMuted,
            Padding = new Padding(0, 18, 0, 0)
        };

        layout.Controls.Add(cards, 0, 2);
        layout.Controls.Add(note, 0, 3);
        Controls.Add(layout);
    }

    private static Control CreateCard(string title, string value)
    {
        var card = UiFactory.Card();

        card.Controls.Add(new Label
        {
            Text = value,
            Dock = DockStyle.Fill,
            Font = AppTheme.TitleFont(22F),
            TextAlign = ContentAlignment.MiddleLeft
        });

        card.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 22,
            Font = AppTheme.BodyFont(),
            ForeColor = AppTheme.TextMuted
        });

        return card;
    }
}
