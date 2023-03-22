using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public sealed class RightFacingDirection : IFacingDirection
{
    public static RightFacingDirection Instance { get; } = new();

    private RightFacingDirection()
    {
    }

    public int DeltaX(int deltaX) => deltaX;
    public IFacingDirection OppositeDirection => LeftFacingDirection.Instance;
    public ActionSprite ChooseActionSprite(ActionSprite left, ActionSprite right) => right;

    public override string ToString() => "right";
}