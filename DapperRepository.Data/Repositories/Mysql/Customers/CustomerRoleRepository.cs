using System.Collections.Generic;
using System.Data;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.BaseInterfaces;

namespace DapperRepository.Data.Repositories.Mysql.Customers
{
    public class CustomerRoleRepository : MysqlRepositoryBase<CustomerRole>, ICustomerRoleRepository, IMysqlRepository
    {
        public IEnumerable<CustomerRole> GetCustomerRoles()
        {
            const string sql = "SELECT Id,Name,SystemName FROM CustomerRole";

            return GetList(sql, commandType: CommandType.Text, useTransaction: true);
        }
    }
}
