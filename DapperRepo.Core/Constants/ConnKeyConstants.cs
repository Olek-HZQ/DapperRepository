﻿namespace DapperRepo.Core.Constants
{
    /// <summary>
    /// 定义数据库连接串的key常量（适应同类数据库的多个不同数据库实例）
    /// </summary>
    public static class ConnKeyConstants
    {
        #region MSSQL

        public const string Mssql = "Mssql"; // 标识当前使用数据库为mssql

        public const string MssqlMasterKey = "MssqlMasterKey";

        // 可根据具体项目定义多个
        // public const string MssqlSecondConnKey = "MssqlSecondConnKey";

        #endregion

        #region MYSQL

        public const string Mysql = "Mysql";

        public const string MysqlMasterKey = "MysqlMasterKey";

        #endregion

        #region ORACLE

        public const string Oracle = "Oracle";

        public const string OracleMasterKey = "OracleMasterKey";

        #endregion
    }
}
