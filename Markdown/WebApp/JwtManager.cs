using WebApp.DB.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace WebApp;

public class JwtManager
{
    private readonly JwtOptions _options;
    public JwtManager(IOptions<JwtOptions> options)
    {
        _options = options.Value; 
    }
    public string Generate(User user)
    {
        Claim[] claims =
        {
            new Claim("userId", user.Id.ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours)
            );

        var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenHandler;
    }
}
