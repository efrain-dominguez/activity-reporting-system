using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ARS.Domain.Enums;

namespace ARS.Domain.Entities
{

    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("assignmentId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AssignmentId { get; set; } = string.Empty;

        [BsonElement("requestId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RequestId { get; set; } = string.Empty;

        [BsonElement("reviewedByUserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReviewedByUserId { get; set; } = string.Empty;

        [BsonElement("decision")]
        [BsonRepresentation(BsonType.String)]
        public ReviewDecision Decision { get; set; }

        [BsonElement("comments")]
        public string Comments { get; set; } = string.Empty;

        [BsonElement("reviewedAt")]
        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}