IF OBJECT_ID('dbo.SupplierStickerSettings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SupplierStickerSettings
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SupplierStickerSettings PRIMARY KEY,
        SupplierId uniqueidentifier NOT NULL,
        StickerWidthMm decimal(8,2) NOT NULL,
        StickerHeightMm decimal(8,2) NOT NULL,
        UpdatedOn datetime2 NOT NULL CONSTRAINT DF_SupplierStickerSettings_UpdatedOn DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT FK_SupplierStickerSettings_Suppliers
            FOREIGN KEY (SupplierId)
            REFERENCES dbo.Suppliers(Id)
    );

    CREATE UNIQUE INDEX IX_SupplierStickerSettings_SupplierId
        ON dbo.SupplierStickerSettings(SupplierId);
END
