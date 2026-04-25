-- ============================================================
-- SGENERGY_DB - Script tạo cơ sở dữ liệu cho hệ thống SGENERGY
-- Bao gồm: Categories, Suppliers, Products, ProductAttributes,
--           ProductPhotos, Projects, ProjectPhotos, ContactCustomers
-- Tương thích với cấu trúc repository hiện tại của SGENERGY.Admin
-- ============================================================

USE [master]
GO

IF DB_ID('SGENERGY_DB') IS NULL
    CREATE DATABASE [SGENERGY_DB]
GO

USE [SGENERGY_DB]
GO

-- ============================================================
-- 1. CATEGORIES (Loại hàng)
-- ============================================================
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories
GO
CREATE TABLE dbo.Categories (
    CategoryID       INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName     NVARCHAR(255) NOT NULL,
    Description      NVARCHAR(1000) NULL
)
GO
CREATE UNIQUE INDEX UX_Categories_CategoryName ON dbo.Categories(CategoryName)
GO


-- ============================================================
-- 2. SUPPLIERS (Nhà cung cấp / Hãng sản xuất)
-- ============================================================
IF OBJECT_ID('dbo.Suppliers', 'U') IS NOT NULL DROP TABLE dbo.Suppliers
GO
CREATE TABLE dbo.Suppliers (
    SupplierID       INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName     NVARCHAR(255) NOT NULL,
    ContactName      NVARCHAR(255) NOT NULL CONSTRAINT DF_Suppliers_ContactName DEFAULT(N''),
    Province         NVARCHAR(100) NULL,
    Address          NVARCHAR(500) NULL,
    Phone            NVARCHAR(50) NULL,
    Email            NVARCHAR(255) NULL
)
GO
CREATE UNIQUE INDEX UX_Suppliers_SupplierName ON dbo.Suppliers(SupplierName)
GO


-- ============================================================
-- 3. PRODUCTS (Mặt hàng / Sản phẩm)
-- ============================================================
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products
GO
CREATE TABLE dbo.Products (
    ProductID            INT IDENTITY(1,1) PRIMARY KEY,
    ProductName          NVARCHAR(255) NOT NULL,
    ProductDescription   NVARCHAR(MAX) NULL,
    SupplierID           INT NULL,
    CategoryID           INT NULL,
    Unit                 NVARCHAR(50) NOT NULL CONSTRAINT DF_Products_Unit DEFAULT(N''),
    Price                DECIMAL(18,2) NOT NULL CONSTRAINT DF_Products_Price DEFAULT(0),
    Photo                NVARCHAR(500) NULL,
    IsSelling            BIT NOT NULL CONSTRAINT DF_Products_IsSelling DEFAULT(1),

    CONSTRAINT FK_Products_Categories
        FOREIGN KEY (CategoryID) REFERENCES dbo.Categories(CategoryID)
        ON UPDATE CASCADE ON DELETE SET NULL,

    CONSTRAINT FK_Products_Suppliers
        FOREIGN KEY (SupplierID) REFERENCES dbo.Suppliers(SupplierID)
        ON UPDATE CASCADE ON DELETE SET NULL
)
GO
CREATE INDEX IX_Products_CategoryID ON dbo.Products(CategoryID)
GO
CREATE INDEX IX_Products_SupplierID ON dbo.Products(SupplierID)
GO


-- ============================================================
-- 4. PRODUCT ATTRIBUTES (Thuộc tính sản phẩm)
-- ============================================================
IF OBJECT_ID('dbo.ProductAttributes', 'U') IS NOT NULL DROP TABLE dbo.ProductAttributes
GO
CREATE TABLE dbo.ProductAttributes (
    AttributeID       BIGINT IDENTITY(1,1) PRIMARY KEY,
    ProductID         INT NOT NULL,
    AttributeName     NVARCHAR(255) NOT NULL,
    AttributeValue    NVARCHAR(MAX) NOT NULL,
    DisplayOrder      INT NOT NULL CONSTRAINT DF_ProductAttributes_DisplayOrder DEFAULT(0),

    CONSTRAINT FK_ProductAttributes_Products
        FOREIGN KEY (ProductID) REFERENCES dbo.Products(ProductID)
        ON UPDATE CASCADE ON DELETE CASCADE
)
GO
CREATE INDEX IX_ProductAttributes_ProductID ON dbo.ProductAttributes(ProductID)
GO


