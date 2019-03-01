using System.Collections.Generic;
using DapperRepository.Core.Data;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Data.Repositories.Customers
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer GetCustomerById(int id);

        CustomerDtoModel GetCustomerBy(int id);

        IEnumerable<CustomerDtoModel> GetAllCustomers();

        int InsertCustomer(Customer customer, int roleId);

        int UpdateCustomer(Customer customer, int roleId);
    }
}
