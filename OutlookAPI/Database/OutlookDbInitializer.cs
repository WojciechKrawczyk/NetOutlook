using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class OutlookDbInitializer
    {
        protected static bool Recreate = false;
        public static void Initialize(OutlookContext context)
        {
            context.Database.EnsureCreated();
            var students = new User[]
                {
                    new User{Name="Jan",Surname="Kowalski",EmailAddress="jan.kowal@rmail.xd",Role=0},
                    new User{Name="Adam",Surname="Nowak",EmailAddress="adam.kowal@rmail.xd",Role=0},
                };
            var groups = new Group[]
                {
                    new Group{OwnerId=1, GroupName="Grupa A"},
                    new Group{OwnerId=2, GroupName="Grupa B"},
                };
            var groupMembers = new GroupMember[]
                {
                    new GroupMember{UserId=1, GroupId=2},
                    new GroupMember{UserId=2, GroupId=1}
                };


            if (Recreate) context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            if (!context.Users.Any())
            {
                foreach (User s in students)
                {
                    context.Users.Add(s);
                }
                context.SaveChanges();
            }
            if (!context.Groups.Any())
            {   
                foreach (Group s in groups)
                {
                    context.Groups.Add(s);
                }
                context.SaveChanges();
            }
            if (!context.GroupMembers.Any())
            {
                foreach (GroupMember s in groupMembers)
                {
                    context.GroupMembers.Add(s);
                }
                context.SaveChanges();
            }
        }
    }
}
