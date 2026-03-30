using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ARS.Domain.Entities
{

    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("relatedEntityType")]
        public string? RelatedEntityType { get; set; }

        [BsonElement("relatedEntityId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? RelatedEntityId { get; set; }

        [BsonElement("isRead")]
        public bool IsRead { get; set; } = false;

        [BsonElement("sentViaEmail")]
        public bool SentViaEmail { get; set; } = false;

        [BsonElement("emailSentAt")]
        public DateTime? EmailSentAt { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Business logic method
        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}