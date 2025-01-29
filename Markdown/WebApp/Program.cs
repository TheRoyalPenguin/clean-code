using System.Text;
using System.Text.Json;
using Markdown.BaseClasses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApp.DB;
using WebApp.DB.DTO;
using WebApp.DB.Repositories;
using WebApp.Interfaces;
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

        builder.Services.AddScoped<MyPasswordHasher>();
        builder.Services.AddScoped<UsersRepository>();
        builder.Services.AddScoped<DocumentsRepository>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<JwtManager>();
        builder.Services.AddScoped<DocumentsService>();
        builder.Services.AddScoped<IFileStorageService, MinioStorageService>();

        builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        var jwtOptions = configuration["JwtOptions:SecretKey"];
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var path = context.Request.Path.ToString();
                        if (!context.Request.Path.StartsWithSegments("/markdown-to-html-convert"))
                        {
                            context.Token = context.Request.Cookies["jwt-cookies"];
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        builder.Services.AddControllers();
        var app = builder.Build();
        
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapPost("/markdown-to-html-convert", async (HttpContext context) =>
        {
            var requestBody = await JsonSerializer.DeserializeAsync<MarkdownRequest>(context.Request.Body);

            if (requestBody == null || string.IsNullOrWhiteSpace(requestBody.InputText))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("����� �� ����� ���� ������.");
                return;
            }

            MarkdownToHtmlRenderer renderer = new MarkdownToHtmlRenderer();
            var htmlText = renderer.Render(requestBody.InputText);

            var response = new
            {
                HtmlText = htmlText
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
