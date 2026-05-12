namespace QuanLyKhoBanHang.DTO.Inventory;

public sealed class StocktakeLineDto
{
    public int Id { get; set; }
    public int StocktakeId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int SystemQuantity { get; set; }
    public int ActualQuantity { get; set; }
    public int Difference => ActualQuantity - SystemQuantity;
}
