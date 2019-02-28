using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Services.Customers
{
    public interface ICustomerService
    {
        #region Customer

        Customer GetCustomerById(int customerId);

        CustomerDtoModel GetCustomerBy(int id);

        IEnumerable<CustomerDtoModel> GetAllCustomers();

        int InsertCustomer(Customer customer, int roleId);

        int UpdateCustomer(Customer customer, int roleId);

        bool DeleteCustomer(Customer customer);

        #endregion

        #region CustomerRole

        //IEnumerable<CustomerRole> GetCustomerRoles();

        #endregion
    }
}
