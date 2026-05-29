namespace UserService.DTOs;
using UserService.Models;

/// <summary>
/// Data der kræves for at oprette en ny brugerprofil i UserDB.
/// Modtages enten direkte via HTTP POST eller indirekte via <c>UserRegisteredEvent</c> fra RabbitMQ.
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// ID fra AuthService — gør det muligt at slå brugeren op via JWT-claims uden at kende MongoDB-ID'et.
    /// </summary>
    public string? AuthId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public MembershipType Membership { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
}