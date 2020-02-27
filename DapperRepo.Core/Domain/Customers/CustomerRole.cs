using System;

namespace DapperRepo.Core.Domain.Customers
{
    public class CustomerRole : BaseEntity
    {
        public string Name { get; set; }

        public string SystemName { get; set; }

        public bool Active { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
