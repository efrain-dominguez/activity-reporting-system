using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ARS.Domain.Enums;

namespace ARS.Domain.Entities
{

    public class RequestAssignment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("requestId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RequestId { get; set; } = string.Empty;

        [BsonElement("assignedToEntityId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AssignedToEntityId { get; set; } = string.Empty;

        [BsonElement("assignedToUserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AssignedToUserId { get; set; }

        [BsonElement("assignedByUserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AssignedByUserId { get; set; } = string.Empty;

        [BsonElement("delegatedFromUserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? DelegatedFromUserId { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;

        [BsonElement("noProgressReason")]
        public string? NoProgressReason { get; set; }

        [BsonElement("extensionRequested")]
        public bool ExtensionRequested { get; set; } = false;

        [BsonElement("extensionRequestedDate")]
        public DateTime? ExtensionRequestedDate { get; set; }

        [BsonElement("extensionGranted")]
        public bool ExtensionGranted { get; set; } = false;

        [BsonElement("extensionGrantedDate")]
        public DateTime? ExtensionGrantedDate { get; set; }

        [BsonElement("newDueDate")]
        public DateTime? NewDueDate { get; set; }

        [BsonElement("submittedAt")]
        public DateTime? SubmittedAt { get; set; }

        [BsonElement("reviewedAt")]
        public DateTime? ReviewedAt { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}