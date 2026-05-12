using QuanLyKhoBanHang.BLL.Services;

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
        Width = 420;
        Height = 300;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(28),
            ColumnCount = 1,
            RowCount = 6
        };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        var title = new Label
        {
            Text = "Quản lý kho & bán hàng",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };

        _txtUsername.Dock = DockStyle.Fill;
        _txtUsername.Text = "admin";
        _txtPassword.Dock = DockStyle.Fill;
        _txtPassword.Text = "admin123";
        _txtPassword.UseSystemPasswordChar = true;

        var loginButton = new Button
        {
            Text = "Đăng nhập",
            Dock = DockStyle.Fill,
            Height = 40
        };
        loginButton.Click += HandleLogin;

        panel.Controls.Add(title);
        panel.Controls.Add(new Label { Text = "Tên đăng nhập", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft });
        panel.Controls.Add(_txtUsername);
        panel.Controls.Add(new Label { Text = "Mật khẩu", Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft });
        panel.Controls.Add(_txtPassword);
        panel.Controls.Add(loginButton);

        Controls.Add(panel);
        AcceptButton = loginButton;
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
