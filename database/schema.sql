IF DB_ID(N'QuanLyKhoBanHang') IS NULL
BEGIN
    CREATE DATABASE QuanLyKhoBanHang;
END
GO

USE QuanLyKhoBanHang;
GO

CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    RoleId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(30) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(30) NOT NULL UNIQUE,
    Name NVARCHAR(150) NOT NULL,
    CategoryId INT NOT NULL,
    Unit NVARCHAR(30) NOT NULL,
    CostPrice DECIMAL(18,2) NOT NULL DEFAULT 0,
    SellingPrice DECIMAL(18,2) NOT NULL DEFAULT 0,
    QuantityOnHand INT NOT NULL DEFAULT 0,
    MinStockLevel INT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT CK_Products_Prices CHECK (CostPrice >= 0 AND SellingPrice >= 0),
    CONSTRAINT CK_Products_Quantity CHECK (QuantityOnHand >= 0 AND MinStockLevel >= 0)
);

CREATE TABLE Suppliers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(30) NOT NULL UNIQUE,
    Name NVARCHAR(150) NOT NULL,
    Phone NVARCHAR(30) NULL,
    Email NVARCHAR(100) NULL,
    Address NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(30) NOT NULL UNIQUE,
    Name NVARCHAR(150) NOT NULL,
    Phone NVARCHAR(30) NULL,
    Email NVARCHAR(100) NULL,
    Address NVARCHAR(255) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE PurchaseReceipts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReceiptCode NVARCHAR(30) NOT NULL UNIQUE,
    SupplierId INT NOT NULL,
    ReceiptDate DATETIME2 NOT NULL,
    CreatedByUserId INT NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Note NVARCHAR(255) NULL,
    CONSTRAINT FK_PurchaseReceipts_Suppliers FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
    CONSTRAINT FK_PurchaseReceipts_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

CREATE TABLE PurchaseReceiptDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseReceiptId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitCost DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_PurchaseReceiptDetails_Receipts FOREIGN KEY (PurchaseReceiptId) REFERENCES PurchaseReceipts(Id),
    CONSTRAINT FK_PurchaseReceiptDetails_Products FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_PurchaseReceiptDetails_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_PurchaseReceiptDetails_UnitCost CHECK (UnitCost >= 0)
);

CREATE TABLE SalesInvoices (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceCode NVARCHAR(30) NOT NULL UNIQUE,
    CustomerId INT NULL,
    InvoiceDate DATETIME2 NOT NULL,
    CreatedByUserId INT NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Note NVARCHAR(255) NULL,
    CONSTRAINT FK_SalesInvoices_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_SalesInvoices_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
    CONSTRAINT CK_SalesInvoices_Amounts CHECK (TotalAmount >= 0 AND DiscountAmount >= 0)
);

CREATE TABLE SalesInvoiceDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SalesInvoiceId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_SalesInvoiceDetails_Invoices FOREIGN KEY (SalesInvoiceId) REFERENCES SalesInvoices(Id),
    CONSTRAINT FK_SalesInvoiceDetails_Products FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_SalesInvoiceDetails_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_SalesInvoiceDetails_UnitPrice CHECK (UnitPrice >= 0)
);

CREATE TABLE StockTransactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    TransactionType NVARCHAR(30) NOT NULL,
    QuantityChange INT NOT NULL,
    QuantityAfter INT NOT NULL,
    ReferenceCode NVARCHAR(30) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedByUserId INT NOT NULL,
    Note NVARCHAR(255) NULL,
    CONSTRAINT FK_StockTransactions_Products FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT FK_StockTransactions_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

CREATE TABLE Stocktakes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StocktakeCode NVARCHAR(30) NOT NULL UNIQUE,
    StocktakeDate DATETIME2 NOT NULL,
    CreatedByUserId INT NOT NULL,
    Note NVARCHAR(255) NULL,
    CONSTRAINT FK_Stocktakes_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

CREATE TABLE StocktakeDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StocktakeId INT NOT NULL,
    ProductId INT NOT NULL,
    SystemQuantity INT NOT NULL,
    ActualQuantity INT NOT NULL,
    CONSTRAINT FK_StocktakeDetails_Stocktakes FOREIGN KEY (StocktakeId) REFERENCES Stocktakes(Id),
    CONSTRAINT FK_StocktakeDetails_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE AuditLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    Action NVARCHAR(80) NOT NULL,
    EntityName NVARCHAR(80) NOT NULL,
    EntityId INT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE INDEX IX_Products_Name ON Products(Name);
CREATE INDEX IX_StockTransactions_ProductId ON StockTransactions(ProductId);
CREATE INDEX IX_SalesInvoices_InvoiceDate ON SalesInvoices(InvoiceDate);
CREATE INDEX IX_PurchaseReceipts_ReceiptDate ON PurchaseReceipts(ReceiptDate);
GO
