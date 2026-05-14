using System.Text.RegularExpressions;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Assistant;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Assistant;

public sealed class FrmAssistant : Form
{
    private readonly AssistantService _assistantService = new();
    private readonly TextBox _txtQuestion = new();
    private readonly Label _headerModeLabel = new();
    private readonly Label _headerModeDot = new();
    private readonly Label _modeTitleLabel = new();
    private readonly Label _modeStatusLabel = new();
    private readonly Label _modeDescriptionLabel = new();
    private readonly RoundedPanel _headerModePill = new();
    private readonly Panel _scrollOuter = new();
    private readonly FlowLayoutPanel _conversationFlow = new();
    private readonly FlowLayoutPanel _recentFlow = new();
    private readonly IconButton _btnSend = new();
    private readonly IconButton _btnSettings = new();
    private readonly ToolTip _toolTip = new();
    private readonly List<RecentQuestion> _recentQuestions = [];

    private static readonly Color PrimaryBlue = AppTheme.Primary;
    private static readonly Color AssistantAccent = Color.FromArgb(79, 70, 229);
    private static readonly Color AssistantSoft = Color.FromArgb(232, 236, 255);
    private static readonly Color UserBubbleBg = Color.FromArgb(232, 240, 254);
    private static readonly Color UserBubbleBorder = Color.FromArgb(191, 219, 254);
    private static readonly Color AssistantBubbleBg = AppTheme.Surface;
    private static readonly Color AssistantBubbleBorder = Color.FromArgb(215, 224, 238);
    private static readonly Color OnlineGreen = Color.FromArgb(34, 197, 94);
    private static readonly Color OfflineBlue = Color.FromArgb(59, 130, 246);
    private static readonly Color FallbackAmber = Color.FromArgb(245, 158, 11);

    public FrmAssistant()
    {
        Text = "Trợ lý AI";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(1120, 700);

        BuildLayout();

        Load += (_, _) =>
        {
            RefreshModeStatus();
            AppendWelcomeCard();
            AppendDateSeparator();
            UpdateRecentQuestions();
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
            RowCount = 2,
            Padding = AppTheme.PagePadding,
            BackColor = AppTheme.AppBackground
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 104));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(BuildTopHeader(), 0, 0);
        root.Controls.Add(BuildBody(), 0, 1);
        Controls.Add(root);

