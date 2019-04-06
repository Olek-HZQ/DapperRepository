
namespace DapperRepository.Web.Models.Customers
{
    public class CustomerPagedResultModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string RoleName { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public string CreationTime { get; set; }
    }
}