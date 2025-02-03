using WebApp.DB.Models;

namespace WebApp.Interfaces;

public interface IUsersRepository
{
    Task<Result> AddAsync(User user);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByIdAsync(Guid id);
}
