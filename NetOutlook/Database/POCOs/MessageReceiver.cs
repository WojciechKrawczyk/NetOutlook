using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class MessageReceiver
    {
        public int MessageId { get; set; }
        public Message Message { get; set; }
        public int UserId { get; set;  }
        public User User { get; set; } 
        public int ReceiveType { get; set; }
        //0 - direct
        //1 - CC
        //2- BCC
        public bool Read { get; set; }
    }
}
