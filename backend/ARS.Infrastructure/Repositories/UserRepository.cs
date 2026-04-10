using ARS.Domain.Entities;
using ARS.Infrastructure.Data;
using MongoDB.Driver;

namespace ARS.Infrastructure.Repositories
{
    public class UserRepository : MongoRepository<User>, IUserRepository
    {
        public UserRepository(MongoDbContext context)
            : base(context.Users)
        {
        }

        public async Task<User?> GetByEntraIdAsync(string entraObjectId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.EntraObjectId, entraObjectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            var filter = Builders<User>.Filter.Eq("role", role);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByEntityIdAsync(string entityId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.EntityId, entityId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            var filter = Builders<User>.Filter.Eq(u => u.IsActive, true);
            return await _collection.Find(filter)
                .SortBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();
        }
    }
}