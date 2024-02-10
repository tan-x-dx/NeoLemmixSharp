using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

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
            State = 1U << LemmingState.ActiveBitIndex
        };
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

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
                _prePlacedLemmingData.Add(currentLemmingData);
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
        var totalNumberOfLemmings = Math.Max(levelData.NumberOfLemmings, _prePlacedLemmingData.Count);
        _prePlacedLemmingData.Capacity = totalNumberOfLemmings;
        levelData.NumberOfLemmings = totalNumberOfLemmings;

        for (var i = _prePlacedLemmingData.Count; i < totalNumberOfLemmings; i++)
        {
            var newLemmingData = new LemmingData();

            _prePlacedLemmingData.Add(newLemmingData);
        }
    }

    public void Dispose()
    {
    }
}