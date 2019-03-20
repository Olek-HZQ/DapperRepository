using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dapper;
using DapperRepository.Core.Data;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.BaseInterfaces;

namespace DapperRepository.Data.Repositories.Mssql.Customers
{
    public class CustomerRepository : MssqlRepositoryBase<Customer>, ICustomerRepository, IMssqlRepository
    {
        public Customer GetCustomerById(int id)
        {
            if (id == 0)
                return null;

            string sql = string.Format("SELECT [Id],[Username],[Email],[Active],[CreationTime] FROM [dbo].[{0}] WHERE [Id] = @id", TableName);

            return GetById(sql, new { id }, commandType: CommandType.Text);
        }

        public CustomerDtoModel GetCustomerBy(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,cr.Id,cr.Name,cr.SystemName FROM [dbo].[{0}] c ", TableName);
            sb.Append("JOIN Customer_CustomerRole_Mapping crm ON c.Id = crm.CustomerId ");
            sb.Append("JOIN CustomerRole cr ON crm.CustomerRoleId = cr.Id WHERE c.Id = @id");

            string sql = sb.ToString();
            IDbSession session = DbSession;

            try
            {
                var customers = session.Connection.Query<CustomerDtoModel, CustomerRole, CustomerDtoModel>(sql, (c, cr) =>
                    {
                        c.CustomerRole = cr;
                        return c;
                    }, new { id }).FirstOrDefault();

                return customers;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                session.Dispose();
            }
        }

        public int InsertList(out long time, List<Customer> customers)
        {
            // 用于获取插入运行时间
            Stopwatch stopwatch = new Stopwatch();

            StringBuilder builder = new StringBuilder(50);
            builder.Append("DECLARE @insertid INT;DECLARE @roleid INT;");
            builder.Append("SET @roleid = (SELECT TOP(1) Id FROM dbo.CustomerRole WHERE SystemName = 'Guest');");
            builder.AppendFormat("INSERT INTO [dbo].[{0}]( Username,Email,Active,CreationTime ) VALUES ( @Username,@Email,@Active,@CreationTime );", TableName);
            builder.Append("SET @insertid = SCOPE_IDENTITY();");
            builder.Append("INSERT INTO [dbo].[Customer_CustomerRole_Mapping]( CustomerId,CustomerRoleId ) VALUES ( @insertid,@roleid );");

            stopwatch.Start();

            int result = Execute(builder.ToString(), customers);

            stopwatch.Stop();

            time = stopwatch.ElapsedMilliseconds;

            return result;
        }

        public IEnumerable<CustomerDtoModel> GetAllCustomers()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,cr.Id,cr.Name,cr.SystemName FROM [dbo].[{0}] c ", TableName);
            sb.Append("JOIN Customer_CustomerRole_Mapping crm ON c.Id = crm.CustomerId ");
            sb.Append("JOIN CustomerRole cr ON crm.CustomerRoleId = cr.Id ORDER BY c.Id DESC");

            string sql = sb.ToString();
            IDbSession session = DbSession;

            try
            {
                session.BeginTrans();

                var customers = session.Connection.Query<CustomerDtoModel, CustomerRole, CustomerDtoModel>(sql, (c, cr) =>
                {
                    c.CustomerRole = cr;
                    return c;
                }, transaction: session.Transaction);
                session.Commit();

                return customers;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                session.Dispose();
            }
        }

