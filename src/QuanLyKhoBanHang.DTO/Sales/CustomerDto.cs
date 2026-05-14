namespace QuanLyKhoBanHang.DTO.Sales
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Code { get; set; }  // Đã thêm Code
        public string Name { get; set; }  // Sửa FullName -> Name
        public string Phone { get; set; } // Sửa PhoneNumber -> Phone
        public string Email { get; set; } // Đã thêm Email
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
}
