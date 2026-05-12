# DAL notes

Tầng DAL sẽ triển khai ADO.NET thật ở các repository. Khi thêm package SQL Server, ưu tiên `Microsoft.Data.SqlClient`.

Quy định:

- Không viết SQL trong WinForms hoặc BLL.
- Không nối chuỗi SQL từ input người dùng.
- Các nghiệp vụ nhập kho, bán hàng, kiểm kê phải dùng SQL transaction.
- Repository chỉ trả dữ liệu hoặc lỗi kỹ thuật; validation nghiệp vụ nằm ở BLL.
