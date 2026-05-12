using QuanLyKhoBanHang.BLL.Services;

namespace QuanLyKhoBanHang.WinForms.Forms.Assistant;

public sealed class FrmAssistant : Form
{
    private readonly AssistantService _assistantService = new();
    private readonly TextBox _txtQuestion = new();
    private readonly TextBox _txtAnswer = new();

    public FrmAssistant()
    {
        Text = "Trợ lý quản lý";
        Padding = new Padding(24);

        var askButton = new Button
        {
            Text = "Hỏi",
            Dock = DockStyle.Right,
            Width = 90
        };
        askButton.Click += HandleAsk;

        _txtQuestion.Dock = DockStyle.Fill;
        _txtQuestion.PlaceholderText = "Ví dụ: doanh thu hôm nay, hàng sắp hết, top sản phẩm bán chạy";

        var inputPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40
        };
        inputPanel.Controls.Add(_txtQuestion);
        inputPanel.Controls.Add(askButton);

        _txtAnswer.Dock = DockStyle.Fill;
        _txtAnswer.Multiline = true;
        _txtAnswer.ReadOnly = true;
        _txtAnswer.ScrollBars = ScrollBars.Vertical;
        _txtAnswer.Font = new Font("Segoe UI", 11F);

        Controls.Add(_txtAnswer);
        Controls.Add(inputPanel);
        AcceptButton = askButton;
    }

    private void HandleAsk(object? sender, EventArgs e)
    {
        var result = _assistantService.Ask(_txtQuestion.Text);
        _txtAnswer.AppendText($"Bạn: {_txtQuestion.Text}{Environment.NewLine}");
        _txtAnswer.AppendText($"Trợ lý: {result.Data?.Answer ?? result.Message}{Environment.NewLine}{Environment.NewLine}");
        _txtQuestion.Clear();
    }
}
