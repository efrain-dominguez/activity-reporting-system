using ARS.Domain.Entities;
using ARS.Infrastructure.Data;
using MongoDB.Driver;

namespace ARS.Infrastructure.Repositories
{

    public class ActivityRepository : MongoRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(MongoDbContext context)
            : base(context.Activities)
        {
        }

        public async Task<IEnumerable<Activity>> GetByAssignmentIdAsync(string assignmentId)
        {
            var filter = Builders<Activity>.Filter.Eq(a => a.AssignmentId, assignmentId);
            return await _collection.Find(filter)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetByRequestIdAsync(string requestId)
        {
            var filter = Builders<Activity>.Filter.Eq(a => a.RequestId, requestId);
            return await _collection.Find(filter)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetBySubmittedByUserIdAsync(string userId)
        {
            var filter = Builders<Activity>.Filter.Eq(a => a.SubmittedByUserId, userId);
            return await _collection.Find(filter)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Activity>> GetEditableActivitiesAsync()
        {
            var filter = Builders<Activity>.Filter.Eq(a => a.IsEditable, true);
            return await _collection.Find(filter)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Activity?> GetByIdWithFilesAsync(string id)
        {
            return await GetByIdAsync(id);
        }
    }
}