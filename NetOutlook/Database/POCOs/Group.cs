using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class Group
    {
        public int Id { get; set; }

        public string GroupName { get; set; }
        public int OwnerId { get; set; }

        // group - users relation
        public List<GroupMember> Members { get; set; }
    }
}
