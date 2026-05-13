namespace QuanLyKhoBanHang.WinForms.Forms.Common;

internal static class AppTheme
{
    public static readonly Color AppBackground = Color.FromArgb(245, 247, 250);
    public static readonly Color ShellBackground = Color.FromArgb(242, 244, 248);
    public static readonly Color Surface = Color.White;
    public static readonly Color SurfaceMuted = Color.FromArgb(245, 248, 252);
    public static readonly Color SurfaceSubtle = Color.FromArgb(248, 250, 252);
    public static readonly Color Border = Color.FromArgb(220, 227, 237);
    public static readonly Color BorderStrong = Color.FromArgb(190, 207, 232);
    public static readonly Color GridLine = Color.FromArgb(218, 226, 237);
    public static readonly Color Sidebar = Color.FromArgb(28, 47, 73);
    public static readonly Color SidebarButton = Color.FromArgb(42, 66, 98);
    public static readonly Color SidebarHover = Color.FromArgb(52, 78, 115);
    public static readonly Color SidebarSelected = Color.FromArgb(64, 101, 149);
    public static readonly Color SidebarSelectedAccent = Color.FromArgb(96, 165, 250);
    public static readonly Color SidebarTextMuted = Color.FromArgb(190, 205, 225);
    public static readonly Color Text = Color.FromArgb(17, 24, 39);
    public static readonly Color TextMuted = Color.FromArgb(96, 108, 129);
    public static readonly Color StatusText = Color.FromArgb(92, 102, 121);
    public static readonly Color Selection = Color.FromArgb(207, 227, 255);
    public static readonly Color Primary = Color.FromArgb(37, 99, 235);
    public static readonly Color PrimarySoft = Color.FromArgb(225, 237, 255);
    public static readonly Color Success = Color.FromArgb(5, 150, 105);
    public static readonly Color SuccessSoft = Color.FromArgb(220, 248, 237);
    public static readonly Color Warning = Color.FromArgb(234, 88, 12);
    public static readonly Color WarningSoft = Color.FromArgb(255, 237, 213);
    public static readonly Color Danger = Color.FromArgb(220, 38, 38);
    public static readonly Color DangerSoft = Color.FromArgb(254, 226, 226);
    public static readonly Color Error = Color.Firebrick;

    public static readonly Padding PagePadding = new(18);
    public static readonly Padding CardPadding = new(16);

    public const string FontFamily = "Segoe UI";

    public static Font TitleFont(float size = 18F) => new(FontFamily, size, FontStyle.Bold);

    public static Font SectionFont(float size = 12F) => new(FontFamily, size, FontStyle.Bold);

    public static Font BodyFont(float size = 10F) => new(FontFamily, size, FontStyle.Regular);
}
