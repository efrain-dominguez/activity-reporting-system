using ARS.Domain.Entities;
using ARS.Domain.Enums;
using ARS.Infrastructure.Data;
using MongoDB.Driver;


namespace ARS.Infrastructure.Repositories
{
    public class TrackingRequestRepository : MongoRepository<TrackingRequest>, ITrackingRequestRepository
    {
        public TrackingRequestRepository(MongoDbContext context) : base(context.TrackingRequests)
        {
        }


        public async Task<IEnumerable<TrackingRequest>> GetByCreatedByUserIdAsync(string userId)
        {
            var filter = Builders<TrackingRequest>.Filter.Eq(e => e.CreatedByUserId, userId);
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<TrackingRequest>> GetByTargetEntityIdAsync(string entityId)
        {
            var filter = Builders<TrackingRequest>.Filter.AnyEq(e => e.TargetEntityIds, entityId);
            return await _collection.Find(filter).SortBy(n => n.DueDate).ToListAsync();
        }

        public async Task<IEnumerable<TrackingRequest>> GetByStatusAsync(RequestStatus status)
        {
            var filter = Builders<TrackingRequest>.Filter.Eq(e => e.Status, status);
            return await _collection.Find(filter).SortBy(n => n.DueDate).ToListAsync();
        }

        public async Task<IEnumerable<TrackingRequest>> GetOverdueRequestsAsync()
        {
            var filter = Builders<TrackingRequest>.Filter.Or(
                // Caso 1: Ya fue marcada como Overdue manualmente
                Builders<TrackingRequest>.Filter.Eq(tr => tr.Status, RequestStatus.Overdue),

                // Caso 2: Fecha vencida pero aún no marcada (y no está completada)
                Builders<TrackingRequest>.Filter.And(
                    Builders<TrackingRequest>.Filter.Lt(tr => tr.DueDate, DateTime.UtcNow),
                    Builders<TrackingRequest>.Filter.Ne(tr => tr.Status, RequestStatus.Completed)
                )
            );
            
            return await _collection.Find(filter).SortBy(n => n.DueDate).ToListAsync();
        }

        public async Task<IEnumerable<TrackingRequest>> GetActiveRequestsAsync()
        {
            var excluded = new[] { RequestStatus.Completed, RequestStatus.Overdue };
            var filter = Builders<TrackingRequest>.Filter.Nin(e => e.Status, excluded);

            return await _collection.Find(filter).SortBy(n => n.DueDate).ToListAsync();
        }

        public async Task<bool> MarkAsOverdueAsync(string requestId)
        {
            var request = await GetByIdAsync(requestId);
            if (request == null || request.Status == RequestStatus.Completed)
                return false;

            request.Status = RequestStatus.Overdue;
            request.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(requestId, request);
        }

        public async Task<bool> MarkAsCompletedAsync(string requestId)
        {
            var request = await GetByIdAsync(requestId);
            if (request == null || request.Status == RequestStatus.Overdue)
                return false;

            request.Status = RequestStatus.Completed;
            request.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(requestId, request);
        }



    }
}
