namespace Markdown.BaseClasses;

public class MarkdownToHtmlRenderer : IMarkdownProcessor
{
    public string Render(string markdownText)
    {
        Tokenizer tokenizer = new Tokenizer();
        string htmlString = tokenizer.Tokenize(markdownText).ToHtml();
        return htmlString;
    }
}