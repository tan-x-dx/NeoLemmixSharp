namespace NeoLemmixSharp.Io.LevelReading.Reading;

public sealed class ShadesReader : IDataReader
{
    private bool _inBlock;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SHADES";

    public void BeginReading(string[] tokens)
    {
        _inBlock = false;
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        switch (tokens[0])
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
    }
}