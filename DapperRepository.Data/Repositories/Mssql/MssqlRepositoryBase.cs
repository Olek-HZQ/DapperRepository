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

        /// <summary>
        /// 当前数据库连接串的key(默认主数据库key)
        /// </summary>
        protected override string ConnStrKey
        {
            get { return ConnKeyConstants.MssqlMasterKey; }
        }

        protected override string TableName
        {
            get { return typeof(T).Name; }
        }
    }
}
