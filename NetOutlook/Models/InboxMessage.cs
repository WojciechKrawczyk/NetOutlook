using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Models
{
    public class InboxMessage
    {
        public string SenderEmailAddress { get; set; }
        public string SenderName { get; set; }
        public string SenderSurname { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public int Id { get; set; }
        public string DateView { get; set; }
        public string DateDay { get; set; }
        public string ContentView { get; set; }
        public string Link { get; set; }
        public string DateDetail { get; set; }
        public string CC { get; set; }
    }
}
