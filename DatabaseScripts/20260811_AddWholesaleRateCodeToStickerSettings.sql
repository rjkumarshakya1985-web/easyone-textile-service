IF COL_LENGTH('dbo.StickerPrintSettings', 'ApplyWholeSaleRateCode') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD ApplyWholeSaleRateCode bit NOT NULL
            CONSTRAINT DF_StickerPrintSettings_ApplyWholeSaleRateCode DEFAULT (0);
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode0') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode0 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode0 DEFAULT ('A');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode1') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode1 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode1 DEFAULT ('B');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode2') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode2 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode2 DEFAULT ('C');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode3') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode3 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode3 DEFAULT ('D');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode4') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode4 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode4 DEFAULT ('E');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode5') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode5 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode5 DEFAULT ('F');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode6') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode6 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode6 DEFAULT ('G');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode7') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode7 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode7 DEFAULT ('H');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode8') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode8 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode8 DEFAULT ('I');
END

IF COL_LENGTH('dbo.StickerPrintSettings', 'WholeSaleRateCode9') IS NULL
BEGIN
    ALTER TABLE dbo.StickerPrintSettings
        ADD WholeSaleRateCode9 nvarchar(10) NOT NULL
            CONSTRAINT DF_StickerPrintSettings_WholeSaleRateCode9 DEFAULT ('J');
END
