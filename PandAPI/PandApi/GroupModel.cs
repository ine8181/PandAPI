using MongoDB.Bson;

namespace PandAPI.PandApi
{
    public class GroupModel
    {
        public ObjectId _id { get; set; }
        public string GroupId { get; set; }
    }
}