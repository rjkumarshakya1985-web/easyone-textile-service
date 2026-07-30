--Create Tally Company Master table
USE [TexttileERP]
GO
/****** Object:  Table [dbo].[TallyCompanies]    Script Date: 17-Apr-26 10:31:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TallyCompanies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[StateId] [int] NOT NULL,
	[GSTIN] [nvarchar](20) NULL,
	[StateName] [nvarchar](100) NULL,
	[GSTRegistrationType] [nvarchar](100) NULL,
	[Consignee] [nvarchar](100) NULL,
	[ConsigneeAddress] [nvarchar](max) NULL,
	[PINCode] [nvarchar](6) NULL,
	[Email] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedOn] [datetime] NULL,
 CONSTRAINT [PK__TallyCom__3214EC078C8D34A1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[TallyCompanies] ON 
GO
INSERT [dbo].[TallyCompanies] ([Id], [Name], [StateId], [GSTIN], [StateName], [GSTRegistrationType], [Consignee], [ConsigneeAddress], [PINCode], [Email], [IsActive], [CreatedOn]) VALUES (1, N'SHIV SAHAY BHAGWAN DAS SAREES PVT. LTD.(2025-26)', 9, N'09AARCS1924P1ZJ', N'Uttar Pradesh', N'Regular', N'SHIV SAHAY BHAGWAN DAS SAREES PVT. LTD.(2025-26)', N'6/20, Yamuna Kinara Road, Belanganj, Agra",
            "Accounts- 7817803383 Sales- 7817803384",
            "MSME- UP-01-0072013', N'282004', N'ssbd432@gmail.com', 1, CAST(N'2026-03-18T12:29:15.117' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[TallyCompanies] OFF
GO
ALTER TABLE [dbo].[TallyCompanies] ADD  CONSTRAINT [DF_TallyCompanies_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[TallyCompanies] ADD  CONSTRAINT [DF__TallyComp__Creat__1C873BEC]  DEFAULT (getdate()) FOR [CreatedOn]
GO


--Start Create Tally Configs reference table
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TallyConfigs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[TransactionType] [nvarchar](20) NOT NULL,	
	[TaxType] [nvarchar](10) NOT NULL,	
	[LedgerName] [nvarchar](200) NOT NULL,
	[CreatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_TallyLedger] UNIQUE NONCLUSTERED 
(
	[CompanyId] ASC,
	[TransactionType] ASC,	
	[TaxType] ASC
	
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TallyConfigs] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO

ALTER TABLE [dbo].[TallyConfigs]  WITH CHECK ADD FOREIGN KEY([CompanyId])
REFERENCES [dbo].[TallyCompanies] ([Id])
GO

--End Create Tally Configs reference table


--✅ DEFAULT DATA
-- PURCHASE
INSERT INTO TallyConfigs VALUES (1,'PURCHASE','MAIN','Purchase',GETDATE());
INSERT INTO TallyConfigs VALUES (1,'PURCHASE','CGST','Input CGST',GETDATE());
INSERT INTO TallyConfigs VALUES (1,'PURCHASE','SGST','Input SGST',GETDATE());
INSERT INTO TallyConfigs VALUES (1,'PURCHASE','IGST','Input IGST',GETDATE());

-- SALE
INSERT INTO TallyConfigs VALUES (1,'SALE','MAIN','Sales',GETDATE());
INSERT INTO TallyConfigs VALUES (1,'SALE','CGST','Output CGST',GETDATE());
INSERT INTO TallyConfigs VALUES (1,'SALE','SGST','Output SGST',GETDATE());
INSERT INTO TallyConfigs VALUES (1,'SALE','IGST','Output IGST',GETDATE());


USE [TexttileERP]
GO

/****** Object:  Table [dbo].[TallyProcessLogs]    Script Date: 26-Mar-26 1:11:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TallyProcessLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[FinanceYearId] [int] NOT NULL,
	[ReferenceNo] [nvarchar](50) NULL,
	[ProcessType] [int] NULL,
	[Step] [int] NULL,
	[IsSuccess] [bit] NULL,
	[RequestData] [nvarchar](max) NULL,
	[ResponseData] [nvarchar](max) NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[CreatedOn] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[TallyProcessLogs] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO

ALTER TABLE [dbo].[TallyProcessLogs]  WITH CHECK ADD FOREIGN KEY([CompanyId])
REFERENCES [dbo].[TallyCompanies] ([Id])
GO

ALTER TABLE [dbo].[TallyProcessLogs]  WITH CHECK ADD FOREIGN KEY([FinanceYearId])
REFERENCES [dbo].[FinanceYears] ([Id])
GO

