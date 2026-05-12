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
    (N'GIADUNG', N'Gia dụng', N'Hàng tiêu dùng gia đình');

INSERT INTO Suppliers (Code, Name, Phone, Email, Address)
VALUES
    (N'NCC001', N'Công ty phân phối Minh Anh', N'0900000001', N'minhanh@example.com', N'TP. Hồ Chí Minh'),
    (N'NCC002', N'Nhà cung cấp Bình Minh', N'0900000002', N'binhminh@example.com', N'Đồng Nai');

INSERT INTO Customers (Code, Name, Phone, Email, Address)
VALUES
    (N'KH001', N'Khách lẻ', NULL, NULL, NULL),
    (N'KH002', N'Cửa hàng Tạp hóa An Phú', N'0911111111', N'anphu@example.com', N'Bình Dương');

INSERT INTO Products (Code, Name, CategoryId, Unit, CostPrice, SellingPrice, QuantityOnHand, MinStockLevel)
VALUES
    (N'SP001', N'Nước suối 500ml', 1, N'Chai', 3500, 6000, 120, 30),
    (N'SP002', N'Nước ngọt cola lon', 1, N'Lon', 7000, 11000, 80, 25),
    (N'SP003', N'Mì gói bò', 2, N'Gói', 3000, 5000, 200, 50),
    (N'SP004', N'Nước rửa chén 750ml', 3, N'Chai', 18000, 25000, 18, 20);

INSERT INTO StockTransactions (ProductId, TransactionType, QuantityChange, QuantityAfter, ReferenceCode, CreatedByUserId, Note)
VALUES
    (1, N'Seed', 120, 120, N'SEED', 1, N'Dữ liệu demo'),
    (2, N'Seed', 80, 80, N'SEED', 1, N'Dữ liệu demo'),
    (3, N'Seed', 200, 200, N'SEED', 1, N'Dữ liệu demo'),
    (4, N'Seed', 18, 18, N'SEED', 1, N'Dữ liệu demo');
GO
