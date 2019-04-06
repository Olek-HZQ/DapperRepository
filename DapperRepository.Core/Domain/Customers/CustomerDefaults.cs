
namespace DapperRepository.Core.Domain.Customers
{
    public static class CustomerDefaults
    {
        public static string CustomerRolesAllCacheKey = "{0}.customer.roles";

        public static string CustomerCountPatternCacheKey = "{0}.count.customer";

        public static string CustomerTotalCountCacheKey = "{0}.count.customer.totalcount";

        public static string CustomerFilterCountCacheKey = "{0}.count.customer.filtercount.{1}";

        #region for my personal test

        public static string TestCustomerRolesAllCacheKey = "test.{0}.customer.roles";

        public static string TestCustomerCountPatternCacheKey = "test.{0}.count.customer";

        public static string TestCustomerTotalCountCacheKey = "test.{0}.count.customer.totalcount";

        public static string TestCustomerFilterCountCacheKey = "test.{0}.count.customer.filtercount.{1}";

        #endregion
    }
}
