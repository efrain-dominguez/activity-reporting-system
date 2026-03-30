using ARS.Domain.Entities;
using ARS.Infrastructure.Data;
using MongoDB.Driver;

namespace ARS.Infrastructure.Repositories
{

    public class EntityRepository : MongoRepository<Entity>, IEntityRepository
    {
        public EntityRepository(MongoDbContext context): base(context.Entities)
        {
        }

        public async Task<Entity?> GetByCodeAsync(string code)
        {
            var filter = Builders<Entity>.Filter.Eq(e => e.Code, code);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Entity>> GetByParentEntityIdAsync(string parentEntityId)
        {
            var filter = Builders<Entity>.Filter.Eq(e => e.ParentEntityId, parentEntityId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Entity>> GetActiveEntitiesAsync()
        {
            var filter = Builders<Entity>.Filter.Eq(e => e.IsActive, true);
            return await _collection.Find(filter).ToListAsync();
        }

   
    }
}