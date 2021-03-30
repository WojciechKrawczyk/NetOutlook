using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetOutlook.Database
{
    public class OutlookContext : DbContext
    {

        public OutlookContext(DbContextOptions<OutlookContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups  { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        public DbSet<MessageReceiver> MessageReceivers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //messsage receiver
            modelBuilder.Entity<MessageReceiver>().HasKey(k => new { k.MessageId, k.UserId });
            modelBuilder.Entity<MessageReceiver>().HasOne(o => o.User).WithMany(o => o.Received)
                .HasForeignKey(o => o.UserId);
            modelBuilder.Entity<MessageReceiver>().HasOne(o => o.Message).WithMany(o => o.MessageReceivers)
                .HasForeignKey(o => o.MessageId);

            //groups
            modelBuilder.Entity<GroupMember>().HasKey(k => new { k.GroupId, k.UserId });
            modelBuilder.Entity<GroupMember>().HasOne(o => o.User).WithMany(o => o.UserGroups)
                .HasForeignKey(o => o.UserId);
            modelBuilder.Entity<GroupMember>().HasOne(o => o.Group).WithMany(o => o.Members)
                .HasForeignKey(o => o.GroupId);


            modelBuilder.Entity<User>().HasKey(k => k.Id);
            modelBuilder.Entity<Group>().HasKey(k => k.Id);
            modelBuilder.Entity<Message>().HasKey(k => k.Id);

            modelBuilder.Entity<MessageReceiver>().ToTable("MessageReceivers");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Message>().ToTable("Messages");
            modelBuilder.Entity<Group>().ToTable("Groups");
            modelBuilder.Entity<GroupMember>().ToTable("GroupMembers");
        }

        public List<string> GetUsersFromGroup(int groupID)
        {
            List<string> emails = new List<string>();
            var members = GroupMembers.Join(Users, x => x.UserId, x => x.Id,
                (group, user) => new
                {
                    group.GroupId,
                    user.EmailAddress
                }).Where(x => x.GroupId == groupID).ToList();
            foreach(var a in members)
            {
                emails.Add(a.EmailAddress);
            }
            return emails;
        }

        public int GetUserId(string email)
        {
            List<User> thatUser = Users.Where(x => x.EmailAddress == email).ToList();
            if(thatUser.Count!=1)
            {
                return -1;
            }
            return thatUser[0].Id;
        }
        public string GetUserEmail(int id)
        {
            List<User> thatUser = Users.Where(x => x.Id == id).ToList();
            if (thatUser.Count != 1)
            {
                return null;
            }
            return thatUser[0].EmailAddress;
        }

        public List<UserRaw> GetAllUsers(string email)
        {          
            List<UserRaw> usersRaw = new List<UserRaw>();
            List<User> users;
            if (email.Length == 0) users = Users.ToList();
            else users = Users.Where(x => x.EmailAddress != email).ToList();
            foreach(User user in users)
            {
                usersRaw.Add(new UserRaw(user));
            }
            return usersRaw;
        }

        public List<GroupRaw> GetUserGroups(string email)
        { 
            List<GroupRaw> groupsRaw = new List<GroupRaw>();
            int id = GetUserId(email);
            List<Group> groups = Groups.Where(x => x.OwnerId==id).ToList();
            foreach (Group g in groups)
            {
                groupsRaw.Add(new GroupRaw(g));
            }
            return groupsRaw;
        }

        public void AddUserIfNotExists(string name, string surname, string email)
        {
            if(!Users.Any(x=>x.EmailAddress==email))
            {
                User user = new User();
                user.Name = name;
                user.Surname = surname;
                user.EmailAddress = email;
                user.Role = 0;
                Users.Add(user);
                SaveChanges();
            }
        }

        public string GetEmailById(int id)
        {
            return Users.Find(id).EmailAddress;
        }
    }
}
