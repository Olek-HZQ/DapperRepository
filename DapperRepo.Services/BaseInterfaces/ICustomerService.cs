using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DapperRepo.Core.Domain.Customers;

namespace DapperRepo.Services.BaseInterfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByIdAsync(int id);

        Task<Customer> GetCustomerByAsync(string name, string email);

        Task<IEnumerable<Customer>> GetAllCustomersAsync();

        Task<Tuple<int, IEnumerable<Customer>>> GetPagedCustomers(string username, string email, int pageIndex, int pageSize);

        Task<int> InsertCustomerAsync(Customer customer);

        Task<int> InsertCustomerListAsync(List<Customer> customers);

        Task<bool> UpdateCustomerAsync(Customer customer);

        Task<bool> DeleteCustomerAsync(Customer customer);
    }
}
