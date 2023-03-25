using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public sealed class RightFacingDirection : IFacingDirection
{
    public static RightFacingDirection Instance { get; } = new();

    private RightFacingDirection()
    {
    }

    public int DeltaX => 1;
    public IFacingDirection OppositeDirection => LeftFacingDirection.Instance;
    public ActionSprite ChooseActionSprite(LemmingActionSpriteBundle actionSpriteBundle, IOrientation orientation)
    {
        return orientation.GetRightActionSprite(actionSpriteBundle);
    }

    public override string ToString() => "right";
}