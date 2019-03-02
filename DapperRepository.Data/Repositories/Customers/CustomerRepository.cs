using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using DapperRepository.Core.Data;
using DapperRepository.Core.Domain;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Data.Repositories.Customers
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        protected override DatabaseType DataType
        {
            get { return DatabaseType.Mssql; }
        }

        public Customer GetCustomerById(int id)
        {
            if (id == 0)
                return null;

            const string sql = "SELECT [Id],[Username],[Email],[Active],[CreationTime] FROM Customer WHERE Id=@id";
            return GetById(sql, new { id }, commandType: CommandType.Text);
        }

        public CustomerDtoModel GetCustomerBy(int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,cr.Id,cr.Name,cr.SystemName FROM Customer c ");
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

        public IEnumerable<CustomerDtoModel> GetAllCustomers()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT c.Id,c.Username,c.Email,c.Active,c.CreationTime,cr.Id,cr.Name,cr.SystemName FROM Customer c ");
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

        public IEnumerable<CustomerDtoModel> GetPagedCustomers(out int totalCount, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            totalCount = 0;
            IDbSession session = DbSession;

            var args = new DynamicParameters();
            args.Add("@PageIndex", pageIndex, DbType.Int32);
            args.Add("@PageSize", pageSize, DbType.Int32);
            args.Add("@TotalRecords", totalCount, DbType.Int32, ParameterDirection.Output);

            try
            {
                session.BeginTrans();

                var customers = session.Connection.Query<CustomerDtoModel, CustomerRole, CustomerDtoModel>("[dbo].[DRD_Customer_GetAllCustomers]", (c, cr) =>
                {
                    c.CustomerRole = cr;
                    return c;
                }, args, session.Transaction, commandType: CommandType.StoredProcedure);

                session.Commit();

                totalCount = args.Get<int>("TotalRecords");

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

        public int InsertCustomer(Customer customer, int roleId)
        {
            StringBuilder builder = new StringBuilder(50);
            builder.Append("DECLARE @insertid INT;");
            builder.Append("INSERT INTO dbo.Customer( Username,Email,Active,CreationTime ) VALUES ( @Username,@Email,@Active,@CreationTime );");
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
            builder.Append("UPDATE [dbo].[Customer] SET [Username] = @Username,[Email] = @Email,[Active] = @Active WHERE [Id] = @Id;");
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
