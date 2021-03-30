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
    public class InboxController : Controller
    {
        private readonly ILogger<InboxController> _logger;
        private readonly OutlookContext context;

        public InboxController(ILogger<InboxController> logger, OutlookContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [Authorize]
        public IActionResult Inbox()
        {
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            int id = context.GetUserId(email);

            List<InboxMessage> inboxMessages = new List<InboxMessage>();
            InboxViewModel inboxViewModel = new InboxViewModel();

            List<string> dates = new List<string>();

            List<MessageReceiver> messageReceivers = context.MessageReceivers.Where(x => x.UserId == id).ToList();
            foreach(var mr in messageReceivers)
            {
                int senderId = context.Messages.Where(x => x.Id == mr.MessageId).ToList()[0].SenderId;
                User sender = context.Users.Where(x => x.Id == senderId).ToList()[0];

                InboxMessage inboxMessage = GetMessageModel(mr, sender);
                inboxMessages.Add(inboxMessage);

                if (!dates.Contains(inboxMessage.DateDay))
                {
                    dates.Add(inboxMessage.DateDay);
                }
            }
            dates.Reverse();

            inboxMessages.Sort((m1, m2) => m1.Date.CompareTo(m2.Date));
            inboxMessages.Reverse();

            inboxViewModel.inboxMessages = inboxMessages;
            inboxViewModel.dates = dates;

            return View(inboxViewModel);
        }

        public IActionResult Message(int id)
        {
            List<System.Security.Claims.Claim> c = User.Claims.Where(x => x.Type.Contains("mail")).ToList();
            string email = c[0].Value;
            int userId = context.GetUserId(email);

            context.MessageReceivers.Where(x => x.MessageId == id && x.UserId == userId).ToList()[0].Read = true;
            context.SaveChanges();

            MessageReceiver mr = context.MessageReceivers.Where(x => x.MessageId == id && x.UserId == userId).ToList()[0];
            int senderId = context.Messages.Where(x => x.Id == mr.MessageId).ToList()[0].SenderId;
            User sender = context.Users.Where(x => x.Id == senderId).ToList()[0];

            InboxMessage message = GetMessageModel(mr, sender);
            string cc = "";
            if (mr.ReceiveType != 2)
            {
                foreach (MessageReceiver m in context.MessageReceivers.Where(x => x.MessageId == mr.MessageId && x.UserId != userId && x.ReceiveType == 1).ToList())
                {
                    User u = context.Users.Where(x => x.Id == m.UserId).ToList()[0];
                    cc += u.Name + " " + u.Surname + " <" + u.EmailAddress + ">, ";
                }
                if (cc.Length > 0)
                    cc = cc.Substring(0, cc.Length - 2);
            }
            message.CC = cc;

            MessageViewModel messageViewModel = new MessageViewModel();
            
            messageViewModel.message = message;

            return View(messageViewModel);
        }

        private InboxMessage GetMessageModel(MessageReceiver mr, User sender)
        {
            InboxMessage inboxMessage = new InboxMessage();

            inboxMessage.SenderEmailAddress = sender.EmailAddress;
            inboxMessage.SenderName = sender.Name;
            inboxMessage.SenderSurname = sender.Surname;
            inboxMessage.Subject = mr.Message.Subject;
            inboxMessage.Content = mr.Message.Content;
            inboxMessage.Date = mr.Message.SendDate;
            inboxMessage.IsRead = mr.Read;
            inboxMessage.Id = mr.MessageId;

            string date, dateDetail;
            DateTime actualDate = DateTime.Now;
            inboxMessage.DateDay = mr.Message.SendDate.ToString("dd'/'MM'/'yyyy");
            if (inboxMessage.Date.Month != actualDate.Month || inboxMessage.Date.Day != actualDate.Day)
            {
                date = inboxMessage.Date.Day + " " + Month(inboxMessage.Date.Month);
                if (inboxMessage.Date.Year != actualDate.Year)
                {
                    date = date + " " + inboxMessage.Date.Year;
                }
                dateDetail = date + ", " + inboxMessage.Date.Hour + ":" + inboxMessage.Date.Minute;
            }
            else
            {
                date = "Today, " + inboxMessage.Date.Hour + ":" + inboxMessage.Date.Minute;
                dateDetail = date;
            }
            inboxMessage.DateView = date;
            inboxMessage.DateDetail = dateDetail;

            string content;
            if (inboxMessage.Content.Length > 20)
                content = inboxMessage.Content.Substring(0, 20) + "...";
            else
                content = inboxMessage.Content;
            inboxMessage.ContentView = content;

            inboxMessage.Link = "Message?id=" + inboxMessage.Id;

            return inboxMessage;
        }

        private string Month(int i)
        {
            string[] months = new string[] {"January", "February", "March", "April", "May", "June", 
                                            "July", "August", "September", "October", "November", "December" };

            return months[i - 1];
        }
    }
}
