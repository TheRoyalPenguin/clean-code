using WebApp.DB.DTO;

namespace WebApp.Interfaces;

public interface IAuthValidator
{
    List<string> ValidateRegistration(RegisterRequest request);
    List<string> ValidateLogin(LoginRequest request);
}
