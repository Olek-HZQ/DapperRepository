using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperRepo.Core.Configuration;
using DapperRepo.Core.Data;
using DapperRepo.Core.Domain.Customers;
using DapperRepo.Data.Repositories.BaseInterfaces;
using SqlKata;

namespace DapperRepo.Data.Repositories.Mssql.Customers
{
    public class CustomerRepository : MssqlRepositoryBase<Customer>, ICustomerRepository
    {
        private readonly AppConfig _appConfig;

        public CustomerRepository(AppConfig appConfig)
        {
            _appConfig = appConfig;
        }

        public virtual async Task<Customer> GetCustomerByIdAsync(int id)
        {
            Customer customer = await GetAsync(id);

            return customer;
        }

        public virtual async Task<Customer> GetCustomerByAsync(string name, string email)
        {
            Query query = new Query(TableName).Select("Username", "Email", "Active", "CreationTime").WhereFalse("Deleted");

            if (!string.IsNullOrEmpty(name))
            {
                query = query.WhereContains("Username", name);
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.WhereContains("Email", email);
            }

            SqlResult sqlResult = GetSqlResult(query);

            return await GetFirstOrDefaultAsync(sqlResult.Sql, sqlResult.NamedBindings);
        }

        public virtual async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            Query query = new Query(TableName).Select("Username", "Email", "Active", "CreationTime").WhereFalse("Deleted");

            SqlResult sqlResult = GetSqlResult(query);

            return await GetListAsync(sqlResult.Sql, sqlResult.NamedBindings);
        }

        public virtual async Task<Tuple<int, IEnumerable<Customer>>> GetPagedCustomers(string username, string email, int pageIndex, int pageSize)
        {
            IDbSession session = await Task.Run(() => DbSession);

            IEnumerable<Customer> customers;
            int totalRecord;

            if (_appConfig.UseProcedureForCustomerPaged)
            {
                DynamicParameters dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("Username", username, DbType.String, ParameterDirection.Input);
                dynamicParameters.Add("Email", email, DbType.String, ParameterDirection.Input);
                dynamicParameters.Add("pageIndex", pageIndex, DbType.Int32, ParameterDirection.Input);
                dynamicParameters.Add("pageSize", pageSize, DbType.Int32, ParameterDirection.Input);
                dynamicParameters.Add("TotalRecords", 0, DbType.Int32, ParameterDirection.Output);

                try
                {
                    customers = await session.Connection.QueryAsync<Customer>("[dbo].[CustomerPaged]", dynamicParameters, commandType: CommandType.StoredProcedure);
                    totalRecord = dynamicParameters.Get<int>("TotalRecords");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    session.Dispose();
                }
            }
            else
            {
                #region TotalCount

                Query totalRecordQuery = new Query().FromRaw($"{TableName} WITH (NOLOCK)").WhereFalse("Deleted").AsCount();

                if (!string.IsNullOrEmpty(username))
                {
                    totalRecordQuery = totalRecordQuery.WhereStarts("Username", username);
                }

                if (!string.IsNullOrEmpty(email))
                {
                    totalRecordQuery = totalRecordQuery.WhereStarts("Email", email);
                }

                SqlResult totalRecordResult = GetSqlResult(totalRecordQuery);

                totalRecord = await session.Connection.QueryFirstOrDefaultAsync<int>(totalRecordResult.Sql, totalRecordResult.NamedBindings);

                #endregion

                #region Paged Customers

                int totalPage = totalRecord <= pageSize ? 1 : totalRecord > pageSize && totalRecord < (pageSize * 2) ? 2 : totalRecord / pageSize; // 总页数

                int midPage = totalPage / 2 + 1; //中间页数，大于该页数则采用倒排优化

                bool isLastPage = pageIndex == totalPage; // 是否最后一页，是最后一页则需要进行取模算出最后一页的记录数（可能小于PageSize）

                int descBound = (totalRecord - pageIndex * pageSize); // 重新计算limit偏移量

                int lastPageSize = 0; // 计算最后一页的记录数

                if (isLastPage)
                {
                    lastPageSize = totalRecord % pageSize; // 取模得到最后一页的记录数
                    descBound -= lastPageSize; // 重新计算最后一页的偏移量
                }
                else
                {
                    descBound -= pageSize; // 正常重新计算除最后一页的偏移量
                }

                bool useDescOrder = pageIndex <= midPage; // 判断是否采取倒排优化

                Query customerQuery = new Query(TableName).Select("Id", "Username", "Email", "Active", "CreationTime").WhereFalse("Deleted");

                if (!string.IsNullOrEmpty(username))
                {
                    customerQuery = customerQuery.WhereStarts("Username", username);
                }

                if (!string.IsNullOrEmpty(email))
                {
                    customerQuery = customerQuery.WhereStarts("Email", email);
                }

                customerQuery = customerQuery.Limit(isLastPage ? lastPageSize : pageSize).Offset(useDescOrder ? pageIndex * pageSize : descBound);

                customerQuery = useDescOrder ? customerQuery.OrderByDesc("Id") : customerQuery.OrderBy("Id");

                SqlResult customerResult = GetSqlResult(customerQuery);

                try
                {
                    customers = await session.Connection.QueryAsync<Customer>(customerResult.Sql, customerResult.NamedBindings);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    session.Dispose();
                }

                #endregion
            }

            return new Tuple<int, IEnumerable<Customer>>(totalRecord, customers);
        }

        public virtual async Task<int> InsertCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            return await InsertAsync(customer);
        }

        public virtual async Task<int> InsertCustomerListAsync(List<Customer> customers)
        {
            if (customers != null && customers.Any())
            {
                StringBuilder builder = new StringBuilder(50);
                builder.AppendFormat("INSERT INTO {0}( Username,Email,Active,Deleted,CreationTime ) VALUES ( @Username,@Email,@Active,@Deleted,@CreationTime );", TableName);

                int result = await ExecuteAsync(builder.ToString(), customers);

                return result;
            }

            return 0;
        }

        public virtual async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            return await UpdateAsync(customer);
        }

        public virtual async Task<bool> DeleteCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            return await DeleteAsync(customer);
        }
    }
}
