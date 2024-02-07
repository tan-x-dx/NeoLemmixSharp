using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

public sealed class TextReader : INeoLemmixDataReader
{
    private readonly List<string> _lines;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken { get; }

    public TextReader(List<string> lines, string identifierToken)
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
                var pretextLine = line.TrimAfterIndex(secondTokenIndex).GetString();
                _lines.Add(pretextLine);
                break;

            case "$END":
                FinishedReading = true;
                break;
        }

        return false;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
    }

    public void Dispose()
    {
    }
}