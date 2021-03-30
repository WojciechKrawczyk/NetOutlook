using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Models
{
    public class InboxViewModel
    {
        public List<InboxMessage> inboxMessages { get; set; }

        public List<string> dates { get; set; }
    }
}
