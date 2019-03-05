using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Services.Customers
{
    public interface ICustomerService
    {
        Customer GetCustomerById(int customerId);

        CustomerDtoModel GetCustomerBy(int id);

        int InsertList(out long time, List<Customer> customers);

        IEnumerable<CustomerDtoModel> GetAllCustomers();

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="totalCount">总数据量</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">当页显示数量</param>
        /// <param name="useStoredProcedure">是否使用存储过程分页（正式上这个参数应该用于配置文件存储或者数据表，我这里是为了演示方便）</param>
        /// <returns></returns>
        IEnumerable<CustomerDtoModel> GetPagedCustomers(out int totalCount, int pageIndex = 0,
            int pageSize = int.MaxValue, bool useStoredProcedure = false);

        int InsertCustomer(Customer customer, int roleId);

        int UpdateCustomer(Customer customer, int roleId);

        bool DeleteCustomer(Customer customer);
    }
}
