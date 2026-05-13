namespace QuanLyKhoBanHang.BLL.Services.Assistant;

internal sealed class DeepSeekOptions
{
    public const string ApiKeyVariable = "DEEPSEEK_API_KEY";
    public const string ModelVariable = "DEEPSEEK_MODEL";
    public const string BaseUrlVariable = "DEEPSEEK_BASE_URL";
    public const string DefaultModel = "deepseek-chat";
    public const string DefaultBaseUrl = "https://api.deepseek.com";

    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = DefaultModel;
    public string BaseUrl { get; init; } = DefaultBaseUrl;

    public bool IsEnabled => !string.IsNullOrWhiteSpace(ApiKey);

    public static DeepSeekOptions FromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable(ApiKeyVariable)?.Trim() ?? string.Empty;
        var model = Environment.GetEnvironmentVariable(ModelVariable)?.Trim();
        var baseUrl = Environment.GetEnvironmentVariable(BaseUrlVariable)?.Trim();

        return new DeepSeekOptions
        {
            ApiKey = apiKey,
            Model = string.IsNullOrWhiteSpace(model) ? DefaultModel : model,
            BaseUrl = string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl.TrimEnd('/')
        };
    }
}
