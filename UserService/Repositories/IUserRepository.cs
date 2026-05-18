using UserService.Models;
namespace UserService.Repositories;

public interface IUserRepository
{
    IEnumerable<User> GetAll();
    User? GetById(Guid userId);
    void Add(User user);
    
    bool Update(Guid userId, User updatedUser);
}