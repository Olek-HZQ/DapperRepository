using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.BaseInterfaces;
using DapperRepository.Services.BaseInterfaces;

namespace DapperRepository.Services.Mysql.Customers
{
    public class CustomerRoleService : ICustomerRoleService, IMysqlService
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
