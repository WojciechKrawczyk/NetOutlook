using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetOutlook.Database;
using NetOutlook.Models;

namespace NetOutlook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OutlookContext context;
        public HomeController(ILogger<HomeController> logger, OutlookContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<System.Security.Claims.Claim> c1 = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
                List<System.Security.Claims.Claim> c2 = User.Claims.Where(x => x.Type.Contains("givenname")).ToList();
                List<System.Security.Claims.Claim> c3 = User.Claims.Where(x => x.Type.Contains("surname")).ToList();
                string email = c1[0].Value;
                string name = c2[0].Value;
                string surname = c3[0].Value;
                context.AddUserIfNotExists(name, surname, email);
            }
            return View();
        }
        [Authorize]
        public IActionResult Inbox()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
