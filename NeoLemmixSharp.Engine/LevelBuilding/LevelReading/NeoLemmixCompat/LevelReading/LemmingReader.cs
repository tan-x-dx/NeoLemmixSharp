using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class LemmingReader : INeoLemmixDataReader
{
    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$LEMMING";
    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        switch (firstToken)
        {
            case "$END":
                FinishedReading = true;
                break;

                /*  default:
                      throw new InvalidOperationException(
                          $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");*/
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