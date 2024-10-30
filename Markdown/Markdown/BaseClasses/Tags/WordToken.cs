using Markdown.AbstractClasses;

namespace Markdown.Tags;
public class WordToken : BaseMarkdownToken
{
    public override TokenNamesEnum TokenName { get; } = TokenNamesEnum.Word;

    public override string ToHtml()
    {
        var htmlResultString = string.Join("", Children.Select(child => child.ToHtml()));
        return htmlResultString;
    }
}
