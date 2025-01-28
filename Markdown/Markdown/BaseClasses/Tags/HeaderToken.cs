using Markdown.AbstractClasses;

namespace Markdown.Tags;

public class HeaderToken : BaseMarkdownToken
{
    private int Level { get; }
    public HeaderToken(int level)
    {
        Level = level;
    }
    public override TokenNamesEnum TokenName { get; } = TokenNamesEnum.Header;
    public override string ToHtml()
    {
        // ƒополнительно раздел€ем пробелами "слова"
        var htmlResultString = string.Join("", Children.Select((child, i) => i != 0 ? " " + child.ToHtml() : child.ToHtml()));
        return ($"<h{Level}>" + htmlResultString + $"</h{Level}>");
    }
}