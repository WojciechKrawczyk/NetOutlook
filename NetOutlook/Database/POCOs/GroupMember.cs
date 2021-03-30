using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class GroupMember
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }

        public Group Group { get; set; }
        public User User { get; set; }
    }
}
