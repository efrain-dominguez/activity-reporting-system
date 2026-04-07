using ARS.Domain.Entities;

namespace ARS.Infrastructure.Repositories
{

    public interface IActivityRepository : IRepository<Activity>
    {
        Task<IEnumerable<Activity>> GetByAssignmentIdAsync(string assignmentId);
        Task<IEnumerable<Activity>> GetByRequestIdAsync(string requestId);
        Task<IEnumerable<Activity>> GetBySubmittedByUserIdAsync(string userId);
        Task<IEnumerable<Activity>> GetEditableActivitiesAsync();
        Task<Activity?> GetByIdWithFilesAsync(string id);
    }
}