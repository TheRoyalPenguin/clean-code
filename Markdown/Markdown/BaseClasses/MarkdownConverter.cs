namespace Markdown.BaseClasses;

public class MarkdownConverter : IMarkdownConverter
{
    public string ConvertToHtml(string markdownText)
    {
        Tokenizer tokenizer = new Tokenizer();
        string htmlString = tokenizer.Tokenize(markdownText).ToHtml();
        return htmlString;
    }
}
