using ARS.Application.Services;
using ARS.Domain.Entities;
using ARS.Domain.Enums;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ARS.API.Controllers
{

    [ApiController]
    [Authorize]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ICurrentUserService CurrentUserService;
        protected readonly IUserRepository UserRepository;

        protected BaseApiController(
            ICurrentUserService currentUserService,
            IUserRepository userRepository)
        {
            CurrentUserService = currentUserService;
            UserRepository = userRepository;
        }

        /// <summary>
        /// Gets the current authenticated user's MongoDB ID.
        /// Auto-syncs the user from Azure AD if not exists in database.
        /// </summary>
        protected async Task<string> GetCurrentUserIdAsync()
        {
            var azureObjectId = CurrentUserService.UserId
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var user = await UserRepository.GetByEntraIdAsync(azureObjectId);

            if (user == null)
            {
                user = await SyncUserFromAzureAdAsync(azureObjectId);
            }

            return user.Id;
        }

        /// <summary>
        /// Gets the current authenticated user entity.
        /// Auto-syncs from Azure AD if not exists in database.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var azureObjectId = CurrentUserService.UserId
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var user = await UserRepository.GetByEntraIdAsync(azureObjectId);

            if (user == null)
            {
                user = await SyncUserFromAzureAdAsync(azureObjectId);
            }

            return user;
        }

        /// <summary>
        /// Syncs a user from Azure AD to MongoDB.
        /// </summary>
        private async Task<User> SyncUserFromAzureAdAsync(string azureObjectId)
        {
            var user = new User
            {
                EntraObjectId = azureObjectId,
                Email = CurrentUserService.Email ?? "unknown@domain.com",
                FirstName = ExtractFirstName(CurrentUserService.Name ?? "Unknown"),
                LastName = ExtractLastName(CurrentUserService.Name ?? "User"),
                Role = DetermineUserRole(CurrentUserService.Roles),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await UserRepository.CreateAsync(user);
        }

        /// <summary>
        /// Extracts first name from full name.
        /// </summary>
        private string ExtractFirstName(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : "Unknown";
        }

        /// <summary>
        /// Extracts last name from full name.
        /// </summary>
        private string ExtractLastName(string fullName)
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "User";
        }

        /// <summary>
        /// Determines user role from Azure AD roles.
        /// Maps Azure AD roles to application UserRole enum.
        /// </summary>
        private UserRole DetermineUserRole(IEnumerable<string> roles)
        {
            if (roles.Contains("Admin")) return UserRole.Admin;
            if (roles.Contains("PMO")) return UserRole.PMO;
            if (roles.Contains("Entity")) return UserRole.Entity;
            if (roles.Contains("Directorate")) return UserRole.Directorate;

            return UserRole.Team; // Default role
        }

        /// <summary>
        /// Checks if current user has a specific role.
        /// </summary>
        protected bool HasRole(UserRole role)
        {
            return CurrentUserService.Roles.Contains(role.ToString());
        }

        /// <summary>
        /// Ensures current user has required role, throws if not.
        /// </summary>
        protected async Task<User> RequireRoleAsync(UserRole role)
        {
            var user = await GetCurrentUserAsync();

            if (user.Role != role)
                throw new UnauthorizedAccessException($"This action requires {role} role");

            return user;
        }
    }
}