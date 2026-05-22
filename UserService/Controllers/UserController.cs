using UserService.Models;
using Microsoft.AspNetCore.Mvc;
using UserService.Repositories;
using UserService.DTOs;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
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
    public ActionResult<User> GetById(string userId)
    {
        var user = _userRepository.GetById(userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{userId}/membership-status", Name = "GetMembershipStatus")]
    public ActionResult<object> GetMembershipStatus(string userId)
    {
        var user = _userRepository.GetById(userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            userId = user.Id,
            membershipStatus = user.MembershipStatus.ToString(),
            isActive = user.MembershipStatus == MembershipStatus.Active
        });
    }

    [HttpGet("{userId}/membership", Name = "GetMembership")]
    public ActionResult<object> GetMembership(string userId)
    {
        var user = _userRepository.GetById(userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            userId = user.Id,
            membership = user.Membership.ToString(),
            membershipType = (int)user.Membership
        });
    }

    [HttpGet("by-membership/{membershipType}", Name = "GetByMembership")]
    public ActionResult<IEnumerable<User>> GetByMembership(MembershipType membershipType)
    {
        var users = _userRepository.GetAll()
            .Where(u => u.Membership == membershipType);

        return Ok(users);
    }

    [HttpPost(Name = "CreateUser")]
    public ActionResult<User> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new User
        {
            FirstName = dto.FirstName!,
            LastName = dto.LastName!,
            Email = dto.Email!,
            PhoneNumber = dto.PhoneNumber,
            Membership = dto.Membership,
            MembershipStatus = dto.MembershipStatus,
            TimeCreated = DateTime.UtcNow
        };

        _userRepository.Add(user);

        return CreatedAtRoute("GetUserById", new { userId = user.Id }, user);
    }

    [HttpPut("{userId}", Name = "UpdateUser")]
    public IActionResult Update(string userId, [FromBody] UpdateUserDto dto)
    {
        var updatedUser = new User
        {
            Id = userId,
            FirstName = dto.FirstName!,
            LastName = dto.LastName!,
            Email = dto.Email!,
            PhoneNumber = dto.PhoneNumber,
            Membership = dto.Membership,
            MembershipStatus = dto.MembershipStatus
        };

        var updated = _userRepository.Update(userId, updatedUser);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{userId}", Name = "DeleteUser")]
    public IActionResult Delete(string userId)
    {
        var deleted = _userRepository.Delete(userId);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}