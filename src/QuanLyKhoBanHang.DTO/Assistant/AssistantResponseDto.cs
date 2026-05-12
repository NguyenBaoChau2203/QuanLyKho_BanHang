namespace QuanLyKhoBanHang.DTO.Assistant;

public sealed class AssistantResponseDto
{
    public string Intent { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool Handled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
