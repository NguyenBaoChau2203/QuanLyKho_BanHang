USE QuanLyKhoBanHang;
GO

INSERT INTO Roles (Name)
VALUES (N'Admin'), (N'Quản lý'), (N'Nhân viên bán hàng'), (N'Thủ kho');

INSERT INTO Users (Username, PasswordHash, FullName, RoleId)
VALUES
    (N'admin', N'v1:100000:QWRtaW5TYWx0VjFGb3JTZQ==:0zD2l4lnoQvE1hsg9fyPoCE85OMwuAlYhNmIHV/rOEo=', N'Châu', 1),
    (N'manager', N'v1:100000:PsZlg2HopgAMAyr/pGZbZQ==:lasevve/HmZtSZ7S3RLkvZo+SRtqj6qW29grRjfTvs4=', N'Quản lý demo', 2),
    (N'du', N'v1:100000:v5R6S1rKhromA7+b0AYeVQ==:pLoDCK5zra96NBXKC9A1Ql3ObY0L62GKqxjmYcThWkM=', N'Dũ', 4),
    (N'hung', N'v1:100000:EUGHejFLenJlt4MMe7YSOw==:Z4Clgdz2PWTk21m9tNon3eyWJUVOuAkAgcRUVdtAtM8=', N'Hùng', 3);

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
    (N'HD0001', 1, DATEADD(DAY, -2, SYSDATETIME()), 4, 106000, 6000, N'Bán demo quầy lẻ'),
    (N'HD0002', 2, DATEADD(DAY, -1, SYSDATETIME()), 4, 198000, 0, N'Giao hàng cho khách sỉ');

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
    (1, N'Sales', -10, 110, N'HD0001', 4, N'Bán demo quầy lẻ'),
    (3, N'Sales', -5, 195, N'HD0001', 4, N'Bán demo quầy lẻ'),
    (6, N'Sales', -2, 118, N'HD0001', 4, N'Bán demo quầy lẻ'),
    (2, N'Sales', -12, 68, N'HD0002', 4, N'Bán demo khách sỉ'),
    (4, N'Sales', -4, 32, N'HD0002', 4, N'Bán demo khách sỉ');

INSERT INTO Stocktakes (StocktakeCode, StocktakeDate, CreatedByUserId, Note)
VALUES
    (N'KK0001', DATEADD(DAY, -1, SYSDATETIME()), 3, N'Kiểm kê demo quầy trưng bày');

INSERT INTO StocktakeDetails (StocktakeId, ProductId, SystemQuantity, ActualQuantity)
VALUES
    (1, 4, 32, 30),
    (1, 5, 30, 30),
    (1, 6, 118, 118);

INSERT INTO AuditLogs (UserId, Action, EntityName, EntityId, Description)
VALUES
    (1, N'SEED', N'Users', 1, N'Tạo dữ liệu demo ban đầu'),
    (1, N'SEED', N'Products', 1, N'Tạo dữ liệu demo sản phẩm'),
    (4, N'SEED', N'SalesInvoices', 1, N'Tạo dữ liệu hóa đơn demo');

INSERT INTO Permissions (FeatureKey, FeatureName, GroupName, Note)
VALUES
    (N'dashboard', N'Dashboard', N'Điều hành', N'Tổng quan doanh thu, đơn hàng và cảnh báo tồn kho.'),
    (N'product', N'Sản phẩm', N'Danh mục', N'Tra cứu và quản lý sản phẩm.'),
    (N'category', N'Loại hàng', N'Danh mục', N'Quản lý nhóm sản phẩm.'),
    (N'supplier', N'Nhà cung cấp', N'Danh mục', N'Quản lý nhà cung cấp.'),
    (N'customer', N'Khách hàng', N'Bán hàng', N'Quản lý khách hàng.'),
    (N'purchase-receipt', N'Nhập kho', N'Kho', N'Lập phiếu nhập hàng.'),
    (N'inventory', N'Tồn kho', N'Kho', N'Tra cứu tồn kho và giao dịch kho.'),
    (N'stocktake', N'Kiểm kê', N'Kho', N'Theo dõi và lập kiểm kê.'),
    (N'sales-invoice', N'Bán hàng', N'Bán hàng', N'Lập hóa đơn bán hàng.'),
    (N'report', N'Báo cáo', N'Điều hành', N'Xem báo cáo doanh thu và top sản phẩm.'),
    (N'assistant', N'Trợ lý AI', N'Điều hành', N'Hỏi nhanh số liệu qua AssistantService.'),
    (N'user-management', N'Tài khoản', N'Quản trị', N'Quản lý tài khoản.'),
    (N'role-permission', N'Phân quyền', N'Quản trị', N'Xem ma trận quyền theo vai trò.'),
    (N'audit-log', N'Nhật ký hệ thống', N'Quản trị', N'Xem nhật ký thao tác hệ thống.');

INSERT INTO RolePermissions (RoleId, PermissionId)
VALUES
    (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8), (1, 9), (1, 10), (1, 11), (1, 12), (1, 13), (1, 14),
    (2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 7), (2, 8), (2, 10), (2, 11),
    (3, 2), (3, 5), (3, 7), (3, 9),
    (4, 2), (4, 3), (4, 4), (4, 6), (4, 7), (4, 8);
GO
