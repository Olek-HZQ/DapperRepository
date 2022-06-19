USE [DapperRepositoryDb];
GO

/****** Object:  StoredProcedure [dbo].[CustomerPaged]    Script Date: 2020/7/20 18:44:09 ******/
SET ANSI_NULLS ON;
GO

SET QUOTED_IDENTIFIER ON;
GO


CREATE PROCEDURE [dbo].[CustomerPaged]
(
    @Username NVARCHAR(32) = NULL,
    @Email NVARCHAR(128) = NULL,
    @PageIndex INT = 0,
    @PageSize INT = 2147483644,
    @TotalRecords INT = NULL OUTPUT
)
AS
BEGIN

    DECLARE @sql NVARCHAR(MAX);

    SET NOCOUNT ON;

    SET @Username = ISNULL(@Username, '');
    SET @Username = RTRIM(LTRIM(@Username));
    SET @Email = ISNULL(@Email, '');
    SET @Email = RTRIM(LTRIM(@Email));

    SET @sql = N'SELECT Id FROM [dbo].[Customer] WITH (NOLOCK) WHERE Deleted = 0';

    IF @Username <> ''
    BEGIN
        SET @sql = @sql + N' AND Username like ''' + @Username + N'%''';
    END;

    IF @Email <> ''
    BEGIN
        SET @sql = @sql + N' AND Email like ''' + @Email + N'%''';
    END;

    SET @sql = @sql + N' ORDER BY Id DESC';
    --paging
    DECLARE @PageLowerBound INT;
    DECLARE @PageUpperBound INT;
    DECLARE @RowsToReturn INT;
    SET @RowsToReturn = @PageSize * (@PageIndex + 1);
    SET @PageLowerBound = @PageSize * @PageIndex;
    SET @PageUpperBound = @PageLowerBound + @PageSize + 1;

    CREATE TABLE #DisplayCustomerTmp
    (
        [Id] INT IDENTITY(1, 1) NOT NULL,
        [CustomerId] INT NOT NULL
    );

    SET @sql = N'INSERT INTO #DisplayCustomerTmp (CustomerId) ' + @sql;

    -- PRINT (@sql)

    EXEC sp_executesql @sql;

    CREATE TABLE #PageIndex
    (
        [IndexId] INT IDENTITY(1, 1) NOT NULL,
        [CustomerId] INT NOT NULL
    );
    INSERT INTO #PageIndex
    (
        [CustomerId]
    )
    SELECT CustomerId
    FROM #DisplayCustomerTmp
    ORDER BY [Id];

    --total records
    SET @TotalRecords = @@rowcount;

    DROP TABLE #DisplayCustomerTmp;

    SELECT TOP (@RowsToReturn)
           p.Id,
           p.Username,
           p.Email,
           p.Active,
           p.Deleted,
           p.CreationTime
    FROM #PageIndex [pi]
        INNER JOIN dbo.Customer p
            ON p.Id = [pi].[CustomerId]
    WHERE [pi].IndexId > @PageLowerBound
          AND [pi].IndexId < @PageUpperBound
    ORDER BY [pi].IndexId;

    DROP TABLE #PageIndex;
END;

GO