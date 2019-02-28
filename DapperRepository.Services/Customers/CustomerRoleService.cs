using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.Customers;

namespace DapperRepository.Services.Customers
{
    public class CustomerRoleService : ICustomerRoleService
    {
        private readonly ICustomerRoleRepository _repository;

        public CustomerRoleService(ICustomerRoleRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<CustomerRole> GetCustomerRoles()
        {
            return _repository.GetCustomerRoles();
        }
    }
}
