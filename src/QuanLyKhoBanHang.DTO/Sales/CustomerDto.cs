namespace QuanLyKhoBanHang.DTO.Sales
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;  // Đã thêm Code
        public string Name { get; set; } = string.Empty;  // Sửa FullName -> Name
        public string Phone { get; set; } = string.Empty; // Sửa PhoneNumber -> Phone
        public string Email { get; set; } = string.Empty; // Đã thêm Email
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
