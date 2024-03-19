using System.Collections;

namespace Autoolbox.App.Utility;

public class Brackets : IEnumerable<char>
{
    private readonly Stack<char> _stack = new();

    public int Count => _stack.Count;
    
    public void Add(char c)
    {
        if (!IsBracket(c))
            throw new Exception($"Invalid bracket character: '{c}'");

        if (IsOpeningBracket(c))
        {
            _stack.Push(c);
            return;
        }

        if (_stack.Count == 0)
            throw new Exception("Unmatched closing bracket: " + c);

        var matching = _stack.Pop();

        if (!IsMatchingBracket(matching, c))
            throw new Exception($"Mismatched bracket: '{matching}' and '{c}'");
    }

    public void Clear() => _stack.Clear();

    public static bool IsBracket(char c) => IsOpeningBracket(c) || IsClosingBracket(c);

    private static bool IsOpeningBracket(char c)
    {
        return c is '{' or '(' or '<' or '[';
    }

    private static bool IsClosingBracket(char c)
    {
        return c is '}' or ')' or '>' or ']';
    }

    private static bool IsMatchingBracket(char opening, char closing)
    {
        return (closing, opening) switch
        {
            (')', '(') => true,
            ('}', '{') => true,
            ('>', '<') => true,
            (']', '[') => true,
            _ => false
        };
    }

    public IEnumerator<char> GetEnumerator() => _stack.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}