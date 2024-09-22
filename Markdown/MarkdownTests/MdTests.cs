namespace MarkdownTests;
using Markdown;

public class MdTests
{
    [Fact]
    public void Render_CursiveText_ReturnHtmlEmTag()
    {
        var processor = new Md();
        string markdownText = "_Проверка текста с заменой на em_test@_";
        string expectedText = "<em>Проверка текста с заменой на em</em>test@_";
        var result = processor.Render(markdownText);
        Assert.Equal(expectedText, result);
    }
}