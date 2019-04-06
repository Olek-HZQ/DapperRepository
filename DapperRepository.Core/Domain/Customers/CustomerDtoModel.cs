using System;

namespace DapperRepository.Core.Domain.Customers
{
    public class BaseCustomerDtoModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime CreationTime { get; set; }
    }

    public class CustomerDtoModel : BaseCustomerDtoModel
    {
        public CustomerRole CustomerRole { get; set; }
    }

    public class CustomerDtoModelForPage : BaseCustomerDtoModel
    {
        public int CustomerRoleId { get; set; }
    }
}
