using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Models;

/// <summary>
/// Repræsenterer et medlem i FitLife-systemet.
/// Gemmes i UserDB og returneres direkte via API'et.
/// </summary>
public class User
{
    /// <summary>
    /// MongoDB ObjectId — sættes automatisk ved indsættelse og er null indtil da.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// ID fra AuthService — bruges til at koble brugerprofilen sammen med login-oplysninger.
    /// Sættes når brugeren oprettes via <c>UserRegisteredEvent</c> fra RabbitMQ.
    /// </summary>
    public string? AuthId { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public MembershipType Membership { get; set; }
    public MembershipStatus MembershipStatus { get; set; }

    /// <summary>Sættes automatisk til UTC-tidspunkt ved oprettelse.</summary>
    public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
}

/// <summary>De medlemskabstyper FitLife tilbyder.</summary>
public enum MembershipType
{
    Student,
    Standard,
    Premium
}

/// <summary>Angiver om et medlemskab er aktivt og giver adgang til systemet.</summary>
public enum MembershipStatus
{
    Active,
    Inactive
}