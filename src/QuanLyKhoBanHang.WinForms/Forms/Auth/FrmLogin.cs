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
        Width = 480;
        Height = 410;
        MinimumSize = new Size(480, 410);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = AppTheme.AppBackground;
        Font = AppTheme.BodyFont();

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30),
            ColumnCount = 1,
            RowCount = 9
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 14));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Text = "Quản lý kho & bán hàng",
            Dock = DockStyle.Fill,
            Font = AppTheme.TitleFont(16F),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var demoHint = new Label
        {
            Text = "Demo: admin/admin123, manager/123456, du/123456, hung/123456",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
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

        panel.Controls.Add(title, 0, 0);
        panel.Controls.Add(demoHint, 0, 1);
        panel.Controls.Add(new Label { Text = "Tên đăng nhập", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 2);
        panel.Controls.Add(_txtUsername, 0, 3);
        panel.Controls.Add(new Label { Text = "Mật khẩu", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 4);
        panel.Controls.Add(_txtPassword, 0, 5);
        panel.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, 6);
        panel.Controls.Add(loginButton, 0, 7);

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
        using var main = new Main.FrmMain(result.Data);
        main.ShowDialog(this);
        Close();
    }
}
