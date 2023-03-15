namespace NeoLemmixSharp.LevelBuilding.Reading;

public sealed class GadgetReader : IDataReader
{
    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$GADGET";
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