-- ============================================================
-- MIGRATION: Thêm cột Slug vào Categories, Suppliers, Products
-- Chạy script này nếu SGENERGY_DB đã tồn tại (không cần tạo lại từ đầu)
-- ============================================================

USE [SGENERGY_DB]
GO

-- 1. Categories.Slug
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Categories') AND name = 'Slug'
)
BEGIN
    ALTER TABLE dbo.Categories ADD Slug NVARCHAR(300) NULL;
    PRINT 'Added Slug to Categories';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.Categories') AND name = 'UX_Categories_Slug'
)
BEGIN
    CREATE UNIQUE INDEX UX_Categories_Slug ON dbo.Categories(Slug) WHERE Slug IS NOT NULL;
    PRINT 'Created UX_Categories_Slug';
END
GO

-- 2. Suppliers.Slug
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Suppliers') AND name = 'Slug'
)
BEGIN
    ALTER TABLE dbo.Suppliers ADD Slug NVARCHAR(300) NULL;
    PRINT 'Added Slug to Suppliers';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.Suppliers') AND name = 'UX_Suppliers_Slug'
)
BEGIN
    CREATE UNIQUE INDEX UX_Suppliers_Slug ON dbo.Suppliers(Slug) WHERE Slug IS NOT NULL;
    PRINT 'Created UX_Suppliers_Slug';
END
GO

-- 3. Products.Slug
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Products') AND name = 'Slug'
)
BEGIN
    ALTER TABLE dbo.Products ADD Slug NVARCHAR(300) NULL;
    PRINT 'Added Slug to Products';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.Products') AND name = 'UX_Products_Slug'
)
BEGIN
    CREATE UNIQUE INDEX UX_Products_Slug ON dbo.Products(Slug) WHERE Slug IS NOT NULL;
    PRINT 'Created UX_Products_Slug';
END
GO

-- 4. Projects.Slug
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Projects') AND name = 'Slug'
)
BEGIN
    ALTER TABLE dbo.Projects ADD Slug NVARCHAR(300) NULL;
    PRINT 'Added Slug to Projects';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.Projects') AND name = 'UX_Projects_Slug'
)
BEGIN
    CREATE UNIQUE INDEX UX_Projects_Slug ON dbo.Projects(Slug) WHERE Slug IS NOT NULL;
    PRINT 'Created UX_Projects_Slug';
END
GO

PRINT 'Migration complete.';
GO
