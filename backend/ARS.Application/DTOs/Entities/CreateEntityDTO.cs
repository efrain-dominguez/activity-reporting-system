namespace ARS.Application.DTOs.Entities
{

    public class CreateEntityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ParentEntityId { get; set; }
        public bool IsActive { get; set; } = true;
    }

}