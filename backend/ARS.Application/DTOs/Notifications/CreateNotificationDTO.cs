namespace ARS.Application.DTOs.Notifications
{

    public class CreateNotificationDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? RelatedEntityType { get; set; }
        public string? RelatedEntityId { get; set; }
    }
}