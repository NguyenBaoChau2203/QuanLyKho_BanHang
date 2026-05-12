using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.Sales;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.Sales;

public sealed class FrmCustomer : CrudListForm<CustomerDto>
{
    private readonly CustomerService _service = new();
    private List<CustomerDto> _items = [];
    private List<CustomerDto> _filteredItems = [];

    public FrmCustomer() : base("Khách hàng", "Quản lý khách hàng, thông tin liên hệ và trạng thái hoạt động.")
    {
        DescriptionBox.Multiline = true;
        Grid.AutoGenerateColumns = false;
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Code), HeaderText = "Mã", FillWeight = 18 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Name), HeaderText = "Tên", FillWeight = 26 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Phone), HeaderText = "Điện thoại", FillWeight = 16 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Email), HeaderText = "Email", FillWeight = 20 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CustomerDto.Address), HeaderText = "Địa chỉ", FillWeight = 20 });
        Grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(CustomerDto.IsActive), HeaderText = "Hoạt động", FillWeight = 10 });
        RefreshData();
        ToggleEditing(false);
    }

    protected override void RefreshData()
    {
        var result = _service.GetAllCustomers();
        _items = result.Success && result.Data is { Count: > 0 } ? result.Data! : CreateStubItems();
        ApplyFilter();
        SetMessage(result.Message);
    }

    protected override void ApplyFilter()
    {
        var keyword = SearchBox.Text.Trim();
        _filteredItems = string.IsNullOrWhiteSpace(keyword)
            ? _items.ToList()
            : _items.Where(x => x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || (x.Phone?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        BindingSource.DataSource = new BindingList<CustomerDto>(_filteredItems);
        ShowEmpty(_filteredItems.Count == 0);
    }

    protected override void OnSelectionChanged()
    {
        if (BindingSource.Current is not CustomerDto item) return;
        SelectedId = item.Id;
        CodeBox.Text = item.Code;
        NameBox.Text = item.Name;
        DescriptionBox.Text = string.Join(Environment.NewLine, new[] { $"Điện thoại: {item.Phone}", $"Email: {item.Email}", $"Địa chỉ: {item.Address}" }.Where(x => !x.EndsWith(": ")));
        ActiveBox.Checked = item.IsActive;
    }

    protected override void BeginAdd() => LoadEditor(new CustomerDto { IsActive = true }, "Đang thêm khách hàng mới.");
    protected override void BeginEdit() => LoadEditor(GetCurrentItem(), "Đang chỉnh sửa khách hàng.");

    protected override void SaveCurrent()
    {
        if (!IsEditing)
        {
            SetMessage("Hãy bấm Thêm hoặc Sửa trước khi lưu.", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(CodeBox.Text) || string.IsNullOrWhiteSpace(NameBox.Text))
        {
            SetMessage("Mã và tên khách hàng là bắt buộc.", true);
            return;
        }
        ToggleEditing(false);
        SetMessage(SelectedId <= 0 ? "Đã lưu tạm khách hàng mới trong chế độ stub." : "Đã lưu thay đổi khách hàng trong chế độ stub.");
    }

    protected override void CancelEdit() { ToggleEditing(false); RefreshData(); SetMessage("Đã hủy thay đổi."); }
    protected override void DeactivateCurrent() => SetMessage(SelectedId > 0 ? "Đã ghi nhận yêu cầu ngừng kích hoạt khách hàng." : "Vui lòng chọn khách hàng.", SelectedId <= 0);

    private void LoadEditor(CustomerDto item, string message)
    {
        SelectedId = item.Id;
        CodeBox.Text = item.Code;
        NameBox.Text = item.Name;
        DescriptionBox.Text = string.Join(Environment.NewLine, new[] { item.Phone ?? string.Empty, item.Email ?? string.Empty, item.Address ?? string.Empty });
        ActiveBox.Checked = item.IsActive;
        ToggleEditing(true);
        SetMessage(message);
    }

    private CustomerDto GetCurrentItem() => BindingSource.Current as CustomerDto ?? new CustomerDto { IsActive = true };

    private static List<CustomerDto> CreateStubItems() =>
    [
        new CustomerDto { Id = 1, Code = "KH001", Name = "Khách lẻ", Phone = null, Email = null, Address = null, IsActive = true },
        new CustomerDto { Id = 2, Code = "KH002", Name = "Cửa hàng Tạp hóa An Phú", Phone = "0911111111", Email = "anphu@example.com", Address = "Bình Dương", IsActive = true },
        new CustomerDto { Id = 3, Code = "KH003", Name = "Siêu thị Hòa Bình", Phone = "0988777666", Email = "hoabinh@example.com", Address = "TP. Hồ Chí Minh", IsActive = true }
    ];
}
