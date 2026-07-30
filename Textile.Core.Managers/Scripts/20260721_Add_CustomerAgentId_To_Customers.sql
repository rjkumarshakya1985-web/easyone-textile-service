IF COL_LENGTH('dbo.Customers', 'CustomerAgentId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Customers] ADD [CustomerAgentId] UNIQUEIDENTIFIER NULL;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Customers_CustomerAgents_CustomerAgentId')
BEGIN
    ALTER TABLE [dbo].[Customers] WITH CHECK
    ADD CONSTRAINT [FK_Customers_CustomerAgents_CustomerAgentId]
        FOREIGN KEY ([CustomerAgentId]) REFERENCES [dbo].[CustomerAgents] ([Id]);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Customers_CustomerAgentId' AND object_id = OBJECT_ID('dbo.Customers'))
BEGIN
    CREATE INDEX [IX_Customers_CustomerAgentId] ON [dbo].[Customers] ([CustomerAgentId]);
END;
GO
