using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public sealed class HitBoxData
{
    public required LemmingSolidityType SolidityType { get; init; }
    public required HitBoxBehaviour HitBoxBehaviour { get; init; }

    public required GadgetActionData[] OnLemmingEnterActions { get; init; }
    public required GadgetActionData[] OnLemmingPresentActions { get; init; }
    public required GadgetActionData[] OnLemmingExitActions { get; init; }

    public required uint[] AllowedLemmingActionIds { get; init; }
    public required uint[] AllowedLemmingStateIds { get; init; }
    public required byte AllowedLemmingOrientationIds { get; init; }
    public required byte AllowedFacingDirectionId { get; init; }
}