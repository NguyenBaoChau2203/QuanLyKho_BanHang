using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Assistant;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Assistant;

public sealed class FrmAssistant : Form
{
    private readonly AssistantService _assistantService = new();

    private readonly TextBox _txtQuestion = new();
    private readonly Label _modeLabel = new();
    private readonly Panel _scrollOuter = new();
    private readonly FlowLayoutPanel _conversationFlow = new();
    private readonly Button _btnSend = new();
    private readonly Button _btnClear = new();

    private static readonly Color PrimaryBlue = Color.FromArgb(37, 99, 235);
    private static readonly Color OnlineGreen = Color.FromArgb(22, 101, 52);
    private static readonly Color FallbackAmber = Color.FromArgb(146, 64, 14);
    private static readonly Color UserBubbleBg = Color.FromArgb(236, 242, 254);

    public FrmAssistant()
    {
        Text = "Trợ lý AI";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(960, 620);
        Padding = AppTheme.PagePadding;

        BuildLayout();
        Load += (_, _) =>
        {
            RefreshModeStatus();
            AppendAssistantCard(
                "Trợ lý AI",
                "Chào bạn! Tôi có thể dùng DeepSeek nếu đã cấu hình, và luôn tự fallback sang trợ lý offline khi thiếu API hoặc có lỗi mạng.\nChọn gợi ý bên dưới hoặc nhập câu hỏi tiếng Việt, sau đó bấm Gửi.",
                "Sẵn sàng");
        };

        _txtQuestion.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                HandleSubmit();
            }
        };
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(UiFactory.HeaderPanel(
            "Trợ lý AI",
            "Hỏi nhanh bằng tiếng Việt, nhận câu trả lời từ AssistantService với DeepSeek tùy chọn và fallback offline an toàn."), 0, 0);

        root.Controls.Add(BuildModeBar(), 0, 1);
        root.Controls.Add(BuildSuggestionBar(), 0, 2);
        root.Controls.Add(BuildInputBar(), 0, 3);

        _scrollOuter.Dock = DockStyle.Fill;
        _scrollOuter.AutoScroll = true;
        _scrollOuter.BackColor = AppTheme.Surface;
        _scrollOuter.Padding = new Padding(12);

        _conversationFlow.FlowDirection = FlowDirection.TopDown;
        _conversationFlow.WrapContents = false;
        _conversationFlow.AutoSize = true;
        _conversationFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _conversationFlow.Dock = DockStyle.Top;
        _conversationFlow.Padding = new Padding(4);
        _conversationFlow.Width = _scrollOuter.ClientSize.Width - 24;

        _scrollOuter.Controls.Add(_conversationFlow);
        _scrollOuter.Resize += (_, _) =>
        {
            _conversationFlow.Width = Math.Max(320, _scrollOuter.ClientSize.Width - 24);
        };

        root.Controls.Add(_scrollOuter, 0, 4);

        Controls.Add(root);

        _btnSend.Text = "Gửi";
        _btnClear.Text = "Xóa hội thoại";

        AcceptButton = _btnSend;
    }

    private Control BuildModeBar()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(12, 6, 12, 6)
        };

        _modeLabel.Dock = DockStyle.Fill;
        _modeLabel.Font = AppTheme.BodyFont(9.5F);
        _modeLabel.ForeColor = AppTheme.TextMuted;
        _modeLabel.TextAlign = ContentAlignment.MiddleLeft;
        _modeLabel.Text = "Chế độ: đang kiểm tra qua AssistantService.";

        panel.Controls.Add(_modeLabel);
        return panel;
    }

    private Control BuildSuggestionBar()
    {
        var wrap = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 4, 0, 0)
        };

        foreach (var label in SuggestedCommands)
        {
            var btn = new Button
            {
                Text = label,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 0, 8, 8),
                Padding = new Padding(12, 6, 12, 6),
                Font = AppTheme.BodyFont(),
                Cursor = Cursors.Hand
            };
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(217, 225, 234);
            btn.BackColor = AppTheme.Surface;
            btn.Click += (_, _) =>
            {
                _txtQuestion.Text = label;
                HandleSubmit();
            };
            wrap.Controls.Add(btn);
        }

        return wrap;
    }

    private Control BuildInputBar()
    {
        var panel = UiFactory.Card();
        panel.Padding = new Padding(12, 8, 12, 8);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132));

        _txtQuestion.Dock = DockStyle.Fill;
        _txtQuestion.Font = AppTheme.BodyFont();
        _txtQuestion.PlaceholderText = "Ví dụ: doanh thu hôm nay, hàng sắp hết...";

        _btnSend.Dock = DockStyle.Fill;
        _btnSend.Margin = new Padding(8, 0, 0, 0);
        _btnSend.Click += (_, _) => HandleSubmit();

        _btnClear.Dock = DockStyle.Fill;
        _btnClear.Margin = new Padding(8, 0, 0, 0);
        _btnClear.Click += (_, _) => ClearConversation();

        layout.Controls.Add(_txtQuestion, 0, 0);
        layout.Controls.Add(_btnSend, 1, 0);
        layout.Controls.Add(_btnClear, 2, 0);

        panel.Controls.Add(layout);
        return panel;
    }

    private static IReadOnlyList<string> SuggestedCommands { get; } =
    [
        "doanh thu hôm nay",
        "hàng sắp hết",
        "top sản phẩm bán chạy",
        "khách hàng mua nhiều nhất",
        "kiểm kê hôm nay"
    ];

    private void RefreshModeStatus()
    {
        var status = _assistantService.GetModeStatus();
        if (status.Success && status.Data is not null)
        {
            UpdateModeLabel(status.Data);
            return;
        }

        _modeLabel.Text = "Chế độ: chưa xác định.";
        _modeLabel.ForeColor = AppTheme.TextMuted;
    }

    private void ClearConversation()
    {
        _conversationFlow.Controls.Clear();
        RefreshModeStatus();
        AppendAssistantCard(
            "Đã xóa hội thoại",
            "Bạn có thể bắt đầu lượt hỏi mới. Gợi ý lệnh vẫn ở phía trên.",
            "Sẵn sàng");
        _scrollOuter.ScrollControlIntoView(_conversationFlow.Controls[^1]);
    }

    private void HandleSubmit()
    {
        var question = _txtQuestion.Text.Trim();
        if (string.IsNullOrEmpty(question))
        {
            AppendAssistantCard("Thiếu nội dung", "Vui lòng nhập câu lệnh hoặc chọn một gợi ý.", "Nhập liệu");
            return;
        }

        AppendUserMessage(question);
        _txtQuestion.Clear();

        var askResult = _assistantService.Ask(question);
        if (!askResult.Success || askResult.Data is null)
        {
            AppendAssistantCard("Không thể xử lý", askResult.Message, "BLL", relatedQuestion: question);
            ScrollToLatest();
            return;
        }

        var response = askResult.Data;
        UpdateModeLabel(response);
        AppendAssistantCard(
            response.Handled ? "Trợ lý AI" : "Trợ lý AI cần hỏi lại",
            response.Answer,
            BuildBadgeText(response),
            relatedQuestion: question);
        ScrollToLatest();
    }

    private void ScrollToLatest()
    {
        if (_conversationFlow.Controls.Count == 0)
        {
            return;
        }

        var last = _conversationFlow.Controls[^1];
        _scrollOuter.ScrollControlIntoView(last);
    }

    private void AppendUserMessage(string text)
    {
        var card = new Panel
        {
            Width = _conversationFlow.ClientSize.Width - 8,
            Margin = new Padding(0, 0, 0, 10),
            Padding = new Padding(12),
            BackColor = UserBubbleBg
        };

        var innerWidth = Math.Max(240, card.Width - card.Padding.Horizontal);
        var stack = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            Width = innerWidth,
            BackColor = UserBubbleBg,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        var header = new Label
        {
            Text = $"Bạn · {DateTime.Now:HH:mm}",
            AutoSize = true,
            MaximumSize = new Size(innerWidth, 0),
            Font = AppTheme.SectionFont(10F),
            ForeColor = AppTheme.TextMuted,
            BackColor = UserBubbleBg,
            Margin = Padding.Empty
        };

        var body = new Label
        {
            Text = text,
            AutoSize = true,
            MaximumSize = new Size(innerWidth, 0),
            Font = AppTheme.BodyFont(),
            ForeColor = Color.FromArgb(31, 41, 55),
            BackColor = UserBubbleBg,
            Margin = new Padding(0, 6, 0, 0),
            UseMnemonic = false
        };

        stack.Controls.Add(header);
        stack.Controls.Add(body);
        card.Controls.Add(stack);

        stack.PerformLayout();
        card.Height = stack.PreferredSize.Height + card.Padding.Vertical + 4;

        _conversationFlow.Controls.Add(card);
    }

    private void AppendAssistantCard(string title, string body, string badgeText, string? relatedQuestion = null)
    {
        var card = new Panel
        {
            Width = _conversationFlow.ClientSize.Width - 8,
            Margin = new Padding(0, 0, 0, 12),
            Padding = new Padding(14),
            BackColor = AppTheme.SurfaceMuted,
            BorderStyle = BorderStyle.FixedSingle
        };

        var titleRow = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 28,
            ColumnCount = 2,
            RowCount = 1
        };
        titleRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        titleRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));

        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = AppTheme.SectionFont(11F),
            ForeColor = PrimaryBlue,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var badge = new Label
        {
            Text = badgeText,
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(9F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleRight
        };

        titleRow.Controls.Add(titleLabel, 0, 0);
        titleRow.Controls.Add(badge, 1, 0);

        var flowWidth = Math.Max(200, card.Width - card.Padding.Horizontal);
        titleRow.Width = flowWidth;
        titleRow.Margin = Padding.Empty;

        var topSection = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            Width = flowWidth,
            BackColor = AppTheme.SurfaceMuted,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        topSection.Controls.Add(titleRow);

        if (!string.IsNullOrWhiteSpace(relatedQuestion))
        {
            var questionLine = new Label
            {
                Text = "Câu hỏi của bạn: " + relatedQuestion.Trim(),
                AutoSize = true,
                MaximumSize = new Size(flowWidth, 0),
                Font = AppTheme.BodyFont(9F),
                ForeColor = AppTheme.TextMuted,
                BackColor = AppTheme.SurfaceMuted,
                Margin = new Padding(0, 0, 0, 8),
                UseMnemonic = false
            };
            topSection.Controls.Add(questionLine);
        }

        var bodyBox = new TextBox
        {
            Text = body,
            ReadOnly = true,
            Multiline = true,
            BorderStyle = BorderStyle.None,
            BackColor = AppTheme.SurfaceMuted,
            Dock = DockStyle.Fill,
            Font = new Font(AppTheme.FontFamily, 10F, FontStyle.Regular),
            ForeColor = Color.FromArgb(31, 41, 55),
            WordWrap = true,
            TabStop = false
        };

        // Dock Fill trước, Top sau — phần đầu nằm trên, nội dung chiếm phần còn lại.
        card.Controls.Add(bodyBox);
        card.Controls.Add(topSection);

        var innerWidth = flowWidth - 8;
        topSection.PerformLayout();
        var topH = topSection.PreferredSize.Height;
        var bodyHeight = TextRenderer.MeasureText(body, bodyBox.Font,
            new Size(innerWidth, int.MaxValue),
            TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;
        card.Height = Math.Min(560, Math.Max(120, topH + bodyHeight + card.Padding.Vertical + 18));

        _conversationFlow.Controls.Add(card);
    }

    private void UpdateModeLabel(AssistantResponseDto response)
    {
        _modeLabel.Text = $"Chế độ: {BuildModeText(response.Mode)} · {response.StatusMessage}";
        _modeLabel.ForeColor = response.Mode switch
        {
            "ai-online" => OnlineGreen,
            "ai-failed-fallback" => FallbackAmber,
            _ => AppTheme.TextMuted
        };
    }

    private static string BuildModeText(string mode)
    {
        return mode switch
        {
            "ai-online" => "AI online",
            "ai-failed-fallback" => "AI failed, fallback used",
            "offline-rule-based" => "Offline rule-based",
            _ => "Không xác định"
        };
    }

    private static string BuildBadgeText(AssistantResponseDto response)
    {
        return response.Mode switch
        {
            "ai-online" => "AI online",
            "ai-failed-fallback" => "Fallback",
            "offline-rule-based" => "Offline",
            _ => response.IsFallback ? "Fallback" : "BLL"
        };
    }
}
