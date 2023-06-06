using NeoLemmixSharp.Engine.Directions.FacingDirections;
using NeoLemmixSharp.Util.GameInput;

namespace NeoLemmixSharp.Engine.LevelInput;

public sealed class DirectionControlKeyAction : BaseKeyAction
{
    public FacingDirection FacingDirection { get; }

    public DirectionControlKeyAction(int id, string actionName, FacingDirection facingDirection)
        : base(id, actionName)
    {
        FacingDirection = facingDirection;
    }
}