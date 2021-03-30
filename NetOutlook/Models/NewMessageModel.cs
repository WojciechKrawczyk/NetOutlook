using NetOutlook.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Models
{
    public class NewMessageModel
    {
        public string SubjectText { get; set; }
        public string MessageContent { get; set; }

        public string Direct { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }


        public Dictionary<string,int> Addresses;

        public NewMessageModel()
        {
            Addresses = new Dictionary<string, int>();
        }

        protected (bool, ValidationResult) CheckExistence(OutlookContext context, int id, string email,string[] addresses, string err_prefix, int send_type)
        {
            bool distinct = false;
            foreach (string user in addresses)
            {
                if (user.Length == 0) continue;
                if (user.Equals(email)) continue;
                if (user.StartsWith("g:"))
                {
                    string group = user.Substring(2);
                    Group g = context.Groups.FirstOrDefault(x => (x.GroupName == group && x.OwnerId == id));
                    if (g==null)
                    {
                        return (false, new ValidationResult($"{err_prefix}: Group {group} doesn't exist!"));
                    }
                    List<string> members = context.GetUsersFromGroup(g.Id);
                    foreach (string member in members)
                    {
                        Debug.WriteLine(member);
                        if (Addresses.ContainsKey(member))
                        {
                            if (Addresses[member] != send_type) return (false, new ValidationResult($"{err_prefix}: User {member} from {group} has multiple delivery rules. Remove unnecesary rules!"));
                        }
                        Addresses[member] = send_type;
                    }
                    distinct = true;
                }
                else
                {
                    User u = context.Users.FirstOrDefault(x => x.EmailAddress == user);
                    if (u == null)
                    {
                        return (false, new ValidationResult($"{err_prefix}: User {user} doesn't exist!"));
                    }
                    if (Addresses.ContainsKey(user))
                    {
                        if(Addresses[user]!=send_type) return (false, new ValidationResult($"{err_prefix}: User {user} has multiple delivery rules. Remove unnecesary rules!"));
                    }
                    Addresses[user] = send_type;
                    distinct = true;
                }
            }
            return (distinct, ValidationResult.Success);
        }


        public ValidationResult Validate(OutlookContext context,int id, string email)
        {
            Addresses.Clear();
            if (CC.Length == 0 && BCC.Length == 0 && Direct.Length == 0)
            {
                return new ValidationResult("Error: No addresses provided!");
            }
            string[] CCs = CC.Split(';');
            string[] BCCs = BCC.Split(';');
            string[] Directs = Direct.Split(';');
            (bool dis1, ValidationResult v1) = CheckExistence(context, id, email, Directs,"Error in direct",0);
            if(v1!=ValidationResult.Success)
            {
                return v1;
            }
            (bool dis2, ValidationResult v2) = CheckExistence(context, id, email, CCs, "Error in CCs",1);
            if (v2 != ValidationResult.Success)
            {
                return v2;
            }
            (bool dis3, ValidationResult v3) = CheckExistence(context, id, email, BCCs, "Error in BCCs",2);
            if (v3 != ValidationResult.Success)
            {
                return v3;
            }
            return ValidationResult.Success;
        }

        public static bool SendMessageParams(OutlookContext context, Message msg, int id, string email, Dictionary<string, int> addresses)
        {
            foreach (string user in addresses.Keys)
            {
                if (user.Length == 0) continue;
                if (user.Equals(email)) continue;
                List<User> dest = context.Users.Where(x => x.EmailAddress.Equals(user)).ToList();
                int DestId = dest[0].Id;
                if (context.MessageReceivers.Where(x => x.MessageId == msg.Id && x.UserId == DestId).ToList().Count == 0)
                {
                    MessageReceiver msgrec = new MessageReceiver();
                    msgrec.MessageId = msg.Id;
                    msgrec.UserId = DestId;
                    msgrec.ReceiveType = addresses[user];
                    msgrec.Read = false;
                    context.MessageReceivers.Add(msgrec);
                }
            }
            return true;
        }
    }
}
