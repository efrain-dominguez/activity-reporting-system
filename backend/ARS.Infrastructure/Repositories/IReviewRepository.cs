using ARS.Domain.Entities;
using ARS.Domain.Enums;

namespace ARS.Infrastructure.Repositories{

    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByAssignmentIdAsync(string assignmentId);
        Task<IEnumerable<Review>> GetByRequestIdAsync(string requestId);
        Task<IEnumerable<Review>> GetByReviewerIdAsync(string reviewerId);
        Task<Review?> GetByAssignmentIdAndReviewerAsync(string assignmentId, string reviewerId);
    }
}