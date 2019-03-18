using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using DapperRepository.Core.Configuration;
using DapperRepository.Core.Data;
using DapperRepository.Core.Domain;
using MySql.Data.MySqlClient;

namespace DapperRepository.Data
{
    public class SessionFactory
    {
        private static IDbConnection CreateConnection(DatabaseType dataType = DatabaseType.Mssql, string connStrKey = "")
        {
            IDbConnection conn;
            switch (dataType)
            {
                case DatabaseType.Mssql:
                    conn = new SqlConnection(ConnConfig.GetConnectionString(connStrKey));
                    break;

                case DatabaseType.Mysql:
                    conn = new MySqlConnection(ConnConfig.GetConnectionString(connStrKey));
                    break;

                case DatabaseType.Oracle:
                    conn = new OracleConnection(ConnConfig.GetConnectionString(connStrKey));
                    break;

                default:
                    conn = new SqlConnection(ConnConfig.GetConnectionString(connStrKey));
                    break;
            }

            conn.Open();

            return conn;
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
