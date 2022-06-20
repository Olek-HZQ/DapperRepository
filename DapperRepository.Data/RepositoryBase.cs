using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using DapperRepository.Core;
using DapperRepository.Core.Data;

namespace DapperRepository.Data
{
    public abstract class RepositoryBase<T> : RepositoryDataTypeBase where T : BaseEntity
    {
        public virtual async Task<T> GetAsync(int id, bool useTransaction = false, int? commandTimeout = null)
        {
            if (id == 0)
                return null;

            IDbSession session = DbSession;

            IDbTransaction transaction = null;
            if (useTransaction)
            {
                session.BeginTrans();
                transaction = session.Transaction;
            }

            var result = await session.Connection.GetAsync<T>(id, transaction, commandTimeout);

            if (useTransaction)
                session.Commit();

            session.Dispose();

            return result;
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(string sql, object param = null, bool useTransaction = false, int? commandTimeout = null)
        {
            IDbSession session = DbSession;

            IDbTransaction transaction = null;
            if (useTransaction)
            {
                session.BeginTrans();
                transaction = session.Transaction;
            }

            var result = await session.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout);

            if (useTransaction)
                session.Commit();

            session.Dispose();

            return result;
        }

        public virtual async Task<IEnumerable<T>> GetListAsync(string sql, object param = null, bool useTransaction = false,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            IDbSession session = DbSession;

            IDbTransaction transaction = null;
            if (useTransaction)
            {
                session.BeginTrans();
                transaction = session.Transaction;
            }

            var result = await session.Connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);

            if (useTransaction)
                session.Commit();

            session.Dispose();

            return result;
        }

        public virtual async Task<int> InsertAsync(T entity, bool useTransaction = false, int? commandTimeout = null)
        {
            IDbSession session = DbSession;

            IDbTransaction transaction = null;
            if (useTransaction)
            {
                session.BeginTrans();
                transaction = session.Transaction;
            }

            int result = 0;

            try
            {
                result = await session.Connection.InsertAsync(entity, transaction, commandTimeout);
            }
            catch
            {
                if (useTransaction)
                {
                    session.Rollback();
                }
                // throw;
            }

            session.Dispose();

            return result;
        }

        public virtual async Task<bool> UpdateAsync(T entity, bool useTransaction = false, int? commandTimeout = null)
        {
            IDbSession session = DbSession;

            IDbTransaction transaction = null;
            if (useTransaction)
            {
                session.BeginTrans();
                transaction = session.Transaction;
            }

            bool result = false;

            try
            {
                result = await session.Connection.UpdateAsync(entity, transaction, commandTimeout);
            }
            catch
            {
                if (useTransaction)
                {
                    session.Rollback();
                }

                // throw;
            }

            session.Dispose();

            return result;
        }

        public virtual async Task<bool> DeleteAsync(T entity, bool useTransaction = false, int? commandTimeout = null)
        {
            IDbSession session = DbSession;

            IDbTransaction transaction = null;
            if (useTransaction)
            {
                session.BeginTrans();
                transaction = session.Transaction;
            }

            bool result = false;

            try
            {
                result = await session.Connection.DeleteAsync(entity, transaction, commandTimeout);
            }
            catch
            {
                if (useTransaction)
                {
                    session.Rollback();
                }

                // throw;
            }

            session.Dispose();

            return result;
        }

        public virtual async Task<int> ExecuteAsync(string sql, object param = null, bool useTransaction = false, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            IDbSession session = DbSession;

            IDbTransaction transaction = null;
            if (useTransaction)
            {
                session.BeginTrans();
                transaction = session.Transaction;
            }

            int result = 0;

            try
            {
                result = await session.Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
            }
            catch
            {
                if (useTransaction)
                {
                    session.Rollback();
                }

                // throw;
            }

            session.Dispose();

            return result;
        }
    }
}
