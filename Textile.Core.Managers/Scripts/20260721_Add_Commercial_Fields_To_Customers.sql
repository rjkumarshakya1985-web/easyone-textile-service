IF COL_LENGTH('dbo.Customers', 'CustomerStatus') IS NULL
    ALTER TABLE [dbo].[Customers] ADD [CustomerStatus] INT NULL;
GO

IF COL_LENGTH('dbo.Customers', 'RateType') IS NULL
    ALTER TABLE [dbo].[Customers] ADD [RateType] INT NULL;
GO

IF COL_LENGTH('dbo.Customers', 'AlternateNo') IS NULL
    ALTER TABLE [dbo].[Customers] ADD [AlternateNo] NVARCHAR(20) NULL;
GO

IF COL_LENGTH('dbo.Customers', 'CreditAlertLimit') IS NULL
    ALTER TABLE [dbo].[Customers] ADD [CreditAlertLimit] DECIMAL(18, 2) NULL;
GO

IF COL_LENGTH('dbo.Customers', 'Incentive') IS NULL
    ALTER TABLE [dbo].[Customers] ADD [Incentive] DECIMAL(18, 2) NULL;
GO

IF COL_LENGTH('dbo.Customers', 'Term') IS NULL
    ALTER TABLE [dbo].[Customers] ADD [Term] DECIMAL(18, 2) NULL;
GO

IF COL_LENGTH('dbo.Customers', 'Reference') IS NULL
    ALTER TABLE [dbo].[Customers] ADD [Reference] NVARCHAR(250) NULL;
GO

IF COL_LENGTH('dbo.Customers', 'CustomerCode') IS NULL
    ALTER TABLE [dbo].[Customers] ADD [CustomerCode] NVARCHAR(50) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Customers_CustomerStatus')
BEGIN
    ALTER TABLE [dbo].[Customers] ADD CONSTRAINT [CK_Customers_CustomerStatus]
        CHECK ([CustomerStatus] IS NULL OR [CustomerStatus] IN (1, 2, 3));
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Customers_RateType')
BEGIN
    ALTER TABLE [dbo].[Customers] ADD CONSTRAINT [CK_Customers_RateType]
        CHECK ([RateType] IS NULL OR [RateType] IN (1, 2));
END;
GO


