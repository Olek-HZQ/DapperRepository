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

namespace DapperRepository.Data.Repositories.Mysql.Customers
{
    public class CustomerRepository : MysqlRepositoryBase<Customer>, ICustomerRepository, IMysqlRepository
    {
        protected override string TableName { get { return string.Format("`{0}`", base.TableName); } }

        public Customer GetCustomerById(int id)
        {
            if (id == 0)
                return null;

            string sql = string.Format("SELECT Id,Username,Email,Active,CreationTime FROM {0} WHERE Id = @id", TableName);
            return GetById(sql, new { id }, commandType: CommandType.Text);
        }

        public CustomerDtoModel GetCustomerBy(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,cr.Id,cr.Name,cr.SystemName FROM {0} c ", TableName);
            sb.Append("JOIN `Customer_CustomerRole_Mapping` crm ON c.Id = crm.CustomerId ");
            sb.Append("JOIN `CustomerRole` cr ON crm.CustomerRoleId = cr.Id WHERE c.Id = @id");

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
            builder.Append("SET @roleid = (SELECT Id FROM `CustomerRole` WHERE SystemName = 'Guest' LIMIT 1);");
            builder.AppendFormat("INSERT INTO {0}( Username,Email,Active,CreationTime ) VALUES ( @Username,@Email,@Active,@CreationTime );", TableName);
            builder.Append("SET @insertid = LAST_INSERT_ID();");
            builder.Append("INSERT INTO `Customer_CustomerRole_Mapping`( CustomerId,CustomerRoleId ) VALUES ( @insertid,@roleid );");

            stopwatch.Start();

            int result = Execute(builder.ToString(), customers);

            stopwatch.Stop();

            time = stopwatch.ElapsedMilliseconds;

            return result;
        }

        public IEnumerable<CustomerDtoModel> GetAllCustomers()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,cr.Id,cr.Name,cr.SystemName FROM {0} c ", TableName);
            sb.Append("JOIN `Customer_CustomerRole_Mapping` crm ON c.Id = crm.CustomerId ");
            sb.Append("JOIN `CustomerRole` cr ON crm.CustomerRoleId = cr.Id ORDER BY c.Id DESC");

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
            parameters.Add("PageIndex", pageIndex, DbType.Int32);
            parameters.Add("PageSize", pageSize, DbType.Int32);

            try
            {
                session.BeginTrans();

                IEnumerable<CustomerDtoModel> customers;

                if (useStoredProcedure)
                {
                    parameters.Add("TotalRecords", totalCount, DbType.Int32, ParameterDirection.Output);
                    customers = session.Connection.Query<CustomerDtoModel, CustomerRole, CustomerDtoModel>("DRD_Customer_GetAllCustomers", (c, cr) =>
                    {
                        c.CustomerRole = cr;
                        return c;
                    }, parameters, session.Transaction, commandType: CommandType.StoredProcedure);

                    totalCount = parameters.Get<int>("TotalRecords");
                }
                else
                {
                    StringBuilder builder = new StringBuilder(50);

                    builder.Append("SET @PageLowerBound = @PageSize * @PageIndex;SET @PageUpperBound = @PageLowerBound + @PageSize + 1;");

                    builder.Append("CREATE TEMPORARY TABLE PageIndex( IndexId INT NOT NULL AUTO_INCREMENT PRIMARY KEY,CustomerId INT NOT NULL);"); // 创建临时表 "PageIndex"
                    builder.AppendFormat("INSERT INTO PageIndex( CustomerId ) SELECT Id FROM {0} ORDER BY Id DESC;", TableName);

                    builder.Append("SELECT row_count();"); // 总数据量
                    builder.Append("SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,cr.Id,cr.Name,cr.SystemName FROM PageIndex pi ");
                    builder.AppendFormat("INNER JOIN {0} c ON c.Id = pi.CustomerId ", TableName);
                    builder.Append("INNER JOIN `Customer_CustomerRole_Mapping` crm ON c.Id = crm.CustomerId ");
                    builder.Append("INNER JOIN `CustomerRole` cr ON crm.CustomerRoleId = cr.Id ");
                    builder.Append("WHERE pi.IndexId > @PageLowerBound AND pi.IndexId < @PageUpperBound ORDER BY pi.IndexId LIMIT @PageSize;");

                    builder.Append("DROP TEMPORARY TABLE PageIndex;"); // 删除临时表 "PageIndex"

                    SqlMapper.GridReader multi = session.Connection.QueryMultiple(builder.ToString(), parameters, session.Transaction, commandType: CommandType.Text);

                    totalCount = Convert.ToInt32(multi.Read<long>().Single());

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
                // log error

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
            builder.AppendFormat("INSERT INTO {0}( Username,Email,Active,CreationTime ) VALUES ( @Username,@Email,@Active,@CreationTime );", TableName);
            builder.Append("SELECT @insertid:= LAST_INSERT_ID();");
            builder.Append("INSERT INTO `Customer_CustomerRole_Mapping`( CustomerId,CustomerRoleId ) VALUES ( @insertid,@roleId );");

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
            builder.AppendFormat("UPDATE {0} SET Username = @Username,Email = @Email,Active = @Active WHERE Id = @Id;", TableName);
            builder.Append("UPDATE `Customer_CustomerRole_Mapping` SET CustomerRoleId = @CustomerRoleId WHERE CustomerId = @CustomerId;");

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
