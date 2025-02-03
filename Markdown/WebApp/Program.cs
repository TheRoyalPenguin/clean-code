using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApp.DB;
using WebApp.JWT;
using WebApp.Services;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // подключение конфигурации
        builder.Configuration.AddEnvironmentVariables();
        var configuration = builder.Configuration;

        builder.Services.AddDbContext<MyDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString(nameof(MyDbContext)));
        });
        
        builder.Services.AddApplicationServices();

        var jwtOptionsSection = configuration.GetSection(nameof(JwtOptions));
        builder.Services.Configure<JwtOptions>(jwtOptionsSection);
        
        var jwtOptions = jwtOptionsSection.Get<JwtOptions>();
        if (string.IsNullOrWhiteSpace(jwtOptions?.SecretKey))
        {
            throw new InvalidOperationException("Секретный ключ JWT не настроен!");
        }
        
        var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        ExtractJwtToken(context);
                        return Task.CompletedTask;
                    }
                };
            });
        
        builder.Services.AddControllers();
        var app = builder.Build();
        
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
    
    private static void ExtractJwtToken(MessageReceivedContext context)
    {
        context.Token = context.Request.Cookies["jwt-cookies"];
    }
}
