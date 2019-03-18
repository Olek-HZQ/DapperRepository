using System.Collections.Generic;
using DapperRepository.Core.Data;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Data.Repositories.BaseInterfaces
{
    public interface ICustomerRepository: IRepository<Customer>
    {
        Customer GetCustomerById(int id);

        CustomerDtoModel GetCustomerBy(int id);

        /// <summary>
        /// 批量插入数据(默认为guest角色)
        /// </summary>
        /// <param name="time">执行时间</param>
        /// <param name="customers">需插入数据列表</param>
        /// <returns>插入数据数量</returns>
        int InsertList(out long time, List<Customer> customers);

        IEnumerable<CustomerDtoModel> GetAllCustomers();

        IEnumerable<CustomerDtoModel> GetPagedCustomers(out int totalCount, int pageIndex = 0, int pageSize = int.MaxValue, bool useStoredProcedure = false);

        int InsertCustomer(Customer customer, int roleId);

        int UpdateCustomer(Customer customer, int roleId);
    }
}