        AcceptButton = _btnSend;
    }

    private Control BuildTopHeader()
    {
        var header = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            FillColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Radius = 8,
            ShadowSize = 1,
            Padding = new Padding(18, 8, 14, 8),
            Margin = new Padding(0, 0, 0, 12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));

        layout.Controls.Add(UiFactory.IconTile(IconChar.Robot, PrimaryBlue, AppTheme.PrimarySoft, 46, 24), 0, 0);

        var titlePanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        var titleLabel = new Label
        {
            Text = "Trợ lý AI",
            Font = AppTheme.TitleFont(17F),
            ForeColor = PrimaryBlue,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            Margin = Padding.Empty
        };
        var subtitleLabel = new Label
        {
            Text = "Trợ lý AI cho quản lý kho bán hàng",
            Font = AppTheme.BodyFont(9.5F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            Margin = Padding.Empty
        };
        titlePanel.Controls.Add(titleLabel);
        titlePanel.Controls.Add(subtitleLabel);
        void ArrangeTitle()
        {
            var width = Math.Max(80, titlePanel.ClientSize.Width - 4);
            titleLabel.SetBounds(2, 8, width, 30);
            subtitleLabel.SetBounds(2, 42, width, 22);
        }

        titlePanel.Resize += (_, _) => ArrangeTitle();
        titlePanel.HandleCreated += (_, _) => ArrangeTitle();
        layout.Controls.Add(titlePanel, 1, 0);

        var right = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty
        };
        var modeCaption = new Label
        {
            Text = "Chế độ:",
            Font = AppTheme.BodyFont(9.5F),
            ForeColor = AppTheme.Text,
            TextAlign = ContentAlignment.MiddleRight,
            Padding = new Padding(0, 0, 8, 0)
        };
        var modePill = BuildModePill();

        _btnSettings.Text = "Cài đặt";
        _btnSettings.Dock = DockStyle.None;
        _btnSettings.Size = new Size(102, 40);
        _btnSettings.Margin = Padding.Empty;
        _btnSettings.FlatStyle = FlatStyle.Flat;
        _btnSettings.BackColor = AppTheme.Surface;
        _btnSettings.ForeColor = PrimaryBlue;
        _btnSettings.IconChar = IconChar.Gear;
        _btnSettings.IconColor = PrimaryBlue;
        _btnSettings.IconFont = IconFont.Auto;
        _btnSettings.IconSize = 15;
        _btnSettings.Font = AppTheme.BodyFont(9.5F);
        _btnSettings.TextImageRelation = TextImageRelation.ImageBeforeText;
        _btnSettings.ImageAlign = ContentAlignment.MiddleLeft;
        _btnSettings.TextAlign = ContentAlignment.MiddleCenter;
        _btnSettings.Padding = new Padding(6, 0, 6, 0);
        _btnSettings.UseVisualStyleBackColor = false;
        _btnSettings.FlatAppearance.BorderColor = Color.FromArgb(191, 212, 250);
        _btnSettings.FlatAppearance.MouseOverBackColor = AppTheme.PrimarySoft;
        _btnSettings.Click += (_, _) => ShowAssistantSettingsInfo();
        _toolTip.SetToolTip(_btnSettings, "Xem cách cấu hình chế độ Trợ lý AI.");
        right.Controls.Add(modeCaption);
        right.Controls.Add(modePill);
        right.Controls.Add(_btnSettings);
        void ArrangeRightHeader()
        {
            const int buttonWidth = 102;
            const int pillWidth = 240;
            const int controlHeight = 40;
            const int gap = 12;

            var y = Math.Max(0, (right.ClientSize.Height - controlHeight) / 2);
            var buttonX = Math.Max(0, right.ClientSize.Width - buttonWidth);
            var pillX = Math.Max(0, buttonX - gap - pillWidth);
            var captionX = Math.Max(0, pillX - 76);

            modeCaption.SetBounds(captionX, y, Math.Max(64, pillX - captionX - 4), controlHeight);
            modePill.SetBounds(pillX, y, Math.Min(pillWidth, Math.Max(160, buttonX - gap - pillX)), controlHeight);
            _btnSettings.SetBounds(buttonX, y, buttonWidth, controlHeight);
        }

        right.Resize += (_, _) => ArrangeRightHeader();
        right.HandleCreated += (_, _) => ArrangeRightHeader();

        layout.Controls.Add(right, 2, 0);
        header.Controls.Add(layout);
        return header;
    }

    private Control BuildModePill()
    {
        _headerModePill.Dock = DockStyle.None;
        _headerModePill.Size = new Size(240, 40);
        _headerModePill.FillColor = AppTheme.SurfaceSubtle;
        _headerModePill.BorderColor = AppTheme.BorderStrong;
        _headerModePill.Radius = 6;
        _headerModePill.ShadowSize = 0;
        _headerModePill.Padding = new Padding(12, 0, 10, 0);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 22));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _headerModeDot.Text = "●";
        _headerModeDot.Dock = DockStyle.Fill;
        _headerModeDot.Font = AppTheme.SectionFont(11F);
        _headerModeDot.ForeColor = OfflineBlue;
        _headerModeDot.TextAlign = ContentAlignment.MiddleCenter;

        _headerModeLabel.Text = "Đang kiểm tra";
        _headerModeLabel.Dock = DockStyle.Fill;
        _headerModeLabel.Font = AppTheme.BodyFont(9.5F);
        _headerModeLabel.ForeColor = AppTheme.Text;
        _headerModeLabel.TextAlign = ContentAlignment.MiddleLeft;
        _headerModeLabel.AutoEllipsis = true;

        layout.Controls.Add(_headerModeDot, 0, 0);
        layout.Controls.Add(_headerModeLabel, 1, 0);
        _headerModePill.Controls.Add(layout);
        return _headerModePill;
    }

    private Control BuildBody()
    {
        var body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 306));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        body.Controls.Add(BuildLeftColumn(), 0, 0);
        body.Controls.Add(BuildChatPanel(), 1, 0);
        return body;
    }

    private Control BuildLeftColumn()
    {
        var shell = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = AppTheme.AppBackground,
            Margin = new Padding(0, 0, 12, 0)
        };

        var stack = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = AppTheme.AppBackground,
            Padding = Padding.Empty
        };

        stack.Controls.Add(BuildTopicsCard());
        stack.Controls.Add(BuildRecentCard());
        stack.Controls.Add(BuildModeCard());
        shell.Controls.Add(stack);

        void ResizeCards()
        {
            var width = Math.Max(260, shell.ClientSize.Width - 6);
            stack.Width = width;
            foreach (Control control in stack.Controls)
            {
                control.Width = width;
            }
        }

        shell.Resize += (_, _) => ResizeCards();
        shell.HandleCreated += (_, _) => ResizeCards();
        return shell;
    }

    private Control BuildTopicsCard()
    {
        var card = CreateSideCard(410);
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 7
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        for (var i = 0; i < TopicSuggestions.Count; i++)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        }

        layout.Controls.Add(BuildSideSectionTitle(IconChar.Lightbulb, "Gợi ý câu hỏi theo chủ đề"), 0, 0);

        for (var i = 0; i < TopicSuggestions.Count; i++)
        {
            var topic = TopicSuggestions[i];
            layout.Controls.Add(BuildTopicButton(topic), 0, i + 1);
        }

        card.Controls.Add(layout);
        return card;
    }

    private Control BuildRecentCard()
    {
        var card = CreateSideCard(124);
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(BuildSideSectionTitle(IconChar.ClockRotateLeft, "Câu hỏi gần đây"), 0, 0);

        _recentFlow.Dock = DockStyle.Fill;
        _recentFlow.FlowDirection = FlowDirection.TopDown;
        _recentFlow.WrapContents = false;
        _recentFlow.AutoScroll = true;
        _recentFlow.BackColor = AppTheme.Surface;
        _recentFlow.Padding = new Padding(0, 2, 0, 0);
        layout.Controls.Add(_recentFlow, 0, 1);

        card.Controls.Add(layout);
        return card;
    }

    private Control BuildModeCard()
    {
        var card = CreateSideCard(156);
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

        layout.Controls.Add(BuildSideSectionTitle(IconChar.Robot, "Chế độ AI hiện tại"), 0, 0);

        _modeTitleLabel.Text = "Đang kiểm tra";
        _modeTitleLabel.Dock = DockStyle.Fill;
        _modeTitleLabel.Font = AppTheme.SectionFont(9.5F);
        _modeTitleLabel.ForeColor = AppTheme.Text;
        _modeTitleLabel.TextAlign = ContentAlignment.MiddleLeft;
        layout.Controls.Add(_modeTitleLabel, 0, 1);

        _modeStatusLabel.Text = string.Empty;
        _modeStatusLabel.Dock = DockStyle.Fill;
        _modeStatusLabel.Font = AppTheme.BodyFont(9F);
        _modeStatusLabel.ForeColor = AppTheme.TextMuted;
        _modeStatusLabel.TextAlign = ContentAlignment.TopLeft;
        _modeStatusLabel.UseMnemonic = false;
        _modeStatusLabel.Padding = new Padding(0, 4, 0, 4);
        layout.Controls.Add(_modeStatusLabel, 0, 2);

        _modeDescriptionLabel.Text = "Tìm hiểu thêm";
        _modeDescriptionLabel.Dock = DockStyle.Fill;
        _modeDescriptionLabel.Font = AppTheme.BodyFont(9F);
        _modeDescriptionLabel.ForeColor = PrimaryBlue;
        _modeDescriptionLabel.TextAlign = ContentAlignment.MiddleCenter;
        _modeDescriptionLabel.Cursor = Cursors.Hand;
        _modeDescriptionLabel.BorderStyle = BorderStyle.FixedSingle;
        _modeDescriptionLabel.Click += (_, _) => ShowAssistantSettingsInfo();
        _toolTip.SetToolTip(_modeDescriptionLabel, "Chế độ AI được xác định trong tầng BLL.");
        layout.Controls.Add(_modeDescriptionLabel, 0, 3);

        card.Controls.Add(layout);
        return card;
    }

    private Control BuildSideSectionTitle(IconChar icon, string title)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.Controls.Add(new IconPictureBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            IconChar = icon,
            IconColor = PrimaryBlue,
            IconFont = IconFont.Auto,
            IconSize = 16,
            Padding = new Padding(0, 4, 10, 8)
        }, 0, 0);
        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = AppTheme.SectionFont(10F),
            ForeColor = PrimaryBlue,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 1, 0);
        return layout;
    }

    private Control BuildTopicButton(TopicSuggestion topic)
    {
        var button = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            FillColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Radius = 6,
            ShadowSize = 0,
            Padding = new Padding(8, 7, 8, 7),
            Margin = new Padding(0, 0, 0, 8),
            Cursor = Cursors.Hand
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(UiFactory.IconTile(topic.Icon, topic.Accent, topic.Fill, 34, 17), 0, 0);

        var textStack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        textStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        textStack.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        textStack.Controls.Add(new Label
        {
            Text = topic.Title,
            Dock = DockStyle.Fill,
            Font = AppTheme.SectionFont(8.8F),
            ForeColor = topic.Accent,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            UseMnemonic = false,
            Margin = Padding.Empty
        }, 0, 0);
        textStack.Controls.Add(new Label
        {
            Text = topic.Description,
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(8.3F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            UseMnemonic = false,
            Margin = Padding.Empty
        }, 0, 1);

        layout.Controls.Add(textStack, 1, 0);
        button.Controls.Add(layout);
        WireRecursiveClick(button, () =>
        {
            if (string.IsNullOrWhiteSpace(topic.Question))
            {
                _txtQuestion.Focus();
                return;
            }

            SubmitSuggestedQuestion(topic.Question);
        });
        return button;
    }

    private Control BuildChatPanel()
    {
        var card = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            FillColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Radius = 8,
            ShadowSize = 1,
            Padding = new Padding(12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

        _scrollOuter.Dock = DockStyle.Fill;
        _scrollOuter.AutoScroll = true;
        _scrollOuter.BackColor = AppTheme.Surface;
        _scrollOuter.Padding = new Padding(4, 4, 4, 10);

        _conversationFlow.FlowDirection = FlowDirection.TopDown;
        _conversationFlow.WrapContents = false;
        _conversationFlow.AutoSize = true;
        _conversationFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _conversationFlow.Dock = DockStyle.Top;
        _conversationFlow.BackColor = AppTheme.Surface;
        _conversationFlow.Padding = Padding.Empty;
        _conversationFlow.Width = 640;

        _scrollOuter.Controls.Add(_conversationFlow);
        _scrollOuter.Resize += (_, _) => ResizeConversationRows();
        layout.Controls.Add(_scrollOuter, 0, 0);
        layout.Controls.Add(BuildQuickQuestionBar(), 0, 1);
        layout.Controls.Add(BuildInputBar(), 0, 2);
        layout.Controls.Add(new Label
        {
            Text = "Mẹo: Hỏi ngắn gọn, rõ ràng để nhận kết quả chính xác hơn.",
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(8.8F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(4, 0, 0, 0)
        }, 0, 3);

        card.Controls.Add(layout);
        return card;
    }

    private Control BuildQuickQuestionBar()
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = true,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 6, 0, 4)
        };

        foreach (var chip in QuickQuestions)
        {
            flow.Controls.Add(BuildQuickChip(chip));
        }

        return flow;
    }

    private Control BuildQuickChip(QuickQuestion chip)
    {
        var button = new IconButton
        {
            Text = chip.Text,
            Width = chip.Width,
            Height = 34,
            Margin = new Padding(0, 0, 8, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.SurfaceSubtle,
            ForeColor = AppTheme.Text,
            IconChar = chip.Icon,
            IconColor = chip.Accent,
            IconFont = IconFont.Auto,
            IconSize = 15,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            TextAlign = ContentAlignment.MiddleCenter,
            ImageAlign = ContentAlignment.MiddleLeft,
            Font = AppTheme.BodyFont(9F),
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        button.FlatAppearance.BorderColor = AppTheme.Border;
        button.FlatAppearance.MouseOverBackColor = AppTheme.PrimarySoft;
        button.Click += (_, _) => SubmitSuggestedQuestion(chip.Question);
        return button;
    }

    private Control BuildInputBar()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(0, 2, 0, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118));

        _txtQuestion.Dock = DockStyle.Fill;
        _txtQuestion.Height = 36;
        _txtQuestion.Font = AppTheme.BodyFont(10F);
        _txtQuestion.PlaceholderText = "Nhập câu hỏi của bạn tại đây...";
        _txtQuestion.BorderStyle = BorderStyle.FixedSingle;
        _txtQuestion.Margin = new Padding(0, 2, 10, 2);

        _btnSend.Text = "Gửi";
        _btnSend.Dock = DockStyle.Fill;
        _btnSend.Margin = new Padding(0, 2, 0, 2);
        _btnSend.FlatStyle = FlatStyle.Flat;
        _btnSend.BackColor = PrimaryBlue;
        _btnSend.ForeColor = Color.White;
        _btnSend.IconChar = IconChar.PaperPlane;
        _btnSend.IconColor = Color.White;
        _btnSend.IconFont = IconFont.Auto;
        _btnSend.IconSize = 16;
        _btnSend.Font = AppTheme.BodyFont(9.5F);
        _btnSend.TextImageRelation = TextImageRelation.ImageBeforeText;
        _btnSend.UseVisualStyleBackColor = false;
        _btnSend.FlatAppearance.BorderSize = 0;
        _btnSend.FlatAppearance.MouseOverBackColor = Color.FromArgb(29, 78, 216);
        _btnSend.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 64, 175);
        _btnSend.Click += (_, _) => HandleSubmit();

        layout.Controls.Add(_txtQuestion, 0, 0);
        layout.Controls.Add(_btnSend, 1, 0);
        return layout;
    }

    private void RefreshModeStatus()
    {
        var status = _assistantService.GetModeStatus();
        if (status.Success && status.Data is not null)
        {
            UpdateModeLabels(status.Data);
            return;
        }

        _headerModeLabel.Text = "Chưa xác định";
        _modeTitleLabel.Text = "Chưa xác định";
        _modeStatusLabel.Text = "Chưa lấy được trạng thái từ AssistantService.";
    }

    private void HandleSubmit()
    {
        var question = _txtQuestion.Text.Trim();
        if (string.IsNullOrEmpty(question))
        {
            AppendAssistantNotice("Thiếu nội dung", "Vui lòng nhập câu hỏi hoặc chọn một gợi ý.");
            ScrollToLatest();
            return;
        }

        AppendUserMessage(question);
        AddRecentQuestion(question);
        _txtQuestion.Clear();

        var askResult = _assistantService.Ask(question);
        if (!askResult.Success || askResult.Data is null)
        {
            AppendAssistantNotice("Không thể xử lý", askResult.Message);
            ScrollToLatest();
            return;
        }

        var response = askResult.Data;
        UpdateModeLabels(response);
        AppendAssistantResponse(response);
        ScrollToLatest();
    }

    private void SubmitSuggestedQuestion(string question)
    {
        _txtQuestion.Text = question;
        HandleSubmit();
    }

    private void AddRecentQuestion(string question)
    {
        _recentQuestions.RemoveAll(x => x.Text.Equals(question, StringComparison.OrdinalIgnoreCase));
        _recentQuestions.Insert(0, new RecentQuestion(question, DateTime.Now));
        if (_recentQuestions.Count > 5)
        {
            _recentQuestions.RemoveRange(5, _recentQuestions.Count - 5);
        }

        UpdateRecentQuestions();
    }

    private void UpdateRecentQuestions()
    {
        _recentFlow.Controls.Clear();

        if (_recentQuestions.Count == 0)
        {
            _recentFlow.Controls.Add(new Label
            {
                Text = "Các câu hỏi bạn vừa gửi sẽ xuất hiện tại đây.",
                Width = Math.Max(220, _recentFlow.ClientSize.Width - 6),
                Height = 44,
                Font = AppTheme.BodyFont(9F),
                ForeColor = AppTheme.TextMuted,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(2, 0, 0, 0)
            });
            return;
        }

        foreach (var item in _recentQuestions)
        {
            var row = new TableLayoutPanel
            {
                Width = Math.Max(220, _recentFlow.ClientSize.Width - 6),
                Height = 30,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 4),
                Cursor = Cursors.Hand,
                BackColor = AppTheme.Surface
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 22));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46));

            row.Controls.Add(new IconPictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                IconChar = IconChar.ClockRotateLeft,
                IconColor = AppTheme.TextMuted,
                IconFont = IconFont.Auto,
                IconSize = 12,
                Padding = new Padding(0, 7, 6, 7)
            }, 0, 0);
            row.Controls.Add(new Label
            {
                Text = item.Text,
                Dock = DockStyle.Fill,
                Font = AppTheme.BodyFont(8.5F),
                ForeColor = AppTheme.TextMuted,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                UseMnemonic = false
            }, 1, 0);
            row.Controls.Add(new Label
            {
                Text = item.CreatedAt.ToString("HH:mm"),
                Dock = DockStyle.Fill,
                Font = AppTheme.BodyFont(8.2F),
                ForeColor = AppTheme.TextMuted,
                TextAlign = ContentAlignment.MiddleRight
            }, 2, 0);

            WireRecursiveClick(row, () =>
            {
                _txtQuestion.Text = item.Text;
                _txtQuestion.Focus();
                _txtQuestion.SelectionStart = _txtQuestion.TextLength;
            });

            _recentFlow.Controls.Add(row);
        }
    }

    private void AppendWelcomeCard()
    {
        var row = new Panel
        {
            Width = GetConversationWidth(),
            Height = 102,
            Margin = new Padding(0, 0, 0, 12),
            BackColor = AppTheme.Surface,
            Tag = ConversationRowKind.FullWidth
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(8, 8, 8, 8)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 82));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.Controls.Add(UiFactory.IconTile(IconChar.Robot, PrimaryBlue, AppTheme.PrimarySoft, 64, 32), 0, 0);

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
            Text = "Xin chào! Tôi là Trợ lý AI của QuanLyKhoBanHang",
            Dock = DockStyle.Fill,
            Font = AppTheme.SectionFont(11F),
            ForeColor = AppTheme.Text,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            UseMnemonic = false
        }, 0, 0);
        textStack.Controls.Add(new Label
        {
            Text = "Tôi có thể giúp bạn tra cứu dữ liệu bán hàng, tồn kho, khách hàng, nhập hàng và báo cáo. Hãy chọn một chủ đề bên trái hoặc nhập câu hỏi của bạn bên dưới.",
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(9.5F),
            ForeColor = AppTheme.Text,
            TextAlign = ContentAlignment.TopLeft,
            UseMnemonic = false
        }, 0, 1);
        layout.Controls.Add(textStack, 1, 0);
        row.Controls.Add(layout);
        _conversationFlow.Controls.Add(row);
    }

    private void AppendDateSeparator()
    {
        var row = new Panel
        {
            Width = GetConversationWidth(),
            Height = 32,
            BackColor = AppTheme.Surface,
            Margin = new Padding(0, 0, 0, 12),
            Tag = ConversationRowKind.FullWidth
        };

        var lineLeft = new Panel
        {
            Height = 1,
            BackColor = AppTheme.Border,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
        };
        var lineRight = new Panel
        {
            Height = 1,
            BackColor = AppTheme.Border,
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
        };
        var label = new Label
        {
            Text = $"Hôm nay - {DateTime.Today:dd/MM/yyyy}",
            AutoSize = false,
            Width = 190,
            Height = 24,
            Font = AppTheme.BodyFont(9F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.Surface
        };

        void ArrangeSeparator()
        {
            label.Location = new Point(Math.Max(0, (row.ClientSize.Width - label.Width) / 2), 4);
            lineLeft.Location = new Point(8, 16);
            lineLeft.Width = Math.Max(8, label.Left - 18);
            lineRight.Location = new Point(label.Right + 10, 16);
            lineRight.Width = Math.Max(8, row.ClientSize.Width - lineRight.Left - 8);
        }

        row.Controls.Add(lineLeft);
        row.Controls.Add(lineRight);
        row.Controls.Add(label);
        row.Resize += (_, _) => ArrangeSeparator();
        ArrangeSeparator();

        _conversationFlow.Controls.Add(row);
    }

    private void AppendUserMessage(string text)
    {
        var rowWidth = GetConversationWidth();
        var bubbleWidth = Math.Min(380, Math.Max(260, rowWidth - 96));
        var bubble = BuildBubble(
            bubbleWidth,
            UserBubbleBg,
            UserBubbleBorder,
            "Bạn",
            text,
            DateTime.Now,
            isUser: true,
            table: null);
        var row = BuildMessageRow(rowWidth, bubble, CreateAvatar(IconChar.CircleUser, PrimaryBlue, AppTheme.PrimarySoft), alignRight: true);
        _conversationFlow.Controls.Add(row);
    }

    private void AppendAssistantNotice(string title, string body)
    {
        var rowWidth = GetConversationWidth();
        var bubbleWidth = Math.Min(620, Math.Max(300, rowWidth - 92));
        var bubble = BuildBubble(
            bubbleWidth,
            AssistantBubbleBg,
            AssistantBubbleBorder,
            title,
            body,
            DateTime.Now,
            isUser: false,
            table: null);
        var row = BuildMessageRow(rowWidth, bubble, CreateAvatar(IconChar.Robot, PrimaryBlue, AppTheme.PrimarySoft), alignRight: false);
        _conversationFlow.Controls.Add(row);
    }

    private void AppendAssistantResponse(AssistantResponseDto response)
    {
        var rowWidth = GetConversationWidth();
        var bubbleWidth = Math.Min(680, Math.Max(330, rowWidth - 92));
        var table = TryBuildResultTable(response, out var parsedTable) ? parsedTable : null;
        var body = table is null ? response.Answer : BuildNarrativeText(response.Answer);
        var title = response.Handled ? "Trợ lý AI" : "Trợ lý AI cần hỏi lại";
        var bubble = BuildBubble(
            bubbleWidth,
            AssistantBubbleBg,
            AssistantBubbleBorder,
            title,
            body,
            response.CreatedAt,
            isUser: false,
            table: table);
        var row = BuildMessageRow(rowWidth, bubble, CreateAvatar(IconChar.Robot, PrimaryBlue, AppTheme.PrimarySoft), alignRight: false);
        _conversationFlow.Controls.Add(row);
    }

    private RoundedPanel BuildBubble(
        int width,
        Color fill,
        Color border,
        string title,
        string body,
        DateTime createdAt,
        bool isUser,
        ResultTable? table)
    {
        var bubble = new RoundedPanel
        {
            Width = width,
            FillColor = fill,
            BorderColor = border,
            Radius = 8,
            ShadowSize = 0,
            Padding = new Padding(14, 10, 14, 10),
            Margin = Padding.Empty
        };

        var contentWidth = Math.Max(220, width - bubble.Padding.Horizontal - 2);
        var stack = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Width = contentWidth,
            BackColor = fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        if (!isUser)
        {
            stack.Controls.Add(new Label
            {
                Text = title,
                Width = contentWidth,
                Height = 24,
                Font = AppTheme.SectionFont(10F),
                ForeColor = PrimaryBlue,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            });
        }

        var bodyLabel = new Label
        {
            Text = body,
            AutoSize = true,
            MaximumSize = new Size(contentWidth, 0),
            Font = AppTheme.BodyFont(9.5F),
            ForeColor = AppTheme.Text,
            BackColor = fill,
            Margin = new Padding(0, isUser ? 0 : 4, 0, table is null ? 6 : 10),
            UseMnemonic = false
        };
        stack.Controls.Add(bodyLabel);

        if (table is not null)
        {
            stack.Controls.Add(BuildResultGrid(table, contentWidth));
        }

        stack.Controls.Add(new Label
        {
            Text = createdAt.ToString("HH:mm"),
            Width = contentWidth,
            Height = 18,
            Font = AppTheme.BodyFont(8.3F),
            ForeColor = AppTheme.TextMuted,
            BackColor = fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 2, 0, 0)
        });

        bubble.Controls.Add(stack);
        stack.PerformLayout();
        bubble.Height = stack.PreferredSize.Height + bubble.Padding.Vertical + 4;
        return bubble;
    }

    private Control BuildResultGrid(ResultTable table, int width)
    {
        var grid = new DataGridView
        {
            Width = width,
            Height = Math.Min(190, 33 + (table.Rows.Count * 29)),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = AppTheme.Surface,
            BorderStyle = BorderStyle.FixedSingle,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ScrollBars = ScrollBars.None,
            Margin = new Padding(0, 0, 0, 4)
        };

        foreach (var header in table.Headers)
        {
            grid.Columns.Add(header, header);
        }

        foreach (var row in table.Rows)
        {
            grid.Rows.Add(row);
        }

        UiFactory.StyleGrid(grid);
        grid.ColumnHeadersHeight = 30;
        grid.RowTemplate.Height = 28;
        grid.DefaultCellStyle.Font = AppTheme.BodyFont(8.8F);
        grid.ColumnHeadersDefaultCellStyle.Font = AppTheme.SectionFont(8.8F);
        foreach (DataGridViewRow row in grid.Rows)
        {
            row.Height = 28;
        }

        grid.ClearSelection();
        return grid;
    }

    private Control BuildMessageRow(int width, RoundedPanel bubble, Control avatar, bool alignRight)
    {
        var row = new TableLayoutPanel
        {
            Width = width,
            Height = bubble.Height + 8,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Margin = new Padding(0, 0, 0, 12),
            Tag = ConversationRowKind.Message
        };
        row.RowStyles.Add(new RowStyle(SizeType.Absolute, bubble.Height + 8));

        if (alignRight)
        {
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, bubble.Width + 10));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
            bubble.Margin = new Padding(0, 0, 10, 0);
            avatar.Margin = new Padding(0, 8, 0, 0);
            row.Controls.Add(bubble, 1, 0);
            row.Controls.Add(avatar, 2, 0);
        }
        else
        {
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, bubble.Width + 10));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            avatar.Margin = new Padding(0, 8, 10, 0);
            row.Controls.Add(avatar, 0, 0);
            row.Controls.Add(bubble, 1, 0);
        }

        return row;
    }

    private Control CreateAvatar(IconChar icon, Color accent, Color fill)
    {
        var avatar = new RoundedPanel
        {
            Width = 34,
            Height = 34,
            FillColor = fill,
            BorderColor = fill,
            Radius = 17,
            ShadowSize = 0,
            Padding = Padding.Empty
        };

        var iconBox = new IconPictureBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            IconChar = icon,
            IconColor = accent,
            IconFont = IconFont.Auto,
            IconSize = 18,
            SizeMode = PictureBoxSizeMode.CenterImage
        };
        avatar.Controls.Add(iconBox);
        return avatar;
    }

    private void ResizeConversationRows()
    {
        var width = GetConversationWidth();
        _conversationFlow.Width = width;
        foreach (Control control in _conversationFlow.Controls)
        {
            if (control.Tag is ConversationRowKind.FullWidth or ConversationRowKind.Message)
            {
                control.Width = width;
            }
        }
    }

    private int GetConversationWidth()
    {
        var width = _scrollOuter.ClientSize.Width - _scrollOuter.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth;
        return Math.Max(420, width);
    }

    private void ScrollToLatest()
    {
        if (_conversationFlow.Controls.Count == 0)
        {
            return;
        }

        ResizeConversationRows();
        _scrollOuter.ScrollControlIntoView(_conversationFlow.Controls[^1]);
    }

    private void UpdateModeLabels(AssistantResponseDto response)
    {
        var color = response.Mode switch
        {
            "ai-online" => OnlineGreen,
            "ai-failed-fallback" => FallbackAmber,
            _ => OfflineBlue
        };

        _headerModeDot.ForeColor = color;
        _headerModeLabel.Text = BuildModeShortText(response.Mode);
        _headerModePill.BorderColor = Color.FromArgb(191, 212, 250);
        _headerModePill.FillColor = response.Mode == "ai-failed-fallback"
            ? Color.FromArgb(255, 251, 235)
            : AppTheme.SurfaceSubtle;
        _headerModePill.Invalidate();

        _modeTitleLabel.Text = BuildModeShortText(response.Mode);
        _modeTitleLabel.ForeColor = response.Mode == "ai-failed-fallback" ? Color.FromArgb(146, 64, 14) : AppTheme.Text;
        _modeStatusLabel.Text = response.StatusMessage;
    }

    private static string BuildModeShortText(string mode)
    {
        return mode switch
        {
            "ai-online" => "AI API (đã cấu hình)",
            "ai-failed-fallback" => "AI lỗi, dùng offline",
            "offline-rule-based" => "Rule-based offline",
            _ => "Không xác định"
        };
    }

    private void ShowAssistantSettingsInfo()
    {
        MessageBox.Show(
            "Chế độ Trợ lý AI được lấy từ AssistantService.\n\n" +
            "- Nếu có biến môi trường DEEPSEEK_API_KEY, hệ thống thử dùng AI API.\n" +
            "- Nếu thiếu khóa, mạng lỗi hoặc API lỗi, hệ thống tự dùng rule-based offline.\n" +
            "- Giao diện không đọc hoặc hiển thị API key.",
            "Cài đặt Trợ lý AI",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private static RoundedPanel CreateSideCard(int height) => new()
    {
        Height = height,
        FillColor = AppTheme.Surface,
        BorderColor = AppTheme.Border,
        Radius = 8,
        ShadowSize = 1,
        Padding = new Padding(14),
        Margin = new Padding(0, 0, 0, 12)
    };

    private static void WireRecursiveClick(Control root, Action action)
    {
        root.Click += (_, _) => action();
        foreach (Control child in root.Controls)
        {
            WireRecursiveClick(child, action);
            child.Cursor = Cursors.Hand;
        }
    }

    private static bool TryBuildResultTable(AssistantResponseDto response, out ResultTable? table)
    {
        table = null;
        if (!response.Handled || string.IsNullOrWhiteSpace(response.Answer))
        {
            return false;
        }

        return response.Intent switch
        {
            "top-products" => TryBuildTopProductsTable(response.Answer, out table),
            "top-customers" => TryBuildTopCustomersTable(response.Answer, out table),
            "low-stock" => TryBuildLowStockTable(response.Answer, out table),
            _ => false
        };
    }

    private static bool TryBuildTopProductsTable(string answer, out ResultTable? table)
    {
        table = null;
        var details = GetDetailsAfterColon(answer);
        var rows = new List<string[]>();
        foreach (var item in details.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var match = Regex.Match(item, @"^(?<rank>\d+)\.\s*(?<name>.+?)\s*\((?<qty>.+?)\s+bán ra,\s*(?<revenue>.+?)\s*đ\)$");
            if (!match.Success)
            {
                continue;
            }

            rows.Add([
                match.Groups["rank"].Value,
                match.Groups["name"].Value,
                match.Groups["qty"].Value,
                match.Groups["revenue"].Value + " đ"
            ]);
        }

        if (rows.Count == 0)
        {
            return false;
        }

        table = new ResultTable(["#", "Sản phẩm", "Số lượng bán", "Doanh thu"], rows);
        return true;
    }

    private static bool TryBuildTopCustomersTable(string answer, out ResultTable? table)
    {
        table = null;
        var details = GetDetailsAfterColon(answer);
        var rows = new List<string[]>();
        foreach (var item in details.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var match = Regex.Match(item, @"^(?<rank>\d+)\.\s*(?<name>.+?)\s*\((?<invoice>.+?)\s+hóa đơn,\s*(?<total>.+?)\s*đ\)$");
            if (!match.Success)
            {
                continue;
            }

            rows.Add([
                match.Groups["rank"].Value,
                match.Groups["name"].Value,
                match.Groups["invoice"].Value,
                match.Groups["total"].Value + " đ"
            ]);
        }

        if (rows.Count == 0)
        {
            return false;
        }

        table = new ResultTable(["#", "Khách hàng", "Hóa đơn", "Tổng tiền"], rows);
        return true;
    }

    private static bool TryBuildLowStockTable(string answer, out ResultTable? table)
    {
        table = null;
        var details = GetDetailsAfterColon(answer);
        var rows = new List<string[]>();
        foreach (var item in details.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var match = Regex.Match(item, @"^(?<code>.+?)\s+-\s+(?<name>.+?):\s+(?<qty>.+?)/(?<min>.+?)\s+(?<unit>.+)$");
            if (!match.Success)
            {
                continue;
            }

            rows.Add([
                match.Groups["code"].Value,
                match.Groups["name"].Value,
                match.Groups["qty"].Value + " " + match.Groups["unit"].Value,
                match.Groups["min"].Value + " " + match.Groups["unit"].Value
            ]);
        }

        if (rows.Count == 0)
        {
            return false;
        }

        table = new ResultTable(["Mã", "Sản phẩm", "Tồn", "Tối thiểu"], rows);
        return true;
    }

    private static string BuildNarrativeText(string answer)
    {
        var index = answer.IndexOf(':', StringComparison.Ordinal);
        if (index < 0)
        {
            return answer;
        }

        return answer[..(index + 1)];
    }

    private static string GetDetailsAfterColon(string answer)
    {
        var index = answer.IndexOf(':', StringComparison.Ordinal);
        return index < 0 ? answer : answer[(index + 1)..].Trim().TrimEnd('.');
    }

    private static IReadOnlyList<TopicSuggestion> TopicSuggestions { get; } =
    [
        new("Doanh thu & Lợi nhuận", "Doanh thu hôm nay, lợi nhuận ước tính.", "doanh thu hôm nay", IconChar.ChartLine, AppTheme.Success, AppTheme.SuccessSoft),
        new("Sản phẩm & Hàng tồn", "Sản phẩm bán chạy, sắp hết hàng.", "hàng sắp hết", IconChar.BoxOpen, AppTheme.Warning, AppTheme.WarningSoft),
        new("Nhập hàng & Nhà cung cấp", "Mặt hàng cần nhập và tình trạng kho.", "sản phẩm nào sắp hết hàng?", IconChar.TruckRampBox, PrimaryBlue, AppTheme.PrimarySoft),
        new("Khách hàng & Công nợ", "Khách hàng mua nhiều nhất.", "khách hàng mua nhiều nhất", IconChar.Users, AssistantAccent, AssistantSoft),
        new("Báo cáo & Thống kê", "Báo cáo bán hàng và kiểm kê.", "kiểm kê hôm nay", IconChar.ChartBar, PrimaryBlue, AppTheme.PrimarySoft),
        new("+ Câu hỏi khác", "Nhập câu hỏi riêng của bạn.", string.Empty, IconChar.Plus, PrimaryBlue, AppTheme.SurfaceSubtle)
    ];

    private static IReadOnlyList<QuickQuestion> QuickQuestions { get; } =
    [
        new("Doanh thu hôm nay", "doanh thu hôm nay", IconChar.ChartLine, AppTheme.Success, 156),
        new("Top sản phẩm bán chạy", "top sản phẩm bán chạy", IconChar.ChartBar, AppTheme.Warning, 188),
        new("Sản phẩm sắp hết", "hàng sắp hết", IconChar.TriangleExclamation, AppTheme.Danger, 158),
        new("Khách hàng mua nhiều", "khách hàng mua nhiều nhất", IconChar.Users, AssistantAccent, 178),
        new("Kiểm kê hôm nay", "kiểm kê hôm nay", IconChar.ClipboardCheck, PrimaryBlue, 154)
    ];

    private sealed record TopicSuggestion(
        string Title,
        string Description,
        string Question,
        IconChar Icon,
        Color Accent,
        Color Fill);

    private sealed record QuickQuestion(
        string Text,
        string Question,
        IconChar Icon,
        Color Accent,
        int Width);

    private sealed record RecentQuestion(string Text, DateTime CreatedAt);

    private sealed record ResultTable(string[] Headers, List<string[]> Rows);

    private enum ConversationRowKind
    {
        FullWidth,
        Message
    }
}
