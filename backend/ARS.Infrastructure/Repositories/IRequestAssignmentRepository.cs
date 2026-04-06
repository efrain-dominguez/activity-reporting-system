using ARS.Domain.Entities;
using ARS.Domain.Enums;

namespace ARS.Infrastructure.Repositories
{

    public interface IRequestAssignmentRepository : IRepository<RequestAssignment>
    {
        Task<IEnumerable<RequestAssignment>> GetByRequestIdAsync(string requestId);
        Task<IEnumerable<RequestAssignment>> GetByAssignedToEntityIdAsync(string entityId);
        Task<IEnumerable<RequestAssignment>> GetByAssignedToUserIdAsync(string userId);
        Task<IEnumerable<RequestAssignment>> GetByDelegatedFromUserIdAsync(string userId);
        Task<IEnumerable<RequestAssignment>> GetByStatusAsync(AssignmentStatus status);
        Task<RequestAssignment?> GetByRequestAndEntityAsync(string requestId, string entityId);
        Task<IEnumerable<RequestAssignment>> GetPendingAssignmentsAsync();
        Task<IEnumerable<RequestAssignment>> GetSubmittedAssignmentsAsync();
        Task<IEnumerable<RequestAssignment>> GetExtensionRequestsAsync();
    }

}