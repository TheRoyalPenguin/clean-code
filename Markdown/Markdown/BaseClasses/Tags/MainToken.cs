using Markdown.AbstractClasses;

namespace Markdown.Tags;

public class MainToken : BaseMarkdownToken
{
    public override TokenNamesEnum TokenName { get; } = TokenNamesEnum.Main;
    public override string ToHtml()
    {
        var htmlResultString = string.Join("\n", Children.Select(child => child.ToHtml()));
        return (htmlResultString);
    }
}