using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class LemmingReader : INeoLemmixDataReader
{
    private readonly List<LemmingData> _lemmingData = new();

    private LemmingData? _currentLemmingData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$LEMMING";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentLemmingData = new LemmingData
        {
            State = 1U << LemmingState.ActiveBitIndex
        };
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        var currentLemmingData = _currentLemmingData!;

        switch (firstToken)
        {
            case "X":
                currentLemmingData.X = int.Parse(secondToken);
                break;

            case "Y":
                currentLemmingData.Y = int.Parse(secondToken);
                break;

            case "FLIP_HORIZONTAL":
                currentLemmingData.FacingDirection = FacingDirection.LeftInstance;
                break;

            case "BLOCKER":
                currentLemmingData.InitialLemmingAction = BlockerAction.Instance;
                break;

            case "CLIMBER":
                currentLemmingData.State |= 1U << LemmingState.ClimberBitIndex;
                break;

            case "DISARMER":
                currentLemmingData.State |= 1U << LemmingState.DisarmerBitIndex;
                break;

            case "FLOATER":
                currentLemmingData.State |= 1U << LemmingState.FloaterBitIndex;
                currentLemmingData.State &= ~(1U << LemmingState.GliderBitIndex); // Deliberately knock out the glider
                break;

            case "GLIDER":
                currentLemmingData.State |= 1U << LemmingState.GliderBitIndex;
                currentLemmingData.State &= ~(1U << LemmingState.FloaterBitIndex); // Deliberately knock out the floater
                break;

            case "NEUTRAL":
                currentLemmingData.State |= 1U << LemmingState.NeutralBitIndex;
                break;

            case "SLIDER":
                currentLemmingData.State |= 1U << LemmingState.SliderBitIndex;
                break;

            case "SWIMMER":
                currentLemmingData.State |= 1U << LemmingState.SwimmerBitIndex;
                break;

            case "ZOMBIE":
                currentLemmingData.State |= 1U << LemmingState.ZombieBitIndex;
                break;

            case "$END":
                _lemmingData.Add(_currentLemmingData!);
                _currentLemmingData = null;
                FinishedReading = true;
                break;

            default:
                ReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
                break;
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