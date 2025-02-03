using WebApp.DB.Models;

namespace WebApp.Interfaces;

public interface IUserService
{
    Task<Result> RegisterAsync(string username, string email, string password);
    Task<User> LoginAsync(string email, string password);
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> GetUserByEmailAsync(string email);
}
