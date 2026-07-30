IF COL_LENGTH('dbo.Customers', 'Mu') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customers]
    ADD [Mu] DECIMAL(18, 2) NULL;
END;
GO

IF COL_LENGTH('dbo.Customers', 'PaymentTerm') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customers]
    ADD [PaymentTerm] INT NULL;

    ALTER TABLE [dbo].[Customers]
    ADD CONSTRAINT [CK_Customers_PaymentTerm]
        CHECK ([PaymentTerm] IS NULL OR [PaymentTerm] IN (1, 2));
END;
GO

