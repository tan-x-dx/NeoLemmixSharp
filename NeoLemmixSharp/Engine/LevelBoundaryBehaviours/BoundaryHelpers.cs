using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public static class BoundaryHelpers
{
    public static IHorizontalBoundaryBehaviour GetHorizontalBoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int width,
        int height) => boundaryBehaviourType switch
        {
            BoundaryBehaviourType.Void => new HorizontalVoidBoundaryBehaviour(width, height),
            //   BoundaryBehaviourType.Solid => expr,
            //   BoundaryBehaviourType.Reflect => expr,
            BoundaryBehaviourType.Wrap => new HorizontalWrapBehaviour(width, height),
            _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType, "Unknown boundary behaviour type")
        };

    public static IVerticalBoundaryBehaviour GetVerticalBoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int width,
        int height) => boundaryBehaviourType switch
        {
            BoundaryBehaviourType.Void => new VerticalVoidBoundaryBehaviour(width, height),
            //   BoundaryBehaviourType.Solid => expr,
            //   BoundaryBehaviourType.Reflect => expr,
            BoundaryBehaviourType.Wrap => new VerticalWrapBoundaryBehaviour(width, height),
            _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType, "Unknown boundary behaviour type")
        };
}