namespace ARS.Application.Services
{

    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        string? Name { get; }
        bool IsAuthenticated { get; }
        IEnumerable<string> Roles { get; }
    }

}