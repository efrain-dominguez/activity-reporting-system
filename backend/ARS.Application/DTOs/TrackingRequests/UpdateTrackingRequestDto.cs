

namespace ARS.Application.DTOs.TrackingRequests
{ 

    public class UpdateTrackingRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GoalType { get; set; } = string.Empty;
        public List<string> TargetEntityIds { get; set; } = new();
        public DateTime DueDate { get; set; }
        public string? Frequency { get; set; }
    }

}