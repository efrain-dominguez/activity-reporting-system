using ARS.Domain.Entities;
using ARS.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ARS.Infrastructure.Repositories
{
    public class NotificationRepository : MongoRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(MongoDbContext context) : base(context.Notifications)
        {
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<Notification>.Filter.Eq(e => e.UserId, userId);
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId)
        {
            var filter = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(n => n.UserId, userId),
                Builders<Notification>.Filter.Eq(n => n.IsRead, false)
            );
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }


        public async Task<IEnumerable<Notification>> GetByNotificationIdAsync(string notificationId)
        {
            var filter = Builders<Notification>.Filter.Eq(e => e.Id, notificationId);
            return await _collection.Find(filter).SortByDescending(n => n.CreatedAt).ToListAsync();
        }


        public async Task<bool> MarkAsReadAsync(string notificationId)
        {
            // Opción 1: Directo con MongoDB (más eficiente)
            var filter = Builders<Notification>.Filter.Eq(n => n.Id, notificationId);
            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;

            // Opción 2: Usando lógica de dominio (más "Clean Architecture")
            /*
            var notification = await GetByIdAsync(notificationId);

            if (notification == null)
                return false;

            notification.MarkAsRead();
            return await UpdateAsync(notificationId, notification);
            */

        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var filter = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(n => n.UserId, userId),
                Builders<Notification>.Filter.Eq(n => n.IsRead, false)
            );

            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);

            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            var filter = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(n => n.UserId, userId),
                Builders<Notification>.Filter.Eq(n => n.IsRead, false)
            );
            var count = await _collection.CountDocumentsAsync(filter);

            return (int)count;
        }
    }
}
