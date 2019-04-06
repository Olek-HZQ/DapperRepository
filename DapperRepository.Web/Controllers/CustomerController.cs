using System;
using System.Linq;
using System.Web.Mvc;
using DapperRepository.Core.Configuration;
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

        public ActionResult Index()
        {
            var customerRoles = _customerRoleService.GetCustomerRoles().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            CustomerModel model = new CustomerModel
            {
                AvailableRoles = customerRoles
            };

            ViewBag.DbSource = _config.ActivedDbTypeName;

            return View(model);
        }

        public ActionResult CustomerList(int pageIndex, int pageSize, string username, string email)
        {
            int total;
            var customers = _customerService.GetPagedCustomers(out total, username, email, pageIndex - 1, pageSize);

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
                    CreationTime = x.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
                };
                return model;
            });

            return Json(new { rows = result, total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var customerRoles = _customerRoleService.GetCustomerRoles().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            CustomerModel model = new CustomerModel
            {
                AvailableRoles = customerRoles
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerModel model)
        {
            if (ModelState.IsValid)
            {
                Customer customer = new Customer
                {
                    Username = model.Username,
                    Email = model.Email,
                    Active = model.Active,
                    CreationTime = DateTime.Now
                };

                _customerService.InsertCustomer(customer, model.RoleId);
            }
            return RedirectToAction("Index");
        }

        public ActionResult EditModal(int id)
        {
            CustomerDtoModel customer = _customerService.GetCustomerBy(id);
            if (customer == null)
                return RedirectToAction("Index");

            var customerRoles = _customerRoleService.GetCustomerRoles().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == customer.CustomerRole.Id
            }).ToList();

            CustomerModel model = new CustomerModel
            {
                Id = customer.Id,
                Username = customer.Username,
                Email = customer.Email,
                Active = customer.Active,
                CreationTime = customer.CreationTime,
                RoleId = customer.CustomerRole.Id,
                AvailableRoles = customerRoles
            };

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            CustomerDtoModel customer = _customerService.GetCustomerBy(id);
            if (customer == null)
                return RedirectToAction("Index");

            var customerRoles = _customerRoleService.GetCustomerRoles().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == customer.CustomerRole.Id
            }).ToList();

            CustomerModel model = new CustomerModel
            {
                Id = customer.Id,
                Username = customer.Username,
                Email = customer.Email,
                Active = customer.Active,
                CreationTime = customer.CreationTime,
                RoleId = customer.CustomerRole.Id,
                AvailableRoles = customerRoles
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerModel model)
        {
            Customer customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                customer.Username = model.Username;
                customer.Email = model.Email;
                customer.Active = model.Active;

                _customerService.UpdateCustomer(customer, model.RoleId);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddCustomer(CustomerModel model)
        {
            try
            {
                Customer customer = new Customer
                {
                    Username = model.Username,
                    Email = model.Email,
                    Active = model.Active,
                    CreationTime = DateTime.Now
                };

                int result = _customerService.InsertCustomer(customer, model.RoleId);

                return Json(new { status = result, msg = result > 0 ? "added successfully" : "added failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateCustomer(CustomerModel model)
        {
            Customer customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return Json(new { status = false, msg = "No customer found with the specified id" });

            try
            {
                customer.Username = model.Username;
                customer.Email = model.Email;
                customer.Active = model.Active;
                int result = _customerService.UpdateCustomer(customer, model.RoleId);

                return Json(new { status = result, msg = result > 0 ? "updated successfully" : "updated failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Customer customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return Json(new { status = false, msg = "No customer found with the specified id" });

            try
            {
                bool result = _customerService.DeleteCustomer(customer);
                return Json(new { status = result, msg = result ? "deleted successfully" : "deleted failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = ex.Message });
            }
        }
    }
}