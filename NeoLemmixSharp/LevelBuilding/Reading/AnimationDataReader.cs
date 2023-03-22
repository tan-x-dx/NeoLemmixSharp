using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.LevelBuilding.Data.SpriteSet;
using System;

namespace NeoLemmixSharp.LevelBuilding.Reading;

public sealed class AnimationDataReader : IDataReader
{
    private readonly ThemeData _themeData;

    private LemmingSpriteData? _currentLemmingSpriteData;
    private FootSetter _footSetter;

    public AnimationDataReader(ThemeData themeData)
    {
        _themeData = themeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$ANIMATIONS";
    public void BeginReading(string[] tokens)
    {
        _currentLemmingSpriteData = null;
        _footSetter = FootSetter.None;
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        if (tokens[0][0] == '$')
        {
            switch (tokens[0])
            {
                case "$RIGHT":
                    _footSetter = FootSetter.RightFoot;
                    return;

                case "$LEFT":
                    _footSetter = FootSetter.LeftFoot;
                    return;

                case "$END":
                    if (_currentLemmingSpriteData == null)
                    {
                        FinishedReading = true;
                        return;
                    }

                    if (_footSetter != FootSetter.None)
                    {
                        _footSetter = FootSetter.None;
                        return;
                    }

                    _currentLemmingSpriteData = null;
                    return;
            }

            _currentLemmingSpriteData = new LemmingSpriteData(tokens[0]);
            _themeData.LemmingSpriteDataLookup.Add(_currentLemmingSpriteData.AnimationIdentifier, _currentLemmingSpriteData);

            return;
        }

        switch (tokens[0])
        {
            case "FRAMES":
                _currentLemmingSpriteData!.NumberOfFrames = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "LOOP_TO_FRAME":
                _currentLemmingSpriteData!.LoopToFrame = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "PEAK_FRAME":
                _currentLemmingSpriteData!.PeakFrame = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "FOOT_X":
                switch (_footSetter)
                {
                    case FootSetter.None:
                        throw new InvalidOperationException("No foot setting type selected!");
                    case FootSetter.LeftFoot:
                        _currentLemmingSpriteData!.LeftFootX = ReadingHelpers.ReadInt(tokens[1]);
                        break;
                    case FootSetter.RightFoot:
                        _currentLemmingSpriteData!.RightFootX = ReadingHelpers.ReadInt(tokens[1]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;

            case "FOOT_Y":

                switch (_footSetter)
                {
                    case FootSetter.None:
                        throw new InvalidOperationException("No foot setting type selected!");
                    case FootSetter.LeftFoot:
                        _currentLemmingSpriteData!.LeftFootY = ReadingHelpers.ReadInt(tokens[1]);
                        break;
                    case FootSetter.RightFoot:
                        _currentLemmingSpriteData!.RightFootY = ReadingHelpers.ReadInt(tokens[1]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");
        }
    }

    private enum FootSetter
    {
        None,
        LeftFoot,
        RightFoot
    }
}