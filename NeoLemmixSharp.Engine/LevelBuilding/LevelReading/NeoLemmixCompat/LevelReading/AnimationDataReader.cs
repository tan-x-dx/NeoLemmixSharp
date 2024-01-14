using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.SpriteSet;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class AnimationDataReader : INeoLemmixDataReader
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

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentLemmingSpriteData = null;
        _footSetter = FootSetter.None;
        FinishedReading = false;
    }

    public void ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);

        if (line[0] == '$')
        {
            switch (firstToken)
            {
                case "$RIGHT":
                    _footSetter = FootSetter.RightFoot;
                    return;
                case "$LEFT":
                    _footSetter = FootSetter.LeftFoot;
                    return;
                case "$END" when _currentLemmingSpriteData == null:
                    FinishedReading = true;
                    return;
                case "$END" when _footSetter != FootSetter.None:
                    _footSetter = FootSetter.None;
                    return;
                case "$END":
                    _currentLemmingSpriteData = null;
                    break;
            }

            _currentLemmingSpriteData = new LemmingSpriteData(firstToken.ToString());
            _themeData.LemmingSpriteDataLookup.Add(_currentLemmingSpriteData.AnimationIdentifier, _currentLemmingSpriteData);

            return;
        }

        var secondToken = ReadingHelpers.GetToken(line, 1, out _);
        switch (firstToken)
        {
            case "FRAMES":
                _currentLemmingSpriteData!.NumberOfFrames = ReadingHelpers.ReadInt(secondToken);
                return;
            case "LOOP_TO_FRAME":
                _currentLemmingSpriteData!.LoopToFrame = ReadingHelpers.ReadInt(secondToken);
                return;
            case "PEAK_FRAME":
                _currentLemmingSpriteData!.PeakFrame = ReadingHelpers.ReadInt(secondToken);
                return;
            case "FOOT_X":
                switch (_footSetter)
                {
                    case FootSetter.None:
                        throw new InvalidOperationException("No foot setting type selected!");
                    case FootSetter.LeftFoot:
                        _currentLemmingSpriteData!.LeftFootX = ReadingHelpers.ReadInt(secondToken);
                        break;
                    case FootSetter.RightFoot:
                        _currentLemmingSpriteData!.RightFootX = ReadingHelpers.ReadInt(secondToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return;

            case "FOOT_Y":
                switch (_footSetter)
                {
                    case FootSetter.None:
                        throw new InvalidOperationException("No foot setting type selected!");
                    case FootSetter.LeftFoot:
                        _currentLemmingSpriteData!.LeftFootY = ReadingHelpers.ReadInt(secondToken);
                        break;
                    case FootSetter.RightFoot:
                        _currentLemmingSpriteData!.RightFootY = ReadingHelpers.ReadInt(secondToken);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return;
        }

        throw new InvalidOperationException(
            $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");
    }

    private enum FootSetter
    {
        None,
        LeftFoot,
        RightFoot
    }
}