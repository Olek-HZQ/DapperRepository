using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Services.Customers
{
    public interface ICustomerService
    {
        Customer GetCustomerById(int customerId);

        CustomerDtoModel GetCustomerBy(int id);

        IEnumerable<CustomerDtoModel> GetAllCustomers();

        IEnumerable<CustomerDtoModel> GetPagedCustomers(out int totalCount, int pageIndex = 0,
            int pageSize = int.MaxValue);

        int InsertCustomer(Customer customer, int roleId);

        int UpdateCustomer(Customer customer, int roleId);

        bool DeleteCustomer(Customer customer);
    }
}
