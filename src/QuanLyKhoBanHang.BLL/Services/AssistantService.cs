using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Services.Assistant;
using QuanLyKhoBanHang.DTO.Assistant;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AssistantService
{
    private const string OfflineStatus = "Rule-based offline: chưa cấu hình API AI bên ngoài.";
    private const string AiReadyStatus = "AI hybrid: API AI bên ngoài đã được cấu hình, tự chuyển về rule-based nếu API lỗi.";
    private const string AiFallbackStatus = "AI API lỗi, đã dùng rule-based offline.";

    private readonly RuleBasedAssistantProvider _ruleBasedProvider;
    private readonly DeepSeekAssistantProvider _deepSeekProvider;
    private readonly DeepSeekOptions _deepSeekOptions;

    public AssistantService()
        : this(null, null)
    {
    }

    public AssistantService(HttpClient? deepSeekHttpClient, TimeSpan? deepSeekTimeout = null)
    {
        _ruleBasedProvider = new RuleBasedAssistantProvider(
            new ReportService(),
            new InventoryService(),
            new StocktakeService());
        _deepSeekOptions = DeepSeekOptions.FromEnvironment();
        _deepSeekProvider = new DeepSeekAssistantProvider(_deepSeekOptions, deepSeekHttpClient, deepSeekTimeout);
    }

    public ServiceResult<AssistantResponseDto> GetModeStatus()
    {
        var mode = _deepSeekOptions.IsEnabled ? AssistantModes.AiOnline : AssistantModes.OfflineRuleBased;
        var status = _deepSeekOptions.IsEnabled ? AiReadyStatus : OfflineStatus;

        return ServiceResult<AssistantResponseDto>.Ok(new AssistantResponseDto
        {
            Intent = AssistantIntentCatalog.Unknown,
            Handled = false,
            Mode = mode,
            StatusMessage = status,
            IsFallback = !_deepSeekOptions.IsEnabled
        }, status);
    }

    public ServiceResult<AssistantResponseDto> Ask(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return ServiceResult<AssistantResponseDto>.Fail("Vui lòng nhập câu hỏi hoặc câu lệnh.");
        }

        if (!_deepSeekOptions.IsEnabled)
        {
            var offline = _ruleBasedProvider.Ask(
                question,
                AssistantModes.OfflineRuleBased,
                OfflineStatus,
                isFallback: true);
            return ServiceResult<AssistantResponseDto>.Ok(offline, offline.StatusMessage);
        }

        try
        {
            var safeContexts = _ruleBasedProvider.BuildSafeContexts();
            var ai = _deepSeekProvider.Ask(question, safeContexts);
            var online = new AssistantResponseDto
            {
                Intent = ai.Intent,
                Answer = ai.Answer,
                Handled = ai.Handled,
                Mode = AssistantModes.AiOnline,
                StatusMessage = AiReadyStatus,
                IsFallback = false
            };

            return ServiceResult<AssistantResponseDto>.Ok(online, online.StatusMessage);
        }
        catch (Exception ex)
        {
            var fallback = _ruleBasedProvider.Ask(
                question,
                AssistantModes.AiFailedFallback,
                BuildFallbackStatus(ex),
                isFallback: true);
            return ServiceResult<AssistantResponseDto>.Ok(fallback, fallback.StatusMessage);
        }
    }

    private static string BuildFallbackStatus(Exception ex)
    {
        var message = string.IsNullOrWhiteSpace(ex.Message) ? ex.GetType().Name : ex.Message;
        message = message
            .Replace(Environment.NewLine, " ", StringComparison.Ordinal)
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal);

        if (message.Length > 140)
        {
            message = message[..140] + "...";
        }

        return $"{AiFallbackStatus} Lý do: {SanitizeProviderMessage(message)}";
    }

    private static string SanitizeProviderMessage(string message)
    {
        return message
            .Replace("DEEPSEEK_API_KEY", "khóa API AI", StringComparison.OrdinalIgnoreCase)
            .Replace("DEEPSEEK_BASE_URL", "địa chỉ API AI", StringComparison.OrdinalIgnoreCase)
            .Replace("DEEPSEEK_MODEL", "model AI", StringComparison.OrdinalIgnoreCase)
            .Replace("api.deepseek.com", "AI API endpoint", StringComparison.OrdinalIgnoreCase)
            .Replace("DeepSeek", "AI API", StringComparison.OrdinalIgnoreCase)
            .Replace("deepseek", "AI API", StringComparison.OrdinalIgnoreCase);
    }
}
