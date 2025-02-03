using Markdown.Interfaces;

namespace Markdown.BaseClasses;

public class MarkdownConverter : IMarkdownConverter
{
    public string ConvertToHtml(string markdownText)
    {
        ITokenizer tokenizer = new Tokenizer();
        string htmlString = tokenizer.Tokenize(markdownText).ToHtml();
        return htmlString;
    }
}
