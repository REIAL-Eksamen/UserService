using UserService.Models;

namespace UserService.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public IEnumerable<User> GetAll()
    {
        return _users;
    }

    public User? GetById(string userId)
    {
        return _users.FirstOrDefault(u => u.Id == userId);
    }

    public User? GetByAuthId(string authId)
    {
        throw new NotImplementedException();
    }

    public void Add(User user)
    {
        user.Id ??= Guid.NewGuid().ToString();
        user.TimeCreated = DateTime.UtcNow;

        _users.Add(user);
    }

    public bool Update(string userId, User updatedUser)
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
        existingUser.Membership = updatedUser.Membership;
        existingUser.MembershipStatus = updatedUser.MembershipStatus;

        return true;
    }

    public bool Delete(string userId)
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