using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Models;
using UserService.Services;
using System.Diagnostics;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;

        _logger.LogInformation("UserController initialized");
    }
    
    [HttpGet("version")]
    public async Task<Dictionary<string,string>> GetVersion()
    {
        _logger.LogInformation("Version endpoint called");

        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;

        properties.Add("service", "UserService");

        var ver = FileVersionInfo.GetVersionInfo(
            typeof(Program).Assembly.Location).ProductVersion ?? "N/A";

        properties.Add("version", ver);

        var hostName = System.Net.Dns.GetHostName();
        var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
        var ipa = ips.First().MapToIPv4().ToString() ?? "N/A";

        properties.Add("ip-address", ipa);

        _logger.LogInformation(
            "Version endpoint returned service={Service}, version={Version}, ip={IpAddress}",
            properties["service"],
            properties["version"],
            properties["ip-address"]);

        return properties;
    }

    [HttpGet(Name = "GetUsers")]
    public IEnumerable<User> Get()
    {
        _logger.LogInformation("Fetching all users");

        var users = _userService.GetAll().ToList();

        _logger.LogInformation("Returned {UserCount} users", users.Count);

        return users;
    }

    [HttpGet("{userId}", Name = "GetUserById")]
    public ActionResult<User> GetById(string userId)
    {
        _logger.LogInformation("Fetching user by id {UserId}", userId);

        var user = _userService.GetById(userId);

        if (user is null)
        {
            _logger.LogWarning("User not found with id {UserId}", userId);
            return NotFound();
        }

        _logger.LogInformation("User found with id {UserId}", userId);
        return Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<User> GetCurrentUser()
    {
        _logger.LogInformation("Fetching current authenticated user");

        var authId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(authId))
        {
            _logger.LogWarning("Current user request failed because authId claim was missing");
            return Unauthorized();
        }

        var user = _userService.GetByAuthId(authId);

        if (user is null)
        {
            _logger.LogWarning("No user found for authId {AuthId}", authId);
            return NotFound();
        }

        _logger.LogInformation("Current user found for authId {AuthId}", authId);
        return Ok(user);
    }

    [HttpGet("{userId}/membership-status", Name = "GetMembershipStatus")]
    public ActionResult<object> GetMembershipStatus(string userId)
    {
        _logger.LogInformation("Fetching membership status for user {UserId}", userId);

        var result = _userService.GetMembershipStatus(userId);

        if (result is null)
        {
            _logger.LogWarning("Membership status not found for user {UserId}", userId);
            return NotFound();
        }

        _logger.LogInformation("Membership status returned for user {UserId}", userId);
        return Ok(result);
    }

    [HttpGet("{userId}/membership", Name = "GetMembership")]
    public ActionResult<object> GetMembership(string userId)
    {
        _logger.LogInformation("Fetching membership for user {UserId}", userId);

        var result = _userService.GetMembership(userId);

        if (result is null)
        {
            _logger.LogWarning("Membership not found for user {UserId}", userId);
            return NotFound();
        }

        _logger.LogInformation("Membership returned for user {UserId}", userId);
        return Ok(result);
    }

    [HttpGet("by-membership/{membershipType}", Name = "GetByMembership")]
    public ActionResult<IEnumerable<User>> GetByMembership(MembershipType membershipType)
    {
        _logger.LogInformation("Fetching users by membership type {MembershipType}", membershipType);

        var users = _userService.GetByMembership(membershipType).ToList();

        _logger.LogInformation(
            "Returned {UserCount} users with membership type {MembershipType}",
            users.Count,
            membershipType);

        return Ok(users);
    }

    [HttpPost(Name = "CreateUser")]
    public ActionResult<User> Create([FromBody] CreateUserDto dto)
    {
        _logger.LogInformation("Create user request received");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Create user request failed because model state is invalid");
            return BadRequest(ModelState);
        }

        var user = _userService.Create(dto);

        _logger.LogInformation("User created with id {UserId}", user.Id);

        return CreatedAtRoute("GetUserById", new { userId = user.Id }, user);
    }

    [HttpPut("{userId}", Name = "UpdateUser")]
    public IActionResult Update(string userId, [FromBody] UpdateUserDto dto)
    {
        _logger.LogInformation("Update user request received for user {UserId}", userId);

        var updated = _userService.Update(userId, dto);

        if (!updated)
        {
            _logger.LogWarning("Update failed because user was not found. UserId={UserId}", userId);
            return NotFound();
        }

        _logger.LogInformation("User updated with id {UserId}", userId);
        return NoContent();
    }

    [HttpDelete("{userId}", Name = "DeleteUser")]
    public IActionResult Delete(string userId)
    {
        _logger.LogInformation("Delete user request received for user {UserId}", userId);

        var deleted = _userService.Delete(userId);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed because user was not found. UserId={UserId}", userId);
            return NotFound();
        }

        _logger.LogInformation("User deleted with id {UserId}", userId);
        return NoContent();
    }
}