using System.Text.RegularExpressions;
using Markdown.AbstractClasses;
using Markdown.Interfaces;
using Markdown.Tags;

namespace Markdown.BaseClasses;
public class Tokenizer : ITokenizer
{
    bool isEscapingSupported;
    HashSet<string> allowedTags;
    CharacterProcessingSequenceManager characterProcessingSequenceManager;
    StringManipulator stringManipulator;
    PointerStackManager pointerStackManager;
    public Tokenizer()
    {
        characterProcessingSequenceManager = new CharacterProcessingSequenceManager();
        stringManipulator = new StringManipulator();
    }
    public MainToken Tokenize(string markdownText)
    {
        // Если пришел пустой текст отдаем его обратно:)
         if (markdownText.Length == 0)
        {
            return new MainToken();
        }
        MainToken mainToken = new MainToken();
        BaseMarkdownToken rootToken = new ParagraphToken();

        isEscapingSupported = true;
        allowedTags = isEscapingSupported ? new HashSet<string> { "_", "__", "#", "<", ">", "\\" } : new HashSet<string> { "_", "__", "#", "<", ">" };

        string[] markdownTextParagraphs = markdownText.Split("\n");
        foreach (var markdownTextParagraphF in markdownTextParagraphs)
        {
            var markdownTextParagraph = markdownTextParagraphF;
            var pointerToCurrentTokenStack = new Stack<BaseMarkdownToken>();
            // string[] wordsMarkdownTextParagraph = markdownTextParagraph.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (markdownTextParagraph != null && markdownTextParagraph.Length != 0 && markdownTextParagraph.TrimStart().StartsWith("#"))
            {
                int headerLevel = markdownTextParagraph.TrimStart().TakeWhile(p => p == '#').Count();
                headerLevel = Math.Min(headerLevel, 6);

                rootToken = new HeaderToken(headerLevel);
                pointerToCurrentTokenStack.Push(rootToken);
                markdownTextParagraph = stringManipulator.RemoveFirstNChars(markdownTextParagraph, '#', headerLevel);
            }
            else
            {
                rootToken = new ParagraphToken();
                pointerToCurrentTokenStack.Push(rootToken);
            }

            mainToken.Children.Add(rootToken);

            string pattern = @"(\s+|\S+)";
            // \s+ один или более пробелов
            // \S+ один или более непробельных символов
            MatchCollection matches = Regex.Matches(markdownTextParagraph, pattern);
            string[] wordsMarkdownTextParagraph = matches
                .Cast<Match>() // преобразуем MatchCollection в IEnumerable<Match>
                .Select(m => m.Value)
                .ToArray();

            //не нужно, теперь строки целиком обрабатываются
            //foreach (var word in wordsMarkdownTextParagraph)
            //{
            var word = string.Join("", wordsMarkdownTextParagraph);
            //обновляем стек для нового слова
            pointerToCurrentTokenStack = new Stack<BaseMarkdownToken>();
                WordToken wordToken = new WordToken();
                rootToken.Children.Add(wordToken);
                pointerToCurrentTokenStack.Push(wordToken);


                Dictionary<TokenNamesEnum, Queue<int>> characterProcessingSequence = characterProcessingSequenceManager.GetCharacterProcessingSequence(word, allowedTags, isEscapingSupported);
                //foreach (var tag in characterProcessingSequence)
                //{
                //    Console.Write(tag.Key + ":");
                //    foreach (var token in tag.Value)
                //    {
                //        Console.Write(token);
                //    }
                //    Console.WriteLine();
                //}
                StringBuffer readParagraphBuffer = new StringBuffer();

                for (int i = 0; i < word.Length; i++)
                {
                    if (i + 1 < word.Length && word[i] == '_' && word[i + 1] == '_' && !(word.ToString().Any(char.IsDigit) && i != 0 && i != word.Length - 1) && characterProcessingSequence[TokenNamesEnum.Bold].Count > 0 && characterProcessingSequence[TokenNamesEnum.Bold].Dequeue() == 1)
                    {
                        pointerStackManager.ManagePointerStack(word, pointerToCurrentTokenStack, TokenNamesEnum.Bold, readParagraphBuffer);
                        i += 1;
                    }
                    else if (word[i] == '_' && !(word.ToString().Any(char.IsDigit) && i!=0 && i!=word.Length-1) && characterProcessingSequence[TokenNamesEnum.Italics].Count > 0 && characterProcessingSequence[TokenNamesEnum.Italics].Dequeue() == 1)
                    {
                        pointerStackManager.ManagePointerStack(word, pointerToCurrentTokenStack, TokenNamesEnum.Italics, readParagraphBuffer);
                    }
                    else if (characterProcessingSequence[TokenNamesEnum.LinkStart].Count > 0 && characterProcessingSequence[TokenNamesEnum.LinkStart].Peek() == 1 && word[i] == '<')
                    {
                        characterProcessingSequence[TokenNamesEnum.LinkStart].Dequeue(); 
                        pointerStackManager.ManagePointerStack(word, pointerToCurrentTokenStack, TokenNamesEnum.LinkStart, readParagraphBuffer);
                    }
                    else if (characterProcessingSequence[TokenNamesEnum.LinkEnd].Count > 0 && characterProcessingSequence[TokenNamesEnum.LinkEnd].Peek() == 1 && word[i] == '>')
                    {
                        characterProcessingSequence[TokenNamesEnum.LinkEnd].Dequeue();
                        pointerStackManager.ManagePointerStack(word, pointerToCurrentTokenStack, TokenNamesEnum.LinkEnd, readParagraphBuffer);
                    }
                    else if (word[i] != '\\' || word[i] == '\\' && characterProcessingSequence[TokenNamesEnum.Escaping].Count > 0 && characterProcessingSequence[TokenNamesEnum.Escaping].Dequeue() == 0)
                    {
                        readParagraphBuffer.AddSymbol(word[i]);
                    }
                }
                //Разбираемся с оставшимся концом слова
                if (readParagraphBuffer.Buffer != "")
                {
                    pointerToCurrentTokenStack.Peek().Children.Add(new TextToken(readParagraphBuffer.Buffer));
                }
            //}
        }

        return mainToken;
    }
}
