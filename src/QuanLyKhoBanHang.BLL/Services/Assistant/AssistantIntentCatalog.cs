namespace QuanLyKhoBanHang.BLL.Services.Assistant;

internal static class AssistantIntentCatalog
{
    public const string Unknown = "unknown";
    public const string RevenueToday = "revenue-today";
    public const string LowStock = "low-stock";
    public const string TopProducts = "top-products";
    public const string TopCustomers = "top-customers";
    public const string StocktakeToday = "stocktake-today";

    private static readonly HashSet<string> SupportedIntents = new(StringComparer.Ordinal)
    {
        Unknown,
        RevenueToday,
        LowStock,
        TopProducts,
        TopCustomers,
        StocktakeToday
    };

    public static IReadOnlyList<string> AllSupported { get; } =
    [
        Unknown,
        RevenueToday,
        LowStock,
        TopProducts,
        TopCustomers,
        StocktakeToday
    ];

    public static IReadOnlyList<string> BusinessIntents { get; } =
    [
        RevenueToday,
        LowStock,
        TopProducts,
        TopCustomers,
        StocktakeToday
    ];

    public static bool IsSupported(string intent) => SupportedIntents.Contains(intent);
}
