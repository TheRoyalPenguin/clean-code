using WebApp.DB.Models;
using WebApp.DB.Repositories;

namespace WebApp.Services;

public class UserService
{
    private readonly UsersRepository _usersRepository;
    private readonly MyPasswordHasher _passwordHasher;
    public UserService(UsersRepository usersRepository, MyPasswordHasher passwordHasher)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }
    public async Task<Result> RegisterAsync(string username, string email, string password)
    {
        var passwordHash = _passwordHasher.HashPassword(password);

        var user = new User(Guid.NewGuid(), username, email, passwordHash);

        var resultAddUser = await _usersRepository.AddAsync(user);

        if (!resultAddUser.IsSuccess)
        {
            return Result.Failure(resultAddUser.Error);
        }

        return Result.Success();
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _usersRepository.GetByEmail(email);

        if (user == null)
        {
            Console.WriteLine("Пользователь не найден");
            return null;
        }

        var passwordIsValid = _passwordHasher.VerifyPassword(password, user.PasswordHash);

        if (!passwordIsValid)
        {
            Console.WriteLine("Пароль неверный");
            return null;
        }

        return user;
    }
    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var user = await _usersRepository.GetByIdAsync(id);

        if (user == null)
        {
            Console.WriteLine("Пользователь не найден");
            return null;
        }

        return user;
    }
}
