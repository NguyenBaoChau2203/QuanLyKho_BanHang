using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.MasterData;

public sealed class FrmSupplier : CrudListForm<SupplierDto>
{
    private readonly SupplierService _service = new();
    private List<SupplierDto> _items = [];
    private List<SupplierDto> _filteredItems = [];

    public FrmSupplier() : base("Nhà cung cấp", "Quản lý nhà cung cấp, liên hệ và trạng thái hoạt động.")
    {
        DescriptionBox.Multiline = true;
        Grid.AutoGenerateColumns = false;
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SupplierDto.Code), HeaderText = "Mã", FillWeight = 18 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SupplierDto.Name), HeaderText = "Tên", FillWeight = 26 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SupplierDto.Phone), HeaderText = "Điện thoại", FillWeight = 16 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SupplierDto.Email), HeaderText = "Email", FillWeight = 20 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(SupplierDto.Address), HeaderText = "Địa chỉ", FillWeight = 20 });
        Grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(SupplierDto.IsActive), HeaderText = "Hoạt động", FillWeight = 10 });
        RefreshData();
        ToggleEditing(false);
    }

    protected override void RefreshData()
    {
        var result = _service.GetAllSuppliers();
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
        BindingSource.DataSource = new BindingList<SupplierDto>(_filteredItems);
        ShowEmpty(_filteredItems.Count == 0);
    }

    protected override void OnSelectionChanged()
    {
        if (BindingSource.Current is not SupplierDto item) return;
        SelectedId = item.Id;
        CodeBox.Text = item.Code;
        NameBox.Text = item.Name;
        DescriptionBox.Text = string.Join(Environment.NewLine, new[] { $"Điện thoại: {item.Phone}", $"Email: {item.Email}", $"Địa chỉ: {item.Address}" }.Where(x => !x.EndsWith(": ")));
        ActiveBox.Checked = item.IsActive;
    }

    protected override void BeginAdd() => LoadEditor(new SupplierDto { IsActive = true }, "Đang thêm nhà cung cấp mới.");
    protected override void BeginEdit() => LoadEditor(GetCurrentItem(), "Đang chỉnh sửa nhà cung cấp.");

    protected override void SaveCurrent()
    {
        if (!IsEditing)
        {
            SetMessage("Hãy bấm Thêm hoặc Sửa trước khi lưu.", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(CodeBox.Text) || string.IsNullOrWhiteSpace(NameBox.Text))
        {
            SetMessage("Mã và tên nhà cung cấp là bắt buộc.", true);
            return;
        }
        ToggleEditing(false);
        SetMessage(SelectedId <= 0 ? "Đã lưu tạm nhà cung cấp mới trong chế độ stub." : "Đã lưu thay đổi nhà cung cấp trong chế độ stub.");
    }

    protected override void CancelEdit() { ToggleEditing(false); RefreshData(); SetMessage("Đã hủy thay đổi."); }
    protected override void DeactivateCurrent() => SetMessage(SelectedId > 0 ? "Đã ghi nhận yêu cầu ngừng kích hoạt nhà cung cấp." : "Vui lòng chọn nhà cung cấp.", SelectedId <= 0);

    private void LoadEditor(SupplierDto item, string message)
    {
        SelectedId = item.Id;
        CodeBox.Text = item.Code;
        NameBox.Text = item.Name;
        DescriptionBox.Text = string.Join(Environment.NewLine, new[] { item.Phone ?? string.Empty, item.Email ?? string.Empty, item.Address ?? string.Empty });
        ActiveBox.Checked = item.IsActive;
        ToggleEditing(true);
        SetMessage(message);
    }

    private SupplierDto GetCurrentItem() => BindingSource.Current as SupplierDto ?? new SupplierDto { IsActive = true };

    private static List<SupplierDto> CreateStubItems() =>
    [
        new SupplierDto { Id = 1, Code = "NCC-01", Name = "Công ty An Phát", Phone = "0909123456", Email = "anphat@example.com", Address = "TP.HCM", IsActive = true },
        new SupplierDto { Id = 2, Code = "NCC-02", Name = "Văn phòng Minh Khang", Phone = "0909789123", Email = "minhkhang@example.com", Address = "Bình Dương", IsActive = true }
    ];
}
