using MongoDB.Driver;
using ARS.Domain.Entities;
using Microsoft.Extensions.Options;

namespace ARS.Infrastructure.Data
{

    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Collections
        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Entity> Entities => _database.GetCollection<Entity>("entities");
        public IMongoCollection<TrackingRequest> TrackingRequests => _database.GetCollection<TrackingRequest>("trackingRequests");
        public IMongoCollection<RequestAssignment> RequestAssignments => _database.GetCollection<RequestAssignment>("requestAssignments");
        public IMongoCollection<Activity> Activities => _database.GetCollection<Activity>("activities");
        public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("reviews");
        public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("notifications");
    }
}