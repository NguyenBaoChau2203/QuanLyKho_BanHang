using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.WinForms.Forms.Main;

namespace QuanLyKhoBanHang.WinForms.Forms.Auth;

public sealed class FrmLogin : Form
{
    private const int AuthCardWidth = 396;
    private const int AuthCardHeight = 500;
    private const int AuthCardLeft = 442;
    private const int AuthCardTop = 30;

    private readonly AuthService _authService = new();

    private readonly Panel _backgroundPanel = new();
    private readonly Panel _pnlLogin = new();
    private readonly Panel _pnlRegister = new();
    private readonly Panel _pnlForgot = new();

    private readonly TextBox _txtLoginUser = new();
    private readonly TextBox _txtLoginPassword = new();
    private readonly CheckBox _chkRemember = new();
    private readonly Button _btnLogin = new();
    private readonly LinkLabel _lnkRegister = new();
    private readonly LinkLabel _lnkForgot = new();

    private readonly TextBox _txtRegFullName = new();
    private readonly TextBox _txtRegUsername = new();
    private readonly TextBox _txtRegPassword = new();
    private readonly TextBox _txtRegConfirm = new();
    private readonly Button _btnRegister = new();
    private readonly LinkLabel _lnkBackFromRegister = new();

    private readonly TextBox _txtForgotIdentity = new();
    private readonly Button _btnForgotSend = new();
    private readonly LinkLabel _lnkBackFromForgot = new();

    private bool _hasRememberedPassword;

