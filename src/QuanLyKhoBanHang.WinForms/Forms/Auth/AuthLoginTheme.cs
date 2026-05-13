namespace QuanLyKhoBanHang.WinForms.Forms.Auth;

/// <summary>Colors and typography for the redesigned auth shell (login / register / forgot).</summary>
internal static class AuthLoginTheme
{
    public static readonly Color FormBackground = Color.FromArgb(0xF2, 0xF5, 0xF9);
    public static readonly Color Navy = Color.FromArgb(0x1E, 0x3A, 0x5F);
    public static readonly Color PrimaryBlue = Color.FromArgb(0x25, 0x63, 0xEB);
    public static readonly Color MutedText = Color.FromArgb(0x6B, 0x72, 0x80);
    public static readonly Color Border = Color.FromArgb(0xE5, 0xE7, 0xEB);
    public static readonly Color CardSurface = Color.White;
    public static readonly Color ShadowTint = Color.FromArgb(0xD1, 0xD9, 0xE6);

    public const string FontFamily = "Segoe UI";

    public static Font CardTitleFont() => new(FontFamily, 20F, FontStyle.Bold);

    public static Font FieldLabelFont() => new(FontFamily, 10.25F, FontStyle.Bold);

    public static Font BodyFont() => new(FontFamily, 10.25F, FontStyle.Regular);

    public static Font PrimaryButtonFont() => new(FontFamily, 12F, FontStyle.Bold);

    public static Font DemoHintFont() => new(FontFamily, 9F, FontStyle.Regular);
}
