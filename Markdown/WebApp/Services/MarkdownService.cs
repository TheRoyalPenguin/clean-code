using Markdown;
using WebApp.DB.Enums;
using WebApp.DB.Models;

namespace WebApp.Services;

public class MarkdownService : IMarkdownService
{
    private readonly IMarkdownConverter _markdownConverter;

    public MarkdownService(IMarkdownConverter markdownConverter)
    {
        _markdownConverter = markdownConverter;
    }

    public async Task<Result<string>> GetHtml(string markdownText)
    {
        try
        {
            return Result<string>.Success(_markdownConverter.ConvertToHtml(markdownText));
        }
        catch (Exception e)
        {
            return Result<string>.Failure(e.Message, Errors.Unknown);
        }
    }
}
