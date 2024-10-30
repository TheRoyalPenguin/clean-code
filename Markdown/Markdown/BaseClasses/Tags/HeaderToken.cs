using Markdown.AbstractClasses;

namespace Markdown.Tags;

public class HeaderToken : BaseMarkdownToken
{
    //public string Content { get; }
    //public HeaderToken(string content)
    //{
    //    Content = content;
    //}
    public override TokenNamesEnum TokenName { get; } = TokenNamesEnum.Header;
    public override string ToHtml()
    {
        // ƒополнительно раздел€ем пробелами "слова"
        var htmlResultString = string.Join("", Children.Select((child, i) => i != 0 ? " " + child.ToHtml() : child.ToHtml()));
        return ("<h1>" + htmlResultString + "</h1>");
    }
}