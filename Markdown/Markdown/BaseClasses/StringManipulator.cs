namespace Markdown.BaseClasses;

public class StringManipulator
{
    public string RemoveFirstNChars(string s, char c, int n)
    {
        string result = "";
        int searchCount = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == c && searchCount < n)
            {
                searchCount++;
            }
            else
            {
                result += s[i];
            }
        }

        return result;
    }
}
