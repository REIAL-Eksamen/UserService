using UserService.Models;
namespace UserService.Repositories;

public interface IUserRepository
{
    IEnumerable<User> GetAll();
    User? GetById(string userId);
    void Add(User user);
    
    bool Update(string userId, User updatedUser);
    bool Delete(string userId);
}