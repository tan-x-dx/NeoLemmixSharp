using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public interface IFacingDirection
{
    int DeltaX(int deltaX);
    IFacingDirection OppositeDirection { get; }

    ActionSprite ChooseActionSprite(ActionSprite left, ActionSprite right);
}