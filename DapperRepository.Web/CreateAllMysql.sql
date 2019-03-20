
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
  `Active` tinyint(1) NULL DEFAULT NULL,
  `CreationTime` datetime(0) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 104 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;

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
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------
-- Records of CustomerRole
-- ----------------------------
BEGIN;
INSERT INTO `CustomerRole` VALUES (1, 'Admin', 'Admin', NOW()), (2, 'Guest', 'Guest', NOW());
COMMIT;

-- ----------------------------
-- Table structure for Customer_CustomerRole_Mapping
-- ----------------------------
DROP TABLE IF EXISTS `Customer_CustomerRole_Mapping`;
CREATE TABLE `Customer_CustomerRole_Mapping`  (
  `CustomerId` int(11) NOT NULL,
  `CustomerRoleId` int(11) NOT NULL
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------
-- Procedure structure for DRD_Customer_GetAllCustomers
-- ----------------------------
DROP PROCEDURE IF EXISTS `DRD_Customer_GetAllCustomers`;
delimiter ;;
CREATE PROCEDURE `DRD_Customer_GetAllCustomers`(IN PageIndex INT,IN PageSize INT, OUT TotalRecords INT)
BEGIN
	DECLARE PageLowerBound INT;
    DECLARE PageUpperBound INT;
	SET PageLowerBound = PageSize * PageIndex;
	SET PageUpperBound = PageSize * PageIndex + PageSize + 1;
	CREATE TEMPORARY TABLE PageIndex
	( IndexId INT NOT NULL AUTO_INCREMENT PRIMARY KEY, CustomerId INT NOT NULL );
	INSERT INTO PageIndex
	( CustomerId ) SELECT
	Id 
	FROM
		Customer 
	ORDER BY
		Id DESC;
	
	SET TotalRecords = row_count( );

	SELECT c.Id,
		c.Username,
		c.Email,
		c.Active,
		c.CreationTime,
		cr.Id,
		cr.Name,
		cr.SystemName 
		FROM PageIndex pi
		INNER JOIN Customer c ON c.Id = pi.CustomerId
		INNER JOIN Customer_CustomerRole_Mapping crm ON c.Id = crm.CustomerId
		INNER JOIN CustomerRole cr ON crm.CustomerRoleId = cr.Id 
	WHERE
		pi.IndexId > PageLowerBound 
		AND pi.IndexId < PageUpperBound 
	ORDER BY
		pi.IndexId;
	DROP TEMPORARY TABLE PageIndex;

END
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
