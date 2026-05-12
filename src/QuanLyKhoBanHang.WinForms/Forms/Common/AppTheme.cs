namespace QuanLyKhoBanHang.WinForms.Forms.Common;

internal static class AppTheme
{
    public static readonly Color AppBackground = Color.FromArgb(245, 247, 250);
    public static readonly Color ShellBackground = Color.FromArgb(242, 244, 248);
    public static readonly Color Surface = Color.White;
    public static readonly Color SurfaceMuted = Color.FromArgb(245, 248, 252);
    public static readonly Color Sidebar = Color.FromArgb(28, 47, 73);
    public static readonly Color SidebarButton = Color.FromArgb(42, 66, 98);
    public static readonly Color SidebarTextMuted = Color.FromArgb(190, 205, 225);
    public static readonly Color TextMuted = Color.FromArgb(96, 108, 129);
    public static readonly Color StatusText = Color.FromArgb(92, 102, 121);
    public static readonly Color Selection = Color.FromArgb(207, 227, 255);
    public static readonly Color Error = Color.Firebrick;

    public static readonly Padding PagePadding = new(18);
    public static readonly Padding CardPadding = new(16);

    public const string FontFamily = "Segoe UI";

    public static Font TitleFont(float size = 18F) => new(FontFamily, size, FontStyle.Bold);

    public static Font SectionFont(float size = 12F) => new(FontFamily, size, FontStyle.Bold);

    public static Font BodyFont(float size = 10F) => new(FontFamily, size, FontStyle.Regular);
}
