using System.Collections.Generic;
using DapperRepository.Core.Domain.Customers;

namespace DapperRepository.Services.BaseInterfaces
{
    public interface ICustomerService
    {
        Customer GetCustomerById(int customerId);

        CustomerDtoModel GetCustomerBy(int id);

        int InsertList(out long time, List<Customer> customers, int roleId);

        IEnumerable<CustomerDtoModel> GetAllCustomers();

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="totalCount">总数据量</param>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">当页显示数量</param>
        /// <param name="useStoredProcedure">是否使用存储过程分页（正式上这个参数应该用于配置文件存储或者数据表，我这里是为了演示方便）</param>
        /// <returns></returns>
        IEnumerable<CustomerDtoModelForPage> GetPagedCustomers(out int totalCount, string username = "", string email = "", int pageIndex = 0,
            int pageSize = int.MaxValue, bool useStoredProcedure = false);

        int InsertCustomer(Customer customer, int roleId);

        int UpdateCustomer(Customer customer, int roleId);

        bool DeleteCustomer(Customer customer);
    }
}
