using System.Collections.Generic;
using System.Data;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.BaseInterfaces;

namespace DapperRepository.Data.Repositories.Mssql.Customers
{
    public class CustomerRoleRepository : MssqlRepositoryBase<CustomerRole>, ICustomerRoleRepository, IMssqlRepository
    {
        public IEnumerable<CustomerRole> GetCustomerRoles()
        {
            const string sql = "SELECT Id,Name,SystemName FROM CustomerRole";

            return GetList(sql, commandType: CommandType.Text, useTransaction: true);
        }
    }
}
