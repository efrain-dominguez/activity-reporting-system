using ARS.Domain.Entities;
using ARS.Infrastructure.Data;
using MongoDB.Driver;

namespace ARS.Infrastructure.Repositories
{
    public class ReviewRepository : MongoRepository<Review>, IReviewRepository
    {
        public ReviewRepository(MongoDbContext context) : base(context.Reviews)
        {
        }


        public async Task<IEnumerable<Review>> GetByAssignmentIdAsync(string assignmentId)
        {
            var filter = Builders<Review>.Filter.Eq(e => e.AssignmentId, assignmentId);
            return await _collection.Find(filter).SortByDescending(n => n.ReviewedAt).ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByRequestIdAsync(string requestId)
        {
            var filter = Builders<Review>.Filter.Eq(e => e.RequestId, requestId);
            return await _collection.Find(filter).SortByDescending(n => n.ReviewedAt).ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByReviewerIdAsync(string reviewerId)
        {
            var filter = Builders<Review>.Filter.Eq(e => e.ReviewedByUserId, reviewerId);
            return await _collection.Find(filter).SortByDescending(n => n.ReviewedAt).ToListAsync();
        }

         public async Task<Review?> GetByAssignmentIdAndReviewerAsync(string assignmentId, string reviewerId)
        {
            var filter =   Builders<Review>.Filter.And(
                    Builders<Review>.Filter.Eq(tr => tr.AssignmentId, assignmentId),
                    Builders<Review>.Filter.Eq(tr => tr.ReviewedByUserId, reviewerId)
            );
            
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}