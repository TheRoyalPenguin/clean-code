using Markdown.AbstractClasses;
using Markdown.Tags;

namespace Markdown.BaseClasses;
public class Tokenizer
{
    bool isEscapingSupported;
    HashSet<string> allowedTags;
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
        foreach (var markdownTextParagraph in markdownTextParagraphs)
        {
            var pointerToCurrentTokenStack = new Stack<BaseMarkdownToken>();
            string[] wordsMarkdownTextParagraph = markdownTextParagraph.Split(" ");

            if (markdownTextParagraph != null && markdownTextParagraph.Length != 0 && markdownTextParagraph[0] == '#')
            {
                rootToken = new HeaderToken();
                pointerToCurrentTokenStack.Push(rootToken);
                wordsMarkdownTextParagraph[0] = wordsMarkdownTextParagraph[0].Substring(1);
            }
            else
            {
                rootToken = new ParagraphToken();
                pointerToCurrentTokenStack.Push(rootToken);
            }

            mainToken.Children.Add(rootToken);

            foreach (var word in wordsMarkdownTextParagraph)
            {
                //обновляем стек для нового слова
                pointerToCurrentTokenStack = new Stack<BaseMarkdownToken>();
                WordToken wordToken = new WordToken();
                rootToken.Children.Add(wordToken);
                pointerToCurrentTokenStack.Push(wordToken);


                Dictionary<TokenNamesEnum, Queue<int>> characterProcessingSequence = GetCharacterProcessingSequence(word);
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
                        ManagePointerStack(word, pointerToCurrentTokenStack, TokenNamesEnum.Bold, readParagraphBuffer);
                        i += 1;
                    }
                    else if (word[i] == '_' && !(word.ToString().Any(char.IsDigit) && i!=0 && i!=word.Length-1) && characterProcessingSequence[TokenNamesEnum.Italics].Count > 0 && characterProcessingSequence[TokenNamesEnum.Italics].Dequeue() == 1)
                    {
                        ManagePointerStack(word, pointerToCurrentTokenStack, TokenNamesEnum.Italics, readParagraphBuffer);
                    }
                    else if (characterProcessingSequence[TokenNamesEnum.LinkStart].Count > 0 && characterProcessingSequence[TokenNamesEnum.LinkStart].Peek() == 1 && word[i] == '<')
                    {
                        characterProcessingSequence[TokenNamesEnum.LinkStart].Dequeue();
                        ManagePointerStack(word, pointerToCurrentTokenStack, TokenNamesEnum.LinkStart, readParagraphBuffer);
                    }
                    else if (characterProcessingSequence[TokenNamesEnum.LinkEnd].Count > 0 && characterProcessingSequence[TokenNamesEnum.LinkEnd].Peek() == 1 && word[i] == '>')
                    {
                        characterProcessingSequence[TokenNamesEnum.LinkEnd].Dequeue();
                        ManagePointerStack(word, pointerToCurrentTokenStack, TokenNamesEnum.LinkEnd, readParagraphBuffer);
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
            }
        }

