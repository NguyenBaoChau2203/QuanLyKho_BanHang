using System.Text.Json;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.WinForms.Forms.Main;

namespace QuanLyKhoBanHang.WinForms.Forms.Auth;

public sealed class FrmLogin : Form
{
    private readonly AuthService _authService = new();

    private readonly PictureBox _brandingPicture = new();
    private readonly Panel _pnlLogin = new();
    private readonly Panel _pnlRegister = new();
    private readonly Panel _pnlForgot = new();

    private readonly TextBox _txtLoginUser = new();
    private readonly TextBox _txtLoginPassword = new();
    private readonly CheckBox _chkRemember = new();
    private readonly Button _btnLogin = new();
    private readonly Button _btnTogglePassword = new();
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

    private bool _passwordVisible;

    public FrmLogin()
    {
        Text = "Đăng nhập - Quản lý kho & bán hàng";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        ClientSize = new Size(860, 560);
        MinimumSize = new Size(860, 560);
        BackColor = AuthLoginTheme.FormBackground;
        Font = AuthLoginTheme.BodyFont();
        DoubleBuffered = true;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(20)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));

        _brandingPicture.Dock = DockStyle.Fill;
        _brandingPicture.SizeMode = PictureBoxSizeMode.Zoom;
        _brandingPicture.Margin = new Padding(0, 0, 10, 0);
        _brandingPicture.BorderStyle = BorderStyle.None;
        _brandingPicture.BackColor = AuthLoginTheme.FormBackground;
        Load += (_, _) => LoadBrandingImage();

        var rightHost = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = new Padding(10, 0, 0, 0),
            BackColor = AuthLoginTheme.FormBackground
        };
        rightHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        rightHost.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 384F));
        rightHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        rightHost.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = AuthLoginTheme.FormBackground }, 0, 0);
        rightHost.Controls.Add(BuildCardHost(), 1, 0);
        rightHost.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = AuthLoginTheme.FormBackground }, 2, 0);

        root.Controls.Add(_brandingPicture, 0, 0);
        root.Controls.Add(rightHost, 1, 0);
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
            Padding = new Padding(0, 0, 3, 3),
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
            Padding = new Padding(24)
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
            RowCount = 9,
            AutoScroll = true
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
            Margin = new Padding(0, 0, 0, 8)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập"), 0, r++);

        RowAbs(38);
        _txtLoginUser.Dock = DockStyle.Fill;
        _txtLoginUser.BorderStyle = BorderStyle.FixedSingle;
        _txtLoginUser.PlaceholderText = "Nhập tên đăng nhập";
        _txtLoginUser.TabIndex = 0;
        layout.Controls.Add(_txtLoginUser, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Mật khẩu", topPad: 10), 0, r++);

        RowAbs(38);
        var passRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0)
        };
        passRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        passRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56F));

        _txtLoginPassword.Dock = DockStyle.Fill;
        _txtLoginPassword.BorderStyle = BorderStyle.FixedSingle;
        _txtLoginPassword.PlaceholderText = "Nhập mật khẩu";
        _txtLoginPassword.UseSystemPasswordChar = true;
        _txtLoginPassword.TabIndex = 1;
        passRow.Controls.Add(_txtLoginPassword, 0, 0);

        _btnTogglePassword.Text = "Hiện";
        _btnTogglePassword.Dock = DockStyle.Fill;
        _btnTogglePassword.Margin = new Padding(6, 0, 0, 0);
        _btnTogglePassword.TabIndex = 2;
        _btnTogglePassword.FlatStyle = FlatStyle.Flat;
        _btnTogglePassword.FlatAppearance.BorderColor = AuthLoginTheme.Border;
        _btnTogglePassword.FlatAppearance.BorderSize = 1;
        _btnTogglePassword.BackColor = AuthLoginTheme.CardSurface;
        _btnTogglePassword.ForeColor = AuthLoginTheme.MutedText;
        _btnTogglePassword.Font = AuthLoginTheme.BodyFont();
        _btnTogglePassword.Cursor = Cursors.Hand;
        _btnTogglePassword.Click += ToggleLoginPasswordVisibility;
        passRow.Controls.Add(_btnTogglePassword, 1, 0);
        layout.Controls.Add(passRow, 0, r++);

        RowAuto();
        _chkRemember.Text = "Nhớ đăng nhập";
        _chkRemember.AutoSize = true;
        _chkRemember.Margin = new Padding(0, 12, 0, 0);
        _chkRemember.ForeColor = AuthLoginTheme.MutedText;
        _chkRemember.TabIndex = 3;
        _chkRemember.FlatStyle = FlatStyle.Flat;
        layout.Controls.Add(_chkRemember, 0, r++);

        RowAbs(44);
        _btnLogin.Text = "Đăng nhập";
        _btnLogin.Dock = DockStyle.Fill;
        _btnLogin.Margin = new Padding(0, 16, 0, 0);
        _btnLogin.TabIndex = 4;
        StylePrimaryButton(_btnLogin);
        _btnLogin.Click += HandleLogin;
        layout.Controls.Add(_btnLogin, 0, r++);

        RowAuto();
        var links = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 14, 0, 0)
        };
        StyleLink(_lnkRegister, "Đăng ký tài khoản");
        _lnkRegister.Margin = new Padding(0, 2, 0, 0);
        _lnkRegister.TabIndex = 5;
        _lnkRegister.Click += (_, _) => ShowAuthView(AuthView.Register);
        links.Controls.Add(_lnkRegister);
        links.Controls.Add(new Label
        {
            Text = "|",
            AutoSize = true,
            ForeColor = AuthLoginTheme.Border,
            Margin = new Padding(10, 2, 10, 0)
        });
        StyleLink(_lnkForgot, "Quên mật khẩu?");
        _lnkForgot.Margin = new Padding(0, 2, 0, 0);
        _lnkForgot.TabIndex = 6;
        _lnkForgot.Click += (_, _) => ShowAuthView(AuthView.ForgotPassword);
        links.Controls.Add(_lnkForgot);
        layout.Controls.Add(links, 0, r++);

        RowAbs(56);
        var demoPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 16, 0, 0),
            BackColor = Color.FromArgb(240, 247, 255),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(10, 8, 10, 8)
        };
        demoPanel.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = "Tài khoản demo: admin/admin123 · manager/123456 · du/123456 · hung/123456",
            ForeColor = AuthLoginTheme.MutedText,
            Font = AuthLoginTheme.DemoHintFont(),
            TextAlign = ContentAlignment.MiddleLeft
        });
        layout.Controls.Add(demoPanel, 0, r++);

        _pnlLogin.Controls.Add(layout);
    }

    private void BuildRegisterPanel()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 12,
            AutoScroll = true
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
            Height = 42,
            ForeColor = AuthLoginTheme.MutedText,
            Font = AuthLoginTheme.BodyFont(),
            Margin = new Padding(0, 0, 0, 4)
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Họ và tên"), 0, r++);
        RowAbs(38);
        StyleTextBox(_txtRegFullName, "Nhập họ và tên", password: false);
        _txtRegFullName.TabIndex = 0;
        layout.Controls.Add(_txtRegFullName, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập", topPad: 8), 0, r++);
        RowAbs(38);
        StyleTextBox(_txtRegUsername, "Chọn tên đăng nhập", password: false);
        _txtRegUsername.TabIndex = 1;
        layout.Controls.Add(_txtRegUsername, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Mật khẩu", topPad: 8), 0, r++);
        RowAbs(38);
        StyleTextBox(_txtRegPassword, "Nhập mật khẩu", password: true);
        _txtRegPassword.TabIndex = 2;
        layout.Controls.Add(_txtRegPassword, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Xác nhận mật khẩu", topPad: 8), 0, r++);
        RowAbs(38);
        StyleTextBox(_txtRegConfirm, "Nhập lại mật khẩu", password: true);
        _txtRegConfirm.TabIndex = 3;
        layout.Controls.Add(_txtRegConfirm, 0, r++);

        RowAbs(44);
        _btnRegister.Text = "Tạo tài khoản";
        _btnRegister.Dock = DockStyle.Fill;
        _btnRegister.Margin = new Padding(0, 16, 0, 0);
        _btnRegister.TabIndex = 4;
        StylePrimaryButton(_btnRegister);
        _btnRegister.Click += HandleRegisterStub;
        layout.Controls.Add(_btnRegister, 0, r++);

        RowAuto();
        StyleLink(_lnkBackFromRegister, "← Quay lại đăng nhập");
        _lnkBackFromRegister.Margin = new Padding(0, 12, 0, 0);
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
            AutoScroll = true
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
            Height = 44,
            ForeColor = AuthLoginTheme.MutedText,
            Font = AuthLoginTheme.BodyFont()
        }, 0, r++);

        RowAuto();
        layout.Controls.Add(MakeFieldLabel("Tên đăng nhập hoặc email"), 0, r++);

        RowAbs(38);
        _txtForgotIdentity.Dock = DockStyle.Fill;
        _txtForgotIdentity.BorderStyle = BorderStyle.FixedSingle;
        _txtForgotIdentity.PlaceholderText = "Nhập tên đăng nhập hoặc email";
        _txtForgotIdentity.TabIndex = 0;
        layout.Controls.Add(_txtForgotIdentity, 0, r++);

        RowAbs(44);
        _btnForgotSend.Text = "Gửi yêu cầu khôi phục";
        _btnForgotSend.Dock = DockStyle.Fill;
        _btnForgotSend.Margin = new Padding(0, 16, 0, 0);
        _btnForgotSend.TabIndex = 1;
        StylePrimaryButton(_btnForgotSend);
        _btnForgotSend.Click += HandleForgotStub;
        layout.Controls.Add(_btnForgotSend, 0, r++);

        RowAuto();
        StyleLink(_lnkBackFromForgot, "← Quay lại đăng nhập");
        _lnkBackFromForgot.Margin = new Padding(0, 12, 0, 0);
        _lnkBackFromForgot.TabIndex = 2;
        _lnkBackFromForgot.Click += (_, _) => ShowAuthView(AuthView.Login);
        layout.Controls.Add(_lnkBackFromForgot, 0, r++);

        _pnlForgot.Controls.Add(layout);
    }

    private static Label MakeFieldLabel(string text, int topPad = 0) =>
        new()
        {
            Text = text,
            AutoSize = true,
            ForeColor = AuthLoginTheme.Navy,
            Font = AuthLoginTheme.FieldLabelFont(),
            Margin = new Padding(0, topPad, 0, 4)
        };

    private static void StyleTextBox(TextBox box, string placeholder, bool password)
    {
        box.Dock = DockStyle.Fill;
        box.BorderStyle = BorderStyle.FixedSingle;
        box.PlaceholderText = placeholder;
        box.UseSystemPasswordChar = password;
    }

    private static void StyleLink(LinkLabel link, string text)
    {
        link.Text = text;
        link.AutoSize = true;
        link.LinkBehavior = LinkBehavior.HoverUnderline;
        link.LinkColor = AuthLoginTheme.PrimaryBlue;
        link.ActiveLinkColor = AuthLoginTheme.Navy;
        link.VisitedLinkColor = AuthLoginTheme.PrimaryBlue;
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

    private void ToggleLoginPasswordVisibility(object? sender, EventArgs e)
    {
        _passwordVisible = !_passwordVisible;
        _txtLoginPassword.UseSystemPasswordChar = !_passwordVisible;
        _btnTogglePassword.Text = _passwordVisible ? "Ẩn" : "Hiện";
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
            + "hoặc dùng tài khoản demo được cung cấp trên màn hình đăng nhập.",
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
