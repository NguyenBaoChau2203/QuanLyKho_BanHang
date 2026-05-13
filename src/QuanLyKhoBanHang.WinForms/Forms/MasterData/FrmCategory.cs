using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.MasterData;

public sealed class FrmCategory : CrudListForm<CategoryDto>
{
    private readonly CategoryService _service = new();
    private List<CategoryDto> _items = [];
    private List<CategoryDto> _filteredItems = [];

    public FrmCategory() : base("Loại hàng", "Quản lý loại hàng, trạng thái hoạt động và tìm kiếm nhanh.")
    {
        Grid.AutoGenerateColumns = false;
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CategoryDto.Code), HeaderText = "Mã", FillWeight = 20 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CategoryDto.Name), HeaderText = "Tên", FillWeight = 28 });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(CategoryDto.Description), HeaderText = "Mô tả", FillWeight = 42 });
        Grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(CategoryDto.IsActive), HeaderText = "Hoạt động", Width = 96, MinimumWidth = 96, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
        RefreshData();
        ToggleEditing(false);
    }

    protected override void RefreshData()
    {
        var result = _service.GetAllCategories();
        _items = result.Success && result.Data is { Count: > 0 } ? result.Data! : CreateStubItems();
        ApplyFilter();
        SetMessage(result.Message);
    }

    protected override void ApplyFilter()
    {
        var keyword = SearchBox.Text.Trim();
        _filteredItems = string.IsNullOrWhiteSpace(keyword)
            ? _items.ToList()
            : _items.Where(x => x.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase) || x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        BindingSource.DataSource = new BindingList<CategoryDto>(_filteredItems);
        ShowEmpty(_filteredItems.Count == 0);
    }

    protected override void OnSelectionChanged()
    {
        if (BindingSource.Current is not CategoryDto item) return;
        SelectedId = item.Id;
        CodeBox.Text = item.Code;
        NameBox.Text = item.Name;
        DescriptionBox.Text = item.Description ?? string.Empty;
        ActiveBox.Checked = item.IsActive;
    }

    protected override void BeginAdd() => LoadEditor(new CategoryDto(), "Đang thêm loại hàng mới.");
    protected override void BeginEdit() => LoadEditor(GetCurrentItem(), "Đang chỉnh sửa loại hàng.");

    protected override void SaveCurrent()
    {
        if (!IsEditing)
        {
            SetMessage("Hãy bấm Thêm hoặc Sửa trước khi lưu.", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(CodeBox.Text) || string.IsNullOrWhiteSpace(NameBox.Text))
        {
            SetMessage("Mã và tên loại hàng là bắt buộc.", true);
            return;
        }
        ToggleEditing(false);
        SetMessage(SelectedId <= 0 ? "Đã lưu tạm loại hàng mới trong chế độ stub." : "Đã lưu thay đổi loại hàng trong chế độ stub.");
    }

    protected override void CancelEdit() { ToggleEditing(false); RefreshData(); SetMessage("Đã hủy thay đổi."); }
    protected override void DeactivateCurrent() => SetMessage(SelectedId > 0 ? "Đã ghi nhận yêu cầu ngừng kích hoạt loại hàng." : "Vui lòng chọn loại hàng.", SelectedId <= 0);

    private void LoadEditor(CategoryDto item, string message)
    {
        SelectedId = item.Id;
        CodeBox.Text = item.Code;
        NameBox.Text = item.Name;
        DescriptionBox.Text = item.Description ?? string.Empty;
        ActiveBox.Checked = item.IsActive;
        ToggleEditing(true);
        SetMessage(message);
    }

    private CategoryDto GetCurrentItem() => BindingSource.Current as CategoryDto ?? new CategoryDto { IsActive = true };

    private static List<CategoryDto> CreateStubItems() =>
    [
        new CategoryDto { Id = 1, Code = "LH-01", Name = "Văn phòng phẩm", Description = "Bút, sổ, giấy", IsActive = true },
        new CategoryDto { Id = 2, Code = "LH-02", Name = "Đóng gói", Description = "Thùng, băng keo, nilon", IsActive = true }
    ];
}
