using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using DapperRepository.Core.Data;
using DapperRepository.Core.Domain;
using MySql.Data.MySqlClient;

namespace DapperRepository.Data
{
    public class SessionFactory
    {
        private static IDbConnection CreateConnection(DatabaseType dataType)
        {
            IDbConnection conn;
            switch (dataType)
            {
                case DatabaseType.Mssql:
                    conn = new SqlConnection(ConnConfig.MssqlConnectionString);
                    break;
                case DatabaseType.Mysql:
                    conn = new MySqlConnection(ConnConfig.MysqlConnectionString);
                    break;
                case DatabaseType.Oracle:
                    conn = new OracleConnection(ConnConfig.OracleConnectionString);
                    break;
                default:
                    conn = new SqlConnection(ConnConfig.MssqlConnectionString);
                    break;
            }

            conn.Open();

            return conn;
        }

        /// <summary>
        /// 创建数据库连接会话
        /// </summary>
        /// <returns></returns>
        public static IDbSession CreateSession(DatabaseType databaseType)
        {
            IDbConnection conn = CreateConnection(databaseType);
            IDbSession session = new DbSession(conn);
            return session;
        }
    }
}
