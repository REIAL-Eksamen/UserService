namespace UserService.DTOs;
using UserService.Models;

/// <summary>
/// Data der kan opdateres på en eksisterende bruger.
/// AuthId og TimeCreated er udeladt — de må ikke ændres efter oprettelse.
/// </summary>
public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public MembershipType Membership { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
}