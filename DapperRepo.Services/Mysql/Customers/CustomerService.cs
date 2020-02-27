using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DapperRepo.Core.Domain.Customers;
using DapperRepo.Data.Repositories.BaseInterfaces;
using DapperRepo.Services.BaseInterfaces;

namespace DapperRepo.Services.Mysql.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            if (id == 0)
                return null;

            return await _customerRepository.GetCustomerByIdAsync(id);
        }

        public Task<Customer> GetCustomerByAsync(string name, string email)
        {
            return _customerRepository.GetCustomerByAsync(name, email);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllCustomersAsync();
        }

        public async Task<Tuple<int, IEnumerable<Customer>>> GetPagedCustomers(string username, string email, int pageIndex, int pageSize)
        {
            return await _customerRepository.GetPagedCustomers(username, email, pageIndex, pageSize);
        }

        public async Task<int> InsertCustomerAsync(Customer customer)
        {
            return await _customerRepository.InsertCustomerAsync(customer);
        }

        public async Task<int> InsertCustomerListAsync(List<Customer> customers)
        {
            return await _customerRepository.InsertCustomerListAsync(customers);
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            return await _customerRepository.UpdateCustomerAsync(customer);
        }

        public async Task<bool> DeleteCustomerAsync(Customer customer)
        {
            return await _customerRepository.DeleteCustomerAsync(customer);
        }
    }
}
