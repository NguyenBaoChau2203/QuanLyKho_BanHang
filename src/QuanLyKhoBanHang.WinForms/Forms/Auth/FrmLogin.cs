using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Auth;

public sealed class FrmLogin : Form
{
    private readonly AuthService _authService = new();
    private readonly TextBox _txtUsername = new();
    private readonly TextBox _txtPassword = new();

    public FrmLogin()
    {
        Text = "Đăng nhập - Quản lý kho bán hàng";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 440;
        Height = 340;
        MinimumSize = new Size(440, 340);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(28),
            ColumnCount = 1,
            RowCount = 7
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 14));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        var title = new Label
        {
            Text = "Quản lý kho & bán hàng",
            Dock = DockStyle.Fill,
            Font = AppTheme.TitleFont(16F),
            TextAlign = ContentAlignment.MiddleCenter
        };

        _txtUsername.Dock = DockStyle.Fill;
        _txtUsername.Text = "admin";
        _txtUsername.PlaceholderText = "Nhập tên đăng nhập";
        _txtUsername.TabIndex = 0;

        _txtPassword.Dock = DockStyle.Fill;
        _txtPassword.Text = "admin123";
        _txtPassword.PlaceholderText = "Nhập mật khẩu";
        _txtPassword.UseSystemPasswordChar = true;
        _txtPassword.TabIndex = 1;

        var loginButton = new Button
        {
            Text = "Đăng nhập",
            Dock = DockStyle.Fill,
            Height = 40,
            TabIndex = 2
        };
        loginButton.Click += HandleLogin;

        panel.Controls.Add(title);
        panel.Controls.Add(new Label { Text = "Tên đăng nhập", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft });
        panel.Controls.Add(_txtUsername);
        panel.Controls.Add(new Label { Text = "Mật khẩu", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft });
        panel.Controls.Add(_txtPassword);
        panel.Controls.Add(new Panel { Dock = DockStyle.Fill });
        panel.Controls.Add(new Panel { Dock = DockStyle.Fill });
        panel.Controls.Add(loginButton);

        Controls.Add(panel);
        AcceptButton = loginButton;
        Shown += (_, _) => _txtUsername.Focus();
    }

    private void HandleLogin(object? sender, EventArgs e)
    {
        var result = _authService.Authenticate(_txtUsername.Text, _txtPassword.Text);
        if (!result.Success || result.Data is null)
        {
            MessageBox.Show(result.Message, "Đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Hide();
        using var main = new Main.FrmMain(result.Data.FullName);
        main.ShowDialog(this);
        Close();
    }
}
