namespace ARS.Application.DTOs.Entities { 


    public class UpdateEntityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ParentEntityId { get; set; }
        public bool IsActive { get; set; }
    }

}