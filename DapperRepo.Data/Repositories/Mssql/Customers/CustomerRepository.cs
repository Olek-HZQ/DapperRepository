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
                dynamicParameters.Add("PageIndex", pageIndex, DbType.Int32, ParameterDirection.Input);
                dynamicParameters.Add("PageSize", pageSize, DbType.Int32, ParameterDirection.Input);
                dynamicParameters.Add("TotalRecords", 0, DbType.Int32, ParameterDirection.Output);

                customers = await session.Connection.QueryAsync<Customer>("[dbo].[CustomerPaged]", dynamicParameters, commandType: CommandType.StoredProcedure);
                totalRecord = dynamicParameters.Get<int>("TotalRecords");
                session.Dispose();
            }
            else
            {
                Query totalRecordQuery = new Query().FromRaw($"{TableName} WITH (NOLOCK)").WhereFalse("Deleted").AsCount();
                Query customerQuery = new Query().FromRaw($"{TableName} WITH (NOLOCK)").Select("Id", "Username", "Email", "Active", "CreationTime").WhereFalse("Deleted");

                if (!string.IsNullOrEmpty(username))
                {
                    totalRecordQuery = totalRecordQuery.WhereStarts("Username", username);
                    customerQuery = customerQuery.WhereStarts("Username", username);
                }

                if (!string.IsNullOrEmpty(email))
                {
                    totalRecordQuery = totalRecordQuery.WhereStarts("Email", email);
                    customerQuery = customerQuery.WhereStarts("Email", email);
                }

                SqlResult totalRecordResult = GetSqlResult(totalRecordQuery);

                customerQuery = customerQuery.OrderByDesc("Id").Limit(pageSize).Offset(pageIndex * pageSize);
                SqlResult customerResult = GetSqlResult(customerQuery);

                var multi = await session.Connection.QueryMultipleAsync(customerResult.Sql + ";" + totalRecordResult.Sql, customerResult.NamedBindings);
                var result = await multi.ReadAsync<Customer>();
                customers = result.ToList();

                totalRecord = multi.ReadFirst<int>();

                session.Dispose();
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
