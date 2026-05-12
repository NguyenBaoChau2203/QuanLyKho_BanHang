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
            RowCount = 3,
            Padding = new Padding(24)
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 8));
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

        root.Controls.Add(titleLabel, 0, 0);
        root.SetRowSpan(bodyLabel, 2);
        root.Controls.Add(bodyLabel, 0, 2);
        Controls.Add(root);
    }
}
