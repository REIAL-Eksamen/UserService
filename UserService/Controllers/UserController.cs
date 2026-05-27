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
    }
    
    [HttpGet("version")]
    public async Task<Dictionary<string,string>> GetVersion()
    {
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
        return properties;
    }

    [HttpGet(Name = "GetUsers")]
    public IEnumerable<User> Get()
    {
        return _userService.GetAll();
    }

    [HttpGet("{userId}", Name = "GetUserById")]
    public ActionResult<User> GetById(string userId)
    {
        var user = _userService.GetById(userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<User> GetCurrentUser()
    {
        var authId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(authId))
        {
            return Unauthorized();
        }

        var user = _userService.GetByAuthId(authId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{userId}/membership-status", Name = "GetMembershipStatus")]
    public ActionResult<object> GetMembershipStatus(string userId)
    {
        var result = _userService.GetMembershipStatus(userId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("{userId}/membership", Name = "GetMembership")]
    public ActionResult<object> GetMembership(string userId)
    {
        var result = _userService.GetMembership(userId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("by-membership/{membershipType}", Name = "GetByMembership")]
    public ActionResult<IEnumerable<User>> GetByMembership(MembershipType membershipType)
    {
        var users = _userService.GetByMembership(membershipType);

        return Ok(users);
    }

    [HttpPost(Name = "CreateUser")]
    public ActionResult<User> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = _userService.Create(dto);

        return CreatedAtRoute("GetUserById", new { userId = user.Id }, user);
    }

    [HttpPut("{userId}", Name = "UpdateUser")]
    public IActionResult Update(string userId, [FromBody] UpdateUserDto dto)
    {
        var updated = _userService.Update(userId, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{userId}", Name = "DeleteUser")]
    public IActionResult Delete(string userId)
    {
        var deleted = _userService.Delete(userId);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}