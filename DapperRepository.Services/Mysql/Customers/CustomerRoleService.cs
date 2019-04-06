using System.Collections.Generic;
using DapperRepository.Core.Cache;
using DapperRepository.Core.Constants;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.BaseInterfaces;
using DapperRepository.Services.BaseInterfaces;

namespace DapperRepository.Services.Mysql.Customers
{
    public class CustomerRoleService : ICustomerRoleService, IMysqlService
    {
        private readonly ICustomerRoleRepository _repository;
        private readonly ICacheManager _cacheManager;

        public CustomerRoleService(ICustomerRoleRepository repository, ICacheManager cacheManager)
        {
            _repository = repository;
            _cacheManager = cacheManager;
        }

        public IEnumerable<CustomerRole> GetCustomerRoles()
        {
            return _cacheManager.Get(string.Format(CustomerDefaults.CustomerRolesAllCacheKey, ConnKeyConstants.Mysql), 1440, () => _repository.GetCustomerRoles());
        }
    }
}
