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
        if (htmlResultString.Length > 0) return (GetLink(htmlResultString));
        return ("<" + htmlResultString);
    }
    private string GetLink(string s)
    {
        var match = System.Text.RegularExpressions.Regex.Match(s, @"\[(.*?)\](.*)"); // ищет текст в формате [текст]ссылка

        string linkText;
        string href;
        if (match.Success)
        {
            linkText = match.Groups[1].Value;
            href = match.Groups[2].Value;

            if (!href.StartsWith("http://") && !href.StartsWith("https://"))
            {
                href = "https://" + href;
            }

            if (href.Length == 0) 
            {
                return s;
            }

            if (linkText.Length == 0)
            {
                string displayText = href.Replace("https://", "").Replace("http://", "");
                return $"<a href=\"{href}\" target=\"_blank\">{displayText}</a>";
            }

            return $"<a href=\"{href}\" target=\"_blank\">{linkText}</a>";
        }

        string url = s;
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "https://" + url;
        }

        string displayUrl = url.Replace("https://", "").Replace("http://", "");
        return $"<a href=\"{url}\" target=\"_blank\">{displayUrl}</a>";
    }
}