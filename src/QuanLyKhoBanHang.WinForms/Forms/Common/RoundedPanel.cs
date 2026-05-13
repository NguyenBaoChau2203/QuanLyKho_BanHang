using System.Drawing.Drawing2D;

namespace QuanLyKhoBanHang.WinForms.Forms.Common;

internal sealed class RoundedPanel : Panel
{
    public RoundedPanel()
    {
        DoubleBuffered = true;
        SetStyle(
            ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw
            | ControlStyles.UserPaint
            | ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.Transparent;
    }

    public int Radius { get; set; } = 8;
    public int BorderWidth { get; set; } = 1;
    public int ShadowSize { get; set; } = 1;
    public Color FillColor { get; set; } = AppTheme.Surface;
    public Color BorderColor { get; set; } = AppTheme.Border;
    public Color ShadowColor { get; set; } = Color.FromArgb(28, 15, 23, 42);

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (Parent is null)
        {
            base.OnPaintBackground(e);
            return;
        }

        using var brush = new SolidBrush(FindParentBackColor());
        e.Graphics.FillRectangle(brush, ClientRectangle);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var shadowOffset = Math.Max(0, ShadowSize);
        var bounds = new Rectangle(
            shadowOffset,
            shadowOffset,
            Width - shadowOffset - 1,
            Height - shadowOffset - 1);

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        if (shadowOffset > 0)
        {
            var shadowBounds = new Rectangle(bounds.X, bounds.Y + shadowOffset, bounds.Width, bounds.Height - shadowOffset);
            using var shadowPath = CreateRoundPath(shadowBounds, Radius);
            using var shadowBrush = new SolidBrush(ShadowColor);
            e.Graphics.FillPath(shadowBrush, shadowPath);
        }

        using var fillPath = CreateRoundPath(bounds, Radius);
        using var fillBrush = new SolidBrush(FillColor);
        e.Graphics.FillPath(fillBrush, fillPath);

        if (BorderWidth <= 0)
        {
            return;
        }

        using var borderPen = new Pen(BorderColor, BorderWidth);
        e.Graphics.DrawPath(borderPen, fillPath);
    }

    private Color FindParentBackColor()
    {
        for (var control = Parent; control is not null; control = control.Parent)
        {
            if (control.BackColor != Color.Transparent)
            {
                return control.BackColor;
            }
        }

        return AppTheme.AppBackground;
    }

    private static GraphicsPath CreateRoundPath(Rectangle rectangle, int radius)
    {
        var path = new GraphicsPath();
        var diameter = Math.Max(1, radius * 2);

        if (diameter >= rectangle.Width || diameter >= rectangle.Height)
        {
            path.AddEllipse(rectangle);
            path.CloseFigure();
            return path;
        }

        path.AddArc(rectangle.X, rectangle.Y, diameter, diameter, 180, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Y, diameter, diameter, 270, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rectangle.X, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}
