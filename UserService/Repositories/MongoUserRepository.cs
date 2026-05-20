using MongoDB.Driver;
using UserService.Models;

namespace UserService.Repositories;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public MongoUserRepository(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];
        var collectionName = configuration["MongoDB:CollectionName"];

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _users = database.GetCollection<User>(collectionName);
    }

    public IEnumerable<User> GetAll() =>
        _users.Find(_ => true).ToList();

    public User? GetById(Guid userId) =>
        _users.Find(u => u.UserId == userId).FirstOrDefault();

    public void Add(User user) =>
        _users.InsertOne(user);

    public bool Update(Guid userId, User updatedUser)
    {
        var result = _users.ReplaceOne(u => u.UserId == userId, updatedUser);
        return result.ModifiedCount > 0;
    }

    public bool Delete(Guid userId)
    {
        var result = _users.DeleteOne(u => u.UserId == userId);
        return result.DeletedCount > 0;
    }
}