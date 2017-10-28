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
0            store.InitialiseIndex();
        }


        [Test]
        public void TestCreateUser()
        {
            var newUserName = Guid.NewGuid().ToString();
            var result = this.store.AddUser(new UserModel {UserId = newUserName, Password = "password"});
            Assert.IsTrue(result);
        }

        [Test]
        public void DuplicateUserCreation()
        {
            var newUserName = Guid.NewGuid().ToString();
            var result = this.store.AddUser(new UserModel {UserId = newUserName, Password = "password"});
            Assert.IsTrue(result);
            var newResult = this.store.AddUser(new UserModel {UserId = newUserName, Password = "password"});
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

        [Test]
        public void AddObjectTest()
        {
            var user1 = Guid.NewGuid().ToString();
            var group1 = Guid.NewGuid().ToString();

            AddUserAndAssert(user1);
            AddGroupAndAssert(group1);

            Assert.IsTrue(store.AddUserToGroup(user1, group1));

            var result = store.AddObject(new {hello = "world"}, user1, group1);
            
            Assert.IsNotNull(result);
        }
        
        [Test]
        public void AddObjectTest_UserNotInGroup()
        {
            var user1 = Guid.NewGuid().ToString();
            var group1 = Guid.NewGuid().ToString();

            AddUserAndAssert(user1);
            AddGroupAndAssert(group1);

            var result = store.AddObject(new {hello = "world"}, user1, group1);
            
            Assert.IsNull(result);
        }
        

        private void AddGroupAndAssert(string newGroupId)
        {
            var addGroupResult = this.store.AddGroup(new GroupModel {GroupId = newGroupId});
            Assert.IsTrue(addGroupResult);
        }

        private void AddUserAndAssert(string newUserGuid)
        {
            var addUserResult = this.store.AddUser(new UserModel {UserId = newUserGuid});
            Assert.IsTrue(addUserResult);
        }
    }
}