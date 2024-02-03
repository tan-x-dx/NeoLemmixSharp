using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

/// <summary>
/// Represents a reader that does nothing. Useful for skipping sections that will not be used.
/// </summary>
public sealed class DudDataReader : INeoLemmixDataReader
{
    private int _indentationLevel;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken { get; }

    public DudDataReader(string identifierToken)
    {
        IdentifierToken = identifierToken;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _indentationLevel = 1;
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out _, out _);

        if (firstToken[0] != '$')
            return false;

        if (firstToken is "$END")
        {
            _indentationLevel--;

            if (_indentationLevel == 0)
            {
                FinishedReading = true;
            }

            return false;
        }

        _indentationLevel++;

        return false;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
    }

    public void Dispose()
    {
    }
}