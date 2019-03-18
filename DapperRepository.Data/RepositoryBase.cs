using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Dapper;
using DapperExtensions;
using DapperRepository.Core;
using DapperRepository.Core.Data;

namespace DapperRepository.Data
{
    public abstract class RepositoryBase<T> : RepositoryDataTypeBase where T : BaseEntity
    {
        /// <summary>
        /// 根据主键获取一条数据
        /// </summary>
        /// <param name="sql">sql语句或者存储过程</param>
        /// <param name="param">语句参数</param>
        /// <param name="buffered">是否缓冲查询数据，详细信息：https://dapper-tutorial.net/buffered </param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="commandType">命令类型（sql语句或是存储过程）</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>当前查询数据</returns>
        public virtual T GetById(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null, bool useTransaction = false)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            IDbSession session = DbSession;

            T result = session.Connection.Query<T>(sql, param, null, buffered, commandTimeout, commandType).SingleOrDefault();

            session.Dispose(); // 释放资源

            return result;
        }

        /// <summary>
        /// 根据相关条件获取一条数据
        /// </summary>
        /// <param name="sql">sql语句或者存储过程</param>
        /// <param name="param">语句参数</param>
        /// <param name="buffered">是否缓冲查询数据，详细信息：https://dapper-tutorial.net/buffered </param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="commandType">命令类型（sql语句或是存储过程）</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>当前查询数据</returns>
        public virtual T GetBy(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null, bool useTransaction = false)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            IDbSession session = DbSession;

            T result = session.Connection.Query<T>(sql, param, null, buffered, commandTimeout, commandType).FirstOrDefault();

            session.Dispose(); // 释放资源

            return result;
        }

        /// <summary>
        /// 获取数据列表（所有、部分或者分页获取）
        /// </summary>
        /// <param name="sql">sql语句或者存储过程</param>
        /// <param name="param">语句参数</param>
        /// <param name="buffered">是否缓冲查询数据，详细信息：https://dapper-tutorial.net/buffered </param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="commandType">命令类型（sql语句或是存储过程）</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>当前查询数据列表</returns>
        public virtual IEnumerable<T> GetList(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null, bool useTransaction = false)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            IEnumerable<T> results;

            IDbSession session = DbSession;
            if (useTransaction)
            {
                session.BeginTrans();

                results = session.Connection.Query<T>(sql, param, session.Transaction, buffered, commandTimeout, commandType).ToList();
                session.Commit();
            }
            else
            {
                results = session.Connection.Query<T>(sql, param, null, buffered, commandTimeout, commandType).ToList();
            }

            session.Dispose(); // 释放资源

            return results;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity">要添加的实体对象</param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>执行结果（一般为添加的Id）</returns>
        public virtual dynamic Insert(T entity, int? commandTimeout = null, bool useTransaction = false)
        {
            IDbSession session = DbSession;

            try
            {
                if (useTransaction)
                {
                    session.BeginTrans();

                    dynamic result = session.Connection.Insert(entity, session.Transaction, commandTimeout);
                    session.Commit();
                    return result;
                }
                else
                {
                    return session.Connection.Insert(entity, null, commandTimeout);
                }
            }
            catch (Exception)
            {
                if (useTransaction)
                {
                    session.Rollback();
                }

                return null;
            }
            finally
            {
                session.Dispose(); // 释放资源
            }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="entity">要修改的实体对象</param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>执行结果（true or false）</returns>
        public virtual bool Update(T entity, int? commandTimeout = null, bool useTransaction = false)
        {
            IDbSession session = DbSession;

            try
            {
                if (useTransaction)
                {
                    session.BeginTrans();

                    bool result = session.Connection.Update(entity, session.Transaction, commandTimeout);
                    session.Commit();
                    return result;
                }
                else
                {
                    return session.Connection.Update(entity, null, commandTimeout);
                }
            }
            catch (Exception)
            {
                if (useTransaction)
                {
                    session.Rollback();
                }

                return false;
            }
            finally
            {
                session.Dispose(); // 释放资源
            }
        }

        /// <summary>
        /// 删除数据（默认以主键删除）
        /// </summary>
        /// <param name="entityId">要删除的实体对象Id</param>
        /// <param name="predicate">where的条件（为空则用主键删除，不为空则以条件为准）</param>
        /// <param name="param">语句参数</param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>执行结果（true or false）</returns>
        public virtual bool Delete(int entityId, string predicate = "", object param = null, int? commandTimeout = null, bool useTransaction = false)
        {
            StringBuilder builder = new StringBuilder(string.Format("DELETE FROM {0}", TableName));
            builder.Append(" WHERE ");

            builder.Append(string.IsNullOrEmpty(predicate) ? string.Format("Id = {0};", entityId) : predicate);

            return Execute(builder.ToString(), param, null, CommandType.Text, useTransaction) > 0;
        }

        /// <summary>
        /// 执行对象sql语句（一般需要事务处理）
        /// </summary>
        /// <param name="sql">sql语句或者存储过程</param>
        /// <param name="param">语句参数</param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="commandType">命令类型（sql语句或是存储过程）</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>执行受影响的行数</returns>
        public virtual int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, bool useTransaction = true)
        {
            if (string.IsNullOrEmpty(sql))
                return 0;

            IDbSession session = DbSession;

            try
            {
                if (useTransaction)
                {
                    session.BeginTrans();

                    int rowsAffected = session.Connection.Execute(sql, param, session.Transaction, commandTimeout, commandType);
                    session.Commit();

                    return rowsAffected;
                }
                else
                {
                    return session.Connection.Execute(sql, param, null, commandTimeout, commandType);
                }
            }
            catch (Exception)
            {
                if (useTransaction)
                {
                    session.Rollback();
                }

                return 0;
            }
            finally
            {
                session.Dispose(); // 释放资源
            }
        }
    }
}
