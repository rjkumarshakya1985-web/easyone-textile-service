IF OBJECT_ID('dbo.StickerPrintSettings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.StickerPrintSettings
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_StickerPrintSettings PRIMARY KEY,
        ShowSupplierCode bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ShowSupplierCode DEFAULT (1),
        ShowCompanyShortName bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ShowCompanyShortName DEFAULT (1),
        ShowWholeSaleRate bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ShowWholeSaleRate DEFAULT (1),
        ShowProductName bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ShowProductName DEFAULT (1),
        ShowPrintDate bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ShowPrintDate DEFAULT (1),
        ShowRetailRate bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ShowRetailRate DEFAULT (1),
        ShowBarcode bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ShowBarcode DEFAULT (1),
        ShowBarcodeText bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ShowBarcodeText DEFAULT (1),
        CompanyShortName nvarchar(30) NOT NULL CONSTRAINT DF_StickerPrintSettings_CompanyShortName DEFAULT ('SSBD'),
        ApplyWholeSaleRateFormula bit NOT NULL CONSTRAINT DF_StickerPrintSettings_ApplyWholeSaleRateFormula DEFAULT (1),
        WholeSaleRatePrefix nvarchar(20) NULL CONSTRAINT DF_StickerPrintSettings_WholeSaleRatePrefix DEFAULT ('5'),
        WholeSaleRatePostfix nvarchar(20) NULL,
        WholeSaleRateAddAmount decimal(18,2) NOT NULL CONSTRAINT DF_StickerPrintSettings_WholeSaleRateAddAmount DEFAULT (500)
    );

    INSERT INTO dbo.StickerPrintSettings
    (
        ShowSupplierCode,
        ShowCompanyShortName,
        ShowWholeSaleRate,
        ShowProductName,
        ShowPrintDate,
        ShowRetailRate,
        ShowBarcode,
        ShowBarcodeText,
        CompanyShortName,
        ApplyWholeSaleRateFormula,
        WholeSaleRatePrefix,
        WholeSaleRatePostfix,
        WholeSaleRateAddAmount
    )
    VALUES
    (
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        'ABC',
        1,
        '5',
        NULL,
        500
    );
END
GO
