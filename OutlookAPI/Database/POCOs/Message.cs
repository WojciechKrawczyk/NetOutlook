using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public int Settings { get; set; }
        // 0 - cc
        // 1 - bcc
        public List<MessageReceiver> MessageReceivers { get; set; }
    }
}
