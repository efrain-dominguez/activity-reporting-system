namespace ARS.Application.DTOs.RequestAssignments
{

    public class CreateRequestAssignmentDto
    {
        public string RequestId { get; set; } = string.Empty;
        public string AssignedToEntityId { get; set; } = string.Empty;
        public string? AssignedToUserId { get; set; }
        public string? DelegatedFromUserId { get; set; }
    }

}