using ARS.Domain.Entities;
using ARS.Domain.Enums;

namespace ARS.Infrastructure.Repositories
{

    public interface ITrackingRequestRepository : IRepository<TrackingRequest>
    {
        Task<IEnumerable<TrackingRequest>> GetByCreatedByUserIdAsync(string userId);
        Task<IEnumerable<TrackingRequest>> GetByTargetEntityIdAsync(string entityId);
        Task<IEnumerable<TrackingRequest>> GetByStatusAsync(RequestStatus status);
        Task<IEnumerable<TrackingRequest>> GetOverdueRequestsAsync();
        Task<IEnumerable<TrackingRequest>> GetActiveRequestsAsync();
        Task<bool> MarkAsOverdueAsync(string requestId);

        Task<bool> MarkAsCompletedAsync(string requestId);
    }
}