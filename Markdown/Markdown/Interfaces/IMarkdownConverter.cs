namespace Markdown;

public interface IMarkdownConverter
{
    string ConvertToHtml(string markdownText);
}
