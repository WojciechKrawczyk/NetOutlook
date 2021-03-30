using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class UserRaw
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public int Role { get; set; } //0 - user, 1 - admin

        public UserRaw(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Surname = user.Surname;
            EmailAddress = user.EmailAddress;
            Role = user.Role;
        }
    }
}
