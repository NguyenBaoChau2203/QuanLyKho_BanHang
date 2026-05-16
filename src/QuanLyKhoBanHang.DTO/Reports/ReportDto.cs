using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKhoBanHang.DTO.Reports;

    // Dữ liệu cho Báo cáo Doanh thu tổng
    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalInvoices { get; set; }
    }

    // Dữ liệu cho Báo cáo Top Sản phẩm bán chạy
    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
    }

    // Dữ liệu cho Báo cáo Khách hàng mua nhiều
    public class TopCustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
    }
