using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.SpriteSet;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class StateRecoloringReader : INeoLemmixDataReader
{
    private readonly ThemeData _themeData;

    private LemmingStateRecoloring? _currentLemmingStateRecoloring;

    private uint? _currentOriginalColor;
    private uint? _currentReplacementColor;

    public StateRecoloringReader(ThemeData themeData)
    {
        _themeData = themeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$STATE_RECOLORING";
    public void BeginReading(string[] tokens)
    {
        _currentLemmingStateRecoloring = null;
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        if (tokens[0][0] == '$')
        {
            if (tokens[0] == "$END")
            {
                if (_currentLemmingStateRecoloring == null)
                {
                    FinishedReading = true;
                    return;
                }

                var tuple = (_currentOriginalColor!.Value, _currentReplacementColor!.Value);
                _currentOriginalColor = null;
                _currentReplacementColor = null;
                _currentLemmingStateRecoloring.Recolorings.Add(tuple);

                _currentLemmingStateRecoloring = null;
                return;
            }

            if (_themeData.LemmingStateRecoloringLookup.TryGetValue(tokens[0], out _currentLemmingStateRecoloring))
                return;

            _currentLemmingStateRecoloring = new LemmingStateRecoloring(tokens[0]);
            _themeData.LemmingStateRecoloringLookup.Add(_currentLemmingStateRecoloring.StateIdentifier, _currentLemmingStateRecoloring);

            return;
        }

        switch (tokens[0])
        {
            case "FROM":
                _currentOriginalColor = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "TO":
                _currentReplacementColor = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");
        }
    }
}