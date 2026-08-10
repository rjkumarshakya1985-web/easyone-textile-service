IF OBJECT_ID('dbo.StickerPrintFieldSettings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.StickerPrintFieldSettings
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_StickerPrintFieldSettings PRIMARY KEY,
        StickerPrintSettingId int NOT NULL,
        FieldKey nvarchar(50) NOT NULL,
        Label nvarchar(80) NOT NULL,
        IsVisible bit NOT NULL CONSTRAINT DF_StickerPrintFieldSettings_IsVisible DEFAULT (1),
        X decimal(8,2) NOT NULL,
        Y decimal(8,2) NOT NULL,
        Width decimal(8,2) NOT NULL,
        Height decimal(8,2) NOT NULL,
        FontSize int NOT NULL,
        FontWeight nvarchar(20) NOT NULL,
        TextAlign nvarchar(20) NOT NULL,
        SortOrder int NOT NULL,
        CONSTRAINT FK_StickerPrintFieldSettings_StickerPrintSettings
            FOREIGN KEY (StickerPrintSettingId)
            REFERENCES dbo.StickerPrintSettings(Id)
            ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX IX_StickerPrintFieldSettings_StickerPrintSettingId_FieldKey
        ON dbo.StickerPrintFieldSettings(StickerPrintSettingId, FieldKey);
END
GO

DECLARE @StickerPrintSettingId int;

SELECT TOP (1) @StickerPrintSettingId = Id
FROM dbo.StickerPrintSettings
ORDER BY Id;

IF @StickerPrintSettingId IS NOT NULL
BEGIN
    INSERT INTO dbo.StickerPrintFieldSettings
    (
        StickerPrintSettingId,
        FieldKey,
        Label,
        IsVisible,
        X,
        Y,
        Width,
        Height,
        FontSize,
        FontWeight,
        TextAlign,
        SortOrder
    )
    SELECT @StickerPrintSettingId, v.FieldKey, v.Label, v.IsVisible, v.X, v.Y, v.Width, v.Height, v.FontSize, v.FontWeight, v.TextAlign, v.SortOrder
    FROM
    (
        VALUES
        ('supplierCode', 'Supplier Code', 1, 10, 8, 82, 24, 20, '800', 'left', 1),
        ('companyShortName', 'Company Short Name', 1, 113, 8, 74, 22, 20, '800', 'center', 2),
        ('wholeSaleRate', 'Wholesale Rate', 1, 162, 8, 128, 24, 20, '800', 'right', 3),
        ('productName', 'Product Name', 1, 42, 32, 216, 24, 18, '800', 'center', 4),
        ('printDate', 'Print Date', 1, 51, 59, 80, 18, 14, '400', 'left', 5),
        ('retailRate', 'Retail Rate', 1, 195, 59, 62, 18, 14, '400', 'right', 6),
        ('barcode', 'Barcode', 1, 51, 78, 188, 34, 14, '400', 'center', 7),
        ('barcodeText', 'Barcode Text', 1, 121, 113, 58, 14, 12, '400', 'center', 8)
    ) AS v(FieldKey, Label, IsVisible, X, Y, Width, Height, FontSize, FontWeight, TextAlign, SortOrder)
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.StickerPrintFieldSettings existing
        WHERE existing.StickerPrintSettingId = @StickerPrintSettingId
          AND existing.FieldKey = v.FieldKey
    );
END
GO
