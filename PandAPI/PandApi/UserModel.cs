using MongoDB.Bson;

namespace PandAPI.PandApi
{
    public class UserModel
    {
        public ObjectId _id { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}