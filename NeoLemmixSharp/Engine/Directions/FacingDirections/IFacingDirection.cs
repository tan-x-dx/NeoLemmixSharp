using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine.Directions.FacingDirections;

public interface IFacingDirection
{
    int DeltaX { get; }
    IFacingDirection OppositeDirection { get; }

    ActionSprite ChooseActionSprite(LemmingActionSpriteBundle actionSpriteBundle, IOrientation orientation);
}