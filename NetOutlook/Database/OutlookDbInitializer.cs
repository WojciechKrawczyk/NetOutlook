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
            if(Recreate)
            {
                context.GroupMembers.RemoveRange(context.GroupMembers);
                context.MessageReceivers.RemoveRange(context.MessageReceivers);
                context.Users.RemoveRange(context.Users);
                context.Groups.RemoveRange(context.Groups);
                context.Messages.RemoveRange(context.Messages);
                context.SaveChanges();
            }
            //var students = new User[]
            //    {
            //        new User{Name="Jan",Surname="Kowalski",EmailAddress="jan.kowal@rmail.xd",Role=0},
            //        new User{Name="Adam",Surname="Nowak",EmailAddress="adam.nowak@rmail.xd",Role=0},
            //        new User{Name="Marcin",Surname="Nowak",EmailAddress="marcin.nowak@rmail.xd",Role=0},
            //        new User{Name="Daria",Surname="Nowak",EmailAddress="daria.nowak@rmail.xd",Role=0},
            //        new User{Name="Tomasz",Surname="Nowak",EmailAddress="tomasz.nowak@rmail.xd",Role=0},
            //        new User{Name="Anna",Surname="Kowalska",EmailAddress="kowal.anna@rmail.xd",Role=0},
            //        new User{Name="Przemysław",Surname="Nowak",EmailAddress="data.mine@rmail.xd",Role=0},
            //        new User{Name="Henryk",Surname="Nowak",EmailAddress="henryk.mine@rmail.xd",Role=0},
            //        new User{Name="Wiele",Surname="wierszy",EmailAddress="row.mine@rmail.xd",Role=0},
            //        new User{Name="Do",Surname="Testowania",EmailAddress="test.mine@rmail.xd",Role=0},
            //        new User{Name="Scrollowania",Surname="Dlugiej",EmailAddress="long.mine@rmail.xd",Role=0},
            //        new User{Name="Listy",Surname="Kontaktow",EmailAddress="listy.mine@rmail.xd",Role=0},
            //    };
            //var groups = new Group[]
            //    {
            //        new Group{OwnerId=1, GroupName="Grupa A"},
            //        new Group{OwnerId=2, GroupName="Grupa B"},
            //    };
            //var groupMembers = new GroupMember[]
            //    {
            //        new GroupMember{UserId=1, GroupId=2},
            //        new GroupMember{UserId=2, GroupId=1}
            //    };
            //if (!context.Users.Any())
            //{
            //    foreach (User s in students)
            //    {
            //        context.Users.Add(s);
            //    }
            //    context.SaveChanges();
            //}
            //if (!context.Groups.Any())
            //{   
            //    foreach (Group s in groups)
            //    {
            //        context.Groups.Add(s);
            //    }
            //    context.SaveChanges();
            //}
            //if (!context.GroupMembers.Any())
            //{
            //    foreach (GroupMember s in groupMembers)
            //    {
            //        context.GroupMembers.Add(s);
            //    }
            //    context.SaveChanges();
            //}
        }
    }
}
