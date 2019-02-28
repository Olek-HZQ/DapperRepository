using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DapperRepository.Web.Models.Customers
{
    public class CustomerModel
    {
        public CustomerModel()
        {
            AvailableRoles = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", ErrorMessage = "The Email format is incorrect.")]
        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime CreationTime { get; set; }

        [Display(Name = "Role")]
        public int RoleId { get; set; }
        public IList<SelectListItem> AvailableRoles { get; set; }
    }
}