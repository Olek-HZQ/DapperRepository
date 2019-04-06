using System.Collections.Generic;
using System.Data;
using DapperRepository.Core.Constants;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.BaseInterfaces;

namespace DapperRepository.Data.Repositories.Mysql.Customers
{
    public class CustomerRoleRepository : MysqlRepositoryBase<CustomerRole>, ICustomerRoleRepository, IMysqlRepository
    {
        //protected override string ConnStrKey
        //{
        //    get { return ConnKeyConstants.LocalMysqlMasterKey; }
        //}

        public IEnumerable<CustomerRole> GetCustomerRoles()
        {
            string sql = string.Format("SELECT Id,Name,SystemName FROM {0}", TableName);

            return GetList(sql, commandType: CommandType.Text, useTransaction: true);
        }
    }
}
