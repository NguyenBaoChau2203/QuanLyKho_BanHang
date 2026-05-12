namespace QuanLyKhoBanHang.DTO.Inventory;

public sealed class StocktakeDto
{
    public int Id { get; set; }
    public string StocktakeCode { get; set; } = string.Empty;
    public DateTime StocktakeDate { get; set; } = DateTime.Today;
    public int CreatedByUserId { get; set; }
    public string? Note { get; set; }
    public List<StocktakeLineDto> Lines { get; set; } = new();
}
