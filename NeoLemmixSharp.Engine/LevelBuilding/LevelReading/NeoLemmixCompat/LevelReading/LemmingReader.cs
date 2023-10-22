namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class LemmingReader : INeoLemmixDataReader
{
    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$LEMMING";
    public void BeginReading(string[] tokens)
    {
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        switch (tokens[0])
        {
            case "$END":
                FinishedReading = true;
                break;

                /*  default:
                      throw new InvalidOperationException(
                          $"Unknown token when parsing {IdentifierToken}: [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");*/
        }
    }
}