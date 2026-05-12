using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuanLyKhoBanHang.BLL.Services.Assistant;

internal sealed class DeepSeekAssistantProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly DeepSeekOptions _options;
    private readonly HttpClient _httpClient;
    private readonly TimeSpan _timeout;

    public DeepSeekAssistantProvider(DeepSeekOptions options, HttpClient? httpClient, TimeSpan? timeout)
    {
        _options = options;
        _httpClient = httpClient ?? new HttpClient();
        _timeout = timeout ?? TimeSpan.FromSeconds(4);
    }

    public DeepSeekAssistantResult Ask(string question, IReadOnlyList<AssistantSafeContext> safeContexts)
    {
        if (!_options.IsEnabled)
        {
            throw new InvalidOperationException("DeepSeek is not configured.");
        }

        using var timeout = new CancellationTokenSource(_timeout);
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildChatCompletionsUri());
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = new StringContent(
            JsonSerializer.Serialize(BuildPayload(question, safeContexts), JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = _httpClient.SendAsync(request, timeout.Token).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"DeepSeek returned HTTP {(int)response.StatusCode}.");
        }

        var responseJson = response.Content.ReadAsStringAsync(timeout.Token).GetAwaiter().GetResult();
        var completion = JsonSerializer.Deserialize<DeepSeekChatCompletionResponse>(responseJson, JsonOptions);
        var assistantContent = completion?.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(assistantContent))
        {
            throw new InvalidOperationException("DeepSeek returned an empty response.");
        }

        return ParseAssistantResult(assistantContent);
    }

    private Uri BuildChatCompletionsUri()
    {
        var baseUrl = _options.BaseUrl.EndsWith("/", StringComparison.Ordinal)
            ? _options.BaseUrl
            : _options.BaseUrl + "/";
        return new Uri(new Uri(baseUrl), "chat/completions");
    }

    private DeepSeekChatCompletionRequest BuildPayload(string question, IReadOnlyList<AssistantSafeContext> safeContexts)
    {
        return new DeepSeekChatCompletionRequest
        {
            Model = _options.Model,
            Messages =
            [
                new DeepSeekMessage
                {
                    Role = "system",
                    Content = BuildSystemPrompt()
                },
                new DeepSeekMessage
                {
                    Role = "user",
                    Content = BuildUserPrompt(question, safeContexts)
                }
            ],
            Temperature = 0.2m,
            MaxTokens = 500,
            Stream = false
        };
    }

    private static string BuildSystemPrompt()
    {
        return """
Bạn là trợ lý quản lý kho và bán hàng cho một ứng dụng WinForms demo.
Chỉ được phân loại câu hỏi tiếng Việt và viết câu trả lời thân thiện từ dữ liệu an toàn do BLL cung cấp.
Không được tạo SQL để thực thi, không đề xuất truy cập database, không nhắc API key, không bịa số liệu ngoài ngữ cảnh.
Chỉ trả về một JSON object hợp lệ, không markdown, theo schema:
{"intent":"revenue-today|low-stock|top-products|top-customers|stocktake-today|unknown","handled":true|false,"answer":"câu trả lời tiếng Việt"}
""";
    }

    private static string BuildUserPrompt(string question, IReadOnlyList<AssistantSafeContext> safeContexts)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Câu hỏi người dùng:");
        builder.AppendLine(question.Trim());
        builder.AppendLine();
        builder.AppendLine("Intent hợp lệ:");
        foreach (var intent in AssistantIntentCatalog.AllSupported)
        {
            builder.AppendLine("- " + intent);
        }

        builder.AppendLine();
        builder.AppendLine("Dữ liệu an toàn từ BLL:");
        foreach (var context in safeContexts)
        {
            builder.AppendLine($"[{context.Intent}] {context.Title}");
            builder.AppendLine(context.Answer);
            builder.AppendLine();
        }

        builder.AppendLine("Hãy chọn intent phù hợp nhất. Nếu không liên quan đến các intent trên, dùng unknown và hướng dẫn người dùng chọn các lệnh gợi ý.");
        return builder.ToString();
    }

    private static DeepSeekAssistantResult ParseAssistantResult(string content)
    {
        var json = ExtractJsonObject(content);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var intent = root.GetProperty("intent").GetString()?.Trim() ?? AssistantIntentCatalog.Unknown;
        var answer = root.GetProperty("answer").GetString()?.Trim() ?? string.Empty;
        var handled = root.TryGetProperty("handled", out var handledElement) && handledElement.ValueKind == JsonValueKind.True;

        if (!AssistantIntentCatalog.IsSupported(intent))
        {
            throw new InvalidOperationException("DeepSeek returned an unsupported intent.");
        }

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new InvalidOperationException("DeepSeek returned an empty answer.");
        }

        if (ContainsSqlExecutionText(answer))
        {
            throw new InvalidOperationException("DeepSeek returned SQL-like content.");
        }

        return new DeepSeekAssistantResult(intent, answer, handled && intent != AssistantIntentCatalog.Unknown);
    }

    private static string ExtractJsonObject(string content)
    {
        var start = content.IndexOf('{');
        var end = content.LastIndexOf('}');
        if (start < 0 || end < start)
        {
            throw new InvalidOperationException("DeepSeek response is not JSON.");
        }

        return content[start..(end + 1)];
    }

    private static bool ContainsSqlExecutionText(string answer)
    {
        var normalized = " " + answer.Trim().ToLowerInvariant() + " ";
        var sqlTokens = new[]
        {
            " select ",
            " insert ",
            " update ",
            " delete ",
            " drop ",
            " alter ",
            " truncate ",
            " exec ",
            " execute "
        };

        return sqlTokens.Any(normalized.Contains);
    }

    private sealed class DeepSeekChatCompletionRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; init; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<DeepSeekMessage> Messages { get; init; } = [];

        [JsonPropertyName("temperature")]
        public decimal Temperature { get; init; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; init; }

        [JsonPropertyName("stream")]
        public bool Stream { get; init; }
    }

    private sealed class DeepSeekMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; init; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; init; } = string.Empty;
    }

    private sealed class DeepSeekChatCompletionResponse
    {
        [JsonPropertyName("choices")]
        public List<DeepSeekChoice>? Choices { get; init; }
    }

    private sealed class DeepSeekChoice
    {
        [JsonPropertyName("message")]
        public DeepSeekChoiceMessage? Message { get; init; }
    }

    private sealed class DeepSeekChoiceMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; init; }
    }
}
