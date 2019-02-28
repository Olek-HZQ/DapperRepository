using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Services.Customers
{
    public interface ICustomerRoleService
    {
        IEnumerable<CustomerRole> GetCustomerRoles();
    }
}
