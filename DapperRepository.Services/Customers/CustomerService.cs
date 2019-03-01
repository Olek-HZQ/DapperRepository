using System;
using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Data.Repositories.Customers;

namespace DapperRepository.Services.Customers
{
    public class CustomerService : ICustomerService
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

        public IEnumerable<CustomerDtoModel> GetAllCustomers()
        {
            return _repository.GetAllCustomers();
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
            return _repository.Delete(customer);
        }
    }
}
