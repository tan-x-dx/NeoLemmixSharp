namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public interface INeoLemmixDataReader
{
    bool FinishedReading { get; }
    string IdentifierToken { get; }

    void BeginReading(ReadOnlySpan<char> line);
    void ReadNextLine(ReadOnlySpan<char> line);
}