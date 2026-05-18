using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserRepository userRepository, ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet(Name = "GetUsers")]
    public IEnumerable<User> Get()
    {
        return _userRepository.GetAll();
    }

    [HttpGet("{userId}", Name = "GetUserById")]
    public ActionResult<User> GetById(Guid userId)
    {
        var user = _userRepository.GetById(userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{userId}/membership-status", Name = "GetMembershipStatus")]
    public ActionResult<object> GetMembershipStatus(Guid userId)
    {
        var user = _userRepository.GetById(userId);

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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (user.UserId == Guid.Empty)
        {
            user.UserId = Guid.NewGuid();
        }

        user.TimeCreated = DateTime.UtcNow;

        _userRepository.Add(user);

        return CreatedAtRoute("GetUserById", new { userId = user.UserId }, user);
    }

    [HttpPut("{userId}", Name = "UpdateUser")]
    public IActionResult Update(Guid userId, [FromBody] User updatedUser)
    {
        var updated = _userRepository.Update(userId, updatedUser);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
    
}