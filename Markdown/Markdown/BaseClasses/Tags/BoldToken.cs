using Markdown.AbstractClasses;
using Markdown.Interfaces;

namespace Markdown.Tags;

public class BoldToken : BaseMarkdownToken, IDoubleTag
{
    //public string Content { get; }
    //public BoldToken(string content)
    //{
    //    Content = content;
    //}
    public override TokenNamesEnum TokenName { get; } = TokenNamesEnum.Bold;
    public DoubleTagStatusEnum Status { get; private set; } = DoubleTagStatusEnum.Open;
    public BoldToken(DoubleTagStatusEnum status) 
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
        if (htmlResultString.Length > 0 && Status == DoubleTagStatusEnum.Close) return ("<strong>" + htmlResultString + "</strong>");
        else return ("__" + htmlResultString);
    }
}