        public IEnumerable<CustomerDtoModel> GetPagedCustomers(out int totalCount, int pageIndex = 0, int pageSize = int.MaxValue, bool useStoredProcedure = false)
        {
            totalCount = 0;
            IDbSession session = DbSession;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PageIndex", pageIndex, DbType.Int32);
            parameters.Add("@PageSize", pageSize, DbType.Int32);

            try
            {
                session.BeginTrans();

                IEnumerable<CustomerDtoModel> customers;

                if (useStoredProcedure)
                {
                    parameters.Add("@TotalRecords", totalCount, DbType.Int32, ParameterDirection.Output);
                    customers = session.Connection.Query<CustomerDtoModel, CustomerRole, CustomerDtoModel>("[dbo].[DRD_Customer_GetAllCustomers]", (c, cr) =>
                    {
                        c.CustomerRole = cr;
                        return c;
                    }, parameters, session.Transaction, commandType: CommandType.StoredProcedure);

                    totalCount = parameters.Get<int>("TotalRecords");
                }
                else
                {
                    StringBuilder builder = new StringBuilder(50);

                    builder.Append("DECLARE @PageLowerBound INT;DECLARE @PageUpperBound INT;"); // page params
                    builder.Append("SET @PageLowerBound = @PageSize * @PageIndex;SET @PageUpperBound = @PageLowerBound + @PageSize + 1;");

                    builder.Append("CREATE TABLE #PageIndex( [IndexId] INT IDENTITY(1, 1) NOT NULL ,[CustomerId] INT NOT NULL);"); // 创建临时表 "PageIndex"
                    builder.AppendFormat("INSERT INTO #PageIndex( CustomerId ) SELECT Id FROM dbo.{0} ORDER BY Id DESC;", TableName);

                    builder.Append("SELECT @@ROWCOUNT;"); // 总数据量
                    builder.Append("SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,cr.Id,cr.Name,cr.SystemName FROM #PageIndex [pi] ");
                    builder.AppendFormat("INNER JOIN [dbo].[{0}] c ON c.Id = [pi].CustomerId ", TableName);
                    builder.Append("INNER JOIN Customer_CustomerRole_Mapping crm ON c.Id = crm.CustomerId ");
                    builder.Append("INNER JOIN CustomerRole cr ON crm.CustomerRoleId = cr.Id ");
                    builder.Append("WHERE pi.IndexId > @PageLowerBound AND pi.IndexId < @PageUpperBound ORDER BY pi.IndexId;");

                    builder.Append("DROP TABLE #PageIndex;"); // 删除临时表 "PageIndex"

                    var multi = session.Connection.QueryMultiple(builder.ToString(), parameters, session.Transaction, commandType: CommandType.Text);

                    totalCount = multi.Read<int>().Single();

                    customers = multi.Read<CustomerDtoModel, CustomerRole, CustomerDtoModel>((c, cr) =>
                    {
                        c.CustomerRole = cr;
                        return c;
                    });
                }

                session.Commit();

                return customers;

            }
            catch
            {
                return null;
            }
            finally
            {
                session.Dispose();
            }
        }

        public int InsertCustomer(Customer customer, int roleId)
        {
            StringBuilder builder = new StringBuilder(50);
            builder.Append("DECLARE @insertid INT;");
            builder.AppendFormat("INSERT INTO [dbo].[{0}]( Username,Email,Active,CreationTime ) VALUES ( @Username,@Email,@Active,@CreationTime );", TableName);
            builder.Append("SET @insertid = SCOPE_IDENTITY();");
            builder.Append("INSERT INTO [dbo].[Customer_CustomerRole_Mapping]( CustomerId,CustomerRoleId ) VALUES ( @insertid,@roleId );");

            return Execute(builder.ToString(), new
            {
                customer.Username,
                customer.Email,
                customer.Active,
                customer.CreationTime,
                roleId
            }, commandType: CommandType.Text);
        }

        /// <summary>
        /// 更新信息（事实上用户有可能具有多个角色，我这里为了演示方便就假设用户只有一个角色处理了）
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="roleId">对应角色id</param>
        /// <returns></returns>
        public int UpdateCustomer(Customer customer, int roleId)
        {
            StringBuilder builder = new StringBuilder(50);
            builder.AppendFormat("UPDATE [dbo].[{0}] SET [Username] = @Username,[Email] = @Email,[Active] = @Active WHERE [Id] = @Id;", TableName);
            builder.Append("UPDATE [dbo].[Customer_CustomerRole_Mapping] SET [CustomerRoleId] = @CustomerRoleId WHERE [CustomerId] = @CustomerId;");

            return Execute(builder.ToString(), new
            {
                customer.Username,
                customer.Email,
                customer.Active,
                customer.Id,
                @CustomerRoleId = roleId,
                @CustomerId = customer.Id
            }, commandType: CommandType.Text);
        }
    }
}
