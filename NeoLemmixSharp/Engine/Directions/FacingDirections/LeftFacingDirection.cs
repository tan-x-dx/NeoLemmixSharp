using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public sealed class LeftFacingDirection : IFacingDirection
{
    public static LeftFacingDirection Instance { get; } = new();

    private LeftFacingDirection()
    {
    }

    public int DeltaX => -1;
    public IFacingDirection OppositeDirection => RightFacingDirection.Instance;
    public ActionSprite ChooseActionSprite(LemmingActionSpriteBundle actionSpriteBundle, IOrientation orientation)
    {
        return orientation.GetLeftActionSprite(actionSpriteBundle);
    }

    public override string ToString() => "left";
}