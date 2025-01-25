using Microsoft.EntityFrameworkCore;
using WebApp.DB.Models;

namespace WebApp.DB.Repositories;

public class UsersRepository
{
    private readonly MyDbContext _dbContext;

    public UsersRepository(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> AddAsync(User user)
    {
        var existingUser = await _dbContext.Users
            .AnyAsync(u => u.Email == user.Email);

        if (existingUser)
        {
            return Result.Failure("Пользователь с такой почтой уже зарегистрирован");
        }

        try
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Result.Failure(ex.ToString());
        }
    }

    public async Task<User> GetByEmail(string email)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        return user;
    }
    public async Task<User> GetByIdAsync(Guid id)
    {
        var user = await _dbContext.Users
            .FindAsync(id);

        return user;
    }
}
