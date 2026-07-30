IF OBJECT_ID('dbo.CustomerAgents', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CustomerAgents]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Code] NVARCHAR(50) NULL,
        [Name] NVARCHAR(255) NOT NULL,
        [ContactPersonName] NVARCHAR(255) NULL,
        [ContactPersonMobile] NVARCHAR(20) NULL,
        [GSTIN] NVARCHAR(15) NULL,
        [PAN] NVARCHAR(10) NULL,
        [CityId] INT NULL,
        [Email] NVARCHAR(255) NULL,
        [Pincode] NVARCHAR(10) NULL,
        [TallyLedgerName] NVARCHAR(255) NULL,
        [Area] NVARCHAR(255) NULL,
        [Address] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_CustomerAgents_IsActive] DEFAULT (1),
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_CustomerAgents_IsDeleted] DEFAULT (0),
        [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
        [CreatedByUserName] NVARCHAR(255) NOT NULL,
        [CreatedOn] DATETIME2 NOT NULL,
        [ModifiedBy] UNIQUEIDENTIFIER NULL,
        [ModifiedByUserName] NVARCHAR(255) NULL,
        [ModifiedOn] DATETIME2 NULL,
        CONSTRAINT [PK_CustomerAgents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CustomerAgents_Cities_CityId]
            FOREIGN KEY ([CityId]) REFERENCES [dbo].[Cities] ([Id])
    );

    CREATE INDEX [IX_CustomerAgents_Name] ON [dbo].[CustomerAgents] ([Name]);
    CREATE INDEX [IX_CustomerAgents_CityId] ON [dbo].[CustomerAgents] ([CityId]);
END;
GO





