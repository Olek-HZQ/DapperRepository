using System;
using System.Linq;
using System.Web.Mvc;
using DapperRepository.Core.Configuration;
using DapperRepository.Core.Constants;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Services.BaseInterfaces;
using DapperRepository.Web.Models.Customers;

namespace DapperRepository.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerRoleService _customerRoleService;
        private readonly DapperRepositoryConfig _config;

        public CustomerController(ICustomerService customerService, ICustomerRoleService customerRoleService, DapperRepositoryConfig config)
        {
            _customerService = customerService;
            _customerRoleService = customerRoleService;
            _config = config;
        }

        public ActionResult List()
        {
            ViewBag.DbSource = _config.ActivedDbTypeName;

            return View();
        }

        [HttpGet]
        public ActionResult CustomerList(int pageIndex, int pageSize, string username, string email)
        {
            int total;
            var customers = _customerService.GetPagedCustomers(out total, username, email, pageIndex - 1, pageSize, (!string.IsNullOrEmpty(_config.ActivedDbTypeName) && _config.ActivedDbTypeName == ConnKeyConstants.Mssql));

            var customerRoles = _customerRoleService.GetCustomerRoles();

            var result = customers.Select(x =>
            {
                string roleName = "";

                var customerRole = customerRoles.FirstOrDefault(c => c.Id == x.CustomerRoleId);
                if (customerRole != null)
                    roleName = customerRole.Name;

                var model = new CustomerPagedResultModel
                {
                    Id = x.Id,
                    Username = x.Username,
                    RoleName = roleName,
                    Email = x.Email,
                    Active = x.Active,
                    CreationTime = x.CreationTime.ToString("yyyy-MM-dd")
                };
                return model;
            });

            return Json(new { code = 0, data = result, count = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PopCustomer(string viewName, int id = 0)
        {
            CustomerModel model = new CustomerModel();

            ViewBag.ViewName = viewName;

            if (id > 0)
            {
                CustomerDtoModel customer = _customerService.GetCustomerBy(id);

                if (customer == null)
                {
                    return RedirectToAction("List");
                }

                model.Id = customer.Id;
                model.Username = customer.Username;
                model.Email = customer.Email;
                model.Active = customer.Active;

                model.AvailableRoles = _customerRoleService.GetCustomerRoles().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = x.Id == customer.CustomerRole.Id
                }).ToList();
            }
            else
            {
                model.AvailableRoles = _customerRoleService.GetCustomerRoles().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
            }

            return PartialView("_PopCustomer", model);
        }

        [HttpPost]
        public ActionResult CreateCustomer(CustomerModel model)
        {
            try
            {
                Customer customer = new Customer
                {
                    Username = model.Username.Trim(),
                    Email = model.Email.Trim(),
                    Active = model.Active,
                    CreationTime = DateTime.Now
                };

                int result = _customerService.InsertCustomer(customer, model.RoleId);

                return Json(new { status = result, msg = result > 0 ? "added successfully" : "added failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "added failed:" + ex.Message });

            }
        }

        [HttpPost]
        public ActionResult EditCustomer(CustomerModel model)
        {
            Customer customer = _customerService.GetCustomerById(model.Id);

            if (customer == null)
            {
                return Json(new { status = false, msg = "no customer found with the specified id" });
            }

            try
            {
                customer.Username = model.Username.Trim();
                customer.Email = model.Email.Trim();
                customer.Active = model.Active;

                int result = _customerService.UpdateCustomer(customer, model.RoleId);

                return Json(new { status = result, msg = result > 0 ? "edited successfully" : "edited failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "edited failed:" + ex.Message });

            }
        }

        [HttpPost]
        public ActionResult DeleteCustomer(int id)
        {
            Customer customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return Json(new { status = false, msg = "no customer found with the specified id" });

            try
            {
                bool result = _customerService.DeleteCustomer(customer);
                return Json(new { status = result, msg = result ? "deleted successfully" : "deleted failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "deleted failed:" + ex.Message });
            }
        }
    }
}