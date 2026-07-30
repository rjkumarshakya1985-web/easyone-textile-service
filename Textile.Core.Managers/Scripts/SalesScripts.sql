CREATE TABLE [dbo].[UserDetails](
	[UserDetailId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[DepartmentId] [int] NULL,
 CONSTRAINT [PK_UserDetails] PRIMARY KEY CLUSTERED 
(
	[UserDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserDetails]  WITH CHECK ADD  CONSTRAINT [FK_UserDetails_Departments] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id])
GO

ALTER TABLE [dbo].[UserDetails] CHECK CONSTRAINT [FK_UserDetails_Departments]
GO

ALTER TABLE [dbo].[UserDetails]  WITH CHECK ADD  CONSTRAINT [FK_UserDetails_UserDetails] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[UserDetails] CHECK CONSTRAINT [FK_UserDetails_UserDetails]
GO


CREATE TABLE [dbo].[VoucherTypes](
	[Id] [int] NOT NULL,
	[Name] [varchar](25) NOT NULL,
	[Prefix] [varchar](10) NOT NULL,
	[NumberLength] [int] NOT NULL,
 CONSTRAINT [PK_VoucherType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE FinanceYears
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    Name NVARCHAR(20) NOT NULL,        -- Example: 2024-2025
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,

    IsActive BIT NOT NULL DEFAULT 0,
    IsClosed BIT NOT NULL DEFAULT 0,

    CreatedBy uniqueidentifier not null,
    CreatedByUserName nvarchar(255) not null,
    CreatedOn datetime2(7) not null,

    ModifiedBy uniqueidentifier  null,
    ModifiedByUserName nvarchar(255)  null,
    ModifiedOn datetime2(7)  null,
);


CREATE TABLE [dbo].[PackingSlips](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SlipNumber] [nvarchar](50) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[FinanceYearId] [int] NOT NULL,
	[VisitorId] [int]  NULL,
	[SalesPersionId] [int] NULL,
	[PackingSlipPersionId] [uniqueidentifier] NULL,
	[Status] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Remarks] [varchar](50) NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedByUserName] [nvarchar](255) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedByUserName] [nvarchar](255) NULL,
	[ModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK__PackingS__3214EC07A0C399D5] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PackingSlips] ADD  CONSTRAINT [DF__PackingSl__IsDel__1D114BD1]  DEFAULT ((0)) FOR [IsDeleted]
GO

ALTER TABLE [dbo].[PackingSlips]  WITH CHECK ADD  CONSTRAINT [FK_PackingSlips_FinanceYears] FOREIGN KEY([FinanceYearId])
REFERENCES [dbo].[FinanceYears] ([Id])
GO

ALTER TABLE [dbo].[PackingSlips] CHECK CONSTRAINT [FK_PackingSlips_FinanceYears]
GO

ALTER TABLE [dbo].[PackingSlips]  WITH CHECK ADD  CONSTRAINT [FK_PackingSlips_PackingSlips] FOREIGN KEY([VisitorId])
REFERENCES [dbo].[Visitors] ([Id])
GO

ALTER TABLE [dbo].[PackingSlips] CHECK CONSTRAINT [FK_PackingSlips_PackingSlips]
GO

