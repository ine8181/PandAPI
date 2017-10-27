using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace PandAPI.PandApi
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // This is injected by the DI
    public class PandaStore
    {
        private const string USERGROUPS = "UserGroups";
        private const string GROUPS = "Groups";
        private const string USERS = "Users";
        private readonly MongoClient client;
        private IMongoDatabase db;

        public PandaStore(MongoClient client)
        {
            this.client = client;
            this.db = GetDatabase();
        }

        public bool AddUser(UserModel userModel)
        {
            var db = GetDatabase();
            var userCollection = db.GetCollection<UserModel>(USERS);
            if (userCollection.Find(u => u.EmailAddress == userModel.EmailAddress).Any())
                return false;
            userCollection.InsertOne(userModel);
            return true;
        }

        private IMongoDatabase GetDatabase()
        {
            var db = client.GetDatabase("PandaStore");
            return db;
        }

        public void InitialiseIndex()
        {
            var db = GetDatabase();

            // users
            {
                var indexKeyBuilder = new IndexKeysDefinitionBuilder<UserModel>();
                db.GetCollection<UserModel>(USERS).Indexes.CreateOne(indexKeyBuilder.Ascending(u => u.EmailAddress));
            }
            // groups
            {
                var keyBuilder = new IndexKeysDefinitionBuilder<GroupModel>();
                db.GetCollection<GroupModel>(GROUPS).Indexes.CreateOne(keyBuilder.Ascending(g => g.GroupId));
            }
            // userGroup
            {
                var keyBuilder = new IndexKeysDefinitionBuilder<UserGroupModel>();
                var coll = db.GetCollection<UserGroupModel>(USERGROUPS);
                coll.Indexes.CreateOne(keyBuilder.Combine(
                    keyBuilder.Ascending(ug => ug.EmailAddress),
                    keyBuilder.Ascending(ug => ug.GroupId)));

                coll.Indexes.CreateOne(keyBuilder.Ascending(ug => ug.GroupId));

            }
        }

        public bool AddGroup(GroupModel groupModel)
        {
            var db = GetDatabase();
            var groupCollection = db.GetCollection<GroupModel>(GROUPS);
            if (groupCollection.Find(g => g.GroupId == groupModel.GroupId).Any())
                return false;
            groupCollection.InsertOne(groupModel);
            return true;
        }

        public bool AddUserToGroup(string emailAddress, string groupId)
        {
            var userGroupCollection = db.GetCollection<UserGroupModel>(USERGROUPS);
            if (userGroupCollection.Find(ug => ug.EmailAddress == emailAddress && ug.GroupId == groupId).Any())
                return false;
            userGroupCollection.InsertOne(new UserGroupModel {EmailAddress = emailAddress, GroupId = groupId});
            return true;
        }

        public IEnumerable<string> GetUsersForGroup(string groupId)
        {
            var userGroupCollection = db.GetCollection<UserGroupModel>(USERGROUPS);
            var users = userGroupCollection.Find(ug => ug.GroupId == groupId).Project(ug => ug.EmailAddress).ToList();
            return users;
        }
    }

    public class UserGroupModel
    {
        public ObjectId _id { get; set; }
        public string EmailAddress { get; set; }
        public string GroupId { get; set; }
    }
}