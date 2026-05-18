namespace UserService.DTOs;


public class UserResponseDto
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public string? MembershipStatus { get; set; }
    public DateTime TimeCreated { get; set; }
}