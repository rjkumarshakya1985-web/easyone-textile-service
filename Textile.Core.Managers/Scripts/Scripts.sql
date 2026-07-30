USE [TexttileERP]
GO

/****** Object:  Table [dbo].[Visitors]    Script Date: 28-Feb-26 2:41:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Visitors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NULL,
	[Name] [nvarchar](150) NULL,
	[Mobile] [nvarchar](15) NULL,
	[CustomerType] [int] NOT NULL,
	[VisitDate] [datetime2](7) NOT NULL,
	[CityId] [int] NULL,
	[Remarks] [nchar](255) NULL,
	[CreatedBy] [uniqueidentifier] NOT NULL,
	[CreatedByUserName] [varchar](255) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedByUserName] [varchar](255) NULL,
	[ModifiedOn] [datetime2](3) NULL,
 CONSTRAINT [PK__Visitors__3214EC0727C8B91B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Visitors] ADD  CONSTRAINT [DF__Visitors__VisitD__12FDD1B2]  DEFAULT (getdate()) FOR [VisitDate]
GO

ALTER TABLE [dbo].[Visitors] ADD  CONSTRAINT [DF__Visitors__Create__13F1F5EB]  DEFAULT (getdate()) FOR [CreatedOn]
GO

ALTER TABLE [dbo].[Visitors]  WITH CHECK ADD  CONSTRAINT [FK_Visitors_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
GO

ALTER TABLE [dbo].[Visitors] CHECK CONSTRAINT [FK_Visitors_Customers]
GO


