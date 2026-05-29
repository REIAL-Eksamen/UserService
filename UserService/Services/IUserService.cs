using UserService.DTOs;
using UserService.Models;

namespace UserService.Services;

/// <summary>
/// Definerer forretningslogikken for brugerhåndtering i UserService.
/// </summary>
public interface IUserService
{
    IEnumerable<User> GetAll();
    /// <returns>Null hvis brugeren ikke findes.</returns>
    User? GetById(string userId);
    /// <summary>Slår bruger op via AuthService-ID — bruges af BookingService og JWT-flow.</summary>
    /// <returns>Null hvis ingen bruger matcher authId.</returns>
    User? GetByAuthId(string authId);
    /// <summary>Opretter bruger ud fra DTO og returnerer det persisterede objekt med tildelt ID.</summary>
    User Create(CreateUserDto dto);
    /// <returns>False hvis brugeren ikke findes.</returns>
    bool Update(string userId, UpdateUserDto dto);
    /// <returns>False hvis brugeren ikke findes.</returns>
    bool Delete(string userId);
    /// <summary>Returnerer et anonymt objekt med status og isActive-flag — bruges til hurtige tjeks.</summary>
    /// <returns>Null hvis brugeren ikke findes.</returns>
    object? GetMembershipStatus(string userId);
    /// <summary>Returnerer et anonymt objekt med membership-navn og numerisk type-værdi.</summary>
    /// <returns>Null hvis brugeren ikke findes.</returns>
    object? GetMembership(string userId);
    IEnumerable<User> GetByMembership(MembershipType membershipType);
}