using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.DTOs; // ← tilføjet

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private static readonly List<User> Users = new()
    {
        new User
        {
            UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FirstName = "Enni",
            LastName = "Korhonen",
            Email = "enni@example.com",
            PhoneNumber = "12345678",
            Role = RoleType.Member,
            MembershipStatus = MembershipStatus.Active,
            TimeCreated = DateTime.UtcNow
        },
        new User
        {
            UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FirstName = "Eman",
            LastName = "Habash",
            Email = "eman@example.com",
            PhoneNumber = "87654321",
            Role = RoleType.Admin,
            MembershipStatus = MembershipStatus.Active,
            TimeCreated = DateTime.UtcNow
        }
    };

    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    // ← tilføjet hjælpemetode
    private static UserResponseDto MapToDto(User u) => new()
    {
        UserId = u.UserId,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        PhoneNumber = u.PhoneNumber,
        Role = u.Role.ToString(),
        MembershipStatus = u.MembershipStatus.ToString(),
        TimeCreated = u.TimeCreated
    };

    [HttpGet(Name = "GetUsers")]
    public IEnumerable<UserResponseDto> Get() // ← User → UserResponseDto
    {
        return Users.Select(MapToDto);
    }

    [HttpGet("{userId}", Name = "GetUserById")]
    public ActionResult<UserResponseDto> GetById(Guid userId) // ← User → UserResponseDto
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(user)); // ← MapToDto tilføjet
    }

    [HttpGet("{userId}/membership-status", Name = "GetMembershipStatus")]
    public ActionResult<object> GetMembershipStatus(Guid userId) // ← uændret
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            userId = user.UserId,
            membershipStatus = user.MembershipStatus.ToString(),
            isActive = user.MembershipStatus == MembershipStatus.Active
        });
    }

    [HttpPost(Name = "CreateUser")]
    public ActionResult<UserResponseDto> Create(CreateUserDto dto) // ← User → CreateUserDto
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
            MembershipStatus = dto.MembershipStatus,
            TimeCreated = DateTime.UtcNow
        };

        Users.Add(user);
        return CreatedAtRoute("GetUserById", new { userId = user.UserId }, MapToDto(user));
    }

    [HttpPut("{userId}", Name = "UpdateUser")]
    public IActionResult Update(Guid userId, UpdateUserDto dto) // ← User → UpdateUserDto
    {
        var existingUser = Users.FirstOrDefault(u => u.UserId == userId);

        if (existingUser is null)
        {
            return NotFound();
        }

        existingUser.FirstName = dto.FirstName;
        existingUser.LastName = dto.LastName;
        existingUser.Email = dto.Email;
        existingUser.PhoneNumber = dto.PhoneNumber;
        existingUser.Role = dto.Role;
        existingUser.MembershipStatus = dto.MembershipStatus;

        return NoContent();
    }
}