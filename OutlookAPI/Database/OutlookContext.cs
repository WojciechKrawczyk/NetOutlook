using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
    }
}
