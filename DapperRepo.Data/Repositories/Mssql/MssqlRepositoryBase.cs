using DapperRepo.Core.Constants;
using DapperRepo.Core.Domain;
using SqlKata;
using SqlKata.Compilers;

namespace DapperRepo.Data.Repositories.Mssql
{
    public class MssqlRepositoryBase<T> : RepositoryBase<T> where T : BaseEntity
    {
        protected sealed override DatabaseType DataType => DatabaseType.Mssql;

        /// <inheritdoc />
        /// <summary>
        /// 当前数据库连接串的key(默认主数据库key)
        /// </summary>
        protected override string ConnStrKey => ConnKeyConstants.MssqlMasterKey;

        /// <inheritdoc />
        /// <summary>
        /// 数据表名(默认类名，如果不是，需要在子类重写)
        /// </summary>
        protected override string TableName => typeof(T).Name;

        protected override SqlResult GetSqlResult(Query query)
        {
            var compiler = new SqlServerCompiler();

            SqlResult sqlResult = compiler.Compile(query);

            return sqlResult;
        }
    }
}
