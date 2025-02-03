using WebApp.DB.DTO;
using WebApp.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace WebApp;

public class AuthValidator : IAuthValidator
{
    public List<string> ValidateRegistration(RegisterRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 5 || request.Username.Length > 50)
        {
            errors.Add("Имя пользователя должно быть от 5 до 50 символов");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            errors.Add("Пароль должен содержать минимум 6 символов");
        }
        
        if (!IsValidEmail(request.Email))
        {
            errors.Add("Некорректный email");
        }

        return errors;
    }

    public List<string> ValidateLogin(LoginRequest request)
    {
        var errors = new List<string>();

        if (!IsValidEmail(request.Email))
        {
            errors.Add("Некорректный email");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add("Пароль обязателен");
        }

        return errors;
    }

    private bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && new EmailAddressAttribute().IsValid(email);
    }
}
