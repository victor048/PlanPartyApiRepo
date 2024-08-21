using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace PlanPartyBack.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("whatsApp")]
        public string WhatsApp { get; set; }

        [BsonElement("recoveryOption")]
        public List<string> RecoveryOption { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
