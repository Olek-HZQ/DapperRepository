using System.ComponentModel.DataAnnotations;

namespace DapperRepo.Core.Domain
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
