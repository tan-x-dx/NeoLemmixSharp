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
    public ActionSprite ChooseActionSprite(ActionSprite left, ActionSprite right) => left;

    public override string ToString() => "left";
}