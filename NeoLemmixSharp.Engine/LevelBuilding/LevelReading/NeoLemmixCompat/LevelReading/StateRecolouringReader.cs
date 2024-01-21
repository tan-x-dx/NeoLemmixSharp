using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.SpriteSet;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class StateRecoloringReader : INeoLemmixDataReader
{
    private readonly ThemeData _themeData = new();

    private LemmingStateRecoloring? _currentLemmingStateRecoloring;

    private uint? _currentOriginalColor;
    private uint? _currentReplacementColor;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$STATE_RECOLORING";
    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentLemmingStateRecoloring = null;
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);

        if (firstToken[0] == '$')
        {
            if (firstToken is "$END")
            {
                if (_currentLemmingStateRecoloring == null)
                {
                    FinishedReading = true;
                    return false;
                }

                var tuple = (_currentOriginalColor!.Value, _currentReplacementColor!.Value);
                _currentOriginalColor = null;
                _currentReplacementColor = null;
                _currentLemmingStateRecoloring.Recolorings.Add(tuple);

                _currentLemmingStateRecoloring = null;
                return false;
            }

            if (ReadingHelpers.TryGetWithSpan(_themeData.LemmingStateRecoloringLookup, firstToken, out _currentLemmingStateRecoloring))
                return false;

            _currentLemmingStateRecoloring = new LemmingStateRecoloring(firstToken.GetString());
            _themeData.LemmingStateRecoloringLookup.Add(_currentLemmingStateRecoloring.StateIdentifier, _currentLemmingStateRecoloring);

            return false;
        }

        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        switch (firstToken)
        {
            case "FROM":
                _currentOriginalColor = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "TO":
                _currentReplacementColor = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");
        }

        return false;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
    }

    public void Dispose()
    {
    }
}