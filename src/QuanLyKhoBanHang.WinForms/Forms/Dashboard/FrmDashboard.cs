namespace QuanLyKhoBanHang.WinForms.Forms.Dashboard;

public sealed class FrmDashboard : Form
{
    public FrmDashboard()
    {
        Text = "Dashboard";
        BackColor = Color.White;
        Padding = new Padding(8);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label
        {
            Text = "Tổng quan hệ thống",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

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
            Font = new Font("Segoe UI", 11F),
            ForeColor = Color.FromArgb(96, 108, 129),
            Padding = new Padding(0, 18, 0, 0)
        };

        layout.Controls.Add(cards, 0, 2);
        layout.Controls.Add(note, 0, 2);
        Controls.Add(layout);
    }

    private static Control CreateCard(string title, string value)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 248, 252),
            Margin = new Padding(0, 0, 12, 0),
            Padding = new Padding(16)
        };

        card.Controls.Add(new Label
        {
            Text = value,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 22F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        });

        card.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 22,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            ForeColor = Color.FromArgb(96, 108, 129)
        });

        return card;
    }
}
