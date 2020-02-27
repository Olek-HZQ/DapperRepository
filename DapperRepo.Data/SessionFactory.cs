using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using DapperRepo.Core;
using DapperRepo.Core.Data;
using DapperRepo.Core.Domain;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

namespace DapperRepo.Data
{
    public class SessionFactory
    {
        private static IDbConnection CreateConnection(DatabaseType dataType = DatabaseType.Mssql, string connStrKey = "")
        {
            try
            {
                IDbConnection conn;

                string connectionString = GetConnectionString(connStrKey);

                switch (dataType)
                {
                    case DatabaseType.Mssql:
                        conn = new SqlConnection(connectionString);
                        break;

                    case DatabaseType.Mysql:
                        conn = new MySqlConnection(connectionString);
                        break;

                    case DatabaseType.Oracle:
                        conn = new OracleConnection(connectionString);
                        break;

                    default:
                        conn = new SqlConnection(connectionString);
                        break;
                }

                conn.Open();

                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string GetConnectionString(string connStrKey)
        {
            if (string.IsNullOrEmpty(connStrKey))
                return string.Empty;

            string filePath = CommonHelper.DefaultFileProvider.MapPath("/App_Data/DbConnSettings.json");

            if (!File.Exists(filePath))
                return string.Empty;

            string text = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            Dictionary<string, string> connStrDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            return connStrDict[connStrKey];
        }

        /// <summary>
        /// 创建数据库连接会话
        /// </summary>
        /// <returns></returns>
        public static IDbSession CreateSession(DatabaseType databaseType, string key)
        {
            IDbConnection conn = CreateConnection(databaseType, key);
            IDbSession session = new DbSession(conn);
            return session;
        }
    }
}
