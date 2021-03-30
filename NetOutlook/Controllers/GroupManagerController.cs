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
using System.Text;

namespace NetOutlook.Controllers
{
    public class GroupManagerController : Controller
    {
        private readonly ILogger<GroupManagerController> _logger;
        private readonly OutlookContext context;

        public GroupManagerController(ILogger<GroupManagerController> logger, OutlookContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [Authorize]
        public IActionResult AddGroup()
        {
            return View();
        }

        [Authorize]
        public IActionResult EditGroup(int id)
        {
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            int Uid = context.GetUserId(email);

            Group group = context.Groups.FirstOrDefault(g => g.OwnerId == Uid && g.Id == id);
            if(group == null)
            {
                throw new ArgumentException();
            }

            group.Members = context.GroupMembers.Where(gm => gm.GroupId == group.Id).ToList();
            HashSet<string> emails = new HashSet<string>();

            foreach(var gm in group.Members)
            {
                emails.Add(context.GetUserEmail(gm.UserId));
            }

            EditGroupModel model = new EditGroupModel();

            model.OldGroupId = id;
            model.GroupName = group.GroupName;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string s in emails)
            {
                stringBuilder.Append(s);
                stringBuilder.Append(";");
            }

            //oldAdress = set;

            model.Emails = stringBuilder.ToString();

            model.Addresses = new HashSet<string>();

            return View(model);
        }

        [Authorize]
        public IActionResult GroupManager()
        {
            GroupManagerModel model = new GroupManagerModel();
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            int id = context.GetUserId(email);
            model.groups = context.Groups.Where(g => g.OwnerId == id).ToList();
            foreach(Group g in model.groups)
            {
                g.Members = context.GroupMembers.Where(gm => gm.GroupId == g.Id).ToList();
            }
            return View(model);
        }

        public IActionResult PopupContact(int id)
        {
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            ViewData["ActionID"] = id;
            return PartialView("_ContactList", context.GetAllUsers(email));
        }

        [HttpPost]
        public ActionResult Delete([FromBody] GroupManagerModel model)
        {
            //int intId = int.Parse(model.OldIdStr);
            int intId = model.OldIdStr;
            DelateGroupFromServer(intId);
            return Json(new { success = true, msg = "Group has been successfully deleted!" });
        }

        [HttpPost]
        public ActionResult Send([FromBody] AddGroupModel group)
        {
            if(group.GroupName.Length==0 || group.Emails.Length==0)
            {
                return Json(new { success = false, msg = "Error: Either group name or group members are empty!" });
            }
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            int id = context.GetUserId(email);
            System.ComponentModel.DataAnnotations.ValidationResult result = group.Validate(context, id, email);
            if (result != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                return Json(new { success = false, msg = result.ErrorMessage });
            }
            SendGroupToServer(group, id, email);
            return Json(new { success = true, msg = "Group has been succesfully added!" });
        }

        [HttpPost]
        public ActionResult Edit([FromBody] EditGroupModel group)
        {
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            int id = context.GetUserId(email);
            group.OldGroupId = int.Parse(group.OldIdStr);
            System.ComponentModel.DataAnnotations.ValidationResult result = group.Validate(context, id, email);
            if (result != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                return Json(new { success = false, msg = result.ErrorMessage });
            }
            EditGroupInServer(group, id, email);
            return Json(new { success = true, msg = "Group has been successfully saved!" });
        }

        protected bool EditGroupInServer(EditGroupModel group, int id, string email)
        {
            Group gr = new Group();
            gr.OwnerId = id;
            gr.GroupName = group.GroupName;
            //gr.Id = context.Groups.Max<Group>(gr => gr.Id) + 1;
            gr.Members = null;

            Group gToDelate = context.Groups.Where(g => g.Id == group.OldGroupId).FirstOrDefault();
            context.Groups.Remove(gToDelate);
            context.GroupMembers.RemoveRange(context.GroupMembers.Where(g => g.GroupId == group.OldGroupId));
            context.SaveChanges();

            context.Groups.Add(gr);
            context.SaveChanges();

            gr.Members = Models.EditGroupModel.GroupManagerParams(context, gr, id, email, group.Addresses);
            context.SaveChanges();

            return true;
        }


        protected bool SendGroupToServer(AddGroupModel group, int id, string email)
        {
            Group gr = new Group();
            gr.OwnerId = id;
            gr.GroupName = group.GroupName;
            //gr.Id = context.Groups.Max<Group>(gr => gr.Id) + 1;
            gr.Members = null;

            context.Groups.Add(gr);
            context.SaveChanges();

            gr.Members = Models.AddGroupModel.GroupManagerParams(context, gr, id, email, group.Addresses);
            context.SaveChanges();
            
            return true;
        }

        protected bool DelateGroupFromServer(int id)
        {
            context.Groups.Remove(context.Groups.FirstOrDefault(g => g.Id == id));
            context.GroupMembers.RemoveRange(context.GroupMembers.Where(g => g.GroupId == id));
            context.SaveChanges();
            return true;
        }


    }
}
