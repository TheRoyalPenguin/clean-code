using WebApp.DB.Models;

namespace WebApp.Interfaces;

public interface IJwtManager
{
    string Generate(User user);
}
