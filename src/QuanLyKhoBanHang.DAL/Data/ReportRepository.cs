using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DTO.Reports;
using System;
using System.Collections.Generic;

namespace QuanLyKhoBanHang.DAL
{
    public class ReportRepository
    {
        private readonly string _connStr = "Server=BaoChau2203;Database=QuanLyKhoBanHang;Trusted_Connection=True;TrustServerCertificate=True";

        // 1. Lấy Doanh thu (Gom nhóm theo ngày)
        public List<RevenueSummaryDto> GetRevenue(DateTime fromDate, DateTime toDate)
        {
            var list = new List<RevenueSummaryDto>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT CAST(InvoiceDate AS DATE) as Date, 
                           COUNT(Id) as InvoiceCount, 
                           ISNULL(SUM(TotalAmount), 0) as Revenue
                    FROM SalesInvoices 
                    WHERE CAST(InvoiceDate AS DATE) >= CAST(@From AS DATE) 
                      AND CAST(InvoiceDate AS DATE) <= CAST(@To AS DATE)
                    GROUP BY CAST(InvoiceDate AS DATE)
                    ORDER BY CAST(InvoiceDate AS DATE)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@From", fromDate);
                    cmd.Parameters.AddWithValue("@To", toDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RevenueSummaryDto
                            {
                                Date = Convert.ToDateTime(reader["Date"]),
                                InvoiceCount = Convert.ToInt32(reader["InvoiceCount"]),
                                Revenue = Convert.ToDecimal(reader["Revenue"]),
                                EstimatedProfit = 0 // Tạm thời để 0 hoặc bạn có thể tính nếu có giá vốn
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 2. Lấy Top Sản phẩm bán chạy
        public List<ProductSalesSummaryDto> GetTopSellingProducts(DateTime fromDate, DateTime toDate, int top)
        {
            var list = new List<ProductSalesSummaryDto>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT TOP (@Top) 
                           p.Id as ProductId, 
                           p.Code as ProductCode, 
                           p.Name as ProductName, 
                           SUM(d.Quantity) as QuantitySold,
                           SUM(d.Quantity * d.UnitPrice) as Revenue
                    FROM SalesInvoiceDetails d 
                    JOIN SalesInvoices i ON d.SalesInvoiceId = i.Id 
                    JOIN Products p ON d.ProductId = p.Id 
                    WHERE CAST(i.InvoiceDate AS DATE) >= CAST(@From AS DATE) 
                      AND CAST(i.InvoiceDate AS DATE) <= CAST(@To AS DATE) 
                    GROUP BY p.Id, p.Code, p.Name 
                    ORDER BY QuantitySold DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Top", top);
                    cmd.Parameters.AddWithValue("@From", fromDate);
                    cmd.Parameters.AddWithValue("@To", toDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ProductSalesSummaryDto
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductCode = reader["ProductCode"]?.ToString(),
                                ProductName = reader["ProductName"]?.ToString(),
                                QuantitySold = Convert.ToInt32(reader["QuantitySold"]),
                                Revenue = Convert.ToDecimal(reader["Revenue"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 3. Lấy Top Khách hàng mua nhiều nhất
        public List<CustomerPurchaseSummaryDto> GetTopCustomers(DateTime fromDate, DateTime toDate, int top)
        {
            var list = new List<CustomerPurchaseSummaryDto>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT TOP (@Top) 
                           c.Id as CustomerId, 
                           c.Name as CustomerName, 
                           COUNT(i.Id) as InvoiceCount,
                           SUM(i.TotalAmount) as TotalAmount 
                    FROM SalesInvoices i 
                    JOIN Customers c ON i.CustomerId = c.Id 
                    WHERE CAST(i.InvoiceDate AS DATE) >= CAST(@From AS DATE) 
                      AND CAST(i.InvoiceDate AS DATE) <= CAST(@To AS DATE) 
                    GROUP BY c.Id, c.Name 
                    ORDER BY TotalAmount DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Top", top);
                    cmd.Parameters.AddWithValue("@From", fromDate);
                    cmd.Parameters.AddWithValue("@To", toDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CustomerPurchaseSummaryDto
                            {
                                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                                CustomerName = reader["CustomerName"]?.ToString(),
                                InvoiceCount = Convert.ToInt32(reader["InvoiceCount"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"])
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
