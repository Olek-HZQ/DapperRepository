SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for Customer
-- ----------------------------
DROP TABLE IF EXISTS `Customer`;
CREATE TABLE `Customer`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Active` tinyint(1) NOT NULL,
  `CreationTime` datetime(0) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `In_Username`(`Username`(30)) USING BTREE,
  INDEX `In_Email`(`Email`(64)) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 13580486 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for CustomerRole
-- ----------------------------
DROP TABLE IF EXISTS `CustomerRole`;
CREATE TABLE `CustomerRole`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SystemName` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreationTime` datetime(0) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 7 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for Customer_CustomerRole_Mapping
-- ----------------------------
DROP TABLE IF EXISTS `Customer_CustomerRole_Mapping`;
CREATE TABLE `Customer_CustomerRole_Mapping`  (
  `CustomerId` int(11) NOT NULL,
  `CustomerRoleId` int(11) NOT NULL,
  INDEX `In_CustomerId`(`CustomerId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Procedure structure for DRD_Customer_GetAllCustomers
-- ----------------------------
DROP PROCEDURE IF EXISTS `DRD_Customer_GetAllCustomers`;
delimiter ;;
CREATE PROCEDURE `DRD_Customer_GetAllCustomers`(IN `PageLowerBound` INT,IN `PageSize` INT,IN `Username` varchar(30),IN `Email` varchar(64),IN `UseDescOrder` INT)
BEGIN
    
		SET @strsql = 'SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,ccrm.CustomerRoleId FROM `Customer` c INNER JOIN `Customer_CustomerRole_Mapping` ccrm ON c.Id = ccrm.CustomerId INNER JOIN (SELECT Id FROM `Customer` WHERE 1 = 1 ';
		
		IF(LENGTH(@Username)>0) THEN
		SET @strsql = CONCAT(@strsql,'AND Username LIKE ''',CONCAT(@Username,'%'),''' ');
		ELSE
		SET @strsql = CONCAT(@strsql,' ');
		END IF;
		
		IF(LENGTH(@Email)>0) THEN
		SET @strsql = CONCAT(@strsql,'AND Email LIKE ''',CONCAT(@Email,'%'),''' ');
		ELSE
		SET @strsql = CONCAT(@strsql,' ');
		END IF;
		
		IF(@UseDescOrder=1) THEN
		SET @strsql = CONCAT(@strsql,'ORDER BY Id DESC LIMIT ',@PageLowerBound,', ', @PageSize,') AS cu USING (Id)');
		ELSE
		SET @strsql = CONCAT(@strsql,'ORDER BY Id LIMIT ',@PageLowerBound,', ', @PageSize,') AS cu USING (Id)');
		END IF;
		
		SET @strSql = CONCAT(@strSql,' ORDER BY c.Id DESC;');
		
		-- SELECT @strsql;
		
		PREPARE stmt1 FROM @strsql; 
    EXECUTE stmt1; 
    DEALLOCATE PREPARE stmt1; 

END
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;