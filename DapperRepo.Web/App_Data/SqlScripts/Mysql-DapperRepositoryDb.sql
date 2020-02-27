/*
 Navicat Premium Data Transfer

 Source Server         : mysql-128
 Source Server Type    : MySQL
 Source Server Version : 80019
 Source Host           : 192.168.247.128:3306
 Source Schema         : DapperRepositoryDb

 Target Server Type    : MySQL
 Target Server Version : 80019
 File Encoding         : 65001

 Date: 27/02/2020 11:02:19
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for Customer
-- ----------------------------
DROP TABLE IF EXISTS `Customer`;
CREATE TABLE `Customer`  (
  `Id` int(0) NOT NULL AUTO_INCREMENT,
  `Username` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Active` tinyint(1) NOT NULL,
  `Deleted` tinyint(1) NOT NULL,
  `CreationTime` datetime(0) NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `In_Username`(`Username`(30)) USING BTREE,
  INDEX `In_Email`(`Email`(64)) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 13581486 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
