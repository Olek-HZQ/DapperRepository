using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DapperRepo.Core.Domain.Customers;
using DapperRepo.Services.BaseInterfaces;
using Microsoft.AspNetCore.Mvc;
using DapperRepo.Web.Models;

namespace DapperRepo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerService _customerService;

        public HomeController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ActionResult> InsertSampleData(int num)
        {
            Random rd = new Random();

            const string lowerStr = "abcdefghijklmnopqrstuvwxyz";
            ArrayList lowerArr = new ArrayList();
            foreach (char t in lowerStr)
            {
                lowerArr.Add(t);
            }

            const string upperStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            ArrayList upperArr = new ArrayList();
            foreach (char t in upperStr)
            {
                upperArr.Add(t);
            }

            const string numStr = "0123456789";
            ArrayList numArr = new ArrayList();
            foreach (char t in numStr)
            {
                numArr.Add(t);
            }

            List<string> resultList = new List<string>();

            while (resultList.Count < num)
            {
                string name = string.Empty;
                for (int i = 0; i < 1; i++)
                {
                    name += upperArr[rd.Next(0, upperStr.Length)];
                    for (int j = 0; j < 6; j++)
                    {
                        name += lowerArr[rd.Next(lowerArr.Count)];
                    }

                    resultList.Add(name);
                }
            }

            List<string> numList = new List<string>();

            while (numList.Count < num)
            {
                string qq = string.Empty;
                for (int j = 0; j < 9; j++)
                {
                    qq += numArr[rd.Next(0, numArr.Count)];
                }

                numList.Add(qq);
            }

            //批量插入数据，用于测试
            List<Customer> customers = new List<Customer>();

            DateTime now = DateTime.Now;

            int resultListCount = resultList.Count;
            for (int i = 0; i < resultListCount; i++)
            {
                customers.Add(new Customer
                {
                    Username = resultList[i],
                    Email = resultList[i] + "@test.com",
                    Active = true,
                    CreationTime = now.AddSeconds(i)
                });
            }

            int numListCount = numList.Count;
            for (int i = 0; i < numListCount; i++)
            {
                customers.Add(new Customer
                {
                    Username = numList[i],
                    Email = numList[i] + "@qq.com",
                    Active = true,
                    CreationTime = now.AddSeconds(resultListCount + i)
                });
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int result = await _customerService.InsertCustomerListAsync(customers);

            stopwatch.Stop();

            return Json(new { ExecuteResult = result, ExecuteTime = stopwatch.ElapsedMilliseconds });
        }
    }
}
