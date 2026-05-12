using QuanLyKhoBanHang.BLL.Common;
using QuanLyKhoBanHang.BLL.Services.Assistant;
using QuanLyKhoBanHang.DTO.Assistant;

namespace QuanLyKhoBanHang.BLL.Services;

public sealed class AssistantService
{
    private const string OfflineStatus = "Offline rule-based: chưa cấu hình DEEPSEEK_API_KEY.";
    private const string AiReadyStatus = "AI online: DeepSeek đã được cấu hình, tự fallback nếu API lỗi.";
    private const string AiFallbackStatus = "AI failed, fallback used: đã dùng trợ lý offline rule-based.";

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
        catch
        {
            var fallback = _ruleBasedProvider.Ask(
                question,
                AssistantModes.AiFailedFallback,
                AiFallbackStatus,
                isFallback: true);
            return ServiceResult<AssistantResponseDto>.Ok(fallback, fallback.StatusMessage);
        }
    }
}
