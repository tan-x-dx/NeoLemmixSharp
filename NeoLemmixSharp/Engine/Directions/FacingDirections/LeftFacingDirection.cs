using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering.LevelRendering;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public sealed class LeftFacingDirection : FacingDirection
{
    public static LeftFacingDirection Instance { get; } = new();

    private LeftFacingDirection()
    {
    }

    public override int DeltaX => -1;
    public override int FacingId => 1;

    public override FacingDirection OppositeDirection => RightFacingDirection.Instance;
    public override ActionSprite ChooseActionSprite(LemmingActionSpriteBundle actionSpriteBundle, Orientation orientation) => orientation.GetLeftActionSprite(actionSpriteBundle);
    public override Orientation ConvertToRelativeOrientation(Orientation orientation) => orientation.RotateClockwise();

    public override string ToString() => "left";
}