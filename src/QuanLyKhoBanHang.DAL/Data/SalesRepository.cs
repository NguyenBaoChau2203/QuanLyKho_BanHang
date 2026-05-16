using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DTO;
using QuanLyKhoBanHang.DTO.Sales;
using System;
using System.Collections.Generic; // Bổ sung thư viện này để dùng được List
using QuanLyKhoBanHang.DAL.Data;

namespace QuanLyKhoBanHang.DAL
{
    public class SalesRepository
    {
        private readonly DatabaseOptions _options;
        private string _connStr => _options.ConnectionString;

        public SalesRepository()
        {
            _options = new DatabaseOptions();
        }

        public SalesRepository(DatabaseOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        // ---------------------------------------------------
        // 1. HÀM TẠO HÓA ĐƠN (LƯU VÀ TRỪ TỒN KHO)
        // ---------------------------------------------------
        public int CreateSalesInvoice(SalesInvoiceDto invoice)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                // BẮT ĐẦU TRANSACTION: Cực kỳ quan trọng để dữ liệu đồng nhất
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. LƯU BẢNG SalesInvoices
                        string sqlInv = @"INSERT INTO SalesInvoices 
                            (InvoiceCode, CustomerId, InvoiceDate, CreatedByUserId, TotalAmount, DiscountAmount, Note) 
                            OUTPUT INSERTED.Id 
                            VALUES (@Code, @CustId, @Date, @UserId, @Total, @Discount, @Note)";

                        int invoiceId;
                        using (var cmdInv = new SqlCommand(sqlInv, conn, trans))
                        {
                            cmdInv.Parameters.AddWithValue("@Code", string.IsNullOrEmpty(invoice.InvoiceCode) ? "HD" + DateTime.Now.ToString("yyMMddHHmmss") : invoice.InvoiceCode);
                            var customerIdValue = invoice.CustomerId.HasValue ? (object)invoice.CustomerId.Value : DBNull.Value;
                            cmdInv.Parameters.AddWithValue("@CustId", customerIdValue);
                            cmdInv.Parameters.AddWithValue("@Date", invoice.InvoiceDate == default ? DateTime.Now : invoice.InvoiceDate);
                            cmdInv.Parameters.AddWithValue("@UserId", invoice.CreatedByUserId);
                            cmdInv.Parameters.AddWithValue("@Total", invoice.TotalAmount);
                            cmdInv.Parameters.AddWithValue("@Discount", invoice.DiscountAmount);
                            cmdInv.Parameters.AddWithValue("@Note", (object)invoice.Note ?? DBNull.Value);

                            // Lấy ra Id của Hóa đơn vừa tạo
                            var invoiceIdValue = cmdInv.ExecuteScalar();
                            if (invoiceIdValue is null || invoiceIdValue == DBNull.Value)
                            {
                                throw new InvalidOperationException("KhÃ´ng láº¥y Ä‘Æ°á»£c mÃ£ hÃ³a Ä‘Æ¡n vá»«a táº¡o.");
                            }

                            invoiceId = Convert.ToInt32(invoiceIdValue);
                        }

                        // 2. LƯU CHI TIẾT & TRỪ TỒN KHO
                        foreach (var line in invoice.Lines)
                        {
                            // A. Lấy tồn kho hiện tại ra kiểm tra trước
                            int currentQty = 0;
                            using (var cmdCheck = new SqlCommand("SELECT QuantityOnHand FROM Products WHERE Id = @PId", conn, trans))
                            {
                                cmdCheck.Parameters.AddWithValue("@PId", line.ProductId);
                                var res = cmdCheck.ExecuteScalar();
                                if (res != null) currentQty = Convert.ToInt32(res);
                                else throw new Exception($"Không tìm thấy sản phẩm có ID {line.ProductId}");
                            }

                            // Rule: Không cho bán vượt tồn kho
                            if (currentQty < line.Quantity)
                                throw new Exception($"Sản phẩm '{line.ProductName}' không đủ tồn kho! Tồn: {currentQty}, Đặt: {line.Quantity}");

                            // B. Lưu bảng SalesInvoiceDetails
                            string sqlDet = "INSERT INTO SalesInvoiceDetails (SalesInvoiceId, ProductId, Quantity, UnitPrice) VALUES (@InvId, @PId, @Qty, @Price)";
                            using (var cmdDet = new SqlCommand(sqlDet, conn, trans))
                            {
                                cmdDet.Parameters.AddWithValue("@InvId", invoiceId);
                                cmdDet.Parameters.AddWithValue("@PId", line.ProductId);
                                cmdDet.Parameters.AddWithValue("@Qty", line.Quantity);
                                cmdDet.Parameters.AddWithValue("@Price", line.UnitPrice);
                                cmdDet.ExecuteNonQuery();
                            }

                            // C. Trừ số lượng tồn trong bảng Products
                            using (var cmdUpd = new SqlCommand("UPDATE Products SET QuantityOnHand = QuantityOnHand - @Qty WHERE Id = @PId", conn, trans))
                            {
                                cmdUpd.Parameters.AddWithValue("@Qty", line.Quantity);
                                cmdUpd.Parameters.AddWithValue("@PId", line.ProductId);
                                cmdUpd.ExecuteNonQuery();
                            }

                            // D. Ghi lịch sử giao dịch (StockTransactions)
                            string sqlTrans = @"INSERT INTO StockTransactions 
                                (ProductId, TransactionType, QuantityChange, QuantityAfter, ReferenceCode, CreatedByUserId, Note) 
                                VALUES (@PId, 'Sales', @Change, @After, @RefCode, @UserId, N'Bán hàng')";
                            using (var cmdTrans = new SqlCommand(sqlTrans, conn, trans))
                            {
                                cmdTrans.Parameters.AddWithValue("@PId", line.ProductId);
                                cmdTrans.Parameters.AddWithValue("@Change", -line.Quantity); // Bán thì âm
                                cmdTrans.Parameters.AddWithValue("@After", currentQty - line.Quantity);
                                cmdTrans.Parameters.AddWithValue("@RefCode", string.IsNullOrEmpty(invoice.InvoiceCode) ? "HD" + DateTime.Now.ToString("yyMMddHHmmss") : invoice.InvoiceCode);
                                cmdTrans.Parameters.AddWithValue("@UserId", invoice.CreatedByUserId);
                                cmdTrans.ExecuteNonQuery();
                            }
                        }

                        // Nếu code chạy trót lọt đến đây -> Ghi nhận toàn bộ!
                        trans.Commit();
                        return invoiceId;
                    }
                    catch
                    {
                        // Nếu có bất kỳ lỗi gì -> Hủy bỏ toàn bộ, không lưu gì cả
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        // ---------------------------------------------------
        // 2. HÀM LẤY DANH SÁCH HÓA ĐƠN THEO NGÀY
        // ---------------------------------------------------
        public List<SalesInvoiceDto> GetInvoices(DateTime fromDate, DateTime toDate)
        {
            var list = new List<SalesInvoiceDto>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                string query = "SELECT Id, InvoiceCode, CustomerId, InvoiceDate, TotalAmount, DiscountAmount, Note FROM SalesInvoices WHERE CAST(InvoiceDate AS DATE) >= CAST(@From AS DATE) AND CAST(InvoiceDate AS DATE) <= CAST(@To AS DATE)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@From", fromDate);
                    cmd.Parameters.AddWithValue("@To", toDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SalesInvoiceDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                InvoiceCode = reader["InvoiceCode"]?.ToString() ?? string.Empty,
                                CustomerId = reader["CustomerId"] != DBNull.Value ? Convert.ToInt32(reader["CustomerId"]) : null,
                                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                DiscountAmount = Convert.ToDecimal(reader["DiscountAmount"]),
                                Note = reader["Note"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            return list;
        }

        // ---------------------------------------------------
        // 3. HÀM LẤY CHI TIẾT 1 HÓA ĐƠN
        // ---------------------------------------------------
        public SalesInvoiceDto GetInvoiceById(int id)
        {
            SalesInvoiceDto? invoice = null;
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                // 1. Lấy thông tin chung của hóa đơn
                using (var cmd = new SqlCommand("SELECT Id, InvoiceCode, CustomerId, InvoiceDate, TotalAmount, DiscountAmount, Note FROM SalesInvoices WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            invoice = new SalesInvoiceDto
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                InvoiceCode = reader["InvoiceCode"]?.ToString() ?? string.Empty,
                                CustomerId = reader["CustomerId"] != DBNull.Value ? Convert.ToInt32(reader["CustomerId"]) : null,
                                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                                TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                DiscountAmount = Convert.ToDecimal(reader["DiscountAmount"]),
                                Note = reader["Note"]?.ToString() ?? string.Empty,
                                Lines = new List<SalesInvoiceLineDto>()
                            };
                        }
                    }
                }

                // 2. Nếu tìm thấy hóa đơn -> Lấy tiếp các món hàng bên trong
                if (invoice != null)
                {
                    string sqlLines = @"SELECT d.Id, d.SalesInvoiceId, d.ProductId, p.Name as ProductName, d.Quantity, d.UnitPrice 
                                      FROM SalesInvoiceDetails d 
                                      JOIN Products p ON d.ProductId = p.Id 
                                      WHERE d.SalesInvoiceId = @InvId";
                    using (var cmdLines = new SqlCommand(sqlLines, conn))
                    {
                        cmdLines.Parameters.AddWithValue("@InvId", id);
                        using (var readerLines = cmdLines.ExecuteReader())
                        {
                            while (readerLines.Read())
                            {
                                invoice.Lines.Add(new SalesInvoiceLineDto
                                {
                                    Id = Convert.ToInt32(readerLines["Id"]),
                                    SalesInvoiceId = Convert.ToInt32(readerLines["SalesInvoiceId"]),
                                    ProductId = Convert.ToInt32(readerLines["ProductId"]),
                                    ProductName = readerLines["ProductName"]?.ToString() ?? string.Empty,
                                    Quantity = Convert.ToInt32(readerLines["Quantity"]),
                                    UnitPrice = Convert.ToDecimal(readerLines["UnitPrice"]),
                                    LineTotal = Convert.ToInt32(readerLines["Quantity"]) * Convert.ToDecimal(readerLines["UnitPrice"])
                                });
                            }
                        }
                    }
                }
            }
            return invoice!;
        }
    }
}
