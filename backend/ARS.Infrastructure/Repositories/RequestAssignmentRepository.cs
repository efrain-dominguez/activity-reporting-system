
using ARS.Domain.Entities;
using ARS.Domain.Enums;
using ARS.Infrastructure.Data;
using MongoDB.Driver;

namespace ARS.Infrastructure.Repositories
{
    public class RequestAssignmentRepository : MongoRepository<RequestAssignment>, IRequestAssignmentRepository
    {
        public RequestAssignmentRepository(MongoDbContext context) : base(context.RequestAssignments)
        {
        }

        public async Task<IEnumerable<RequestAssignment>> GetByRequestIdAsync(string requestId)
        {
            var filter = Builders<RequestAssignment>.Filter.Eq(e => e.RequestId, requestId);
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }


        public async Task<IEnumerable<RequestAssignment>> GetByAssignedToEntityIdAsync(string entityId)
        {
            var filter = Builders<RequestAssignment>.Filter.Eq(e => e.AssignedToEntityId, entityId);
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<RequestAssignment>> GetByAssignedToUserIdAsync(string userId)
        {
            var filter = Builders<RequestAssignment>.Filter.Eq(e => e.AssignedToUserId, userId);
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<RequestAssignment>> GetByDelegatedFromUserIdAsync(string userId)
        {
            var filter = Builders<RequestAssignment>.Filter.Eq(e => e.DelegatedFromUserId, userId);
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }
        public async Task<IEnumerable<RequestAssignment>> GetByStatusAsync(AssignmentStatus status)
        {
            var filter = Builders<RequestAssignment>.Filter.Eq(e => e.Status, status);
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<RequestAssignment?> GetByRequestAndEntityAsync(string requestId, string entityId)
        {
            var filter = Builders<RequestAssignment>.Filter.And(
                      Builders<RequestAssignment>.Filter.Eq(tr => tr.RequestId, requestId),
                      Builders<RequestAssignment>.Filter.Eq(tr => tr.AssignedToEntityId, entityId)
             );

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RequestAssignment>> GetPendingAssignmentsAsync()
        {
            var filter = Builders<RequestAssignment>.Filter.Eq(e => e.Status, AssignmentStatus.Pending);
            return await _collection.Find(filter).SortBy(n => n.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<RequestAssignment>> GetSubmittedAssignmentsAsync()
        {
            var filter = Builders<RequestAssignment>.Filter.Eq(e => e.Status, AssignmentStatus.Submitted);
            return await _collection.Find(filter).SortByDescending(n => n.SubmittedAt).ToListAsync();
        }

        public async Task<IEnumerable<RequestAssignment>> GetExtensionRequestsAsync()
        {
            var filter = Builders<RequestAssignment>.Filter.And(
                Builders<RequestAssignment>.Filter.Eq(ra => ra.ExtensionRequested, true),
                Builders<RequestAssignment>.Filter.Eq(ra => ra.ExtensionGranted, false)
            );

            return await _collection.Find(filter).SortBy(ra => ra.CreatedAt).ToListAsync();
        }


    }
}

