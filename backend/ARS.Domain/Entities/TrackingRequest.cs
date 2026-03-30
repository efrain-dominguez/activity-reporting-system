using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ARS.Domain.Enums;

namespace ARS.Domain.Entities
{

    public class TrackingRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("goalType")]
        public string GoalType { get; set; } = string.Empty;

        [BsonElement("createdByUserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CreatedByUserId { get; set; } = string.Empty;

        [BsonElement("targetEntityIds")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> TargetEntityIds { get; set; } = new();

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("dueDate")]
        public DateTime DueDate { get; set; }

        [BsonElement("frequency")]
        public string? Frequency { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public RequestStatus Status { get; set; } = RequestStatus.Draft;

        [BsonElement("isRecurring")]
        public bool IsRecurring { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Business logic method
        public bool IsOverdue()
        {
            return DateTime.UtcNow > DueDate && Status != RequestStatus.Completed;
        }
    }
}