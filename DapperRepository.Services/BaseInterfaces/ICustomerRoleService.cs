using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Services.BaseInterfaces
{
    public interface ICustomerRoleService
    {
        IEnumerable<CustomerRole> GetCustomerRoles();
    }
}
