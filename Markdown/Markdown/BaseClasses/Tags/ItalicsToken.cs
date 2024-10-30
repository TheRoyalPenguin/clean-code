using Markdown.AbstractClasses;
using Markdown.Interfaces;

namespace Markdown.Tags;

public class ItalicsToken : BaseMarkdownToken, IDoubleTag
{
    //public string Content { get; }
    //public ItalicsToken(string content)
    //{
    //    Content = content;
    //}
    public DoubleTagStatusEnum Status { get; private set; } = DoubleTagStatusEnum.Open;
    public override TokenNamesEnum TokenName { get; } = TokenNamesEnum.Italics;
    public ItalicsToken(DoubleTagStatusEnum status)
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
        if (htmlResultString.Length > 0 && Status == DoubleTagStatusEnum.Close) return ("<em>" + htmlResultString + "</em>");
        else return ("_" + htmlResultString);
    }
}