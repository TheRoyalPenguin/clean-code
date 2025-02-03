using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.DB.DTO;
using WebApp.Interfaces;

namespace WebApp.Controllers;

[ApiController]
[Route("api/auth")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtManager _jwtManager;
    private readonly IAuthValidator _authValidator;

    public UserController(IUserService userService, IJwtManager jwtManager, IAuthValidator authValidator)
    {
        _userService = userService;
        _jwtManager = jwtManager;
        _authValidator = authValidator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var errors = _authValidator.ValidateRegistration(request);
        if (errors.Any())
        {
            return BadRequest(new { Message = errors.FirstOrDefault() });
        }
        
        var resultRegisterUser = await _userService.RegisterAsync(request.Username, request.Email, request.Password);
        if (!resultRegisterUser.IsSuccess)
        {
            return BadRequest(new { Message = "Пользователь с такой почтой уже существует." });
        }

        return Ok(new { Message = "Регистрация прошла успешно!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var errors = _authValidator.ValidateLogin(request);
        if (errors.Any())
        {
            return BadRequest(new { Message = errors.FirstOrDefault() });
        }
        
        var user = await _userService.LoginAsync(request.Email, request.Password);

        if (user == null)
        {
            return Unauthorized(new { Message = "Неверный логин или пароль." });
        }
        var token = _jwtManager.Generate(user);
        Response.Cookies.Append("jwt-cookies", token, new CookieOptions
        {
            HttpOnly = true, // кука недоступна через JavaScript
            Secure = true, // кука передается только по HTTPS
            SameSite = SameSiteMode.Strict, // защита от CSRF
            Expires = DateTime.UtcNow.AddMinutes(30)
        });

        return Ok(new
        {
            user.Username,
            user.Email
        });
    }
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { Message = "Неверный или отсутствующий идентификатор пользователя." });
        }
        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new { Message = "Пользователь не найден." });
        }

        return Ok(new
        {
            user.Username,
            user.Email
        });
    }
    [HttpPost("logout")]
    public IActionResult LogoutAsync()
    {
        Response.Cookies.Delete("jwt-cookies", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(-1)
        });

        return Ok(new { message = "Успешный выход" });
    }
}
