using Markdown.AbstractClasses;

namespace Markdown.Tags;

public class ParagraphToken : BaseMarkdownToken
{
    public override TokenNamesEnum TokenName { get; } = TokenNamesEnum.Paragraph;
    public override string ToHtml()
    {
        // Дополнительно разделяем пробелами "слова"
        var htmlResultString = string.Join("", Children.Select((child, i) => i != 0 ? " " + child.ToHtml() : child.ToHtml()));
        if (string.IsNullOrWhiteSpace(htmlResultString) )
        {
            return "<br>";
        }
        else return "<p>" + htmlResultString + "</p>";
    }
}