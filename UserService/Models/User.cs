namespace UserService.Models;
public class User
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public RoleType Role { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
    public DateTime TimeCreated { get; set; }
}

public enum RoleType
{
    Admin,
    Instructor,
    Member
}

public enum MembershipStatus
{
    Active,
    Inactive
}