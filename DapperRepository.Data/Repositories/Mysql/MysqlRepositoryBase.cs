using DapperRepository.Core;
using DapperRepository.Core.Constants;
using DapperRepository.Core.Domain;

namespace DapperRepository.Data.Repositories.Mysql
{
    public class MysqlRepositoryBase<T> : RepositoryBase<T> where T : BaseEntity
    {
        protected sealed override DatabaseType DataType
        {
            get { return DatabaseType.Mysql; }
        }

        /// <summary>
        /// 当前数据库连接串的key(默认主数据库key)
        /// </summary>
        protected override string ConnStrKey
        {
            get { return ConnKeyConstants.MysqlMasterKey; }
        }

        /// <inheritdoc />
        /// <summary>
        /// 数据表名(默认类名，如果不是，需要在子类重写)
        /// </summary>
        protected override string TableName
        {
            get { return string.Format("`{0}`", typeof(T).Name); }
        }
    }
}
