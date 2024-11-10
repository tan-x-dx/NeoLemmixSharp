using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers;

public sealed class LemmingReader : INeoLemmixDataReader
{
    private readonly List<LemmingData> _prePlacedLemmingData;
    private LemmingData? _currentLemmingData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$LEMMING";

    public LemmingReader(List<LemmingData> prePlacedLemmingData)
    {
        _prePlacedLemmingData = prePlacedLemmingData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentLemmingData = new LemmingData
        {
            // Pre-placed lemmings are always active
            State = 1U << EngineConstants.ActiveBitIndex
        };
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

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
                currentLemmingData.State |= 1U << EngineConstants.ClimberBitIndex;
                break;

            case "DISARMER":
                currentLemmingData.State |= 1U << EngineConstants.DisarmerBitIndex;
                break;

            case "FLOATER":
                currentLemmingData.State |= 1U << EngineConstants.FloaterBitIndex;
                currentLemmingData.State &= ~(1U << EngineConstants.GliderBitIndex); // Deliberately knock out the glider
                break;

            case "GLIDER":
                currentLemmingData.State |= 1U << EngineConstants.GliderBitIndex;
                currentLemmingData.State &= ~(1U << EngineConstants.FloaterBitIndex); // Deliberately knock out the floater
                break;

            case "NEUTRAL":
                currentLemmingData.State |= 1U << EngineConstants.NeutralBitIndex;
                break;

            case "SHIMMIER":
                currentLemmingData.InitialLemmingAction = ShimmierAction.Instance;
                break;

            case "SLIDER":
                currentLemmingData.State |= 1U << EngineConstants.SliderBitIndex;
                break;

            case "SWIMMER":
                currentLemmingData.State |= 1U << EngineConstants.SwimmerBitIndex;
                break;

            case "ZOMBIE":
                currentLemmingData.State |= 1U << EngineConstants.ZombieBitIndex;
                break;

            case "$END":
                _prePlacedLemmingData.Add(currentLemmingData);
                _currentLemmingData = null;
                FinishedReading = true;
                break;

            default:
                NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
                break;
        }

        return false;
    }
}