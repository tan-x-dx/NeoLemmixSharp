namespace NeoLemmixSharp.Io.LevelReading.Reading;

public sealed class SkillSetReader : IDataReader
{
    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SKILLSET";
    public void BeginReading(string[] tokens)
    {
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        if (tokens[0] == "$END")
            FinishedReading = true;
    }
}