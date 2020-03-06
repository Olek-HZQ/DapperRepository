using DapperRepository.Core.Data;
using DapperRepository.Core.Domain;
using SqlKata;

namespace DapperRepository.Data
{
    public abstract class RepositoryDataTypeBase
    {
        protected IDbSession DbSession => SessionFactory.CreateSession(DataType, ConnStrKey);

        /// <summary>
        /// 当前数据库连接串的key
        /// </summary>
        protected abstract string ConnStrKey { get; }

        /// <summary>
        /// 数据库类型（MSSQL,MYSQL...）
        /// </summary>
        protected abstract DatabaseType DataType { get; }

        /// <summary>
        /// 数据表名(默认类名，如果不是，需要在子类重写)
        /// </summary>
        protected abstract string TableName { get; }

        protected abstract SqlResult GetSqlResult(Query query);
    }
}
