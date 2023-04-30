using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering.LevelRendering;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public sealed class RightFacingDirection : FacingDirection
{
    public static RightFacingDirection Instance { get; } = new();

    private RightFacingDirection()
    {
    }

    public override int DeltaX => 1;
    public override FacingDirection OppositeDirection => LeftFacingDirection.Instance;
    public override ActionSprite ChooseActionSprite(LemmingActionSpriteBundle actionSpriteBundle, Orientation orientation)
    {
        return orientation.GetRightActionSprite(actionSpriteBundle);
    }

    public override string ToString() => "right";
}