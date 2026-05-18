using UserService.Models;

namespace UserService.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users =
    [
        new User
        {
            UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FirstName = "Enni",
            LastName = "Test",
            Email = "enni@example.com",
            PhoneNumber = "12345678",
            Role = RoleType.Member,
            MembershipStatus = MembershipStatus.Active,
            TimeCreated = DateTime.UtcNow
        },
        new User
        {
            UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FirstName = "Mads",
            LastName = "Admin",
            Email = "admin@example.com",
            PhoneNumber = "87654321",
            Role = RoleType.Admin,
            MembershipStatus = MembershipStatus.Active,
            TimeCreated = DateTime.UtcNow
        }
    ];

    public IEnumerable<User> GetAll()
    {
        return _users;
    }

    public User? GetById(Guid userId)
    {
        return _users.FirstOrDefault(user => user.UserId == userId);
    }

    public void Add(User user)
    {
        _users.Add(user);
    }

    public bool Update(Guid userId, User updatedUser)
    {
        var existingUser = GetById(userId);

        if (existingUser is null)
        {
            return false;
        }

        existingUser.FirstName = updatedUser.FirstName;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Email = updatedUser.Email;
        existingUser.PhoneNumber = updatedUser.PhoneNumber;
        existingUser.Role = updatedUser.Role;
        existingUser.MembershipStatus = updatedUser.MembershipStatus;

        return true;
    }

    public bool Delete(Guid userId)
    {
        var user = GetById(userId);

        if (user is null)
        {
            return false;
        }

        _users.Remove(user);
        return true;
    }
}