using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class GadgetStateArchetypeData
{
    public required IGadgetAction[] OnLemmingEnterActions { get; init; }
    public required IGadgetAction[] OnLemmingPresentActions { get; init; }
    public required IGadgetAction[] OnLemmingExitActions { get; init; }

    public required RectangularTriggerData? TriggerData { get; init; }

    public SimpleSet<LemmingAction>? AllowedActions { get; init; }
    public SimpleSet<ILemmingStateChanger>? AllowedStates { get; init; }
    public SimpleSet<Orientation>? AllowedOrientations { get; init; }
    public FacingDirection? AllowedFacingDirection { get; init; }
}