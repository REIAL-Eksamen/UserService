using UserService.DTOs;
using UserService.Models;

namespace UserService.Services;

public interface IUserService
{
    IEnumerable<User> GetAll();

    User? GetById(string userId);

    User? GetByAuthId(string authId);

    User Create(CreateUserDto dto);

    bool Update(string userId, UpdateUserDto dto);

    bool Delete(string userId);

    object? GetMembershipStatus(string userId);

    object? GetMembership(string userId);

    IEnumerable<User> GetByMembership(MembershipType membershipType);
}