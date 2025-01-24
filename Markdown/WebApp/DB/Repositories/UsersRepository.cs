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

    public async Task<bool> AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<User> GetByEmail(string email)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        return user;
    }
}
