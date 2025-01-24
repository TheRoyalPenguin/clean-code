using Microsoft.AspNetCore.Mvc;
using WebApp.DB.DTO;
using WebApp.Services;

namespace WebApp;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var isRegistered = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        if (!isRegistered)
        {
            return BadRequest("Пользователь с такой почтой уже существует.");
        }

        return Ok("Регистрация прошла успешно!.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.LoginAsync(request.Email, request.Password);

        if (user == null)
        {
            return Unauthorized("Неверный логин или пароль.");
        }

        return Ok("Успешный вход!");
    }
}
