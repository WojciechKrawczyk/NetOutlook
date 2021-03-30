using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public int Role { get; set; } //0 - user, 1 - admin

        public List<GroupMember> UserGroups { get; set; }
        public List<MessageReceiver> Received { get; set; }
    }
}
