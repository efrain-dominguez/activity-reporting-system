namespace ARS.Application.DTOs.Activities
{

    public class CreateActivityDto
    {
        public string AssignmentId { get; set; } = string.Empty;
        public string RequestId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        // Los archivos se manejan por separado (upload)
    }

}