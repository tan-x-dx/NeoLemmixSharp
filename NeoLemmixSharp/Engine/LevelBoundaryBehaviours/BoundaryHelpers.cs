using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public static class BoundaryHelpers
{
    public static IHorizontalViewPortBehaviour GetHorizontalBoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int width,
        int height) => boundaryBehaviourType switch
        {
            BoundaryBehaviourType.Void => new HorizontalVoidViewPortBehaviour(width),
            //   BoundaryBehaviourType.Solid => expr,
            //   BoundaryBehaviourType.Reflect => expr,
            BoundaryBehaviourType.Wrap => new HorizontalWrapBehaviour(width),
            _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType, "Unknown boundary behaviour type")
        };

    public static IVerticalViewPortBehaviour GetVerticalBoundaryBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int width,
        int height) => boundaryBehaviourType switch
        {
            BoundaryBehaviourType.Void => new VerticalVoidViewPortBehaviour(height),
            //   BoundaryBehaviourType.Solid => expr,
            //   BoundaryBehaviourType.Reflect => expr,
            BoundaryBehaviourType.Wrap => new VerticalWrapViewPortBehaviour(height),
            _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType, "Unknown boundary behaviour type")
        };
}