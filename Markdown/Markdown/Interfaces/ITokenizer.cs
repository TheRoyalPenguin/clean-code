using Markdown.Tags;

namespace Markdown.Interfaces;

public interface ITokenizer
{
    MainToken Tokenize(string markdownText);
}
