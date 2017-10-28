using MongoDB.Bson;

namespace PandAPI.PandApi
{
    public class UserModel
    {
        public ObjectId _id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}