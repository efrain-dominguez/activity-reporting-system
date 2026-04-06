using ARS.Domain.Enums;

namespace ARS.Application.DTOs.RequestAssignments
{

    public class UpdateRequestAssignmentDto
    {
        public AssignmentStatus Status { get; set; }
        public string? NoProgressReason { get; set; }
        public bool ExtensionRequested { get; set; }
    }

}