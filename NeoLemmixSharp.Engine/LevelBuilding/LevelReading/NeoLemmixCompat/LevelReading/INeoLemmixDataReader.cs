using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public interface INeoLemmixDataReader : IDisposable
{
    bool FinishedReading { get; }
    string IdentifierToken { get; }

    bool MatchesToken(ReadOnlySpan<char> token)
    {
        var itemSpan = IdentifierToken.AsSpan();

        return itemSpan.SequenceEqual(token);
    }

    void BeginReading(ReadOnlySpan<char> line);
    bool ReadNextLine(ReadOnlySpan<char> line);

    void ApplyToLevelData(LevelData levelData);
}