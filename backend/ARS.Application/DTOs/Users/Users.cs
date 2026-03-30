using ARS.Domain.Enums;

namespace ARS.Application.DTOs.Users
{

    public class CreateUserDto
    {
        public string EntraObjectId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}