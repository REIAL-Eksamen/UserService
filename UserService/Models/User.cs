using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public MembershipType Membership { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
    public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
}

public enum MembershipType
{
    Student,
    Standard,
    Premium
}

public enum MembershipStatus
{
    Active,
    Inactive
}