-- ============================================================
-- 5. PRODUCT PHOTOS (Hình ảnh sản phẩm)
-- ============================================================
IF OBJECT_ID('dbo.ProductPhotos', 'U') IS NOT NULL DROP TABLE dbo.ProductPhotos
GO
CREATE TABLE dbo.ProductPhotos (
    PhotoID           BIGINT IDENTITY(1,1) PRIMARY KEY,
    ProductID         INT NOT NULL,
    Photo             NVARCHAR(500) NOT NULL,
    Description       NVARCHAR(500) NOT NULL CONSTRAINT DF_ProductPhotos_Description DEFAULT(N''),
    DisplayOrder      INT NOT NULL CONSTRAINT DF_ProductPhotos_DisplayOrder DEFAULT(0),
    IsHidden          BIT NOT NULL CONSTRAINT DF_ProductPhotos_IsHidden DEFAULT(0),

    CONSTRAINT FK_ProductPhotos_Products
        FOREIGN KEY (ProductID) REFERENCES dbo.Products(ProductID)
        ON UPDATE CASCADE ON DELETE CASCADE
)
GO
CREATE INDEX IX_ProductPhotos_ProductID ON dbo.ProductPhotos(ProductID)
GO


-- ============================================================
-- 6. PROJECTS (Dự án)
-- ============================================================
IF OBJECT_ID('dbo.Projects', 'U') IS NOT NULL DROP TABLE dbo.Projects
GO
CREATE TABLE dbo.Projects (
    ProjectID             INT IDENTITY(1,1) PRIMARY KEY,
    ProjectName           NVARCHAR(255) NOT NULL,
    Slug                  NVARCHAR(255) NULL,
    Location              NVARCHAR(500) NULL,
    Investor              NVARCHAR(255) NULL,
    ScaleDescription      NVARCHAR(500) NULL,
    Summary               NVARCHAR(1000) NULL,
    DetailDescription     NVARCHAR(MAX) NULL,
    Thumbnail             NVARCHAR(500) NULL,
    DisplayOrder          INT NOT NULL CONSTRAINT DF_Projects_DisplayOrder DEFAULT(0),
    IsFeatured            BIT NOT NULL CONSTRAINT DF_Projects_IsFeatured DEFAULT(0),
    IsActive              BIT NOT NULL CONSTRAINT DF_Projects_IsActive DEFAULT(1),
    CreatedAt             DATETIME2 NOT NULL CONSTRAINT DF_Projects_CreatedAt DEFAULT(SYSDATETIME()),
    UpdatedAt             DATETIME2 NULL
)
GO
CREATE INDEX IX_Projects_IsActive ON dbo.Projects(IsActive, DisplayOrder)
GO


-- ============================================================
-- 7. PROJECT PHOTOS (Hình ảnh dự án)
-- ============================================================
IF OBJECT_ID('dbo.ProjectPhotos', 'U') IS NOT NULL DROP TABLE dbo.ProjectPhotos
GO
CREATE TABLE dbo.ProjectPhotos (
    PhotoID           BIGINT IDENTITY(1,1) PRIMARY KEY,
    ProjectID         INT NOT NULL,
    Photo             NVARCHAR(500) NOT NULL,
    Description       NVARCHAR(500) NOT NULL CONSTRAINT DF_ProjectPhotos_Description DEFAULT(N''),
    DisplayOrder      INT NOT NULL CONSTRAINT DF_ProjectPhotos_DisplayOrder DEFAULT(0),
    IsHidden          BIT NOT NULL CONSTRAINT DF_ProjectPhotos_IsHidden DEFAULT(0),

    CONSTRAINT FK_ProjectPhotos_Projects
        FOREIGN KEY (ProjectID) REFERENCES dbo.Projects(ProjectID)
        ON UPDATE CASCADE ON DELETE CASCADE
)
GO
CREATE INDEX IX_ProjectPhotos_ProjectID ON dbo.ProjectPhotos(ProjectID)
GO


