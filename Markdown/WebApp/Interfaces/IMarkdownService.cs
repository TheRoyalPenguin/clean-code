using WebApp.DB.Models;

namespace WebApp.Services;

public interface IMarkdownService
{
    Task<Result<string>> GetHtml(string markdownText);
}
