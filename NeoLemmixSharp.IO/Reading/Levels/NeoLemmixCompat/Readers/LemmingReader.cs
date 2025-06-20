using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

internal sealed class LemmingReader : NeoLemmixDataReader
{
    private readonly List<LemmingData> _prePlacedLemmingData;
    private LemmingData? _currentLemmingData;

    public LemmingReader(
        List<LemmingData> prePlacedLemmingData)
        : base("$LEMMING")
    {
        _prePlacedLemmingData = prePlacedLemmingData;

        SetNumberOfTokens(14);

        RegisterTokenAction("X", SetLemmingX);
        RegisterTokenAction("Y", SetLemmingY);
        RegisterTokenAction("FLIP_HORIZONTAL", SetFlipHorizontal);
        RegisterTokenAction("BLOCKER", SetBlocker);
        RegisterTokenAction("CLIMBER", SetClimber);
        RegisterTokenAction("DISARMER", SetDisarmer);
        RegisterTokenAction("FLOATER", SetFloater);
        RegisterTokenAction("GLIDER", SetGlider);
        RegisterTokenAction("NEUTRAL", SetNeutral);
        RegisterTokenAction("SHIMMIER", SetShimmier);
        RegisterTokenAction("SLIDER", SetSlider);
        RegisterTokenAction("SWIMMER", SetSwimmer);
        RegisterTokenAction("ZOMBIE", SetZombie);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        _currentLemmingData = new LemmingData
        {
            // Pre-placed lemmings are always active
            State = 1U << EngineConstants.ActiveBitIndex
        };
        FinishedReading = false;
        return false;
    }

    private void SetLemmingX(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var x = int.Parse(secondToken);
        _currentLemmingData!.Position = new Point(x, _currentLemmingData.Position.Y);
    }

    private void SetLemmingY(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var y = int.Parse(secondToken);
        _currentLemmingData!.Position = new Point(_currentLemmingData.Position.X, y);
    }

    private void SetFlipHorizontal(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.FacingDirection = FacingDirection.Left;
    }

    private void SetBlocker(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.InitialLemmingActionId = LemmingActionConstants.BlockerActionId;
    }

    private void SetClimber(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.State |= 1U << EngineConstants.ClimberBitIndex;
    }

    private void SetDisarmer(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.State |= 1U << EngineConstants.DisarmerBitIndex;
    }

    private void SetFloater(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.State |= 1U << EngineConstants.FloaterBitIndex;
        _currentLemmingData!.State &= ~(1U << EngineConstants.GliderBitIndex); // Deliberately knock out the glider
    }

    private void SetGlider(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.State |= 1U << EngineConstants.GliderBitIndex;
        _currentLemmingData!.State &= ~(1U << EngineConstants.FloaterBitIndex); // Deliberately knock out the floater
    }

    private void SetNeutral(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.State |= 1U << EngineConstants.NeutralBitIndex;
    }

    private void SetShimmier(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.InitialLemmingActionId = LemmingActionConstants.ShimmierActionId;
    }

    private void SetSlider(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.State |= 1U << EngineConstants.SliderBitIndex;
    }

    private void SetSwimmer(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.State |= 1U << EngineConstants.SwimmerBitIndex;
    }

    private void SetZombie(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentLemmingData!.State |= 1U << EngineConstants.ZombieBitIndex;
    }

    private void OnEnd(ReadOnlySpan<char> span, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _prePlacedLemmingData.Add(_currentLemmingData!);
        _currentLemmingData = null;
        FinishedReading = true;
    }
}
