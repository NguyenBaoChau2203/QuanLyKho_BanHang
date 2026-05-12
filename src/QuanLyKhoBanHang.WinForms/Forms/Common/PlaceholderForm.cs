namespace QuanLyKhoBanHang.WinForms.Forms.Common;

public class PlaceholderForm : Form
{
    protected PlaceholderForm(string title, string description)
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;
        MinimumSize = new Size(820, 520);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(24)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var bodyLabel = new Label
        {
            Text = description,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            TextAlign = ContentAlignment.TopLeft,
            AutoSize = false
        };

        var infoCard = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 248, 252),
            Padding = new Padding(18),
            Margin = new Padding(0, 6, 0, 6)
        };
        infoCard.Controls.Add(new Label
        {
            Text = "Khu vực này sẽ dùng lại style chung khi các màn hình nghiệp vụ thật được triển khai.",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(96, 108, 129),
            TextAlign = ContentAlignment.MiddleLeft
        });

        root.Controls.Add(titleLabel, 0, 0);
        root.Controls.Add(bodyLabel, 0, 2);
        root.Controls.Add(infoCard, 0, 3);
        Controls.Add(root);
    }
}
