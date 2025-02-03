using Markdown;
using Markdown.BaseClasses;
using WebApp.DB.Repositories;
using WebApp.Interfaces;
using WebApp.JWT;

namespace WebApp.Services;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, MyPasswordHasher>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IDocumentsRepository, DocumentsRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtManager, JwtManager>();
        services.AddScoped<IDocumentsService, DocumentsService>();
        services.AddScoped<IFileStorageService, MinioStorageService>();
        services.AddScoped<IMarkdownConverter, MarkdownConverter>();
        services.AddScoped<IMarkdownService, MarkdownService>();
        services.AddScoped<IAuthValidator, AuthValidator>();
    }
}
