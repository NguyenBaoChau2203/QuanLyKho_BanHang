USE QuanLyKhoBanHang;
GO

INSERT INTO Roles (Name)
VALUES (N'Admin'), (N'Quản lý'), (N'Nhân viên bán hàng'), (N'Thủ kho');

INSERT INTO Users (Username, PasswordHash, FullName, RoleId)
VALUES
    (N'admin', N'admin123', N'Châu', 1),
    (N'du', N'123456', N'Dũ', 4),
    (N'hung', N'123456', N'Hùng', 3);

INSERT INTO Categories (Code, Name, Description)
VALUES
    (N'DOUONG', N'Đồ uống', N'Nước giải khát và đồ uống đóng chai'),
    (N'THUCPHAM', N'Thực phẩm', N'Hàng thực phẩm đóng gói'),
    (N'GIADUNG', N'Gia dụng', N'Hàng tiêu dùng gia đình'),
    (N'VESINH', N'Vệ sinh', N'Chăm sóc và vệ sinh cá nhân');

INSERT INTO Suppliers (Code, Name, Phone, Email, Address)
VALUES
    (N'NCC001', N'Công ty phân phối Minh Anh', N'0900000001', N'minhanh@example.com', N'TP. Hồ Chí Minh'),
    (N'NCC002', N'Nhà cung cấp Bình Minh', N'0900000002', N'binhminh@example.com', N'Đồng Nai'),
    (N'NCC003', N'Nhà máy Sạch Sẽ', N'0900000003', N'sachse@example.com', N'Bình Dương');

INSERT INTO Customers (Code, Name, Phone, Email, Address)
VALUES
    (N'KH001', N'Khách lẻ', NULL, NULL, NULL),
    (N'KH002', N'Cửa hàng Tạp hóa An Phú', N'0911111111', N'anphu@example.com', N'Bình Dương'),
    (N'KH003', N'Siêu thị Hòa Bình', N'0988777666', N'hoabinh@example.com', N'TP. Hồ Chí Minh');

INSERT INTO Products (Code, Name, CategoryId, Unit, CostPrice, SellingPrice, QuantityOnHand, MinStockLevel)
VALUES
    (N'SP001', N'Nước suối 500ml', 1, N'Chai', 3500, 6000, 110, 30),
    (N'SP002', N'Nước ngọt cola lon', 1, N'Lon', 7000, 11000, 68, 25),
    (N'SP003', N'Mì gói bò', 2, N'Gói', 3000, 5000, 195, 50),
    (N'SP004', N'Nước rửa chén 750ml', 3, N'Chai', 18000, 25000, 32, 35),
    (N'SP005', N'Kem đánh răng 110g', 4, N'Tuýp', 12000, 18000, 30, 35),
    (N'SP006', N'Khăn giấy 100 tờ', 4, N'Gói', 8000, 12500, 118, 15);

INSERT INTO PurchaseReceipts (ReceiptCode, SupplierId, ReceiptDate, CreatedByUserId, TotalAmount, Note)
VALUES
    (N'PN0001', 1, DATEADD(DAY, -10, SYSDATETIME()), 1, 1540000, N'Nhập hàng demo ban đầu'),
    (N'PN0002', 2, DATEADD(DAY, -5, SYSDATETIME()), 1, 860000, N'Bổ sung tồn kho trưng bày');

INSERT INTO PurchaseReceiptDetails (PurchaseReceiptId, ProductId, Quantity, UnitCost)
VALUES
    (1, 1, 120, 3500),
    (1, 2, 80, 7000),
    (1, 3, 200, 3000),
    (2, 4, 18, 18000),
    (2, 5, 15, 12000),
    (2, 6, 60, 8000);

INSERT INTO SalesInvoices (InvoiceCode, CustomerId, InvoiceDate, CreatedByUserId, TotalAmount, DiscountAmount, Note)
VALUES
    (N'HD0001', 1, DATEADD(DAY, -2, SYSDATETIME()), 3, 106000, 6000, N'Bán demo quầy lẻ'),
    (N'HD0002', 2, DATEADD(DAY, -1, SYSDATETIME()), 3, 198000, 0, N'Giao hàng cho khách sỉ');

INSERT INTO SalesInvoiceDetails (SalesInvoiceId, ProductId, Quantity, UnitPrice)
VALUES
    (1, 1, 10, 6000),
    (1, 3, 5, 5000),
    (1, 6, 2, 12500),
    (2, 2, 12, 11000),
    (2, 4, 4, 25000);

INSERT INTO StockTransactions (ProductId, TransactionType, QuantityChange, QuantityAfter, ReferenceCode, CreatedByUserId, Note)
VALUES
    (1, N'Seed', 120, 120, N'SEED', 1, N'Dữ liệu demo ban đầu'),
    (2, N'Seed', 80, 80, N'SEED', 1, N'Dữ liệu demo ban đầu'),
    (3, N'Seed', 200, 200, N'SEED', 1, N'Dữ liệu demo ban đầu'),
    (4, N'Seed', 18, 18, N'SEED', 1, N'Dữ liệu demo ban đầu'),
    (5, N'Seed', 15, 15, N'SEED', 1, N'Dữ liệu demo ban đầu'),
    (6, N'Seed', 60, 60, N'SEED', 1, N'Dữ liệu demo ban đầu'),
    (4, N'Purchase', 18, 36, N'PN0002', 1, N'Nhập thêm để demo báo cáo tồn kho'),
    (5, N'Purchase', 15, 30, N'PN0002', 1, N'Nhập thêm để demo báo cáo tồn kho'),
    (6, N'Purchase', 60, 120, N'PN0002', 1, N'Nhập thêm để demo báo cáo tồn kho'),
    (1, N'Sales', -10, 110, N'HD0001', 3, N'Bán demo quầy lẻ'),
    (3, N'Sales', -5, 195, N'HD0001', 3, N'Bán demo quầy lẻ'),
    (6, N'Sales', -2, 118, N'HD0001', 3, N'Bán demo quầy lẻ'),
    (2, N'Sales', -12, 68, N'HD0002', 3, N'Bán demo khách sỉ'),
    (4, N'Sales', -4, 32, N'HD0002', 3, N'Bán demo khách sỉ');

INSERT INTO Stocktakes (StocktakeCode, StocktakeDate, CreatedByUserId, Note)
VALUES
    (N'KK0001', DATEADD(DAY, -1, SYSDATETIME()), 4, N'Kiểm kê demo quầy trưng bày');

INSERT INTO StocktakeDetails (StocktakeId, ProductId, SystemQuantity, ActualQuantity)
VALUES
    (1, 4, 32, 30),
    (1, 5, 30, 30),
    (1, 6, 118, 118);

INSERT INTO AuditLogs (UserId, Action, EntityName, EntityId, Description)
VALUES
    (1, N'SEED', N'Users', 1, N'Tạo dữ liệu demo ban đầu'),
    (1, N'SEED', N'Products', 1, N'Tạo dữ liệu demo sản phẩm'),
    (3, N'SEED', N'SalesInvoices', 1, N'Tạo dữ liệu hóa đơn demo');
GO
