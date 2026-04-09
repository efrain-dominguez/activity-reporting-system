using ARS.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ARS.Infrastructure.Services
{

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")  // ← FIXED
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("oid")
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.NameIdentifier)
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("sub");

        public string? Email => _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")  // ← FIXED
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.Upn)
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("upn")
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("unique_name")
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("email")
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.Email);

        public string? Name => _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("name")
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.Name)
            ?? BuildNameFromParts();

        public string? FirstName => _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")  // ← FIXED
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.GivenName)
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("given_name")
            ?? ExtractFirstName(Name ?? "Unknown");

        public string? LastName => _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")  // ← FIXED
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue(ClaimTypes.Surname)
            ?? _httpContextAccessor.HttpContext?.User?
            .FindFirstValue("family_name")
            ?? ExtractLastName(Name ?? "User");

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?
            .FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")  // ← FIXED
            .Select(c => c.Value)
            .Union(_httpContextAccessor.HttpContext?.User?
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value) ?? Enumerable.Empty<string>())
            .Union(_httpContextAccessor.HttpContext?.User?
                .FindAll("roles")
                .Select(c => c.Value) ?? Enumerable.Empty<string>())
            ?? Enumerable.Empty<string>();

        private string? BuildNameFromParts()
        {
            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                return $"{FirstName} {LastName}";

            return FirstName ?? LastName;
        }

        private string ExtractFirstName(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : "Unknown";
        }

        private string ExtractLastName(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "User";
        }
    }
}