using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public sealed class LeftFacingDirection : IFacingDirection
{
    public static LeftFacingDirection Instance { get; } = new();

    private LeftFacingDirection()
    {
    }

    public int DeltaX(int deltaX) => -deltaX;
    public IFacingDirection OppositeDirection => RightFacingDirection.Instance;
    public LevelPosition MoveInDirection(IOrientation orientation, LevelPosition pos, int step) => orientation.MoveLeft(pos, step);
    public ActionSprite ChooseActionSprite(ActionSprite left, ActionSprite right) => left;

    public override string ToString() => "left";
}