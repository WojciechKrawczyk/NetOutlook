using NetOutlook.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Models
{
    public class GroupManagerModel
    {
        public int OldIdStr { get; set; }
        public List<Group> groups { get; set; }


    }
}
