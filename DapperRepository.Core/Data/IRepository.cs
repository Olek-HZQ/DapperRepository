using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DapperRepository.Core.Data
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Returns a single entity by a single id from table "Ts" asynchronously using Task. T must be of interface type.
        /// Id must be marked with [Key] attribute.
        /// Created entity is tracked/intercepted for changes and used by the Update() extension.
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <param name="useTransaction">Use transaction or not</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>Entity of T</returns>
        Task<T> GetAsync(int id, bool useTransaction = false, int? commandTimeout = null);

        /// <summary>
        /// Returns a single entity by a single id from table "Ts" asynchronously using Task. T must be of interface type.
        /// Id must be marked with [Key] attribute.
        /// Created entity is tracked/intercepted for changes and used by the Update() extension.
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="sql">query sql</param>
        /// <param name="param">parameters</param>
        /// <param name="useTransaction">Use transaction or not</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>Entity of T</returns>
        Task<T> GetFirstOrDefaultAsync(string sql, object param = null, bool useTransaction = false,
            int? commandTimeout = null);

        /// <summary>
        /// get data list on relevant conditions
        /// </summary>
        /// <param name="sql">sql query or stored procedure</param>
        /// <param name="param">parameters</param>
        /// <param name="useTransaction">Use transaction or not</param>
        /// <param name="commandTimeout">command time out</param>
        /// <param name="commandType">command type (sql or stored procedure)</param>
        /// <returns>data list</returns>
        Task<IEnumerable<T>> GetListAsync(string sql, object param = null, bool useTransaction = false,
            int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Inserts an entity into table "Ts" and returns identity id or number of inserted rows if inserting a list.
        /// </summary>
        /// <typeparam name="T">The type to insert.</typeparam>
        /// <param name="entity">Entity to insert, can be list of entities</param>
        /// <param name="useTransaction">Use transaction or not</param>
        /// <param name="commandTimeout">command time out</param>
        /// <returns>Identity of inserted entity, or number of inserted rows if inserting a list</returns>
        Task<int> InsertAsync(T entity, bool useTransaction = false, int? commandTimeout = null);

        /// <summary>
        /// Updates entity in table "Ts" asynchronously using Task, checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="entity">Entity to be updated</param>
        /// <param name="useTransaction">Use transaction or not</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        Task<bool> UpdateAsync(T entity, bool useTransaction = false, int? commandTimeout = null);

        /// <summary>
        /// Delete entity in table "Ts" asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="entity">Entity to delete</param>
        /// <param name="useTransaction">Use transaction or not</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
        /// <returns>true if deleted, false if not found</returns>
        Task<bool> DeleteAsync(T entity, bool useTransaction = false, int? commandTimeout = null);

        /// <summary>Execute a command asynchronously using Task.</summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <param name="useTransaction">Use transaction or not</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The number of rows affected.</returns>
        Task<int> ExecuteAsync(string sql, object param = null, bool useTransaction = false,
            int? commandTimeout = null, CommandType? commandType = null);
    }
}
