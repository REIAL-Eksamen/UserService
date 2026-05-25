using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Models;
// User modelklasse repræsenterer et medlem i FitLife-systemet.
// Klassen bruges både til at gemme brugerdata i MongoDB og til at sende brugerdata via API'et.
public class User
{
    //MongoDB bruger ObjectId som standard. BsonId fortæller mongo,at dette er dokumentets primære id.
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    //authId bruges til at koble brugeren smmen med et login/auth-system
    public string? AuthId { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public MembershipType Membership { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
    public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
}
//membership-type beskriver de medlemskabtyper vi tilbyder.
public enum MembershipType
{
    Student,
    Standard,
    Premium
}
//memberShip-status bruges til at vise, om en bruger har et aktivt medlemskab eller ej.
public enum MembershipStatus
{
    Active,
    Inactive
}