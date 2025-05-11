using NeoLemmixSharp.Common.Util.Collections;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

public sealed class NeoLemmixTextReader : NeoLemmixDataReader
{
    private readonly UniqueStringSet _uniqueStringSet;
    private readonly List<string> _lines;

    public NeoLemmixTextReader(
        UniqueStringSet uniqueStringSet,
        List<string> lines,
        string identifierToken)
        : base(identifierToken)
    {
        _uniqueStringSet = uniqueStringSet;
        _lines = lines;

        SetNumberOfTokens(2);

        RegisterTokenAction("LINE", AddLine);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
        return false;
    }

    private void AddLine(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var parsedLine = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
        _lines.Add(parsedLine);
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        FinishedReading = true;
    }
}