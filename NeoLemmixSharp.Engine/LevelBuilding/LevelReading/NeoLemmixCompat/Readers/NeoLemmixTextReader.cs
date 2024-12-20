namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class NeoLemmixTextReader : NeoLemmixDataReader
{
    private readonly List<string> _lines;

    public NeoLemmixTextReader(
        List<string> lines,
        string identifierToken)
        : base(identifierToken)
    {
        _lines = lines;

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
        var parsedLine = line.TrimAfterIndex(secondTokenIndex).ToString();
        _lines.Add(parsedLine);
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        FinishedReading = true;
    }
}