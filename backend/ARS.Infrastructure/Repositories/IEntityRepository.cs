using ARS.Domain.Entities;

namespace ARS.Infrastructure.Repositories
{
    public interface IEntityRepository : IRepository<Entity>
    {
        //buscar por codigo/clave de entidad
        Task<Entity?> GetByCodeAsync(string code);
        Task<IEnumerable<Entity>> GetByParentEntityIdAsync(string parentEntityId);
        Task<IEnumerable<Entity>> GetActiveEntitiesAsync();
    }
}