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

    public User? GetById(string id) =>
        _users.Find(u => u.Id == id).FirstOrDefault();
    
    public User? GetByAuthId(string authId) =>
        _users.Find(u => u.AuthId == authId).FirstOrDefault();

    public void Add(User user)
    {
        user.Id = null; // MongoDB creates _id automatically
        user.TimeCreated = DateTime.UtcNow;

        _users.InsertOne(user);
    }

    public bool Update(string id, User updatedUser)
    {
        updatedUser.Id = id;

        var result = _users.ReplaceOne(u => u.Id == id, updatedUser);
        return result.ModifiedCount > 0;
    }

    public bool Delete(string id)
    {
        var result = _users.DeleteOne(u => u.Id == id);
        return result.DeletedCount > 0;
    }
}