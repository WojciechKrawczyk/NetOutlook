using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class GroupRaw
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int OwnerId { get; set; }

        public GroupRaw(Group g)
        {
            Id = g.Id;
            GroupName = g.GroupName;
            OwnerId = g.OwnerId;
        }
    }
}
