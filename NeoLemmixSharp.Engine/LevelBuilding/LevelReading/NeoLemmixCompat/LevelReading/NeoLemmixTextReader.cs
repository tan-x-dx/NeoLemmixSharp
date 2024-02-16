namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class NeoLemmixTextReader : INeoLemmixDataReader
{
    private readonly List<string> _lines;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken { get; }

    public NeoLemmixTextReader(List<string> lines, string identifierToken)
    {
        _lines = lines;
        IdentifierToken = identifierToken;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out _, out var secondTokenIndex);

        switch (firstToken)
        {
            case "LINE":
                var parsedLine = line.TrimAfterIndex(secondTokenIndex).GetString();
                _lines.Add(parsedLine);
                break;

            case "$END":
                FinishedReading = true;
                break;
        }

        return false;
    }
}