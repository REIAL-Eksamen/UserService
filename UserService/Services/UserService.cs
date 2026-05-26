using UserService.DTOs;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Services;

// UserService indeholder brugerlogikken.
// Repository-laget bruges kun til at hente og gemme data.
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<User> GetAll()
    {
        return _repository.GetAll();
    }

    public User? GetById(string userId)
    {
        return _repository.GetById(userId);
    }

    public User? GetByAuthId(string authId)
    {
        return _repository.GetByAuthId(authId);
    }

    public User Create(CreateUserDto dto)
    {
        var user = new User
        {
            AuthId = dto.AuthId,
            FirstName = dto.FirstName!,
            LastName = dto.LastName!,
            Email = dto.Email!,
            PhoneNumber = dto.PhoneNumber,
            Membership = dto.Membership,
            MembershipStatus = dto.MembershipStatus,
            TimeCreated = DateTime.UtcNow
        };

        _repository.Add(user);

        return user;
    }

    public bool Update(string userId, UpdateUserDto dto)
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

        return _repository.Update(userId, updatedUser);
    }

    public bool Delete(string userId)
    {
        return _repository.Delete(userId);
    }

    public object? GetMembershipStatus(string userId)
    {
        var user = _repository.GetById(userId);

        if (user is null)
        {
            return null;
        }

        return new
        {
            userId = user.Id,
            membershipStatus = user.MembershipStatus.ToString(),
            isActive = user.MembershipStatus == MembershipStatus.Active
        };
    }

    public object? GetMembership(string userId)
    {
        var user = _repository.GetById(userId);

        if (user is null)
        {
            return null;
        }

        return new
        {
            userId = user.Id,
            membership = user.Membership.ToString(),
            membershipType = (int)user.Membership
        };
    }

    public IEnumerable<User> GetByMembership(MembershipType membershipType)
    {
        return _repository.GetAll()
            .Where(user => user.Membership == membershipType);
    }
}