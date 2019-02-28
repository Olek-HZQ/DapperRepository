using System.Configuration;
using System.Web.Configuration;

namespace DapperRepository.Data
{
    public class ConnConfig
    {
        private readonly static Configuration Config = WebConfigurationManager.OpenWebConfiguration("~");

        /// <summary>
        /// mssql 连接字符串
        /// </summary>
        private static string _mssqlConnectionString = Config.AppSettings.Settings["MssqlConnectionString"].Value;
        /// <summary>
        /// mysql 连接字符串
        /// </summary>
        private static string _mysqlConnectionString = Config.AppSettings.Settings["MysqlConnectionString"].Value;
        /// <summary>
        /// oracle 连接字符串
        /// </summary>
        private static string _oracleConnectionString = Config.AppSettings.Settings["OracleConnectionString"].Value;

        public static string MssqlConnectionString
        {
            get { return _mssqlConnectionString; }
            set { _mssqlConnectionString = value; }
        }

        public static string MysqlConnectionString
        {
            get { return _mysqlConnectionString; }
            set { _mysqlConnectionString = value; }
        }

        public static string OracleConnectionString
        {
            get { return _oracleConnectionString; }
            set { _oracleConnectionString = value; }
        }
    }
}
