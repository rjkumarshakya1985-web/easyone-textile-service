IF COL_LENGTH('dbo.Customers', 'CustomerCategory') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customers]
    ADD [CustomerCategory] INT NULL;

    ALTER TABLE [dbo].[Customers]
    ADD CONSTRAINT [CK_Customers_CustomerCategory]
        CHECK ([CustomerCategory] IS NULL OR [CustomerCategory] IN (1, 2, 3));
END;
GO


