namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public interface INeoLemmixDataReader
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
}