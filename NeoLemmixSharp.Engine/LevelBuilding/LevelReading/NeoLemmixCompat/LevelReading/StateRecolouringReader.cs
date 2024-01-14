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
    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentLemmingStateRecoloring = null;
        FinishedReading = false;
    }

    public void ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        if (firstToken[0] == '$')
        {
            if (firstToken is "$END")
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

            if (ReadingHelpers.TryGetWithSpan(_themeData.LemmingStateRecoloringLookup, firstToken, out _currentLemmingStateRecoloring))
                return;

            _currentLemmingStateRecoloring = new LemmingStateRecoloring(firstToken.ToString());
            _themeData.LemmingStateRecoloringLookup.Add(_currentLemmingStateRecoloring.StateIdentifier, _currentLemmingStateRecoloring);

            return;
        }

        switch (firstToken)
        {
            case "FROM":
                _currentOriginalColor = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "TO":
                _currentReplacementColor = ReadingHelpers.ReadUint(secondToken, false);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");
        }
    }
}