    public FrmLogin()
    {
        Text = "Đăng nhập - Quản lý kho & bán hàng";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        ClientSize = new Size(900, 560);
        MinimumSize = new Size(900, 560);
        BackColor = AuthLoginTheme.FormBackground;
        Font = AuthLoginTheme.BodyFont();
        DoubleBuffered = true;

        _backgroundPanel.Dock = DockStyle.Fill;
        _backgroundPanel.BackColor = AuthLoginTheme.FormBackground;
        _backgroundPanel.BackgroundImageLayout = ImageLayout.Stretch;
        Controls.Add(_backgroundPanel);
        Load += (_, _) => LoadBrandingImage();

        var overlay = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3,
            BackColor = Color.Transparent,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        overlay.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AuthCardLeft));
        overlay.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AuthCardWidth));
        overlay.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        overlay.RowStyles.Add(new RowStyle(SizeType.Absolute, AuthCardTop));
        overlay.RowStyles.Add(new RowStyle(SizeType.Absolute, AuthCardHeight));
        overlay.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var cardHost = BuildCardHost();
        cardHost.Dock = DockStyle.Fill;
        overlay.Controls.Add(cardHost, 1, 1);
        _backgroundPanel.Controls.Add(overlay);

        BuildLoginPanel();
        BuildRegisterPanel();
        BuildForgotPanel();

        ApplyRememberedLoginPrefs();
        ShowAuthView(AuthView.Login);
        Shown += (_, _) =>
        {
            if (_hasRememberedPassword)
            {
                _btnLogin.Focus();
                return;
            }

            _txtLoginUser.Focus();
        };
    }

    private Panel BuildCardHost()
    {
        var shadow = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AuthLoginTheme.ShadowTint,
            CornerRadius = 8,
            Padding = new Padding(0, 0, 4, 4),
            Margin = new Padding(0)
        };

        var inner = new RoundedPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AuthLoginTheme.CardSurface,
            BorderColor = AuthLoginTheme.Border,
            CornerRadius = 8,
            Padding = new Padding(28, 30, 28, 24),
            AutoScroll = true
        };

        _pnlLogin.Dock = DockStyle.Fill;
        _pnlRegister.Dock = DockStyle.Fill;
        _pnlForgot.Dock = DockStyle.Fill;
        _pnlLogin.BackColor = AuthLoginTheme.CardSurface;
        _pnlRegister.BackColor = AuthLoginTheme.CardSurface;
        _pnlForgot.BackColor = AuthLoginTheme.CardSurface;
        inner.Controls.Add(_pnlLogin);
        inner.Controls.Add(_pnlRegister);
        inner.Controls.Add(_pnlForgot);

        shadow.Controls.Add(inner);
        return shadow;
    }

    private void BuildLoginPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 9,
            BackColor = AuthLoginTheme.CardSurface,
            AutoScroll = false
        };

        int r = 0;
        void RowAuto() => layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        void RowAbs(float h) => layout.RowStyles.Add(new RowStyle(SizeType.Absolute, h));

        RowAuto();
        layout.Controls.Add(new Label
        {
            Text = "Đăng nhập",
            AutoSize = true,
            ForeColor = AuthLoginTheme.Navy,
            Font = AuthLoginTheme.CardTitleFont(),
            Margin = new Padding(0, 0, 0, 20)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập"), 0, r++);

        RowAbs(40);
        layout.Controls.Add(WrapLightBorderTextBox(_txtLoginUser, "Nhập tên đăng nhập", password: false), 0, r++);
        _txtLoginUser.TabIndex = 0;

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Mật khẩu", topPad: 14), 0, r++);

        RowAbs(40);
        layout.Controls.Add(WrapLightBorderTextBox(_txtLoginPassword, "Nhập mật khẩu", password: true), 0, r++);
        _txtLoginPassword.TabIndex = 1;

        RowAuto();
        _chkRemember.Text = "Nhớ đăng nhập và mật khẩu";
        _chkRemember.AutoSize = true;
        _chkRemember.Margin = new Padding(0, 12, 0, 0);
        _chkRemember.ForeColor = AuthLoginTheme.MutedText;
        _chkRemember.TabIndex = 2;
        _chkRemember.FlatStyle = FlatStyle.Flat;
        layout.Controls.Add(_chkRemember, 0, r++);

        RowAbs(68);
        _btnLogin.Text = "Đăng nhập";
        _btnLogin.Dock = DockStyle.Fill;
        _btnLogin.Margin = new Padding(0, 24, 0, 0);
        _btnLogin.TabIndex = 3;
        StylePrimaryButton(_btnLogin);
        _btnLogin.Click += HandleLogin;
        layout.Controls.Add(_btnLogin, 0, r++);

        RowAuto();
        var links = new TableLayoutPanel
        {
            AutoSize = true,
            ColumnCount = 3,
            RowCount = 1,
            Margin = new Padding(0, 16, 0, 0),
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.Top
        };
        links.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        links.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        links.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        StyleSecondaryLink(_lnkRegister, "Đăng ký tài khoản");
        _lnkRegister.Margin = new Padding(0, 2, 0, 0);
        _lnkRegister.TabIndex = 4;
        _lnkRegister.Click += (_, _) => ShowAuthView(AuthView.Register);
        links.Controls.Add(_lnkRegister, 0, 0);

        var sep = new Label
        {
            Text = "|",
            AutoSize = true,
            ForeColor = AuthLoginTheme.Border,
            Margin = new Padding(12, 4, 12, 0),
            TextAlign = ContentAlignment.MiddleCenter
        };
        links.Controls.Add(sep, 1, 0);

        StyleSecondaryLink(_lnkForgot, "Quên mật khẩu?");
        _lnkForgot.Margin = new Padding(0, 2, 0, 0);
        _lnkForgot.TabIndex = 5;
        _lnkForgot.Click += (_, _) => ShowAuthView(AuthView.ForgotPassword);
        links.Controls.Add(_lnkForgot, 2, 0);

        layout.Controls.Add(links, 0, r++);

        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, r++);

        _pnlLogin.Controls.Add(layout);
    }

    private void BuildRegisterPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 13,
            BackColor = AuthLoginTheme.CardSurface,
            AutoScroll = false
        };

        int r = 0;
        void RowAuto() => layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        void RowAbs(float h) => layout.RowStyles.Add(new RowStyle(SizeType.Absolute, h));

        RowAuto();
        layout.Controls.Add(new Label
        {
            Text = "Tạo tài khoản",
            AutoSize = true,
            ForeColor = AuthLoginTheme.Navy,
            Font = AuthLoginTheme.CardTitleFont(),
            Margin = new Padding(0, 0, 0, 4)
        }, 0, r++);

        RowAbs(42);
        layout.Controls.Add(new Label
        {
            Text = "Tài khoản mới cần quản trị viên phê duyệt\r\nvà phân quyền trước khi đăng nhập.",
            AutoSize = false,
            Dock = DockStyle.Fill,
            ForeColor = AuthLoginTheme.MutedText,
            Font = AuthLoginTheme.BodyFont(),
            Margin = new Padding(0, 0, 0, 4)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Họ và tên"), 0, r++);
        RowAbs(36);
        layout.Controls.Add(WrapLightBorderTextBox(_txtRegFullName, "Nhập họ và tên", password: false), 0, r++);
        _txtRegFullName.TabIndex = 0;

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập", topPad: 5), 0, r++);
        RowAbs(36);
        layout.Controls.Add(WrapLightBorderTextBox(_txtRegUsername, "Chọn tên đăng nhập", password: false), 0, r++);
        _txtRegUsername.TabIndex = 1;

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Mật khẩu", topPad: 5), 0, r++);
        RowAbs(36);
        layout.Controls.Add(WrapLightBorderTextBox(_txtRegPassword, "Nhập mật khẩu", password: true), 0, r++);
        _txtRegPassword.TabIndex = 2;

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Xác nhận mật khẩu", topPad: 5), 0, r++);
        RowAbs(36);
        layout.Controls.Add(WrapLightBorderTextBox(_txtRegConfirm, "Nhập lại mật khẩu", password: true), 0, r++);
        _txtRegConfirm.TabIndex = 3;

        RowAbs(56);
        _btnRegister.Text = "Tạo tài khoản";
        _btnRegister.Dock = DockStyle.Fill;
        _btnRegister.Margin = new Padding(0, 12, 0, 0);
        _btnRegister.TabIndex = 4;
        StylePrimaryButton(_btnRegister);
        _btnRegister.Click += HandleRegisterStub;
        layout.Controls.Add(_btnRegister, 0, r++);

        RowAuto();
        StyleSecondaryLink(_lnkBackFromRegister, "← Quay lại đăng nhập");
        _lnkBackFromRegister.Margin = new Padding(0, 8, 0, 0);
        _lnkBackFromRegister.TabIndex = 5;
        _lnkBackFromRegister.Click += (_, _) => ShowAuthView(AuthView.Login);
        layout.Controls.Add(_lnkBackFromRegister, 0, r++);

        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, r++);

        _pnlRegister.Controls.Add(layout);
    }

    private void BuildForgotPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 7,
            BackColor = AuthLoginTheme.CardSurface,
            AutoScroll = false
        };

        int r = 0;
        void RowAuto() => layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        void RowAbs(float h) => layout.RowStyles.Add(new RowStyle(SizeType.Absolute, h));

        RowAuto();
        layout.Controls.Add(new Label
        {
            Text = "Khôi phục mật khẩu",
            AutoSize = true,
            ForeColor = AuthLoginTheme.Navy,
            Font = AuthLoginTheme.CardTitleFont(),
            Margin = new Padding(0, 0, 0, 8)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(new Label
        {
            Text = "Nhập tên đăng nhập hoặc email đã đăng ký. Phiên bản thật sẽ gửi hướng dẫn qua email.",
            AutoSize = false,
            Dock = DockStyle.Fill,
            Height = 42,
            ForeColor = AuthLoginTheme.MutedText,
            Font = AuthLoginTheme.BodyFont(),
            Margin = new Padding(0, 0, 0, 8)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập hoặc email"), 0, r++);

        RowAbs(40);
        layout.Controls.Add(WrapLightBorderTextBox(_txtForgotIdentity, "Nhập tên đăng nhập hoặc email", password: false), 0, r++);
        _txtForgotIdentity.TabIndex = 0;

        RowAbs(60);
        _btnForgotSend.Text = "Gửi yêu cầu khôi phục";
        _btnForgotSend.Dock = DockStyle.Fill;
        _btnForgotSend.Margin = new Padding(0, 16, 0, 0);
        _btnForgotSend.TabIndex = 1;
        StylePrimaryButton(_btnForgotSend);
        _btnForgotSend.Click += HandleForgotStub;
        layout.Controls.Add(_btnForgotSend, 0, r++);

        RowAuto();
        StyleSecondaryLink(_lnkBackFromForgot, "← Quay lại đăng nhập");
        _lnkBackFromForgot.Margin = new Padding(0, 12, 0, 0);
        _lnkBackFromForgot.TabIndex = 2;
        _lnkBackFromForgot.Click += (_, _) => ShowAuthView(AuthView.Login);
        layout.Controls.Add(_lnkBackFromForgot, 0, r++);

        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, r++);

        _pnlForgot.Controls.Add(layout);
    }

    private static Panel WrapLightBorderTextBox(TextBox textBox, string placeholder, bool password)
    {
        textBox.BorderStyle = BorderStyle.None;
        textBox.BackColor = Color.White;
        textBox.PlaceholderText = placeholder;
        textBox.UseSystemPasswordChar = password;
        textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;

        var innerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };
        
        innerPanel.Layout += (s, e) => 
        {
            textBox.Width = innerPanel.Width - 24;
            textBox.Left = 12;
            textBox.Top = (innerPanel.Height - textBox.Height) / 2;
        };
        innerPanel.Controls.Add(textBox);

        var borderPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AuthLoginTheme.Border,
            Padding = new Padding(1)
        };
        borderPanel.Controls.Add(innerPanel);
        return borderPanel;
    }

    private static Label MakeFieldLabel(string text, int topPad = 0) =>
        new()
        {
            Text = text,
            AutoSize = true,
            ForeColor = AuthLoginTheme.Navy,
            Font = AuthLoginTheme.FieldLabelFont(),
            Margin = new Padding(0, topPad, 0, 3)
        };

    private static void StyleSecondaryLink(LinkLabel link, string text)
    {
        link.Text = text;
        link.AutoSize = true;
        link.LinkBehavior = LinkBehavior.HoverUnderline;
        link.LinkColor = AuthLoginTheme.MutedText;
        link.ActiveLinkColor = AuthLoginTheme.PrimaryBlue;
        link.VisitedLinkColor = AuthLoginTheme.MutedText;
        link.DisabledLinkColor = AuthLoginTheme.MutedText;
        link.BackColor = Color.Transparent;
        link.TabStop = true;
    }

    private static void StylePrimaryButton(Button b)
    {
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderSize = 0;
        b.BackColor = AuthLoginTheme.PrimaryBlue;
        b.ForeColor = Color.White;
        b.Font = AuthLoginTheme.PrimaryButtonFont();
        b.Cursor = Cursors.Hand;
        b.UseVisualStyleBackColor = false;
    }

    private void LoadBrandingImage()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Assets", "login-branding.png");
            if (!File.Exists(path))
            {
                return;
            }

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var img = Image.FromStream(fs);
            _backgroundPanel.BackgroundImage?.Dispose();
            _backgroundPanel.BackgroundImage = new Bitmap(img);
        }
        catch
        {
            // Asset missing or unreadable
        }
    }

    private void ShowAuthView(AuthView view)
    {
        _pnlLogin.Visible = view == AuthView.Login;
        _pnlRegister.Visible = view == AuthView.Register;
        _pnlForgot.Visible = view == AuthView.ForgotPassword;

        AcceptButton = view switch
        {
            AuthView.Login => _btnLogin,
            AuthView.Register => _btnRegister,
            AuthView.ForgotPassword => _btnForgotSend,
            _ => _btnLogin
        };

        switch (view)
        {
            case AuthView.Login:
                _txtLoginUser.Focus();
                break;
            case AuthView.Register:
                _txtRegFullName.Focus();
                break;
            case AuthView.ForgotPassword:
                _txtForgotIdentity.Focus();
                break;
        }
    }

    private void HandleLogin(object? sender, EventArgs e)
    {
        var result = _authService.Authenticate(_txtLoginUser.Text, _txtLoginPassword.Text);
        if (!result.Success || result.Data is null)
        {
            MessageBox.Show(result.Message, "Đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        SaveLoginPrefs();
        Hide();
        using var main = new FrmMain(result.Data);
        main.ShowDialog(this);
        Close();
    }

    private void HandleRegisterStub(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtRegFullName.Text)
            || string.IsNullOrWhiteSpace(_txtRegUsername.Text)
            || string.IsNullOrWhiteSpace(_txtRegPassword.Text)
            || string.IsNullOrWhiteSpace(_txtRegConfirm.Text))
        {
            MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!string.Equals(_txtRegPassword.Text, _txtRegConfirm.Text, StringComparison.Ordinal))
        {
            MessageBox.Show("Mật khẩu xác nhận không khớp.", "Đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show(
            "Phiên bản demo: Chưa có tạo tài khoản tự phục vụ qua cơ sở dữ liệu. "
            + "Quản trị viên có thể tạo tài khoản trên màn hình Quản trị. "
            + "Yêu cầu đăng ký của bạn được ghi nhận (mô phỏng).",
            "Đăng ký tài khoản",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        ShowAuthView(AuthView.Login);
    }

    private void HandleForgotStub(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtForgotIdentity.Text))
        {
            MessageBox.Show("Vui lòng nhập tên đăng nhập hoặc email.", "Khôi phục mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        MessageBox.Show(
            "Phiên bản demo: Không gửi email thật. Vui lòng liên hệ quản trị viên để đặt lại mật khẩu, "
            + "hoặc dùng tài khoản demo theo tài liệu hướng dẫn dự án.",
            "Khôi phục mật khẩu",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        ShowAuthView(AuthView.Login);
    }

    private void ApplyRememberedLoginPrefs()
    {
        var prefs = ReadLoginPrefs();
        if (prefs?.RememberMe == true && !string.IsNullOrWhiteSpace(prefs.Username))
        {
            _chkRemember.Checked = true;
            _txtLoginUser.Text = prefs.Username;

            var password = UnprotectPassword(prefs.ProtectedPassword);
            if (!string.IsNullOrEmpty(password))
            {
                _txtLoginPassword.Text = password;
                _hasRememberedPassword = true;
            }
        }
    }

    private void SaveLoginPrefs()
    {
        try
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuanLyKhoBanHang");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "login_prefs.json");

            if (!_chkRemember.Checked)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return;
            }

            var prefs = new LoginPrefs(
                RememberMe: true,
                Username: _txtLoginUser.Text.Trim(),
                ProtectedPassword: ProtectPassword(_txtLoginPassword.Text));

            File.WriteAllText(path, JsonSerializer.Serialize(prefs));
        }
        catch
        {
            // Non-critical
        }
    }

    private static LoginPrefs? ReadLoginPrefs()
    {
        try
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuanLyKhoBanHang", "login_prefs.json");
            if (!File.Exists(path))
            {
                return null;
            }

            return JsonSerializer.Deserialize<LoginPrefs>(File.ReadAllText(path));
        }
        catch
        {
            return null;
        }
    }

    private static string ProtectPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return string.Empty;
        }

        var protectedBytes = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(password),
            LoginPrefsEntropy,
            DataProtectionScope.CurrentUser);

        return Convert.ToBase64String(protectedBytes);
    }

    private static string? UnprotectPassword(string? protectedPassword)
    {
        if (string.IsNullOrWhiteSpace(protectedPassword))
        {
            return null;
        }

        try
        {
            var bytes = Convert.FromBase64String(protectedPassword);
            var plainBytes = ProtectedData.Unprotect(bytes, LoginPrefsEntropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch
        {
            return null;
        }
    }

    private static readonly byte[] LoginPrefsEntropy = Encoding.UTF8.GetBytes("QuanLyKhoBanHang.WinForms.LoginPrefs.v1");

    private sealed record LoginPrefs(bool RememberMe, string? Username, string? ProtectedPassword);

    private enum AuthView
    {
        Login,
        Register,
        ForgotPassword
    }

    private sealed class RoundedPanel : Panel
    {
        public int CornerRadius { get; init; } = 8;

        public Color BorderColor { get; init; } = Color.Transparent;

        public RoundedPanel()
        {
            SetStyle(
                ControlStyles.UserPaint
                    | ControlStyles.AllPaintingInWmPaint
                    | ControlStyles.OptimizedDoubleBuffer
                    | ControlStyles.ResizeRedraw
                    | ControlStyles.SupportsTransparentBackColor,
                true);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using var path = CreateRoundedPath(new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius);
            using var brush = new SolidBrush(BackColor);
            e.Graphics.FillPath(brush, path);

            if (BorderColor != Color.Transparent)
            {
                using var pen = new Pen(BorderColor);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void UpdateRegion()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            using var path = CreateRoundedPath(new Rectangle(0, 0, Width, Height), CornerRadius);
            Region?.Dispose();
            Region = new Region(path);
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var diameter = Math.Max(1, radius * 2);

            if (diameter >= bounds.Width || diameter >= bounds.Height)
            {
                path.AddEllipse(bounds);
                path.CloseFigure();
                return path;
            }

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _backgroundPanel.BackgroundImage?.Dispose();
        }

        base.Dispose(disposing);
    }
}
