using Markdown.AbstractClasses;
using Markdown.Interfaces;

namespace Markdown.Tags;

public class LinkToken : BaseMarkdownToken, IDoubleTag
{
    public override TokenNamesEnum TokenName { get; } = TokenNamesEnum.LinkStart;
    public DoubleTagStatusEnum Status { get; private set; } = DoubleTagStatusEnum.Open;
    public LinkToken(TokenNamesEnum tokenName)
    {
        TokenName = tokenName;
    }
    public LinkToken(DoubleTagStatusEnum status)
    {
        Status = status;
    }
    public void ChangeStatus(DoubleTagStatusEnum status)
    {
        Status = status;
    }
    public override string ToHtml()
    {
        var htmlResultString = string.Join("", Children.Select(child => child.ToHtml()));
        if (htmlResultString.Length > 0) return ("<a href=\"" + htmlResultString + "\">" + htmlResultString + "</a>");
        else return ("<" + htmlResultString);
    }
}