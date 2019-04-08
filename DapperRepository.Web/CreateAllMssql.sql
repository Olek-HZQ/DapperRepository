USE [DapperRepositoryDb]
GO
/****** Object:  StoredProcedure [dbo].[DRD_Customer_GetAllCustomers]    Script Date: 4/5/2019 9:01:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Huangzhongqiu>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DRD_Customer_GetAllCustomers] 
	-- Add the parameters for the stored procedure here
    @PageLowerBound NVARCHAR(10) ,
    @PageSize NVARCHAR(10) ,
    @Username NVARCHAR(30) = '' ,
    @Email NVARCHAR(64) = '' ,
    @UseDescOrder BIT = 0
AS
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
        SET NOCOUNT ON;

        DECLARE @StrSql NVARCHAR(512) = '';
        SET @Username = ISNULL(@Username, '');
        SET @Email = ISNULL(@Email, '');
        SET @StrSql = 'SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,ccrm.CustomerRoleId FROM Customer c 
		INNER JOIN Customer_CustomerRole_Mapping ccrm ON c.Id = ccrm.CustomerId 
		INNER JOIN (SELECT Id FROM Customer WHERE 1 = 1 ';
		
        IF ( @Username != '' )
            BEGIN
                SET @StrSql = CONCAT(@StrSql, 'AND Username LIKE ''',
                                     CONCAT(@Username, '%'), ''' ');
            END;
        ELSE
            BEGIN
                SET @StrSql = CONCAT(@StrSql, ' ');
            END;
		
        IF ( @Email != '' )
            BEGIN
                SET @StrSql = CONCAT(@StrSql, 'AND Email LIKE ''',
                                     CONCAT(@Email, '%'), ''' ');
            END;
        ELSE
            BEGIN
                SET @StrSql = CONCAT(@StrSql, ' ');
            END;
		
        IF ( @UseDescOrder = 1 )
            BEGIN
                SET @StrSql = CONCAT(@StrSql,
                                     'ORDER BY Id DESC OFFSET '
                                     + CAST(@PageLowerBound AS VARCHAR)
                                     + ' ROWS FETCH NEXT '
                                     + CAST(@PageSize AS VARCHAR)
                                     + ' ROWS ONLY ) AS cu ON cu.Id = c.Id');
            END;
        ELSE
            BEGIN
                SET @StrSql = CONCAT(@StrSql,
                                     'ORDER BY Id OFFSET '
                                     + CAST(@PageLowerBound AS VARCHAR)
                                     + ' ROWS FETCH NEXT '
                                     + CAST(@PageSize AS VARCHAR)
                                     + ' ROWS ONLY ) AS cu ON cu.Id = c.Id');
            END;    
						
			SET @strSql = CONCAT(@strSql,' ORDER BY c.Id DESC;');

        EXEC (@StrSql);

    END;

GO
/****** Object:  Table [dbo].[Customer]    Script Date: 4/5/2019 9:01:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](32) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[Active] [bit] NOT NULL,
	[CreationTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Customer_CustomerRole_Mapping]    Script Date: 4/5/2019 9:01:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer_CustomerRole_Mapping](
	[CustomerId] [int] NOT NULL,
	[CustomerRoleId] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CustomerRole]    Script Date: 4/5/2019 9:01:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
	[SystemName] [nvarchar](32) NOT NULL,
	[CreationTime] [datetime] NOT NULL,
 CONSTRAINT [PK_CustomerRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Customer_Email]    Script Date: 4/5/2019 9:01:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [dbo].[Customer]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Customer_Username]    Script Date: 4/5/2019 9:01:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [dbo].[Customer]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CustomerId]    Script Date: 4/5/2019 9:01:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomerId] ON [dbo].[Customer_CustomerRole_Mapping]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
