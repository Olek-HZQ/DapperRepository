using System.Data;
using System.Collections.Generic;

namespace DapperRepository.Core.Data
{
    public interface IRepository<T> where T : BaseEntity
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
        T GetById(string sql, object param = null, bool buffered = true, int? commandTimeout = null,
            CommandType? commandType = null, bool useTransaction = false);

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
        T GetBy(string sql, object param = null, bool buffered = true, int? commandTimeout = null,
            CommandType? commandType = null, bool useTransaction = false);

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
        IEnumerable<T> GetList(string sql, object param = null, bool buffered = true, int? commandTimeout = null,
            CommandType? commandType = null, bool useTransaction = false);

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity">要添加的实体对象</param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>执行结果（一般为添加的Id）</returns>
        dynamic Insert(T entity, int? commandTimeout = null, bool useTransaction = false);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="entity">要修改的实体对象</param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>执行结果（true or false）</returns>
        bool Update(T entity, int? commandTimeout = null, bool useTransaction = false);

        /// <summary>
        /// 删除数据（默认以主键删除）
        /// </summary>
        /// <param name="entityId">要删除的实体对象Id</param>
        /// <param name="predicate">where的条件（为空则用主键删除，不为空则以条件为准）</param>
        /// <param name="param">语句参数</param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>执行结果（true or false）</returns>
        bool Delete(int entityId, string predicate = "", object param = null, int? commandTimeout = null,
            bool useTransaction = false);

        /// <summary>
        /// 执行对象sql语句（一般需要事务处理）
        /// </summary>
        /// <param name="sql">sql语句或者存储过程</param>
        /// <param name="param">语句参数</param>
        /// <param name="commandTimeout">执行超时时间</param>
        /// <param name="commandType">命令类型（sql语句或是存储过程）</param>
        /// <param name="useTransaction">是否开启事务</param>
        /// <returns>执行受影响的行数</returns>
        int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null,
            bool useTransaction = true);
    }
}
