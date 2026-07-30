IF COL_LENGTH('dbo.Customers', 'TransportId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customers] ADD [TransportId] INT NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Customers_Transports_TransportId')
BEGIN
    ALTER TABLE [dbo].[Customers] WITH CHECK
    ADD CONSTRAINT [FK_Customers_Transports_TransportId]
        FOREIGN KEY ([TransportId]) REFERENCES [dbo].[Transports] ([Id]);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Customers_TransportId' AND object_id = OBJECT_ID('dbo.Customers'))
BEGIN
    CREATE INDEX [IX_Customers_TransportId] ON [dbo].[Customers] ([TransportId]);
END;
GO
