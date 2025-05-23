﻿namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

internal sealed class StateRecoloringReader : NeoLemmixDataReader
{
    // private readonly ThemeData _themeData = new();

    //  private LemmingStateRecoloring? _currentLemmingStateRecoloring;

    private uint? _currentOriginalColor;
    private uint? _currentReplacementColor;

    public StateRecoloringReader()
        : base("$STATE_RECOLORING")
    {

    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        //  _currentLemmingStateRecoloring = null;
        FinishedReading = false;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

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
                   _currentOriginalColor = 0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                   break;

               case "TO":
                   _currentReplacementColor = 0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                   break;

               default:
                   NxlvReadingHelpers.ThrowUnknownTokenException("Gadget Archetype Data", firstToken, line);
                   break;
           }*/

        return false;
    }
}