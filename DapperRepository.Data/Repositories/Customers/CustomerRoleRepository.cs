using System.Data;
using System.Collections.Generic;
using DapperRepository.Core.Domain;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Data.Repositories.Customers
{
    public class CustomerRoleRepository : RepositoryBase<CustomerRole>, ICustomerRoleRepository
    {
        protected override DatabaseType DataType
        {
            get { return DatabaseType.Mssql; }
        }

        public IEnumerable<CustomerRole> GetCustomerRoles()
        {
            const string sql = "SELECT Id,Name,SystemName FROM CustomerRole";

            return GetList(sql, commandType: CommandType.Text, useTransaction: true);
        }
    }
}
