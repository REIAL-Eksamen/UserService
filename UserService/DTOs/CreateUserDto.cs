namespace UserService.DTOs;
using UserService.Models;

public class CreateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public RoleType Role { get; set; }
    public MembershipStatus MembershipStatus { get; set; }
}