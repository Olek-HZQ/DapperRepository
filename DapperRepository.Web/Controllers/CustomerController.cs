using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DapperRepo.Web.Models.Customers;
using DapperRepository.Core.Domain.Customers;
using DapperRepository.Services.BaseInterfaces;

namespace DapperRepository.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> BootstrapCustomerList(SearchCustomerModel model)
        {
            var result = await _customerService.GetPagedCustomers(model.Username, model.Email, model.PageIndex - 1, model.PageSize);

            List<CustomerModel> customers = result.Item2.Select(x =>
            {
                CustomerModel customerModel = new CustomerModel
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Active = x.Active,
                    CreationTime = x.CreationTime.ToString("yyyy-MM-dd")
                };
                return customerModel;
            }).ToList();

            return Json(new { rows = customers, total = result.Item1 });
        }

        public async Task<ActionResult> LayuiCustomerList(SearchCustomerModel model)
        {
            var result = await _customerService.GetPagedCustomers(model.Username, model.Email, model.PageIndex - 1, model.PageSize);

            List<CustomerModel> customers = result.Item2.Select(x =>
            {
                CustomerModel customerModel = new CustomerModel
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Active = x.Active,
                    CreationTime = x.CreationTime.ToString("yyyy-MM-dd")
                };
                return customerModel;
            }).ToList();

            return Json(new { code = 0, data = customers, count = result.Item1 }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> PopCustomer(string viewName, int id = 0)
        {
            CustomerModel model = new CustomerModel();

            ViewBag.ViewName = viewName;

            if (id > 0)
            {
                Customer customer = await _customerService.GetCustomerByIdAsync(id);

                if (customer == null)
                {
                    return RedirectToAction("List");
                }

                model.Id = customer.Id;
                model.Username = customer.Username;
                model.Email = customer.Email;
                model.Active = customer.Active;
            }

            return PartialView("_PopCustomer", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer(CustomerModel model)
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

                int result = await _customerService.InsertCustomerAsync(customer);

                return Json(new { status = result, msg = result > 0 ? "Added successfully" : "Added failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Added failed:" + ex.Message });

            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            Customer customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return RedirectToAction("Index");
            }

            CustomerModel model = new CustomerModel
            {
                Id = customer.Id,
                Username = customer.Username,
                Email = customer.Email,
                Active = customer.Active,
                CreationTime = customer.CreationTime.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return PartialView("_EditModal", model);
        }

        [HttpPost]
        public async Task<ActionResult> EditCustomer(CustomerModel model)
        {
            Customer customer = await _customerService.GetCustomerByIdAsync(model.Id);

            if (customer == null)
            {
                return Json(new { status = false, msg = "No customer found with the specified id" });
            }

            try
            {
                customer.Username = model.Username.Trim();
                customer.Email = model.Email.Trim();
                customer.Active = model.Active;

                bool result = await _customerService.UpdateCustomerAsync(customer);

                return Json(new { status = result, msg = result ? "Edited successfully" : "Edited failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Edited failed:" + ex.Message });

            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            Customer customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return Json(new { status = false, msg = "No customer found with the specified id" });

            try
            {
                bool result = await _customerService.DeleteCustomerAsync(customer);
                return Json(new { status = result, msg = result ? "Deleted successfully" : "Deleted failed" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, msg = "Deleted failed:" + ex.Message });
            }
        }
    }
}