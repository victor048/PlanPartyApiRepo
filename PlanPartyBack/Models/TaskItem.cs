using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlanPartyBack.Models
{
    public class TaskItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }
        public string TaskName { get; set; }
        public int DurationInMinutes { get; set; }
        public string Status { get; set; }
        public int ElapsedMinutes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
