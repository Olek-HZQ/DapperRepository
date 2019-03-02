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

        IEnumerable<CustomerDtoModel> GetPagedCustomers(out int totalCount, int pageIndex = 0, int pageSize = int.MaxValue);

        int InsertCustomer(Customer customer, int roleId);

        int UpdateCustomer(Customer customer, int roleId);
    }
}
