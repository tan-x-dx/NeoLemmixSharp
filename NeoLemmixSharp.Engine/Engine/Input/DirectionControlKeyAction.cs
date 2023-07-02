using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.Engine.Engine.FacingDirections;

namespace NeoLemmixSharp.Engine.Engine.Input;

public sealed class DirectionControlKeyAction : BaseKeyAction
{
    public FacingDirection FacingDirection { get; }

    public DirectionControlKeyAction(int id, string actionName, FacingDirection facingDirection)
        : base(id, actionName)
    {
        FacingDirection = facingDirection;
    }
}