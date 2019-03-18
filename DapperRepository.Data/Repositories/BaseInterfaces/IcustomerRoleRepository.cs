using System.Collections.Generic;
using DapperRepository.Core.Data;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Data.Repositories.BaseInterfaces
{
    public interface ICustomerRoleRepository : IRepository<CustomerRole>
    {
        IEnumerable<CustomerRole> GetCustomerRoles();
    }
}
