USE [DapperRepositoryDb];
GO

/****** Object:  Table [dbo].[Customer]    Script Date: 2020/2/27 11:04:49 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO

CREATE TABLE [dbo].[Customer]
(
    [Id] [INT] IDENTITY(1, 1) NOT NULL,
    [Username] [NVARCHAR](32) NOT NULL,
    [Email] [NVARCHAR](128) NOT NULL,
    [Active] [BIT] NOT NULL,
    [Deleted] [BIT] NOT NULL,
    [CreationTime] [DATETIME] NOT NULL,
    CONSTRAINT [PK_Customer]
        PRIMARY KEY CLUSTERED ([Id] ASC)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
              ALLOW_PAGE_LOCKS = ON
             ) ON [PRIMARY]
) ON [PRIMARY];
GO