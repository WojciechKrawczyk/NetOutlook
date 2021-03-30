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
    public class NewMessageController : Controller
    {
        private readonly ILogger<NewMessageController> _logger;
        private readonly OutlookContext context;
        public NewMessageController(ILogger<NewMessageController> logger, OutlookContext context)
        {
            _logger = logger;
            this.context = context;
        }
        [Authorize]
        public IActionResult NewMessage()
        {
            return View();
        }

        public IActionResult PopupContact(int id)
        {
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            ViewData["ActionID"] = id;
            return PartialView("_ContactList", (context.GetAllUsers(email),context.GetUserGroups(email)));
        }

        [HttpPost]
        public ActionResult Send([FromBody] NewMessageModel message)
        {
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            int id = context.GetUserId(email);
            System.ComponentModel.DataAnnotations.ValidationResult result = message.Validate(context, id,email);
            if (result != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                return Json(new { success = false, msg = result.ErrorMessage });
            }
            SendMessageToServer(message, id, email);
            return Json(new { success = true, msg = "Wiadomość została wysłana!" });
        }

        protected bool SendMessageToServer(NewMessageModel message, int id, string email)
        {
            Message msg = new Message();
            msg.SenderId = id;
            msg.Content = message.MessageContent;
            msg.Subject = message.SubjectText;
            msg.SendDate = DateTime.Now;

            context.Messages.Add(msg);
            context.SaveChanges();
            Models.NewMessageModel.SendMessageParams(context, msg, id, email, message.Addresses);
            context.SaveChanges();
            return true;
        }

    }
}
