namespace MarkdownTests;
using Markdown.BaseClasses;

public class MarkdownToHtmlRendererTests
{
    private readonly MarkdownToHtmlRenderer renderer;

    public MarkdownToHtmlRendererTests()
    {
        renderer = new MarkdownToHtmlRenderer();
    }

    [Theory]
    [InlineData("_test_", "<p><em>test</em></p>")]
    [InlineData("__test__", "<p><strong>test</strong></p>")]
    [InlineData("\\__te\\st__", "<p>_<em>te\\st</em>_</p>")]
    [InlineData("\\\\__test__", "<p>\\<strong>test</strong></p>")]
    [InlineData("__t_es_t__", "<p><strong>t<em>es</em>t</strong></p>")]
    [InlineData("_t__es__t_", "<p><em>t</em><em>es</em><em>t</em></p>")]
    [InlineData("_t1_e_s3t_", "<p><em>t1_e_s3t</em></p>")]
    [InlineData("_te_st", "<p><em>te</em>st</p>")]
    [InlineData("t_es_t", "<p>t<em>es</em>t</p>")]
    [InlineData("te_st_", "<p>te<em>st</em></p>")]
    [InlineData("t_est tes_t", "<p>t_est tes_t</p>")]
    [InlineData("_te__s_t__", "<p><em>te</em><em>s</em>t__</p>")]
    [InlineData("#_test_", "<h1><em>test</em></h1>")]
    private void Render_RenderMarkdownToHtml(string markdown, string expectedHtml)
    {
        var result = renderer.Render(markdown);

        Assert.Equal(expectedHtml, result);
    }
}