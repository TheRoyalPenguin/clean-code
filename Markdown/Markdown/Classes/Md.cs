namespace Markdown;

public class Md: IMarkdownProcessor
{
    public string Render(string markdownText)
    {
        string result = markdownText;
        result = ReplaceFirstSubstringFound(markdownText, "_", "<em>", "</em>");
        return result;
    }

    private string ReplaceFirstSubstringFound(string input, string oldTag, string newTag1, string newTag2)
    {
        var countOldTagInInput = GetCountSubStringInString(input, oldTag);
        while (countOldTagInInput > 1)
        {
            input = input.Substring(0, input.IndexOf(oldTag, StringComparison.Ordinal)) + newTag1 +
                        input.Substring(input.IndexOf(oldTag, StringComparison.Ordinal) + 1);
            input = input.Substring(0, input.IndexOf(oldTag, StringComparison.Ordinal)) + newTag2 +
                    input.Substring(input.IndexOf(oldTag, StringComparison.Ordinal) + 1);
            countOldTagInInput -= 2;
        }
        return input;
    }

    private int GetCountSubStringInString(string str, string subString)
    {
        int count = 0;
        if (str.Contains(subString))
        {
            int subStringLength = subString.Length;
            for (int i = 0; i < str.Length; i+=subStringLength)
            {
                if (str.Substring(i, subStringLength) == subString) count++;
            }
        }

        return count;
    }
}