using System.Text.Json;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.WinForms.Forms.Main;

namespace QuanLyKhoBanHang.WinForms.Forms.Auth;

public sealed class FrmLogin : Form
{
    private const int AuthCardWidth = 380;
    private const int AuthCardHeight = 450;

    private readonly AuthService _authService = new();

    private readonly PictureBox _brandingPicture = new();
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

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(24)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46F));

        _brandingPicture.Dock = DockStyle.Fill;
        _brandingPicture.SizeMode = PictureBoxSizeMode.Zoom;
        _brandingPicture.Margin = new Padding(12, 12, 12, 12);
        _brandingPicture.BorderStyle = BorderStyle.None;
        _brandingPicture.BackColor = AuthLoginTheme.FormBackground;
        Load += (_, _) => LoadBrandingImage();

        var rightCenter = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3,
            BackColor = AuthLoginTheme.FormBackground,
            Margin = new Padding(0)
        };
        rightCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        rightCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AuthCardWidth));
        rightCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        rightCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        rightCenter.RowStyles.Add(new RowStyle(SizeType.Absolute, AuthCardHeight));
        rightCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        void AddSpacer(int col, int row)
        {
            rightCenter.Controls.Add(new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AuthLoginTheme.FormBackground
            }, col, row);
        }

        AddSpacer(0, 0);
        AddSpacer(1, 0);
        AddSpacer(2, 0);
        AddSpacer(0, 1);
        var cardHost = BuildCardHost();
        cardHost.Dock = DockStyle.Fill;
        rightCenter.Controls.Add(cardHost, 1, 1);
        AddSpacer(2, 1);
        AddSpacer(0, 2);
        AddSpacer(1, 2);
        AddSpacer(2, 2);

        root.Controls.Add(_brandingPicture, 0, 0);
        root.Controls.Add(rightCenter, 1, 0);
        Controls.Add(root);

        BuildLoginPanel();
        BuildRegisterPanel();
        BuildForgotPanel();

        ApplyRememberedUsername();
        ShowAuthView(AuthView.Login);
        Shown += (_, _) => _txtLoginUser.Focus();
    }

    private Panel BuildCardHost()
    {
        var shadow = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AuthLoginTheme.ShadowTint,
            Padding = new Padding(0, 0, 2, 2),
            Margin = new Padding(0)
        };

        var border = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AuthLoginTheme.Border,
            Padding = new Padding(1)
        };

        var inner = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AuthLoginTheme.CardSurface,
            Padding = new Padding(28),
            AutoScroll = true
        };

        _pnlLogin.Dock = DockStyle.Fill;
        _pnlRegister.Dock = DockStyle.Fill;
        _pnlForgot.Dock = DockStyle.Fill;
        inner.Controls.Add(_pnlLogin);
        inner.Controls.Add(_pnlRegister);
        inner.Controls.Add(_pnlForgot);

        border.Controls.Add(inner);
        shadow.Controls.Add(border);
        return shadow;
    }

    private void BuildLoginPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 8,
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
            Margin = new Padding(0, 0, 0, 6)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập"), 0, r++);

        RowAbs(38);
        layout.Controls.Add(WrapLightBorderTextBox(_txtLoginUser, "Nhập tên đăng nhập", password: false), 0, r++);
        _txtLoginUser.TabIndex = 0;

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Mật khẩu", topPad: 8), 0, r++);

        RowAbs(38);
        layout.Controls.Add(WrapLightBorderTextBox(_txtLoginPassword, "Nhập mật khẩu", password: true), 0, r++);
        _txtLoginPassword.TabIndex = 1;

        RowAuto();
        _chkRemember.Text = "Nhớ đăng nhập";
        _chkRemember.AutoSize = true;
        _chkRemember.Margin = new Padding(0, 10, 0, 0);
        _chkRemember.ForeColor = AuthLoginTheme.MutedText;
        _chkRemember.TabIndex = 2;
        _chkRemember.FlatStyle = FlatStyle.Flat;
        layout.Controls.Add(_chkRemember, 0, r++);

        RowAbs(44);
        _btnLogin.Text = "Đăng nhập";
        _btnLogin.Dock = DockStyle.Fill;
        _btnLogin.Margin = new Padding(0, 14, 0, 0);
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
            Margin = new Padding(0, 12, 0, 0),
            BackColor = Color.Transparent
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

        _pnlLogin.Controls.Add(layout);
    }

    private void BuildRegisterPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 12,
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

        RowAuto();
        layout.Controls.Add(new Label
        {
            Text = "Tài khoản mới cần quản trị viên phê duyệt và phân quyền trước khi đăng nhập.",
            AutoSize = false,
            Height = 40,
            ForeColor = AuthLoginTheme.MutedText,
            Font = AuthLoginTheme.BodyFont(),
            Margin = new Padding(0, 0, 0, 4)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Họ và tên"), 0, r++);
        RowAbs(38);
        layout.Controls.Add(WrapLightBorderTextBox(_txtRegFullName, "Nhập họ và tên", password: false), 0, r++);
        _txtRegFullName.TabIndex = 0;

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập", topPad: 6), 0, r++);
        RowAbs(38);
        layout.Controls.Add(WrapLightBorderTextBox(_txtRegUsername, "Chọn tên đăng nhập", password: false), 0, r++);
        _txtRegUsername.TabIndex = 1;

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Mật khẩu", topPad: 6), 0, r++);
        RowAbs(38);
        layout.Controls.Add(WrapLightBorderTextBox(_txtRegPassword, "Nhập mật khẩu", password: true), 0, r++);
        _txtRegPassword.TabIndex = 2;

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Xác nhận mật khẩu", topPad: 6), 0, r++);
        RowAbs(38);
        layout.Controls.Add(WrapLightBorderTextBox(_txtRegConfirm, "Nhập lại mật khẩu", password: true), 0, r++);
        _txtRegConfirm.TabIndex = 3;

        RowAbs(44);
        _btnRegister.Text = "Tạo tài khoản";
        _btnRegister.Dock = DockStyle.Fill;
        _btnRegister.Margin = new Padding(0, 12, 0, 0);
        _btnRegister.TabIndex = 4;
        StylePrimaryButton(_btnRegister);
        _btnRegister.Click += HandleRegisterStub;
        layout.Controls.Add(_btnRegister, 0, r++);

        RowAuto();
        StyleSecondaryLink(_lnkBackFromRegister, "← Quay lại đăng nhập");
        _lnkBackFromRegister.Margin = new Padding(0, 10, 0, 0);
        _lnkBackFromRegister.TabIndex = 5;
        _lnkBackFromRegister.Click += (_, _) => ShowAuthView(AuthView.Login);
        layout.Controls.Add(_lnkBackFromRegister, 0, r++);

        _pnlRegister.Controls.Add(layout);
    }

    private void BuildForgotPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
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
            Margin = new Padding(0, 0, 0, 6)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(new Label
        {
            Text = "Nhập tên đăng nhập hoặc email đã đăng ký. Phiên bản thật sẽ gửi hướng dẫn qua email.",
            AutoSize = false,
            Height = 42,
            ForeColor = AuthLoginTheme.MutedText,
            Font = AuthLoginTheme.BodyFont()
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập hoặc email"), 0, r++);

        RowAbs(38);
        layout.Controls.Add(WrapLightBorderTextBox(_txtForgotIdentity, "Nhập tên đăng nhập hoặc email", password: false), 0, r++);
        _txtForgotIdentity.TabIndex = 0;

        RowAbs(44);
        _btnForgotSend.Text = "Gửi yêu cầu khôi phục";
        _btnForgotSend.Dock = DockStyle.Fill;
        _btnForgotSend.Margin = new Padding(0, 14, 0, 0);
        _btnForgotSend.TabIndex = 1;
        StylePrimaryButton(_btnForgotSend);
        _btnForgotSend.Click += HandleForgotStub;
        layout.Controls.Add(_btnForgotSend, 0, r++);

        RowAuto();
        StyleSecondaryLink(_lnkBackFromForgot, "← Quay lại đăng nhập");
        _lnkBackFromForgot.Margin = new Padding(0, 10, 0, 0);
        _lnkBackFromForgot.TabIndex = 2;
        _lnkBackFromForgot.Click += (_, _) => ShowAuthView(AuthView.Login);
        layout.Controls.Add(_lnkBackFromForgot, 0, r++);

        _pnlForgot.Controls.Add(layout);
    }

    private static Panel WrapLightBorderTextBox(TextBox textBox, string placeholder, bool password)
    {
        textBox.BorderStyle = BorderStyle.None;
        textBox.BackColor = Color.White;
        textBox.PlaceholderText = placeholder;
        textBox.UseSystemPasswordChar = password;
        textBox.Dock = DockStyle.Fill;

        var host = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AuthLoginTheme.Border,
            Padding = new Padding(1)
        };
        host.Controls.Add(textBox);
        return host;
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
            _brandingPicture.Image?.Dispose();
            _brandingPicture.Image = new Bitmap(img);
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

    private void ApplyRememberedUsername()
    {
        var prefs = ReadLoginPrefs();
        if (prefs?.RememberMe == true && !string.IsNullOrWhiteSpace(prefs.Username))
        {
            _chkRemember.Checked = true;
            _txtLoginUser.Text = prefs.Username;
        }
    }

    private void SaveLoginPrefs()
    {
        try
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuanLyKhoBanHang");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "login_prefs.json");
            var prefs = new LoginPrefs(_chkRemember.Checked, _chkRemember.Checked ? _txtLoginUser.Text.Trim() : string.Empty);
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

    private sealed record LoginPrefs(bool RememberMe, string? Username);

    private enum AuthView
    {
        Login,
        Register,
        ForgotPassword
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _brandingPicture.Image?.Dispose();
        }

        base.Dispose(disposing);
    }
}
