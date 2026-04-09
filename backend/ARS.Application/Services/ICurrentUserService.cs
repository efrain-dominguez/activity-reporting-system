namespace ARS.Application.Services
{

    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        string? Name { get; }
        string? FirstName { get; } 
        string? LastName { get; } 
        bool IsAuthenticated { get; }
        IEnumerable<string> Roles { get; }
    }

}