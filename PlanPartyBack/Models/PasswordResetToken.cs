using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PlanPartyBack.Models
{
    public class PasswordResetToken
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]
        public ObjectId UserId { get; set; }

        [BsonElement("token")]
        public string Token { get; set; }

        [BsonElement("expirationDate")]
        public DateTime ExpirationDate { get; set; }
    }
}
