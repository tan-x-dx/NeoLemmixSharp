using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using System;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public static class BoundaryHelpers
{
    public static IHorizontalViewPortBehaviour GetHorizontalViewPortBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int width) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Void => new HorizontalVoidViewPortBehaviour(width),
        //   BoundaryBehaviourType.Solid => expr,
        //   BoundaryBehaviourType.Reflect => expr,
        BoundaryBehaviourType.Wrap => new HorizontalWrapBehaviour(width),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };

    public static IVerticalViewPortBehaviour GetVerticalViewPortBehaviour(
        BoundaryBehaviourType boundaryBehaviourType,
        int height) => boundaryBehaviourType switch
    {
        BoundaryBehaviourType.Void => new VerticalVoidViewPortBehaviour(height),
        //   BoundaryBehaviourType.Solid => expr,
        //   BoundaryBehaviourType.Reflect => expr,
        BoundaryBehaviourType.Wrap => new VerticalWrapViewPortBehaviour(height),
        _ => throw new ArgumentOutOfRangeException(nameof(boundaryBehaviourType), boundaryBehaviourType,
            "Unknown boundary behaviour type")
    };

    public static IHorizontalBoundaryBehaviour GetHorizontalBoundaryBehaviour(
        bool wrap,
        int width) => wrap
        ? new HorizontalWrapBoundaryBehaviour(width)
        : new HorizontalVoidBoundaryBehaviour();

    public static IVerticalBoundaryBehaviour GetVerticalBoundaryBehaviour(
        bool wrap,
        int height) => wrap
        ? new VerticalWrapBoundaryBehaviour(height)
        : new VerticalVoidBoundaryBehaviour();
}