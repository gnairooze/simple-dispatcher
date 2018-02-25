USE [LoggerDB]
GO

CREATE TABLE [dbo].[LogDataModels](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[BusinessID] [uniqueidentifier] NOT NULL,
	[Group] [uniqueidentifier] NOT NULL,
	[Counter] [int] NOT NULL,
	[LogTypeID] [tinyint] NOT NULL,
	[Who] [nvarchar](500) NOT NULL,
	[What] [nvarchar](max) NOT NULL,
	[When] [datetime] NOT NULL,
	[ReferenceName] [nvarchar](30) NOT NULL,
	[ReferenceValue] [nvarchar](30) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.LogDataModels] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

/****** Object:  Index [IX_BusinessID]    Script Date: 2017-06-23 7:11:35 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_BusinessID] ON [dbo].[LogDataModels]
(
	[BusinessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_Group]    Script Date: 2017-06-23 7:11:35 PM ******/
CREATE NONCLUSTERED INDEX [IX_Group] ON [dbo].[LogDataModels]
(
	[Group] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_LogTypeID]    Script Date: 2017-06-23 7:11:35 PM ******/
CREATE NONCLUSTERED INDEX [IX_LogTypeID] ON [dbo].[LogDataModels]
(
	[LogTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


