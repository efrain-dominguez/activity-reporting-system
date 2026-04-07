using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ARS.Domain.Entities
{

    public class Activity
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

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("activityDate")]
        public DateTime ActivityDate { get; set; }

        [BsonElement("submittedByUserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SubmittedByUserId { get; set; } = string.Empty;

        [BsonElement("files")]
        public List<ActivityFile> Files { get; set; } = new();

        [BsonElement("isEditable")]
        public bool IsEditable { get; set; } = true;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("submittedAt")]
        public DateTime? SubmittedAt { get; set; }

        // Business logic method
        public void Submit()
        {
            if (!IsEditable)
                throw new InvalidOperationException("Activity has already been submitted and cannot be modified.");

            IsEditable = false;
            SubmittedAt = DateTime.UtcNow;
        }
    }

    public class ActivityFile
    {
        [BsonElement("fileId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FileId { get; set; } = string.Empty;

        [BsonElement("fileName")]
        public string FileName { get; set; } = string.Empty;

        [BsonElement("fileType")]
        public string FileType { get; set; } = string.Empty;

        [BsonElement("fileSizeBytes")]
        public long FileSizeBytes { get; set; }

        [BsonElement("blobUrl")]
        public string BlobUrl { get; set; } = string.Empty;

        [BsonElement("uploadedAt")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}