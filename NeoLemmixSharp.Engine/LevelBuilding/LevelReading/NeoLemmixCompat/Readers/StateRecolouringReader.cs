using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class StateRecoloringReader : INeoLemmixDataReader
{
   // private readonly ThemeData _themeData = new();

  //  private LemmingStateRecoloring? _currentLemmingStateRecoloring;

    private uint? _currentOriginalColor;
    private uint? _currentReplacementColor;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$STATE_RECOLORING";
    public void BeginReading(ReadOnlySpan<char> line)
    {
      //  _currentLemmingStateRecoloring = null;
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

     /*   if (firstToken[0] == '$')
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

            var key = firstToken.GetString();

            if (_themeData.LemmingStateRecoloringLookup.TryGetValue(key, out _currentLemmingStateRecoloring))
                return false;

            _currentLemmingStateRecoloring = new LemmingStateRecoloring(key);
            _themeData.LemmingStateRecoloringLookup.Add(_currentLemmingStateRecoloring.StateIdentifier, _currentLemmingStateRecoloring);

            return false;
        }

        switch (firstToken)
        {
            case "FROM":
                _currentOriginalColor = 0xff000000U | ReadingHelpers.ParseHex<uint>(secondToken);
                break;

            case "TO":
                _currentReplacementColor = 0xff000000U | ReadingHelpers.ParseHex<uint>(secondToken);
                break;

            default:
                ReadingHelpers.ThrowUnknownTokenException("Gadget Archetype Data", firstToken, line);
                break;
        }*/

        return false;
    }
}