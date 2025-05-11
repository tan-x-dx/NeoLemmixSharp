using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class HitBoxData
{
    public required LemmingSolidityType SolidityType { get; init; }
    public required HitBoxBehaviour HitBoxBehaviour { get; init; }

    /* public required IGadgetAction[] OnLemmingEnterActions { get; init; }
     public required IGadgetAction[] OnLemmingPresentActions { get; init; }
     public required IGadgetAction[] OnLemmingExitActions { get; init; }

     public required LemmingActionSet? AllowedActions { get; init; }
     public required StateChangerSet? AllowedStates { get; init; }
     public required OrientationSet? AllowedOrientations { get; init; }*/
    public required FacingDirection? AllowedFacingDirection { get; init; }
}