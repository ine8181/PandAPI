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
        private const string OBJECTS = "Objects";
        private const string OBJECTGROUPS = "ObjectGroups";
        
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
            if (userCollection.Find(u => u.UserId == userModel.UserId).Any())
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
                db.GetCollection<UserModel>(USERS).Indexes.CreateOne(indexKeyBuilder.Ascending(u => u.UserId));
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
                    keyBuilder.Ascending(ug => ug.UserId),
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

        public bool AddUserToGroup(string userId, string groupId)
        {
            var userGroupCollection = db.GetCollection<UserGroupModel>(USERGROUPS);
            if (userGroupCollection.Find(ug => ug.UserId == userId && ug.GroupId == groupId).Any())
                return false;
            userGroupCollection.InsertOne(new UserGroupModel {UserId = userId, GroupId = groupId});
            return true;
        }

        public IEnumerable<string> GetUsersForGroup(string groupId)
        {
            var userGroupCollection = db.GetCollection<UserGroupModel>(USERGROUPS);
            var users = userGroupCollection.Find(ug => ug.GroupId == groupId).Project(ug => ug.UserId).ToList();
            return users;
        }

        public string AddObject(object payload, string userId, string groupId)
        {
            var objectsCollection = db.GetCollection<ObjectModel>(OBJECTS);
            if (!IsUserInGroup(userId, groupId))
                return null;
            var objectModel = new ObjectModel{Payload = payload, GroupId = groupId, UserId = userId};
            objectsCollection.InsertOne(objectModel);
            var objectId = objectModel._id;
            var objectIdStr = objectId.ToString();

            var objectGroupCollection = db.GetCollection<ObjectGroupModel>(OBJECTGROUPS);
            objectGroupCollection.InsertOne(new ObjectGroupModel{GroupId = groupId, ObjectId = objectId});
            
            return objectIdStr;
        }

        private bool IsUserInGroup(string userId, string groupId)
        {
            var userGroupsCollection = db.GetCollection<UserGroupModel>(USERGROUPS);
            return userGroupsCollection.Find(ug => ug.UserId == userId && ug.GroupId == groupId).Any();
        }
    }

    public class ObjectGroupModel
    {
        public ObjectId _id { get; set; }
        public ObjectId ObjectId { get; set; }
        public string GroupId { get; set; }
                
    }
    public class ObjectModel
    {
        public ObjectId _id { get; set; }
        public object Payload { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }
    }

    public class UserGroupModel
    {
        public ObjectId _id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}