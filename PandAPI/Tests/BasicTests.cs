using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using MongoDB.Driver;
using NUnit.Framework;
using PandAPI.PandApi;

namespace PandAPI.Tests
{
    [TestFixture]
    public class BasicTests
    {
        private PandaStore store;

        [SetUp]
        public void Init()
        {
            this.store = new PandaStore(new MongoClient("mongodb://localhost:27017"));
        }

        [Test]
        public void TestInitStore()
        {
            store.InitialiseIndex();
        }


        [Test]
        public void TestCreateUser()
        {
            var newUserName = Guid.NewGuid().ToString();
            var result = this.store.AddUser(new UserModel {EmailAddress = newUserName, Password = "password"});
            Assert.IsTrue(result);
        }

        [Test]
        public void DuplicateUserCreation()
        {
            var newUserName = Guid.NewGuid().ToString();
            var result = this.store.AddUser(new UserModel {EmailAddress = newUserName, Password = "password"});
            Assert.IsTrue(result);
            var newResult = this.store.AddUser(new UserModel {EmailAddress = newUserName, Password = "password"});
            Assert.IsFalse(newResult);
        }

        [Test]
        public void AddGroup()
        {
            var newGroupId = Guid.NewGuid().ToString();
            var result = this.store.AddGroup(new GroupModel {GroupId = newGroupId});
            Assert.IsTrue(result);
        }

        [Test]
        public void AddDuplicateGroup()
        {
            var newGroupId = Guid.NewGuid().ToString();
            var result = this.store.AddGroup(new GroupModel {GroupId = newGroupId});
            Assert.IsTrue(result);
            result = this.store.AddGroup(new GroupModel {GroupId = newGroupId});
            Assert.IsFalse(result);
        }

        [Test]
        public void AddUserGroupRelationship()
        {
            var newUserGuid = Guid.NewGuid().ToString();
            var newGroupId = Guid.NewGuid().ToString();

            AddUserAndAssert(newUserGuid);
            AddGroupAndAssert(newGroupId);

            var userGroupResult = this.store.AddUserToGroup(newUserGuid, newGroupId);
            Assert.IsTrue(userGroupResult);
        }

        [Test]
        public void AddUserToGroupAndGet()
        {
            var user1 = Guid.NewGuid().ToString();
            var user2 = Guid.NewGuid().ToString();

            var groupId = Guid.NewGuid().ToString();

            AddUserAndAssert(user1);
            AddUserAndAssert(user2);
            AddGroupAndAssert(groupId);


            store.AddUserToGroup(user1, groupId);
            store.AddUserToGroup(user2, groupId);


            var users = store.GetUsersForGroup(groupId);

            Assert.AreEqual(2, users.Count());
            Assert.IsTrue(users.Any(u => u  == user1));
            Assert.IsTrue(users.Any(u => u == user2));
        }


        private void AddGroupAndAssert(string newGroupId)
        {
            var addGroupResult = this.store.AddGroup(new GroupModel {GroupId = newGroupId});
            Assert.IsTrue(addGroupResult);
        }

        private void AddUserAndAssert(string newUserGuid)
        {
            var addUserResult = this.store.AddUser(new UserModel {EmailAddress = newUserGuid});
            Assert.IsTrue(addUserResult);
        }
    }
}