-- ============================================================
-- 8. CONTACT CUSTOMERS (Liên hệ khách hàng)
-- ============================================================
IF OBJECT_ID('dbo.ContactCustomers', 'U') IS NOT NULL DROP TABLE dbo.ContactCustomers
GO
CREATE TABLE dbo.ContactCustomers (
    ContactID         BIGINT IDENTITY(1,1) PRIMARY KEY,
    FullName          NVARCHAR(255) NOT NULL,
    Phone             NVARCHAR(50) NULL,
    Email             NVARCHAR(255) NULL,
    CompanyName       NVARCHAR(255) NULL,
    Address           NVARCHAR(500) NULL,
    Subject           NVARCHAR(255) NULL,
    Message           NVARCHAR(MAX) NULL,
    ProductID         INT NULL,
    ProjectID         INT NULL,
    SourcePage        NVARCHAR(500) NULL,
    IsHandled         BIT NOT NULL CONSTRAINT DF_ContactCustomers_IsHandled DEFAULT(0),
    HandledBy         NVARCHAR(100) NULL,
    HandledAt         DATETIME2 NULL,
    CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_ContactCustomers_CreatedAt DEFAULT(SYSDATETIME()),

    CONSTRAINT FK_ContactCustomers_Products
        FOREIGN KEY (ProductID) REFERENCES dbo.Products(ProductID)
        ON UPDATE NO ACTION ON DELETE SET NULL,

    CONSTRAINT FK_ContactCustomers_Projects
        FOREIGN KEY (ProjectID) REFERENCES dbo.Projects(ProjectID)
        ON UPDATE NO ACTION ON DELETE SET NULL
)
GO
CREATE INDEX IX_ContactCustomers_CreatedAt ON dbo.ContactCustomers(CreatedAt DESC)
GO
CREATE INDEX IX_ContactCustomers_IsHandled ON dbo.ContactCustomers(IsHandled)
GO


-- ============================================================
-- DỮ LIỆU MẪU (Seed data)
-- ============================================================

-- Loại hàng
INSERT INTO dbo.Categories(CategoryName, Description) VALUES
    (N'Điện mặt trời áp mái', N'Hệ thống điện mặt trời lắp đặt trên mái nhà'),
    (N'Điện mặt trời mặt đất', N'Hệ thống điện mặt trời lắp đặt trên mặt đất'),
    (N'Hệ thống lưu trữ năng lượng', N'Pin lưu trữ và hệ thống ESS'),
    (N'Phụ kiện & Thiết bị', N'Inverter, mounting, cáp và phụ kiện')
GO

-- Nhà cung cấp / Hãng sản xuất
INSERT INTO dbo.Suppliers(SupplierName, ContactName, Phone, Email) VALUES
    (N'LONGi Solar', N'Bộ phận kinh doanh', N'02812345678', N'sales@longi.com'),
    (N'JA Solar', N'Bộ phận kinh doanh', N'02812345679', N'sales@jasolar.com'),
    (N'SolarEdge', N'Bộ phận kinh doanh', N'02812345680', N'sales@solaredge.com'),
    (N'Growatt', N'Bộ phận kinh doanh', N'02812345681', N'sales@growatt.com'),
    (N'Fronius', N'Bộ phận kinh doanh', N'02812345682', N'sales@fronius.com')
GO
