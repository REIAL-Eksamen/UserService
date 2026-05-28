using UserService.Models;

namespace UserService.Repositories;

/// <summary>
/// Definerer dataaccesskontrakten for User-entiteter.
/// Gør det muligt at bytte MongoDB-implementationen ud med f.eks. en in-memory version til tests.
/// </summary>
public interface IUserRepository
{
    IEnumerable<User> GetAll();
    User? GetById(string userId);
    /// <summary>Slår bruger op via AuthService-ID — bruges til JWT-claim-validering.</summary>
    User? GetByAuthId(string authId);
    void Add(User user);
    /// <returns>True hvis brugeren blev fundet og opdateret.</returns>
    bool Update(string userId, User updatedUser);
    /// <returns>True hvis brugeren blev fundet og slettet.</returns>
    bool Delete(string userId);
}