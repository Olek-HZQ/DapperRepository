USE [DapperRepositoryDb]
GO

/****** Object:  Table [dbo].[Customer]    Script Date: 2019/2/28 14:54:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Customer](
	[Id] [INT] IDENTITY(1,1) NOT NULL,
	[Username] [NVARCHAR](32) NOT NULL,
	[Email] [NVARCHAR](128) NOT NULL,
	[Active] [BIT] NOT NULL,
	[CreationTime] [DATETIME] NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[CustomerRole]    Script Date: 2019/2/28 14:54:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CustomerRole](
	[Id] [INT] IDENTITY(1,1) NOT NULL,
	[Name] [NVARCHAR](32) NOT NULL,
	[SystemName] [NVARCHAR](32) NOT NULL,
	[CreationTime] [DATETIME] NOT NULL,
 CONSTRAINT [PK_CustomerRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Customer_CustomerRole_Mapping]    Script Date: 2019/2/28 14:54:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Customer_CustomerRole_Mapping](
	[CustomerId] [INT] NOT NULL,
	[CustomerRoleId] [INT] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Customer_CustomerRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Customer_CustomerRole_Mapping_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Customer_CustomerRole_Mapping] CHECK CONSTRAINT [FK_Customer_CustomerRole_Mapping_Customer]
GO

ALTER TABLE [dbo].[Customer_CustomerRole_Mapping]  WITH CHECK ADD  CONSTRAINT [FK_Customer_CustomerRole_Mapping_CustomerRole] FOREIGN KEY([CustomerRoleId])
REFERENCES [dbo].[CustomerRole] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Customer_CustomerRole_Mapping] CHECK CONSTRAINT [FK_Customer_CustomerRole_Mapping_CustomerRole]
GO


INSERT INTO [dbo].[CustomerRole]
           ([Name]
           ,[SystemName]
           ,[CreationTime])
     VALUES
           ('Admin',
           'Admin',
           GETDATE())
GO

INSERT INTO [dbo].[CustomerRole]
           ([Name]
           ,[SystemName]
           ,[CreationTime])
     VALUES
           ('Guest',
           'Guest',
           GETDATE())
GO


CREATE PROCEDURE [dbo].[DRD_Customer_GetAllCustomers]
    (
      @PageIndex INT ,
      @PageSize INT ,
      @TotalRecords INT OUTPUT
    )
AS
    BEGIN
        SET NOCOUNT ON;

		-- paging
        DECLARE @PageLowerBound INT;
        DECLARE @PageUpperBound INT;
        SET @PageLowerBound = @PageSize * @PageIndex;
        SET @PageUpperBound = @PageLowerBound + @PageSize + 1;

        CREATE TABLE #PageIndex
            (
              [IndexId] INT IDENTITY(1, 1)
                            NOT NULL ,
              [CustomerId] INT NOT NULL
            );

        INSERT  INTO #PageIndex
                ( CustomerId
                )
                SELECT  Id
                FROM    dbo.Customer
                ORDER BY Id DESC;

		-- total records
        SET @TotalRecords = @@ROWCOUNT;

		-- return customers
        SELECT  c.Id ,
                c.Username ,
                c.Email ,
                c.Active ,
                c.CreationTime ,
                cr.Id ,
                cr.Name ,
                cr.SystemName
        FROM    #PageIndex [pi]
                INNER JOIN dbo.Customer c ON c.Id = [pi].CustomerId
                INNER JOIN Customer_CustomerRole_Mapping crm ON c.Id = crm.CustomerId
                INNER JOIN CustomerRole cr ON crm.CustomerRoleId = cr.Id
        WHERE   pi.IndexId > @PageLowerBound
                AND pi.IndexId < @PageUpperBound
        ORDER BY pi.IndexId;

        DROP TABLE #PageIndex;

    END;
