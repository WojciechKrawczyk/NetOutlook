using NetOutlook.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetOutlook.Models
{
    public class EditGroupModel
    {
        public string GroupName { get; set; }
        public string Emails { get; set; }
        public string OldIdStr { get; set; }
        public int OldGroupId { get; set; }

        //public Group oldGroup;
        //public HashSet<string> oldAdress;

        public HashSet<string> Addresses;

        public EditGroupModel()
        {
            Addresses = new HashSet<string>();
        }

        

        public ValidationResult Validate(OutlookContext context, int id, string email)
        {
            Addresses.Clear();
            if (Emails.Length == 0)
            {
                return new ValidationResult("Error: No addresses provided!");
            }
            if (GroupName.Length == 0)
            {
                return new ValidationResult("Error: No Group Name provided!");
            }
            if (GroupName.StartsWith("g:"))
            {
                return new ValidationResult("Error: Invalid Group Name, start with 'g:' is not allowed");
            }
            if (GroupName.Contains(";"))
            {
                return new ValidationResult("Error: Invalid Group Name, using ';' is not allowed");
            }
            if (context.Groups.Where(g => g.GroupName == GroupName && g.OwnerId == id && g.Id != int.Parse(OldIdStr)).ToList().Count > 0)
            {
                return new ValidationResult("Error: Invalid Group Name, you have already group with the same name");
            }
            string[] EmailsTab = Emails.Split(';');
            (bool dis1, ValidationResult v1) = CheckExistence(context, id, email, EmailsTab, "Error in group members", 0);
            if (v1 != ValidationResult.Success)
            {
                return v1;
            }


            return ValidationResult.Success;
        }

        protected (bool, ValidationResult) CheckExistence(OutlookContext context, int id, string email, string[] addresses, string err_prefix, int send_type)
        {
            char[] charsToTrim = { '\t', ' ', '\n' };
            bool distinct = false;
            foreach (string userT in addresses)
            {
                string user = userT.Trim(charsToTrim);
                if (user.Length == 0) continue;
                if (user.Equals(email)) continue;
                if (user.StartsWith("g:"))
                {
                    //string group = user.Substring(2);
                    Group g = context.Groups.FirstOrDefault(x => (x.GroupName == user && x.OwnerId == id));
                    if (g == null)
                    {
                        return (false, new ValidationResult($"{err_prefix}: Group {user} doesn't exist!"));
                    }

                    Addresses.Add(user);
                    distinct = true;
                }
                else
                {
                    User u = context.Users.FirstOrDefault(x => x.EmailAddress == user);
                    if (u == null)
                    {
                        return (false, new ValidationResult($"{err_prefix}: User {user} doesn't exist!"));
                    }

                    Addresses.Add(user);
                    distinct = true;
                }
            }
            return (distinct, ValidationResult.Success);
        }

        public static List<GroupMember> GroupManagerParams(OutlookContext context, Group gr, int id, string email, HashSet<string> addresses)
        {
            HashSet<string> emailsToGroup = new HashSet<string>();
            List<GroupMember> groupMembers = new List<GroupMember>();

            foreach (string user in addresses)
            {
                if (user.Length == 0) continue;
                if (user.Equals(email)) continue;
                if (!user.StartsWith("g:") && emailsToGroup.Add(user))
                {


                    List<User> dest = context.Users.Where(x => x.EmailAddress.Equals(user)).ToList();
                    int DestId = dest[0].Id;

                    GroupMember grmem = new GroupMember();
                    grmem.GroupId = gr.Id;
                    grmem.UserId = DestId;
                    grmem.User = dest[0];
                    grmem.Group = gr;

                    groupMembers.Add(grmem);
                    context.GroupMembers.Add(grmem);

                }
                else
                {
                    string group = user.Substring(2);
                    List<Group> dest = context.Groups.Where((x => x.GroupName.Equals(group) && x.OwnerId == id)).ToList();
                    List<GroupMember> receivers = dest[0].Members.ToList();
                    foreach (GroupMember gm in receivers)
                    {
                        if (gm.UserId == id) continue;
                        if (!emailsToGroup.Add(gm.User.EmailAddress)) continue;

                        List<User> dest2 = context.Users.Where(x => x.Id == gm.UserId).ToList();
                        int DestId = dest2[0].Id;

                        GroupMember grmem = new GroupMember();
                        grmem.GroupId = gr.Id;
                        grmem.UserId = DestId;
                        grmem.User = dest2[0];
                        grmem.Group = gr;

                        groupMembers.Add(grmem);
                        context.GroupMembers.Add(grmem);

                    }
                }
            }
            return groupMembers;
        }
    }
}
