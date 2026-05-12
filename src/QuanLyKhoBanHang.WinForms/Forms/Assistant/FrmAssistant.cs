using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Assistant;
using QuanLyKhoBanHang.DTO.Inventory;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.DTO.Reports;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Assistant;

public sealed class FrmAssistant : Form
{
    private readonly AssistantService _assistantService = new();
    private readonly ReportService _reportService = new();
    private readonly InventoryService _inventoryService = new();
    private readonly StocktakeService _stocktakeService = new();

    private readonly TextBox _txtQuestion = new();
    private readonly Panel _scrollOuter = new();
    private readonly FlowLayoutPanel _conversationFlow = new();
    private readonly Button _btnSend = new();
    private readonly Button _btnClear = new();

    private static readonly Color PrimaryBlue = Color.FromArgb(37, 99, 235);
    private static readonly Color UserBubbleBg = Color.FromArgb(236, 242, 254);

    public FrmAssistant()
    {
        Text = "Trợ lý quản lý";
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();
        MinimumSize = new Size(960, 620);
        Padding = AppTheme.PagePadding;

        BuildLayout();
        Load += (_, _) => AppendAssistantCard(
            "Trợ lý quản lý",
            "Chào bạn! Đây là chế độ demo nội bộ (không gọi AI).\nChọn gợi ý bên dưới hoặc nhập lệnh tiếng Việt, sau đó bấm Gửi.",
            false);

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
            RowCount = 4
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(UiFactory.HeaderPanel(
            "Trợ lý quản lý",
            "Gõ lệnh nhanh như chat nội bộ — phản hồi xác định từ dịch vụ BLL + dữ liệu minh họa khi chưa có bản ghi."), 0, 0);

        root.Controls.Add(BuildSuggestionBar(), 0, 1);
        root.Controls.Add(BuildInputBar(), 0, 2);

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

        root.Controls.Add(_scrollOuter, 0, 3);

        Controls.Add(root);

        _btnSend.Text = "Gửi";
        _btnClear.Text = "Xóa hội thoại";

        AcceptButton = _btnSend;
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
        _txtQuestion.PlaceholderText = "Ví dụ: doanh thu hôm nay, hàng sắp hết…";

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

    private void ClearConversation()
    {
        _conversationFlow.Controls.Clear();
        AppendAssistantCard(
            "Đã xóa hội thoại",
            "Bạn có thể bắt đầu lượt hỏi mới. Gợi ý lệnh vẫn ở phía trên.",
            false);
        _scrollOuter.ScrollControlIntoView(_conversationFlow.Controls[^1]);
    }

    private void HandleSubmit()
    {
        var question = _txtQuestion.Text.Trim();
        if (string.IsNullOrEmpty(question))
        {
            AppendAssistantCard("Thiếu nội dung", "Vui lòng nhập câu lệnh hoặc chọn một gợi ý.", false);
            return;
        }

        AppendUserMessage(question);
        _txtQuestion.Clear();

        var askResult = _assistantService.Ask(question);
        if (!askResult.Success)
        {
            AppendAssistantCard("Không thể xử lý", askResult.Message, false);
            ScrollToLatest();
            return;
        }

        var intent = ResolveIntent(askResult, question);
        if (intent == AssistantIntent.Unknown)
        {
            var fallback = askResult.Data?.Answer ?? askResult.Message;
            AppendAssistantCard("Trợ lý", fallback, false);
            ScrollToLatest();
            return;
        }

        var response = BuildIntentResponse(intent);
        AppendAssistantCard(response.Title, response.Body, response.IsDemoFallback);
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

        var header = new Label
        {
            Text = $"Bạn · {DateTime.Now:HH:mm}",
            Dock = DockStyle.Top,
            Height = 20,
            Font = AppTheme.SectionFont(10F),
            ForeColor = AppTheme.TextMuted
        };

        var body = new TextBox
        {
            Text = text,
            ReadOnly = true,
            Multiline = true,
            BorderStyle = BorderStyle.None,
            BackColor = UserBubbleBg,
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(),
            ForeColor = Color.FromArgb(31, 41, 55),
            WordWrap = true,
            TabStop = false
        };

        card.Controls.Add(header);
        card.Controls.Add(body);

        var estimated = TextRenderer.MeasureText(text, body.Font,
            new Size(card.Width - card.Padding.Horizontal - 8, int.MaxValue),
            TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height + card.Padding.Vertical + header.Height + 12;
        card.Height = Math.Max(estimated, 72);

        _conversationFlow.Controls.Add(card);
    }

    private void AppendAssistantCard(string title, string body, bool isDemoFallback)
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
        titleRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));

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
            Text = isDemoFallback ? "Demo stub" : "BLL",
            Dock = DockStyle.Fill,
            Font = AppTheme.BodyFont(9F),
            ForeColor = AppTheme.TextMuted,
            TextAlign = ContentAlignment.MiddleRight
        };

        titleRow.Controls.Add(titleLabel, 0, 0);
        titleRow.Controls.Add(badge, 1, 0);

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

        card.Controls.Add(bodyBox);
        card.Controls.Add(titleRow);

        var bodyHeight = TextRenderer.MeasureText(body, bodyBox.Font,
            new Size(card.Width - card.Padding.Horizontal - 8, int.MaxValue),
            TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl).Height;
        card.Height = Math.Min(520, Math.Max(120, bodyHeight + card.Padding.Vertical + titleRow.Height + 18));

        _conversationFlow.Controls.Add(card);
    }

    private static AssistantIntent ResolveIntent(ServiceResult<AssistantResponseDto> ask, string raw)
    {
        var dto = ask.Data;
        if (dto is { Intent: not null })
        {
            var mapped = MapServiceIntent(dto.Intent);
            if (mapped != AssistantIntent.Unknown)
            {
                return mapped;
            }
        }

        return ParseLocalIntent(raw);
    }

    private static AssistantIntent MapServiceIntent(string intent) =>
        intent switch
        {
            "revenue" => AssistantIntent.RevenueToday,
            "low-stock" => AssistantIntent.LowStock,
            "top-products" => AssistantIntent.TopProducts,
            _ => AssistantIntent.Unknown
        };

    private static AssistantIntent ParseLocalIntent(string raw)
    {
        var n = raw.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(n))
        {
            return AssistantIntent.Unknown;
        }

        if (n.Contains("kiểm kê", StringComparison.Ordinal) || n.Contains("kiem ke", StringComparison.Ordinal))
        {
            return AssistantIntent.StocktakeToday;
        }

        if ((n.Contains("khách", StringComparison.Ordinal) || n.Contains("khach", StringComparison.Ordinal))
            && (n.Contains("mua", StringComparison.Ordinal) || n.Contains("nhiều", StringComparison.Ordinal)))
        {
            return AssistantIntent.TopCustomers;
        }

        if (n.Contains("doanh thu", StringComparison.Ordinal))
        {
            return AssistantIntent.RevenueToday;
        }

        if (n.Contains("sắp hết", StringComparison.Ordinal) || n.Contains("sap het", StringComparison.Ordinal)
                                                         || n.Contains("tồn thấp", StringComparison.Ordinal)
                                                         || n.Contains("ton thap", StringComparison.Ordinal))
        {
            return AssistantIntent.LowStock;
        }

        if ((n.Contains("top", StringComparison.Ordinal) || n.Contains("bán chạy", StringComparison.Ordinal))
            && (n.Contains("sản phẩm", StringComparison.Ordinal) || n.Contains("san pham", StringComparison.Ordinal)
                                                                  || n.Contains("bán chạy", StringComparison.Ordinal)))
        {
            return AssistantIntent.TopProducts;
        }

        return AssistantIntent.Unknown;
    }

    private (string Title, string Body, bool IsDemoFallback) BuildIntentResponse(AssistantIntent intent)
    {
        return intent switch
        {
            AssistantIntent.RevenueToday => BuildRevenueToday(),
            AssistantIntent.LowStock => BuildLowStock(),
            AssistantIntent.TopProducts => BuildTopProducts(),
            AssistantIntent.TopCustomers => BuildTopCustomers(),
            AssistantIntent.StocktakeToday => BuildStocktakeToday(),
            _ => ("Trợ lý quản lý", "Không xử lý được yêu cầu này trong demo. Hãy dùng các lệnh gợi ý để xem báo cáo, tồn kho, kiểm kê và top sản phẩm.", false)
        };
    }

    private (string Title, string Body, bool IsDemoFallback) BuildRevenueToday()
    {
        var today = DateTime.Today;
        var result = _reportService.GetRevenue(today, today);
        var rows = result.Success && result.Data is { Count: > 0 } data ? data : CreateStubRevenue(today, today);
        var demo = !(result.Success && result.Data is { Count: > 0 });

        var lines = new List<string>();
        if (demo)
        {
            lines.Add("(Demo) Backend hiện trả danh sách rỗng — hiển thị minh họa để demo không bị trống.");
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                lines.Add($"Ghi chú BLL: {result.Message}");
            }
        }

        foreach (var row in rows)
        {
            lines.Add(
                $"{row.Date:dd/MM/yyyy} · HĐ: {row.InvoiceCount:N0} · Doanh thu: {row.Revenue:N0} đ · LN ước tính: {row.EstimatedProfit:N0} đ");
        }

        return ("Doanh thu hôm nay", string.Join(Environment.NewLine, lines), demo);
    }

    private (string Title, string Body, bool IsDemoFallback) BuildLowStock()
    {
        var result = _inventoryService.GetLowStockProducts();
        var rows = result.Success && result.Data is { Count: > 0 } data ? data : CreateStubLowStock();
        var demo = !(result.Success && result.Data is { Count: > 0 });

        var lines = new List<string>();
        if (demo)
        {
            lines.Add("(Demo) Chưa có bản ghi tồn thấp từ BLL — hiển thị ví dụ.");
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                lines.Add($"Ghi chú BLL: {result.Message}");
            }
        }

        foreach (var p in rows)
        {
            lines.Add($"{p.Code} · {p.Name} · SL: {p.QuantityOnHand:N0} {p.Unit} · Tối thiểu: {p.MinStockLevel:N0}");
        }

        return ("Hàng sắp hết / tồn thấp", string.Join(Environment.NewLine, lines), demo);
    }

    private (string Title, string Body, bool IsDemoFallback) BuildTopProducts()
    {
        var to = DateTime.Today;
        var from = to.AddDays(-29);
        var result = _reportService.GetTopSellingProducts(from, to, 5);
        var rows = result.Success && result.Data is { Count: > 0 } data ? data : CreateStubTopProducts();
        var demo = !(result.Success && result.Data is { Count: > 0 });

        var lines = new List<string>
        {
            $"Khung thời gian: {from:dd/MM/yyyy} → {to:dd/MM/yyyy}"
        };

        if (demo)
        {
            lines.Add("(Demo) Chưa có dữ liệu top SP — hiển thị ví dụ.");
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                lines.Add($"Ghi chú BLL: {result.Message}");
            }
        }

        var rank = 1;
        foreach (var r in rows)
        {
            lines.Add($"{rank}. {r.ProductCode} · {r.ProductName} · SL {r.QuantitySold:N0} · DT {r.Revenue:N0} đ");
            rank++;
        }

        return ("Top sản phẩm bán chạy", string.Join(Environment.NewLine, lines), demo);
    }

    private (string Title, string Body, bool IsDemoFallback) BuildTopCustomers()
    {
        var to = DateTime.Today;
        var from = to.AddDays(-29);
        var result = _reportService.GetTopCustomers(from, to, 5);
        var rows = result.Success && result.Data is { Count: > 0 } data ? data : CreateStubTopCustomers();
        var demo = !(result.Success && result.Data is { Count: > 0 });

        var lines = new List<string>
        {
            $"Khung thời gian: {from:dd/MM/yyyy} → {to:dd/MM/yyyy}"
        };

        if (demo)
        {
            lines.Add("(Demo) Chưa có dữ liệu khách hàng — hiển thị ví dụ.");
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                lines.Add($"Ghi chú BLL: {result.Message}");
            }
        }

        var rank = 1;
        foreach (var r in rows)
        {
            lines.Add($"{rank}. {r.CustomerName} · {r.InvoiceCount:N0} hóa đơn · {r.TotalAmount:N0} đ");
            rank++;
        }

        return ("Khách hàng mua nhiều nhất", string.Join(Environment.NewLine, lines), demo);
    }

    private (string Title, string Body, bool IsDemoFallback) BuildStocktakeToday()
    {
        var today = DateTime.Today;
        var result = _stocktakeService.GetStocktakes(today, today);
        var rows = result.Success && result.Data is { Count: > 0 } data ? data : CreateStubStocktakes(today);
        var demo = !(result.Success && result.Data is { Count: > 0 });

        var lines = new List<string>();
        if (demo)
        {
            lines.Add("(Demo) Chưa có phiếu kiểm kê trong BLL — hiển thị ví dụ phiếu trong ngày.");
            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                lines.Add($"Ghi chú BLL: {result.Message}");
            }
        }

        foreach (var s in rows)
        {
            lines.Add($"{s.StocktakeCode} · {s.StocktakeDate:dd/MM/yyyy HH:mm} · {s.Note}");
        }

        lines.Add(string.Empty);
        lines.Add("Gợi ý: mở màn Kiểm kê để nhập chi tiết khi backend sẵn sàng.");

        return ("Kiểm kê hôm nay", string.Join(Environment.NewLine, lines), demo);
    }

    private static List<RevenueSummaryDto> CreateStubRevenue(DateTime from, DateTime to)
    {
        var rows = new List<RevenueSummaryDto>();
        var day = from;
        var revenue = 8200000m;
        while (day <= to)
        {
            rows.Add(new RevenueSummaryDto
            {
                Date = day,
                InvoiceCount = 8 + day.Day % 5,
                Revenue = revenue,
                EstimatedProfit = revenue * 0.18m
            });
            revenue += 125000m;
            day = day.AddDays(1);
        }

        return rows;
    }

    private static List<ProductDto> CreateStubLowStock() =>
    [
        new ProductDto
        {
            Code = "SP-012",
            Name = "Keo dán đa năng",
            QuantityOnHand = 4,
            MinStockLevel = 12,
            Unit = "chai"
        },
        new ProductDto
        {
            Code = "SP-018",
            Name = "Băng keo trong",
            QuantityOnHand = 9,
            MinStockLevel = 20,
            Unit = "cuộn"
        },
        new ProductDto
        {
            Code = "SP-024",
            Name = "Thùng carton 3 lớp",
            QuantityOnHand = 14,
            MinStockLevel = 25,
            Unit = "thùng"
        }
    ];

    private static List<ProductSalesSummaryDto> CreateStubTopProducts() =>
    [
        new ProductSalesSummaryDto { ProductId = 1, ProductCode = "SP-001", ProductName = "Bút bi Thiên Long", QuantitySold = 120, Revenue = 600000 },
        new ProductSalesSummaryDto { ProductId = 2, ProductCode = "SP-014", ProductName = "Sổ tay A5", QuantitySold = 86, Revenue = 1548000 },
        new ProductSalesSummaryDto { ProductId = 3, ProductCode = "SP-020", ProductName = "Thùng carton 5 lớp", QuantitySold = 44, Revenue = 1980000 }
    ];

    private static List<CustomerPurchaseSummaryDto> CreateStubTopCustomers() =>
    [
        new CustomerPurchaseSummaryDto { CustomerId = 1, CustomerName = "Nguyễn Văn An", InvoiceCount = 7, TotalAmount = 12850000 },
        new CustomerPurchaseSummaryDto { CustomerId = 2, CustomerName = "Trần Thị Mai", InvoiceCount = 5, TotalAmount = 9450000 },
        new CustomerPurchaseSummaryDto { CustomerId = 3, CustomerName = "Công ty Minh Phát", InvoiceCount = 4, TotalAmount = 8760000 }
    ];

    private static List<StocktakeDto> CreateStubStocktakes(DateTime today) =>
    [
        new StocktakeDto
        {
            StocktakeCode = "KK-DEMO-01",
            StocktakeDate = today.AddHours(9.5),
            Note = "Kiểm đếm khu A — khớp sổ (demo)."
        },
        new StocktakeDto
        {
            StocktakeCode = "KK-DEMO-02",
            StocktakeDate = today.AddHours(14),
            Note = "Đối chiếu lệch nhẹ ở pallet 03 (demo)."
        }
    ];

    private enum AssistantIntent
    {
        Unknown,
        RevenueToday,
        LowStock,
        TopProducts,
        TopCustomers,
        StocktakeToday
    }
}
