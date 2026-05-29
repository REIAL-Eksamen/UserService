using MongoDB.Driver;
using UserService.Models;

namespace UserService.Repositories;

/// <summary>
/// MongoDB-implementation af <see cref="IUserRepository"/>.
/// Forbindelsesoplysninger læses fra konfiguration (appsettings / environment variables).
/// </summary>
public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public MongoUserRepository(IConfiguration configuration, ILogger<MongoUserRepository> logger)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];
        var collectionName = configuration["MongoDB:CollectionName"];

        logger.LogInformation("MongoDB Database: {Database}", databaseName);
        logger.LogInformation("MongoDB Collection: {Collection}", collectionName);
        logger.LogInformation("MongoDB ConnectionString: {Conn}", connectionString);

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _users = database.GetCollection<User>(collectionName);
    }

    /// <summary>Henter alle brugere — svarer til SELECT * uden filtrering.</summary>
    public IEnumerable<User> GetAll() =>
        _users.Find(_ => true).ToList();

    public User? GetById(string id) =>
        _users.Find(u => u.Id == id).FirstOrDefault();

    /// <summary>
    /// Finder bruger via AuthService-ID.
    /// Bruges når JWT-claimet <c>NameIdentifier</c> skal omsættes til en UserDB-bruger.
    /// </summary>
    public User? GetByAuthId(string authId) =>
        _users.Find(u => u.AuthId == authId).FirstOrDefault();

    public void Add(User user)
    {
        // Nulstil Id så MongoDB genererer et nyt ObjectId — forhindrer konflikter ved genbrug af objekter
        user.Id = null;
        user.TimeCreated = DateTime.UtcNow;

        _users.InsertOne(user);
    }

    /// <summary>
    /// Erstatter hele dokumentet (ReplaceOne) frem for at opdatere enkeltfelter.
    /// ID'et tvinges inden erstatningen for at sikre konsistens.
    /// </summary>
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