        return mainToken;
    }
    void ManagePointerStack(string markdownTextParagraph, Stack<BaseMarkdownToken> pointerToCurrentTokenStack, TokenNamesEnum tokenName, StringBuffer readParagraphBuffer)
    {
        if (tokenName == TokenNamesEnum.Bold)
        {
            if (pointerToCurrentTokenStack.Peek().TokenName == TokenNamesEnum.Bold) // Закрытие токена
            {
                var tempToken = new TextToken(readParagraphBuffer.Buffer);
                readParagraphBuffer.Clear();
                ((BoldToken)pointerToCurrentTokenStack.Peek()).ChangeStatus(DoubleTagStatusEnum.Close);
                pointerToCurrentTokenStack.Pop().Children.Add(tempToken);
            }
            else // Создание токена
            {
                if (GetElementOfStackByIndex(pointerToCurrentTokenStack, 1) != null && GetElementOfStackByIndex(pointerToCurrentTokenStack, 1).TokenName == TokenNamesEnum.Bold) //Когда предпредыдущий тег был __
                {
                    pointerToCurrentTokenStack.Pop();
                    readParagraphBuffer.AddSymbolToStartingString('_');
                }

                if (readParagraphBuffer.Buffer.Length > 0)
                {
                    var tempToken1 = new TextToken(readParagraphBuffer.Buffer);
                    readParagraphBuffer.Clear();
                    pointerToCurrentTokenStack.Peek().Children.Add(tempToken1);
                }

                var tempToken = new BoldToken(DoubleTagStatusEnum.Open);
                pointerToCurrentTokenStack.Peek().Children.Add(tempToken);
                pointerToCurrentTokenStack.Push(tempToken);
            }
        }
        else if (tokenName == TokenNamesEnum.Italics)
        {
            if (pointerToCurrentTokenStack.Peek().TokenName == TokenNamesEnum.Italics) // Закрытие токена
            {
                var tempToken = new TextToken(readParagraphBuffer.Buffer);
                readParagraphBuffer.Clear();
                ((ItalicsToken)pointerToCurrentTokenStack.Peek()).ChangeStatus(DoubleTagStatusEnum.Close);
                pointerToCurrentTokenStack.Pop().Children.Add(tempToken);
            }
            else // Создание токена
            {
                if (readParagraphBuffer.Buffer.Length > 0)
                {
                    var tempToken1 = new TextToken(readParagraphBuffer.Buffer);
                    readParagraphBuffer.Clear();
                    pointerToCurrentTokenStack.Peek().Children.Add(tempToken1);
                }
                var tempToken = new ItalicsToken(DoubleTagStatusEnum.Open);
                pointerToCurrentTokenStack.Peek().Children.Add(tempToken);
                pointerToCurrentTokenStack.Push(tempToken);
            }
        }
        else if (tokenName == TokenNamesEnum.LinkStart)
        {
            if (readParagraphBuffer.Buffer.Length > 0)
            {
                var tempToken1 = new TextToken(readParagraphBuffer.Buffer);
                readParagraphBuffer.Clear();
                pointerToCurrentTokenStack.Peek().Children.Add(tempToken1);
            }

            var tempToken = new LinkToken(TokenNamesEnum.LinkStart);
            pointerToCurrentTokenStack.Peek().Children.Add(tempToken);
            pointerToCurrentTokenStack.Push(tempToken);
        }
        else if (tokenName == TokenNamesEnum.LinkEnd)
        {
            if (pointerToCurrentTokenStack.Peek().TokenName == TokenNamesEnum.LinkStart)
            {
                var tempToken = new TextToken(readParagraphBuffer.Buffer);
                readParagraphBuffer.Clear();
                pointerToCurrentTokenStack.Pop().Children.Add(tempToken);
            }
        }
    }
    Dictionary<TokenNamesEnum, Queue<int>> GetCharacterProcessingSequence(string s)
    {
        // Храним последовательность из 0 и 1, где 1 - тег нужно обработать как тег, а 0 - тег нужно обработать как обычный символ
        Dictionary<TokenNamesEnum, Queue<int>> characterProcessingSequence = new Dictionary<TokenNamesEnum, Queue<int>>
        { 
            {TokenNamesEnum.Bold, new Queue<int>()},
            {TokenNamesEnum.Italics, new Queue<int>()}, 
            {TokenNamesEnum.LinkStart, new Queue<int>()}, 
            {TokenNamesEnum.LinkEnd, new Queue<int>()},
            {TokenNamesEnum.Escaping, new Queue<int>()}
        };

        Dictionary<TokenNamesEnum, int> tagsFound = new Dictionary<TokenNamesEnum, int> { { TokenNamesEnum.Bold, 0 }, { TokenNamesEnum.Italics, 0 }, { TokenNamesEnum.LinkStart, 0 }, { TokenNamesEnum.LinkEnd, 0 }, { TokenNamesEnum.Escaping, 0 } };
        string lastTag = "";
        int consecutiveEscapeCharactersCount = 0;

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '\\')
            {
                //Если нечего экранировать символу экранирования
                if (i+1 <= s.Length - 1 && !allowedTags.Contains(s[i + 1].ToString()))
                {
                    characterProcessingSequence[TokenNamesEnum.Escaping].Enqueue(0);
                }
                else
                {
                    if (consecutiveEscapeCharactersCount % 2 != 0)
                    {
                        characterProcessingSequence[TokenNamesEnum.Escaping].Enqueue(0);
                    }
                    else
                    {
                        characterProcessingSequence[TokenNamesEnum.Escaping].Enqueue(1);
                    }
                }
                consecutiveEscapeCharactersCount++;
            }
            else if (!(lastTag == "_" && tagsFound[TokenNamesEnum.Italics] % 2 != 0) && lastTag != "<" && ((i == 0 || !isEscapingSupported) || (isEscapingSupported && i > 0 && !(s[i - 1] == '\\' && consecutiveEscapeCharactersCount%2!=0))) && i + 1 < s.Length && s[i] == '_' && s[i + 1] == '_')
            {
                characterProcessingSequence[TokenNamesEnum.Bold].Enqueue(1);
                lastTag = "__";
                i += 1;
            }
            else if (lastTag != "<" && s[i] == '_' && (!isEscapingSupported || i == 0 || i > 0 && s[i - 1] != '\\'))
            {
                lastTag = "_";
                tagsFound[TokenNamesEnum.Italics]++;
                characterProcessingSequence[TokenNamesEnum.Italics].Enqueue(1);
            }
            else if (s[i] == '_')
            {
                characterProcessingSequence[TokenNamesEnum.Italics].Enqueue(0);
            }
            else if (s[i] == '>' && (!isEscapingSupported || i == 0 || i > 0 && s[i - 1] != '\\'))
            {
                lastTag = ">";
                characterProcessingSequence[TokenNamesEnum.LinkStart].Enqueue(1);
            }
            else if (lastTag != "<" && s[i] == '<' && (!isEscapingSupported || i == 0 || i > 0 && s[i - 1] != '\\'))
            {
                lastTag = "<";
                characterProcessingSequence[TokenNamesEnum.LinkEnd].Enqueue(1);
            }
            else if (s[i] == '<')
            {
                characterProcessingSequence[TokenNamesEnum.LinkStart].Enqueue(0);
            }
            // сбрасываем последовательность символов экранирования
            if (s[i] != '\\')
            {
                consecutiveEscapeCharactersCount = 0;
            }
            // дополнительно помечаем не обрабатывать тег "__"
            if (i + 1 < s.Length && s[i] == '_' && s[i + 1] == '_')
            {
                characterProcessingSequence[TokenNamesEnum.Bold].Enqueue(0);
            }
        }
        return characterProcessingSequence;
    }
    T? GetElementOfStackByIndex<T>(Stack<T> stack, int index)
    {
        List<T> extractedElementsOfStack = new List<T>();
        if (index >= 0 && stack.Count > 0)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                T stackElement = stack.Pop();
                extractedElementsOfStack.Add(stackElement);
                if (i == index)
                {
                    for (int j = extractedElementsOfStack.Count - 1; j >= 0; j--)
                    {
                        stack.Push(extractedElementsOfStack[j]);
                    }
                    return stackElement;
                }
            }
        }
        for (int j = extractedElementsOfStack.Count - 1; j >= 0; j--)
        {
            stack.Push(extractedElementsOfStack[j]);
        }
        return default;
    }
}
