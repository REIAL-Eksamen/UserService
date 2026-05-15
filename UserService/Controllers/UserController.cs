using Microsoft.AspNetCore.Mvc;
using UserService.Models;
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


    [HttpGet(Name = "GetUsers")]
    public IEnumerable<User> Get()
    {
        return Users;
    }

    [HttpGet("{userId}", Name = "GetUserById")]
    public ActionResult<User> GetById(Guid userId)
    {
        var user = Users.FirstOrDefault(u => u.UserId == userId);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpGet("{userId}/membership-status", Name = "GetMembershipStatus")]
    public ActionResult<object> GetMembershipStatus(Guid userId)
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
    public ActionResult<User> Create(User user)
    {
        if (user.UserId == Guid.Empty)
        {
            user.UserId = Guid.NewGuid();
        }

        user.TimeCreated = DateTime.UtcNow;
        Users.Add(user);

        return CreatedAtRoute("GetUserById", new { userId = user.UserId }, user);
    }
    
    [HttpPut("{userId}", Name = "UpdateUser")]
    public IActionResult Update(Guid userId, User updatedUser)
    {
        var existingUser = Users.FirstOrDefault(u => u.UserId == userId);

        if (existingUser is null)
        {
            return NotFound();
        }

        existingUser.FirstName = updatedUser.FirstName;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Email = updatedUser.Email;
        existingUser.PhoneNumber = updatedUser.PhoneNumber;
        existingUser.Role = updatedUser.Role;
        existingUser.MembershipStatus = updatedUser.MembershipStatus;

        return NoContent();
    }
}
