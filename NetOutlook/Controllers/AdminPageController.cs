using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetOutlook.Database;
using NetOutlook.Models;
using System.Security.Claims;
using System.Diagnostics;


namespace NetOutlook.Controllers
{
    public class AdminPageController : Controller
    {
        private readonly ILogger<AdminPageController> _logger;
        private readonly OutlookContext context;
        private AdminPageModel AdminPageModel { get; set; }
        private bool t = false;
        public AdminPageController(ILogger<AdminPageController> logger, OutlookContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult AdminPage()
        {
            if(this.t)
            {
                t = false;
                return View(this.AdminPageModel);
            }

            AdminPageModel adminPageModel = new AdminPageModel();

            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            int adminId = context.GetUserId(email);
            User admin = context.Users.Where(x => x.Id == adminId).ToList()[0];

            if (admin.Role != 1)
            {
                adminPageModel.IsValid = false;
                return View(adminPageModel);
            }

            adminPageModel.IsValid = true;
            adminPageModel.AdminId = adminId;
            adminPageModel.Users = GetActiveUsers(adminId);

            return View(adminPageModel);
        }

        public ActionResult Delete(int id, int adminId)
        {
            context.Users.Where(x => x.Id == id).ToList()[0].Role = -2;
            context.SaveChanges();

            AdminPageModel adminPageModel = new AdminPageModel();
            adminPageModel.IsValid = true;
            adminPageModel.AdminId = adminId;
            adminPageModel.Users = GetActiveUsers(adminId);

            this.AdminPageModel = adminPageModel;
            this.t = true;
            return RedirectToAction("AdminPage");
        }

        public ActionResult Accept(int id, int adminId)
        {
            context.Users.Where(x => x.Id == id).ToList()[0].Role = 0;
            context.SaveChanges();

            AdminPageModel adminPageModel = new AdminPageModel();
            adminPageModel.IsValid = true;
            adminPageModel.AdminId = adminId;
            adminPageModel.Users = GetActiveUsers(adminId);

            this.AdminPageModel = adminPageModel;
            this.t = true;

            return RedirectToAction("AdminPage");
        }

        private List<UserAdminPageModel> GetActiveUsers(int adminId)
        {
            List<UserAdminPageModel> users = new List<UserAdminPageModel>();
            var tab = context.Users.ToArray();
            foreach (var u in tab)
            {
                if (u.Id != adminId && u.Role != -2) 
                {
                    UserAdminPageModel user = new UserAdminPageModel();
                    user.UserRaw = new UserRaw(u);
                    user.DeleteLink = "Delete?id=" + u.Id + "&adminId=" + adminId;
                    user.AcceptLink = "Accept?id=" + u.Id + "&adminId=" + adminId;
                    users.Add(user);
                }
            }
            return users;
        }
    }
}
