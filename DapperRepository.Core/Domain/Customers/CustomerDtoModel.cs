using System;

namespace DapperRepository.Core.Domain.Customers
{
    public class CustomerDtoModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime CreationTime { get; set; }

        public virtual CustomerRole CustomerRole { get; set; }
    }
}