ALTER TABLE [dbo].[PackingSlips]  WITH CHECK ADD  CONSTRAINT [FK_PackingSlips_Users] FOREIGN KEY([PackingSlipPersionId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[PackingSlips] CHECK CONSTRAINT [FK_PackingSlips_Users]
GO




CREATE TABLE [dbo].[PackingSlipItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PackingSlipId] [int] NOT NULL,
	[StockId] [uniqueidentifier] NOT NULL,
	[SaleRate] [decimal](18, 2) NOT NULL,
	[Qty] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PackingSlipItems]  WITH CHECK ADD  CONSTRAINT [FK_PackingSlipItems_PackingSlips] FOREIGN KEY([PackingSlipId])
REFERENCES [dbo].[PackingSlips] ([Id])
GO

ALTER TABLE [dbo].[PackingSlipItems] CHECK CONSTRAINT [FK_PackingSlipItems_PackingSlips]
GO

ALTER TABLE [dbo].[PackingSlipItems]  WITH CHECK ADD  CONSTRAINT [FK_PackingSlipItems_Stocks] FOREIGN KEY([StockId])
REFERENCES [dbo].[Stocks] ([Id])
GO

ALTER TABLE [dbo].[PackingSlipItems] CHECK CONSTRAINT [FK_PackingSlipItems_Stocks]
GO


CREATE TABLE [dbo].[VoucherNumberSeries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VoucherType] [int] NOT NULL,
	[FinanceYearId] [int] NOT NULL,
	[CurrentNumber] [int] NOT NULL,
 CONSTRAINT [PK_VoucherNumberSeries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[VoucherNumberSeries] ADD  CONSTRAINT [DF__VoucherNu__Curre__70FDBF69]  DEFAULT ((0)) FOR [CurrentNumber]
GO

ALTER TABLE [dbo].[VoucherNumberSeries]  WITH CHECK ADD  CONSTRAINT [FK_VoucherNumberSeries_FinanceYears] FOREIGN KEY([FinanceYearId])
REFERENCES [dbo].[FinanceYears] ([Id])
GO

ALTER TABLE [dbo].[VoucherNumberSeries] CHECK CONSTRAINT [FK_VoucherNumberSeries_FinanceYears]
GO


CREATE PROCEDURE GetNextVoucherNumber
(
    @VoucherType INT,
    @FinanceYearId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE VoucherNumberSeries
    SET CurrentNumber = CurrentNumber + 1
    OUTPUT INSERTED.CurrentNumber
    WHERE VoucherType = @VoucherType
    AND FinanceYearId = @FinanceYearId;
END

--Above table updated


CREATE TABLE OrderForms
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    OrderNumber NVARCHAR(50) NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Date DATETIME2(7) NOT NULL,
    FinanceYearId INT NOT NULL,

    CustomerId UNIQUEIDENTIFIER NOT NULL,

    Status INT NOT NULL,  
    -- 0=Draft
    -- 1=Confirmed
    -- 2=PartiallyDelivered
    -- 3=FullyDelivered
    -- 4=Closed

    IsDeleted BIT NOT NULL DEFAULT 0,

    Remarks VARCHAR(100) NULL,

    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedByUserName NVARCHAR(255) NOT NULL,
    CreatedOn DATETIME2(7) NOT NULL,

    ModifiedBy UNIQUEIDENTIFIER NULL,
    ModifiedByUserName NVARCHAR(255) NULL,
    ModifiedOn DATETIME2(7) NULL,

    CONSTRAINT FK_OrderForms_FinanceYear 
        FOREIGN KEY (FinanceYearId) REFERENCES FinanceYears(Id)
);


CREATE TABLE OrderFormItems
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    OrderFormId INT NOT NULL,
    StockId UNIQUEIDENTIFIER NOT NULL,

    SaleRate DECIMAL(18,2) NOT NULL,
    Qty INT NOT NULL,

    DeliveredQty INT NOT NULL DEFAULT 0,
    ReturnQty INT NOT NULL DEFAULT 0,

    FOREIGN KEY (OrderFormId) REFERENCES OrderForms(Id),
    FOREIGN KEY (StockId) REFERENCES Stocks(Id)
);

CREATE TABLE OrderPackingSlipMap
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    OrderFormId INT NOT NULL,
    PackingSlipId INT NOT NULL,

    FOREIGN KEY (OrderFormId) REFERENCES OrderForms(Id),
    FOREIGN KEY (PackingSlipId) REFERENCES PackingSlips(Id)
);



CREATE TABLE SaleInvoices
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    InvoiceNumber NVARCHAR(50) NOT NULL,
    Date DATETIME2(7) NOT NULL,
    FinanceYearId INT NOT NULL,

    OrderFormId INT NULL,  -- 🔥 Optional

    CustomerId UNIQUEIDENTIFIER NOT NULL,

    SubTotal DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    GrandTotal DECIMAL(18,2) NOT NULL,

    Status INT NOT NULL, -- 0=Draft,1=Posted,2=Cancelled

    IsDeleted BIT NOT NULL DEFAULT 0,

    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedByUserName NVARCHAR(255) NOT NULL,
    CreatedOn DATETIME2(7) NOT NULL,

    ModifiedBy UNIQUEIDENTIFIER NULL,
    ModifiedByUserName NVARCHAR(255) NULL,
    ModifiedOn DATETIME2(7) NULL,

    CONSTRAINT FK_SaleInvoices_FinanceYears 
        FOREIGN KEY (FinanceYearId) REFERENCES FinanceYears(Id),

    CONSTRAINT FK_SaleInvoices_OrderForms 
        FOREIGN KEY (OrderFormId) REFERENCES OrderForms(Id)
);

CREATE UNIQUE INDEX UX_SaleInvoices_OrderFormId
ON SaleInvoices(OrderFormId)
WHERE OrderFormId IS NOT NULL;




CREATE TABLE SaleInvoiceItems
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    SaleInvoiceId INT NOT NULL,
    PackingSlipItemId INT  NULL,

    StockId UNIQUEIDENTIFIER NOT NULL,

    SaleRate DECIMAL(18,2) NOT NULL,
    Qty INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,

    FOREIGN KEY (SaleInvoiceId) REFERENCES SaleInvoices(Id),
    FOREIGN KEY (PackingSlipItemId) REFERENCES PackingSlipItems(Id),
    FOREIGN KEY (StockId) REFERENCES Stocks(Id)
);


CREATE TABLE InvoicePackingMap
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    SaleInvoiceId INT NOT NULL,
    PackingSlipId INT NOT NULL,

    FOREIGN KEY (SaleInvoiceId) REFERENCES SaleInvoices(Id),
    FOREIGN KEY (PackingSlipId) REFERENCES PackingSlips(Id)
);
