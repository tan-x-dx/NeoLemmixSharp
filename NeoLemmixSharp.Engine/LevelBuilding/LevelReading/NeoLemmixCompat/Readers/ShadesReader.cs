namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class ShadesReader : NeoLemmixDataReader
{
    private bool _inBlock;

    public ShadesReader()
        : base("$SHADES")
    {

    }

    public override void BeginReading(ReadOnlySpan<char> line)
    {
        _inBlock = false;
        FinishedReading = false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out _, out _);

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
}