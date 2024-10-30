namespace Markdown.BaseClasses;
public class StringBuffer
{
    public string Buffer { get; private set; }
    public int CountEscapeSymbolsAtEndString { get; private set; }
    public StringBuffer()
    {
        Buffer = "";
        CountEscapeSymbolsAtEndString = 0;
    }
    public StringBuffer(string text)
    {
        Buffer = text;
        CountEscapeSymbolsAtEndString = 0;
    }
    public void AddSymbol(char c)
    {
        Buffer += c;
        if (c == '\\') CountEscapeSymbolsAtEndString++;
        else CountEscapeSymbolsAtEndString = 0;
    }
    public void AddSymbolToStartingString(char c)
    {
        Buffer = c + Buffer;
    }
    public int RemoveEscapeSymbolsFromEndString()
    {
        Buffer = Buffer.Substring(0, Buffer.Length - CountEscapeSymbolsAtEndString);
        int temp = CountEscapeSymbolsAtEndString;
        CountEscapeSymbolsAtEndString = 0;
        return temp;
    }
    public void Clear()
    {
        Buffer = "";
        CountEscapeSymbolsAtEndString = 0;
    }
}
