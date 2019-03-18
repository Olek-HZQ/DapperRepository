using System;
using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.BaseInterfaces;
using DapperRepository.Services.BaseInterfaces;

namespace DapperRepository.Services.Mysql.Customers
{
    public class CustomerService : ICustomerService, IMysqlService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
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

        public int InsertList(out long time, List<Customer> customers)
        {
            return _repository.InsertList(out time, customers);
        }

        public IEnumerable<CustomerDtoModel> GetAllCustomers()
        {
            return _repository.GetAllCustomers();
        }

        public IEnumerable<CustomerDtoModel> GetPagedCustomers(out int totalCount, int pageIndex = 0, int pageSize = int.MaxValue, bool useStoredProcedure = false)
        {
            return _repository.GetPagedCustomers(out totalCount, pageIndex, pageSize, useStoredProcedure);
        }

        public int InsertCustomer(Customer customer, int roleId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            return _repository.InsertCustomer(customer, roleId);
        }

        public int UpdateCustomer(Customer customer, int roleId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            return _repository.UpdateCustomer(customer, roleId);
        }

        public bool DeleteCustomer(Customer customer)
        {
            return _repository.Delete(customer.Id);
        }
    }
}
