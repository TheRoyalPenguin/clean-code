using Markdown.AbstractClasses;
using Markdown.Tags;

namespace Markdown.BaseClasses;

public class PointerStackManager
{
    public void ManagePointerStack(string markdownTextParagraph, Stack<BaseMarkdownToken> pointerToCurrentTokenStack, TokenNamesEnum tokenName, StringBuffer readParagraphBuffer)
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
    public T? GetElementOfStackByIndex<T>(Stack<T> stack, int index)
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
