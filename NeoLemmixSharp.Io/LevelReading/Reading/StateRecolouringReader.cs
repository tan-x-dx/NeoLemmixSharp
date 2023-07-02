using NeoLemmixSharp.Io.LevelReading.Data;
using NeoLemmixSharp.Io.LevelReading.Data.SpriteSet;

namespace NeoLemmixSharp.Io.LevelReading.Reading;

public sealed class StateRecolouringReader : IDataReader
{
    private readonly ThemeData _themeData;

    private LemmingStateRecolouring? _currentLemmingStateRecolouring;

    private uint? _currentOriginalColour;
    private uint? _currentReplacementColour;

    public StateRecolouringReader(ThemeData themeData)
    {
        _themeData = themeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$STATE_RECOLORING";
    public void BeginReading(string[] tokens)
    {
        _currentLemmingStateRecolouring = null;
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        if (tokens[0][0] == '$')
        {
            if (tokens[0] == "$END")
            {
                if (_currentLemmingStateRecolouring == null)
                {
                    FinishedReading = true;
                    return;
                }

                var tuple = (_currentOriginalColour!.Value, _currentReplacementColour!.Value);
                _currentOriginalColour = null;
                _currentReplacementColour = null;
                _currentLemmingStateRecolouring.Recolourings.Add(tuple);

                _currentLemmingStateRecolouring = null;
                return;
            }

            if (_themeData.LemmingStateRecolouringLookup.TryGetValue(tokens[0], out _currentLemmingStateRecolouring))
                return;

            _currentLemmingStateRecolouring = new LemmingStateRecolouring(tokens[0]);
            _themeData.LemmingStateRecolouringLookup.Add(_currentLemmingStateRecolouring.StateIdentifier, _currentLemmingStateRecolouring);

            return;
        }

        switch (tokens[0])
        {
            case "FROM":
                _currentOriginalColour = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "TO":
                _currentReplacementColour = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");
        }
    }
}