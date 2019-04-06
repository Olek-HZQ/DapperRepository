using System;
using System.Collections.Generic;
using DapperRepository.Core;
using DapperRepository.Core.Cache;
using DapperRepository.Core.Constants;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.BaseInterfaces;
using DapperRepository.Services.BaseInterfaces;

namespace DapperRepository.Services.Mssql.Customers
{
    public class CustomerService : ICustomerService, IMssqlService
    {
        private readonly ICustomerRepository _repository;
        private readonly ICacheManager _cacheManager;

        public CustomerService(ICustomerRepository repository, ICacheManager cacheManager)
        {
            _repository = repository;
            _cacheManager = cacheManager;
        }

        public Customer GetCustomerById(int customerId)
        {
            if (customerId == 0)
                return null;

            return _repository.GetCustomerById(customerId);
        }

        public CustomerDtoModel GetCustomerBy(int id)
        {
            if (id <= 0)
                return null;

            return _repository.GetCustomerBy(id);
        }

        public int InsertList(out long time, List<Customer> customers, int roleId)
        {
            var result = _repository.InsertList(out time, customers, roleId);
            if (result > 0)
            {
                _cacheManager.RemoveByPattern(string.Format(CustomerDefaults.CustomerCountPatternCacheKey, ConnKeyConstants.Mssql));
            }

            return result;
        }

        public IEnumerable<CustomerDtoModel> GetAllCustomers()
        {
            return _repository.GetAllCustomers();
        }

        public IEnumerable<CustomerDtoModelForPage> GetPagedCustomers(out int totalCount, string username = "", string email = "", int pageIndex = 0, int pageSize = int.MaxValue, bool useStoredProcedure = false)
        {
            int total;

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
            {
                // 缓存无搜索条件的总记录数
                total = _cacheManager.Get(string.Format(CustomerDefaults.CustomerTotalCountCacheKey, ConnKeyConstants.Mssql), 1440, () => _repository.GetCustomerCount());
            }
            else
            {
                string filterKey = string.Format(CustomerDefaults.CustomerFilterCountCacheKey, ConnKeyConstants.Mssql, CommonHelper.GetHashString(username + email));

                total = _cacheManager.Get(filterKey, 1440, () => _repository.GetCustomerCount(username, email));
            }

            totalCount = total;

            return _repository.GetPagedCustomers(total, username, email, pageIndex, pageSize, useStoredProcedure);
        }

        public int InsertCustomer(Customer customer, int roleId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var result = _repository.InsertCustomer(customer, roleId);
            if (result > 0)
            {
                _cacheManager.RemoveByPattern(string.Format(CustomerDefaults.CustomerCountPatternCacheKey, ConnKeyConstants.Mssql));
            }

            return result;
        }

        public int UpdateCustomer(Customer customer, int roleId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            return _repository.UpdateCustomer(customer, roleId);
        }

        public bool DeleteCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var result = _repository.Delete(customer.Id);

            if (result)
            {
                _cacheManager.RemoveByPattern(string.Format(CustomerDefaults.CustomerCountPatternCacheKey, ConnKeyConstants.Mssql));
            }

            return result;
        }
    }
}
