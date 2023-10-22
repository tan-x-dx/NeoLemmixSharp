namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public interface INeoLemmixDataReader
{
    bool FinishedReading { get; }
    string IdentifierToken { get; }

    void BeginReading(string[] tokens);
    void ReadNextLine(string[] tokens);
}