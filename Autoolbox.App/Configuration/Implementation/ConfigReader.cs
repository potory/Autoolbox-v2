using System.Text;
using Autoolbox.App.Configuration.Abstraction;
using Autoolbox.App.Utility;

namespace Autoolbox.App.Configuration.Implementation;

public class ConfigReader : IConfigReader
{
    private readonly StringBuilder _sb = new();
    private readonly Brackets _brackets = new();

    public IEnumerable<string> Read(string source)
    {
        _sb.Clear();
        _brackets.Clear();
        
        for (int index = 0; index < source.Length; index++)
        {
            index = FindSegmentStart(source, index);

            if (index < 0)
            {
                break;
            }

            StartSegment(source, index);
            index = ReadToSegmentEnd(source, index + 1);
            yield return ReleaseSegment();
        }
    }

    private void StartSegment(string source, int index)
    {
        _brackets.Add(source[index]);
        _sb.Append(source[index]);
    }

    private int ReadToSegmentEnd(string source, int index)
    {
        while (_brackets.Any())
        {
            if (Brackets.IsBracket(source[index]))
            {
                _brackets.Add(source[index]);
            }

            _sb.Append(source[index]);
            index++;
        }

        return index;
    }

    private string ReleaseSegment()
    {
        var segment = _sb.ToString();
        _sb.Clear();
        return segment;
    }

    private static int FindSegmentStart(string source, int from) => source.IndexOf('{', from);
}