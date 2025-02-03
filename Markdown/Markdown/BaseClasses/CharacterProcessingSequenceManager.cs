namespace Markdown.BaseClasses;

public class CharacterProcessingSequenceManager
{ 
    public Dictionary<TokenNamesEnum, Queue<int>> GetCharacterProcessingSequence(string s,HashSet<string> allowedTags,  bool isEscapingSupported)
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
                if ((i+1 <= s.Length - 1 && !allowedTags.Contains(s[i + 1].ToString())) || i == s.Length - 1)
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
}
