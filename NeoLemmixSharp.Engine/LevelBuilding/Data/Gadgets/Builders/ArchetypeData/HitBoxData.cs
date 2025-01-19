using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class HitBoxData
{
    public required LemmingSolidityType SolidityType { get; init; }
    public required HitBoxBehaviour HitBoxBehaviour { get; init; }

    public LemmingActionSet? AllowedActions { get; init; }
    public StateChangerSet? AllowedStates { get; init; }
    public OrientationSet? AllowedOrientations { get; init; }
    public FacingDirection? AllowedFacingDirection { get; init; }

    public required IGadgetAction[] OnLemmingEnterActions { get; init; }
    public required IGadgetAction[] OnLemmingPresentActions { get; init; }
    public required IGadgetAction[] OnLemmingExitActions { get; init; }
}