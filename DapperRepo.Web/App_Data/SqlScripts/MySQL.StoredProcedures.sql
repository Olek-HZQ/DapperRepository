CREATE DEFINER=`root`@`%` PROCEDURE `CustomerPaged`(
	`Username`		      VARCHAR(32),
	`Email`		          VARCHAR(128),
	`PageIndex`	        INT, 
	`PageSize`		      INT,
	OUT `TotalRecords`	INT
)
    READS SQL DATA
    SQL SECURITY INVOKER
BEGIN

		SET @sql_command = 'SELECT `Id` FROM `Customer` WHERE Deleted = 0';

		SET @Username = trim(COALESCE(`Username`, ''));
		SET @Email = trim(COALESCE(`Email`,''));
		
		IF @Username != '' THEN
		  SET @sql_command = CONCAT(@sql_command,' AND `Username` LIKE ''',CONCAT(@Username,'%'''));
    END IF;
		
		IF @Email != '' THEN
		  SET @sql_command = CONCAT(@sql_command,' AND `Email` LIKE ''',CONCAT(@Email,'%'''));
    END IF;
		
		SET @sql_command= CONCAT(@sql_command,' ORDER BY `Id` DESC');
		
		CREATE TEMPORARY TABLE `DisplayCustomerTmp`
	    (
		     Id INT NOT NULL auto_increment,
		     CustomerId INT NOT NULL,
         PRIMARY KEY (id)
	    );

		SET @sql_command = CONCAT('INSERT INTO `DisplayCustomerTmp` (`CustomerId`) ',@sql_command);
		
    PREPARE sql_do_stmts FROM @sql_command;
	  EXECUTE sql_do_stmts;
	  DEALLOCATE PREPARE sql_do_stmts;

		SELECT COUNT(Id) FROM `DisplayCustomerTmp` INTO `TotalRecords`;

	SELECT c.*
	FROM
		 `DisplayCustomerTmp` dct JOIN `Customer` c ON c.Id = dct.CustomerId
	WHERE
		dct.Id > `PageIndex` * `PageSize`
	ORDER BY
		dct.Id
	LIMIT `PageSize`;
	
	DROP TEMPORARY TABLE IF exists `DisplayCustomerTmp`;
END