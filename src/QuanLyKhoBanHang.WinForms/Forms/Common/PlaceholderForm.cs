namespace QuanLyKhoBanHang.WinForms.Forms.Common;

public class PlaceholderForm : Form
{
    protected PlaceholderForm(string title, string description)
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        Width = 960;
        Height = 620;
        MinimumSize = new Size(820, 520);

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 52,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(24, 0, 0, 0)
        };

        var bodyLabel = new Label
        {
            Text = description,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11F),
            TextAlign = ContentAlignment.TopLeft,
            Padding = new Padding(24),
            AutoSize = false
        };

        Controls.Add(bodyLabel);
        Controls.Add(titleLabel);
    }
}
