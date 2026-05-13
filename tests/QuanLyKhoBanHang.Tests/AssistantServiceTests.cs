using System.Net;
using QuanLyKhoBanHang.BLL.Services;

namespace QuanLyKhoBanHang.Tests;

[TestClass]
[DoNotParallelize]
public sealed class AssistantServiceTests
{
    [TestMethod]
    public void Ask_NoDeepSeekApiKey_UsesRuleBasedFallback()
    {
        using var env = new EnvironmentVariableScope(
            (EnvironmentVariableScope.DeepSeekApiKey, null),
            (EnvironmentVariableScope.DeepSeekBaseUrl, null),
            (EnvironmentVariableScope.DeepSeekModel, null));
        var service = new AssistantService();

        var result = service.Ask("doanh thu hôm nay");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("offline-rule-based", result.Data.Mode);
        Assert.IsTrue(result.Data.IsFallback);
        Assert.IsTrue(result.Data.Handled);
        StringAssert.Contains(result.Data.Answer, "Doanh thu hôm nay");
    }

    [TestMethod]
    public void Ask_DeepSeekHttpFailure_UsesRuleBasedFallback()
    {
        using var env = new EnvironmentVariableScope(
            (EnvironmentVariableScope.DeepSeekApiKey, "fake-deepseek-key-for-tests"),
            (EnvironmentVariableScope.DeepSeekBaseUrl, "https://api.deepseek.test"),
            (EnvironmentVariableScope.DeepSeekModel, null));
        using var httpClient = new HttpClient(new StaticFailureHandler(HttpStatusCode.Unauthorized));
        var service = new AssistantService(httpClient, TimeSpan.FromMilliseconds(100));

        var result = service.Ask("hàng sắp hết");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("ai-failed-fallback", result.Data.Mode);
        Assert.IsTrue(result.Data.IsFallback);
        Assert.IsTrue(result.Data.Handled);
        StringAssert.Contains(result.Data.StatusMessage, "HTTP 401");
        StringAssert.Contains(result.Data.Answer, "sản phẩm");
    }

    [TestMethod]
    public void Ask_DeepSeekSuccess_ReturnsAiOnlineAndNormalizesIntent()
    {
        using var env = new EnvironmentVariableScope(
            (EnvironmentVariableScope.DeepSeekApiKey, "fake-deepseek-key-for-tests"),
            (EnvironmentVariableScope.DeepSeekBaseUrl, "https://api.deepseek.test"),
            (EnvironmentVariableScope.DeepSeekModel, "deepseek-v4-flash"));
        const string responseBody = """
        {
          "choices": [
            {
              "message": {
                "content": "{\"intent\":\"low_stock\",\"handled\":true,\"answer\":\"Có 2 sản phẩm cần nhập thêm ngay.\"}"
              }
            }
          ]
        }
        """;
        using var httpClient = new HttpClient(new StaticSuccessHandler(responseBody));
        var service = new AssistantService(httpClient, TimeSpan.FromMilliseconds(100));

        var result = service.Ask("hôm nay mặt hàng nào cần nhập thêm?");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("ai-online", result.Data.Mode);
        Assert.IsFalse(result.Data.IsFallback);
        Assert.IsTrue(result.Data.Handled);
        Assert.AreEqual("low-stock", result.Data.Intent);
        StringAssert.Contains(result.Data.Answer, "2 sản phẩm");
    }

    [TestMethod]
    public void Ask_ExistingDemoCommands_ReturnUsefulAnswersWithoutApiKey()
    {
        using var env = new EnvironmentVariableScope(
            (EnvironmentVariableScope.DeepSeekApiKey, null),
            (EnvironmentVariableScope.DeepSeekBaseUrl, null),
            (EnvironmentVariableScope.DeepSeekModel, null));
        var service = new AssistantService();

        var commands = new[]
        {
            "doanh thu hôm nay",
            "hàng sắp hết",
            "top sản phẩm bán chạy",
            "khách hàng mua nhiều nhất",
            "kiểm kê hôm nay"
        };

        foreach (var command in commands)
        {
            var result = service.Ask(command);

            Assert.IsTrue(result.Success, command);
            Assert.IsNotNull(result.Data, command);
            Assert.AreEqual("offline-rule-based", result.Data.Mode, command);
            Assert.IsTrue(result.Data.Handled, command);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Data.Answer), command);
        }
    }

    [TestMethod]
    public void Ask_EmptyUnknownAndApiFailure_DoNotThrow()
    {
        using var noKeyEnv = new EnvironmentVariableScope(
            (EnvironmentVariableScope.DeepSeekApiKey, null),
            (EnvironmentVariableScope.DeepSeekBaseUrl, null),
            (EnvironmentVariableScope.DeepSeekModel, null));
        var offlineService = new AssistantService();

        var empty = CaptureException(() =>
        {
            var result = offlineService.Ask("");
            Assert.IsFalse(result.Success);
        });
        Assert.IsNull(empty);

        var unknown = CaptureException(() =>
        {
            var result = offlineService.Ask("mở màn hình đổi mật khẩu");
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.IsFalse(result.Data.Handled);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Data.Answer));
        });
        Assert.IsNull(unknown);

        noKeyEnv.Dispose();
        using var failingEnv = new EnvironmentVariableScope(
            (EnvironmentVariableScope.DeepSeekApiKey, "fake-deepseek-key-for-tests"),
            (EnvironmentVariableScope.DeepSeekBaseUrl, "https://api.deepseek.test"),
            (EnvironmentVariableScope.DeepSeekModel, null));
        using var httpClient = new HttpClient(new ThrowingHandler());
        var failingService = new AssistantService(httpClient, TimeSpan.FromMilliseconds(100));

        var apiFailure = CaptureException(() =>
        {
            var result = failingService.Ask("doanh thu hôm nay");
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("ai-failed-fallback", result.Data.Mode);
        });
        Assert.IsNull(apiFailure);
    }

    private static Exception? CaptureException(Action action)
    {
        try
        {
            action();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private sealed class StaticFailureHandler(HttpStatusCode statusCode) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent("{}")
            });
        }
    }

    private sealed class StaticSuccessHandler(string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body)
            });
        }
    }

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new HttpRequestException("Synthetic DeepSeek failure for tests.");
        }
    }

    private sealed class EnvironmentVariableScope : IDisposable
    {
        public const string DeepSeekApiKey = "DEEPSEEK_API_KEY";
        public const string DeepSeekBaseUrl = "DEEPSEEK_BASE_URL";
        public const string DeepSeekModel = "DEEPSEEK_MODEL";

        private readonly Dictionary<string, string?> _previousValues = [];
        private bool _disposed;

        public EnvironmentVariableScope(params (string Name, string? Value)[] values)
        {
            foreach (var (name, value) in values)
            {
                _previousValues[name] = Environment.GetEnvironmentVariable(name);
                Environment.SetEnvironmentVariable(name, value);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            foreach (var (name, value) in _previousValues)
            {
                Environment.SetEnvironmentVariable(name, value);
            }

            _disposed = true;
        }
    }
}
