USE [DapperRepositoryDb];
GO

/****** Object:  StoredProcedure [dbo].[DRD_Customer_GetAllCustomers]    Script Date: 3/3/2019 1:23:45 PM ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
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
        DECLARE @RowsToReturn INT;
        SET @RowsToReturn = @PageSize * ( @PageIndex + 1 );
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
        SELECT TOP ( @RowsToReturn )
                c.Id ,
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
