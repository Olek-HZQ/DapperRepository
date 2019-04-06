using DapperRepository.Core;
using DapperRepository.Core.Constants;
using DapperRepository.Core.Domain;

namespace DapperRepository.Data.Repositories.Mssql
{
    public class MssqlRepositoryBase<T> : RepositoryBase<T> where T : BaseEntity
    {
        protected sealed override DatabaseType DataType
        {
            get { return DatabaseType.Mssql; }
        }

        /// <inheritdoc />
        /// <summary>
        /// 当前数据库连接串的key(默认主数据库key)
        /// </summary>
        protected override string ConnStrKey
        {
            get { return ConnKeyConstants.MssqlMasterKey; }
        }

        /// <inheritdoc />
        /// <summary>
        /// 数据表名(默认类名，如果不是，需要在子类重写)
        /// </summary>
        protected override string TableName
        {
            get { return string.Format("[dbo].[{0}]", typeof(T).Name); }
        }
    }
}
