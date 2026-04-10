using ARS.Domain.Entities;

namespace ARS.Infrastructure.Repositories
{

    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEntraIdAsync(string entraObjectId);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task<IEnumerable<User>> GetByEntityIdAsync(string entityId);
        Task<IEnumerable<User>> GetActiveUsersAsync();
    }
}