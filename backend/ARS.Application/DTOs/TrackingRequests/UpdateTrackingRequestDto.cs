using ARS.Domain.Enums; 
namespace ARS.Application.DTOs.TrackingRequests{

    public class UpdateTrackingRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GoalType { get; set; } = string.Empty;
        public List<string> TargetEntityIds { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Frequency { get; set; }
        public bool IsRecurring { get; set; }
        public RequestStatus Status { get; set; } 
    }
}