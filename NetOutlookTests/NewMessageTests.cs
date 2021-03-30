using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetOutlook.Database;
using NetOutlook.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace NetOutlookTests
{
    [TestClass]
    public class NewMessageTests
    {
        public void ClearContext(OutlookContext context)
        {
            context.GroupMembers.RemoveRange(context.GroupMembers);
            context.MessageReceivers.RemoveRange(context.MessageReceivers);
            context.Users.RemoveRange(context.Users);
            context.Groups.RemoveRange(context.Groups);
            context.Messages.RemoveRange(context.Messages);
            context.SaveChanges();
        }
        [TestMethod]
        public void ValidAddress()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "adam.kowal@rmail.xd";
                model.CC = "";
                model.BCC = "";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreEqual(testResult, ValidationResult.Success);
        }

        [TestMethod]
        public void NotExists()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "adam.kowal@rmail.xd;monika.kowalska@gmail.rom;";
                model.CC = "";
                model.BCC = "";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreNotEqual(testResult, ValidationResult.Success);
        }

        [TestMethod]
        public void InvalidAddresses()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "adam.kowal@rmail.xd;;;a;;";
                model.CC = "";
                model.BCC = "";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreNotEqual(testResult, ValidationResult.Success);
        }

        [TestMethod]
        public void MultipleRules()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Monika", Surname = "Kowalska", EmailAddress = "mon.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Janusz", Surname = "Kowalski", EmailAddress = "jsz.kowal@rmail.xd" });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "adam.kowal@rmail.xd;jsz.kowal@rmail.xd";
                model.CC = "jurg.kowal@rmail.xd";
                model.BCC = "mon.kowal@rmail.xd";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreEqual(testResult, ValidationResult.Success);
        }

        [TestMethod]
        public void MultipleOverlappingRules()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Monika", Surname = "Kowalska", EmailAddress = "mon.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Janusz", Surname = "Kowalski", EmailAddress = "jsz.kowal@rmail.xd" });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "adam.kowal@rmail.xd;jsz.kowal@rmail.xd";
                model.CC = "";
                model.BCC = "mon.kowal@rmail.xd;adam,kowal@rmail.xd";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreNotEqual(testResult, ValidationResult.Success);
        }

        [TestMethod]
        public void MultipleNotValid()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Janusz", Surname = "Kowalski", EmailAddress = "jsz.kowal@rmail.xd" });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "adam.kowal@rmail.xd;jsz.kowal@rmail.xd";
                model.CC = "";
                model.BCC = "mon.kowal@rmail.xd;adam,kowal@rmail.xd";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreNotEqual(testResult, ValidationResult.Success);
        }

        [TestMethod]
        public void Group()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Janusz", Surname = "Kowalski", EmailAddress = "jsz.kowal@rmail.xd" });
                context.Groups.Add(new Group { GroupName = "Kowalscy", OwnerId = 2 });
                context.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = 1 });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "g:Kowalscy;jsz.kowal@rmail.xd";
                model.CC = "";
                model.BCC = "";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreEqual(testResult, ValidationResult.Success);
        }

        [TestMethod]
        public void GroupNotExists()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Janusz", Surname = "Kowalski", EmailAddress = "jsz.kowal@rmail.xd" });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "g:Kowalscy;jsz.kowal@rmail.xd";
                model.CC = "";
                model.BCC = "";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreNotEqual(testResult, ValidationResult.Success);
        }

        [TestMethod]
        public void GroupNotOwned()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Janusz", Surname = "Kowalski", EmailAddress = "jsz.kowal@rmail.xd" });
                context.Groups.Add(new Group { GroupName = "Kowalscy", OwnerId = 3 });
                context.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = 1 });
                context.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = 2 });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "g:Kowalscy;jsz.kowal@rmail.xd";
                model.CC = "";
                model.BCC = "";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreNotEqual(testResult, ValidationResult.Success);
        }

        public void GroupOverlappingRules()
        {
            var options = new DbContextOptionsBuilder<OutlookContext>()
           .UseInMemoryDatabase(databaseName: "NetOutlookDatabaseMock")
           .Options;
            ValidationResult testResult;
            using (var context = new OutlookContext(options))
            {
                ClearContext(context);
                context.Users.Add(new User { Name = "Adam", Surname = "Kowalski", EmailAddress = "adam.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Jurgen", Surname = "Kowalski", EmailAddress = "jurg.kowal@rmail.xd" });
                context.Users.Add(new User { Name = "Janusz", Surname = "Kowalski", EmailAddress = "jsz.kowal@rmail.xd" });
                context.Groups.Add(new Group { GroupName = "Kowalscy", OwnerId = 3 });
                context.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = 1 });
                context.GroupMembers.Add(new GroupMember { GroupId = 1, UserId = 2 });
                context.SaveChanges();
                NewMessageModel model = new NewMessageModel();
                model.Direct = "g:Kowalscy;";
                model.CC = "";
                model.BCC = "adam.kowal@rmail.xd";
                testResult = model.Validate(context, 2, "jurg.kowal@rmail.xd");
            }
            Assert.AreNotEqual(testResult, ValidationResult.Success);
        }
    }
}
