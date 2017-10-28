using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using MongoDB.Driver;
using NUnit.Framework;
using PandAPI.PandApi;
using Remotion.Linq.Clauses.ResultOperators;

namespace PandAPI.Tests
{
    [TestFixture]
    public class PerfTest
    {
        private PandaStore store;

        /// <summary>
        /// 1 user, 100 groups, 100 items each.
        /// </summary>
        [Test]
        public void TestGetItems()
        {
            this.store = new PandaStore(new MongoClient("mongodb://localhost:27017"));

            var userId = Guid.NewGuid().ToString();
            store.AddUser(new UserModel()
            {
                UserId = userId,
                Password = "password"
            });

            var groupIds = Enumerable.Range(0, 100).Select(i => Guid.NewGuid().ToString()).ToList();

            foreach (var groupId in groupIds)
            {
                store.AddGroup(new GroupModel {GroupId = groupId});
                store.AddUserToGroup(userId, groupId);
                var items = Enumerable.Range(0, 100).Select(i => Guid.NewGuid().ToString());
                foreach (var item in items)
                {
                    store.AddObject(new { item = item }, userId, groupId);
                    
                }
            }

            var objects = store.GetObjectsForUser(userId, 0, int.MaxValue);
            Console.WriteLine(objects.Count());
            var sw = new Stopwatch();
            sw.Start();
            objects = store.GetObjectsForUser(userId, 0, int.MaxValue);
            Console.WriteLine($"getting all items took {sw.ElapsedMilliseconds}ms.");
        }
    }
}