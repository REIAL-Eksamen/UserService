namespace UserService.Models;

public class User
{
    public Guid UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public MembershipType Membership { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
    public DateTime TimeCreated { get; set; }
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