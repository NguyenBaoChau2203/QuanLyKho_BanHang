using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Auth;

public sealed class FrmChangePassword : Form
{
    private readonly UserAccountService _userAccountService = new();
    private readonly int _userId;
    private readonly string _currentPassword;

    private readonly TextBox _txtNewPassword = new();
    private readonly TextBox _txtConfirmPassword = new();
    private readonly IconButton _btnSave;
    private readonly IconButton _btnCancel;

    public FrmChangePassword(int userId, string currentPassword)
    {
        _userId = userId;
        _currentPassword = currentPassword;

        Text = "Yêu cầu đổi mật khẩu";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(400, 320);
        BackColor = AppTheme.Surface;
        Font = AppTheme.BodyFont();

        _txtNewPassword.UseSystemPasswordChar = true;
        _txtConfirmPassword.UseSystemPasswordChar = true;

        _btnSave = UiFactory.IconActionButton("Đổi mật khẩu", FontAwesome.Sharp.IconChar.FloppyDisk, HandleSave, 140);
        _btnCancel = UiFactory.IconActionButton("Hủy", FontAwesome.Sharp.IconChar.Xmark, (_, _) => DialogResult = DialogResult.Cancel, 90);

        BuildUi();
        
        AcceptButton = _btnSave;
        CancelButton = _btnCancel;
    }

    private void BuildUi()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Padding = new Padding(20)
        };

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var lblHeader = new Label
        {
            Text = "Tài khoản của bạn cần được đổi mật khẩu\ntrước khi tiếp tục sử dụng hệ thống.",
            AutoSize = true,
            ForeColor = AppTheme.Warning,
            Font = AppTheme.BodyFont(9.5f),
            Margin = new Padding(0, 0, 0, 16)
        };

        layout.Controls.Add(lblHeader, 0, 0);

        layout.Controls.Add(new Label { Text = "Mật khẩu mới", AutoSize = true, ForeColor = AppTheme.Text, Margin = new Padding(0, 0, 0, 4) }, 0, 1);
        _txtNewPassword.Dock = DockStyle.Fill;
        _txtNewPassword.BorderStyle = BorderStyle.FixedSingle;
        _txtNewPassword.Font = AppTheme.BodyFont(10);
        layout.Controls.Add(_txtNewPassword, 0, 2);

        layout.Controls.Add(new Label { Text = "Xác nhận mật khẩu", AutoSize = true, ForeColor = AppTheme.Text, Margin = new Padding(0, 8, 0, 4) }, 0, 3);
        _txtConfirmPassword.Dock = DockStyle.Fill;
        _txtConfirmPassword.BorderStyle = BorderStyle.FixedSingle;
        _txtConfirmPassword.Font = AppTheme.BodyFont(10);
        layout.Controls.Add(_txtConfirmPassword, 0, 4);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            Margin = new Padding(0, 20, 0, 0)
        };

        buttonPanel.Controls.Add(_btnCancel);
        buttonPanel.Controls.Add(_btnSave);

        layout.Controls.Add(buttonPanel, 0, 5);
        Controls.Add(layout);
    }

    private void HandleSave(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtNewPassword.Text))
        {
            MessageBox.Show("Vui lòng nhập mật khẩu mới.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtNewPassword.Focus();
            return;
        }

        if (_txtNewPassword.Text != _txtConfirmPassword.Text)
        {
            MessageBox.Show("Mật khẩu xác nhận không khớp.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _txtConfirmPassword.Focus();
            return;
        }

        var result = _userAccountService.ChangePassword(_userId, _currentPassword, _txtNewPassword.Text);

        if (!result.Success)
        {
            MessageBox.Show(result.Message, "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        MessageBox.Show(result.Message, "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
    }
}
