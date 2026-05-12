using QuanLyKhoBanHang.BLL.Services;

namespace QuanLyKhoBanHang.WinForms.Forms.Dashboard;

public sealed class FrmDashboard : Form
{
    private readonly DashboardService _dashboardService = new();

    public FrmDashboard()
    {
        Text = "Dashboard";
        Padding = new Padding(24);

        var result = _dashboardService.GetDashboardSummary(DateTime.Today);
        var summary = result.Data;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 4,
            RowCount = 2,
            Height = 180
        };

        for (var i = 0; i < 4; i++)
        {
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        }

        layout.Controls.Add(CreateCard("Doanh thu hôm nay", summary?.TodayRevenue.ToString("N0") + " VND"), 0, 0);
        layout.Controls.Add(CreateCard("Doanh thu tháng", summary?.MonthRevenue.ToString("N0") + " VND"), 1, 0);
        layout.Controls.Add(CreateCard("Hóa đơn hôm nay", summary?.TodayInvoiceCount.ToString() ?? "0"), 2, 0);
        layout.Controls.Add(CreateCard("Hàng tồn thấp", summary?.LowStockProductCount.ToString() ?? "0"), 3, 0);

        var note = new Label
        {
            Text = "Dashboard UI thuộc Châu. Dữ liệu thật sẽ được nối qua DashboardService khi DAL/BLL hoàn thiện.",
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 24, 0, 0),
            Font = new Font("Segoe UI", 11F)
        };

        Controls.Add(note);
        Controls.Add(layout);
    }

    private static Control CreateCard(string title, string value)
    {
        return new GroupBox
        {
            Text = title,
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            Controls =
            {
                new Label
                {
                    Text = value,
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter
                }
            }
        };
    }
}
