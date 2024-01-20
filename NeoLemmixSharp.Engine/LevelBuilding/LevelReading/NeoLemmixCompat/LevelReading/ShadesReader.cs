using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class ShadesReader : INeoLemmixDataReader
{
    private bool _inBlock;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SHADES";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _inBlock = false;
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);

        switch (firstToken)
        {
            case "$SHADE":
                _inBlock = true;
                break;

            case "$END":
                if (_inBlock)
                {
                    _inBlock = false;
                    break;
                }

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