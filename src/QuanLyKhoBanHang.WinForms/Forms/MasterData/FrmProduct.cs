using System.ComponentModel;
using QuanLyKhoBanHang.BLL.Services;
using QuanLyKhoBanHang.DTO.MasterData;
using QuanLyKhoBanHang.WinForms.Forms.Common;

namespace QuanLyKhoBanHang.WinForms.Forms.MasterData;

public sealed class FrmProduct : CrudListForm<ProductDto>
{
    private readonly ProductService _service = new();
    private List<ProductDto> _items = [];
    private List<ProductDto> _filteredItems = [];

    public FrmProduct() : base("Sản phẩm", "Quản lý danh mục sản phẩm, giá và tồn kho theo contract BLL.")
    {
        BuildGridColumns();
        RefreshData();
        ToggleEditing(false);
    }

    protected override void RefreshData()
    {
        var result = _service.GetAllProducts();
        _items = result.Success && (result.Data?.Count > 0)
            ? result.Data!
            : CreateStubItems();
        _filteredItems = _items.ToList();
        BindingSource.DataSource = new BindingList<ProductDto>(_filteredItems);
        ShowEmpty(_filteredItems.Count == 0);
        SetMessage(result.Message);
    }

    protected override void ApplyFilter()
    {
        var keyword = SearchBox.Text.Trim();
        _filteredItems = string.IsNullOrWhiteSpace(keyword)
            ? _items.ToList()
            : _items.Where(p => p.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                || p.CategoryName.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        BindingSource.DataSource = new BindingList<ProductDto>(_filteredItems);
        ShowEmpty(_filteredItems.Count == 0);
    }

    protected override void OnSelectionChanged()
    {
        if (BindingSource.Current is not ProductDto item) return;
        SelectedId = item.Id;
        CodeBox.Text = item.Code;
        NameBox.Text = item.Name;
        DescriptionBox.Text = $"Loại: {item.CategoryName}\r\nĐơn vị: {item.Unit}\r\nTồn: {item.QuantityOnHand}";
        ActiveBox.Checked = item.IsActive;
    }

    protected override void BeginAdd()
    {
        SelectedId = 0;
        CodeBox.Clear();
        NameBox.Clear();
        DescriptionBox.Clear();
        ActiveBox.Checked = true;
        ToggleEditing(true);
        SetMessage("Đang tạo sản phẩm mới.");
    }

    protected override void BeginEdit()
    {
        if (SelectedId <= 0)
        {
            SetMessage("Vui lòng chọn một sản phẩm để sửa.", true);
            return;
        }
        ToggleEditing(true);
        SetMessage("Đang chỉnh sửa sản phẩm đã chọn.");
    }

    protected override void SaveCurrent()
    {
        if (!IsEditing)
        {
            SetMessage("Hãy chọn Thêm hoặc Sửa trước khi lưu.", true);
            return;
        }

        if (string.IsNullOrWhiteSpace(CodeBox.Text) || string.IsNullOrWhiteSpace(NameBox.Text))
        {
            SetMessage("Mã và tên sản phẩm là bắt buộc.", true);
            return;
        }

        ToggleEditing(false);
        SetMessage("Đã lưu tạm trong chế độ stub. Backend thật sẽ nối ở phase sau.");
    }

    protected override void CancelEdit()
    {
        ToggleEditing(false);
        RefreshData();
        SetMessage("Đã hủy chỉnh sửa.");
    }

    protected override void DeactivateCurrent()
    {
        if (SelectedId <= 0)
        {
            SetMessage("Vui lòng chọn một sản phẩm để ngừng kích hoạt.", true);
            return;
        }
        SetMessage("Đã ghi nhận yêu cầu ngừng kích hoạt ở lớp UI stub.");
    }

    private void BuildGridColumns()
    {
        Grid.AutoGenerateColumns = false;
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Code), HeaderText = "Mã" });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Name), HeaderText = "Tên" });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.CategoryName), HeaderText = "Loại hàng" });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.Unit), HeaderText = "Đơn vị" });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.SellingPrice), HeaderText = "Giá bán", DefaultCellStyle = { Format = "N0" } });
        Grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ProductDto.QuantityOnHand), HeaderText = "Tồn kho" });
        Grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = nameof(ProductDto.IsActive), HeaderText = "Hoạt động" });
    }

    private static List<ProductDto> CreateStubItems() =>
    [
        new ProductDto { Id = 1, Code = "SP001", Name = "Nước suối 500ml", CategoryName = "Đồ uống", Unit = "Chai", SellingPrice = 6000, QuantityOnHand = 110, MinStockLevel = 30 },
        new ProductDto { Id = 2, Code = "SP002", Name = "Nước ngọt cola lon", CategoryName = "Đồ uống", Unit = "Lon", SellingPrice = 11000, QuantityOnHand = 68, MinStockLevel = 25 },
        new ProductDto { Id = 3, Code = "SP003", Name = "Mì gói bò", CategoryName = "Thực phẩm", Unit = "Gói", SellingPrice = 5000, QuantityOnHand = 195, MinStockLevel = 50 },
        new ProductDto { Id = 4, Code = "SP004", Name = "Nước rửa chén 750ml", CategoryName = "Gia dụng", Unit = "Chai", SellingPrice = 25000, QuantityOnHand = 32, MinStockLevel = 35 },
        new ProductDto { Id = 5, Code = "SP005", Name = "Kem đánh răng 110g", CategoryName = "Vệ sinh", Unit = "Tuýp", SellingPrice = 18000, QuantityOnHand = 30, MinStockLevel = 35 },
        new ProductDto { Id = 6, Code = "SP006", Name = "Khăn giấy 100 tờ", CategoryName = "Vệ sinh", Unit = "Gói", SellingPrice = 12500, QuantityOnHand = 118, MinStockLevel = 15 }
    ];
}
