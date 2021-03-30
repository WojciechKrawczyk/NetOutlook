using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetOutlook.Database;

namespace NetOutlook.Models
{
    public class AdminPageModel
    {
        public bool IsValid { get; set; }

        public List<UserAdminPageModel> Users { get; set; }
        public int AdminId { get; set; }

    }

    public class UserAdminPageModel
    {
        public UserRaw UserRaw { get; set; }
        public string DeleteLink { get; set; }
        public string AcceptLink { get; set; }
    }
}
